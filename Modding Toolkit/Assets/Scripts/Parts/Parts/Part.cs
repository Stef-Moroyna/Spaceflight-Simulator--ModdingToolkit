using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFS.Variables;
using SFS.Parts.Modules;
using UnityEngine.Events;
using SFS.World;
using SFS.Translations;
using SFS.World.Drag;

namespace SFS.Parts
{
    [HideMonoScript]
    public class Part : MonoBehaviour, I_HeatModule, I_JointNode
    {
        // Refs
        [BoxGroup("Description", false)] public TranslationVariable displayName = new TranslationVariable();
        [BoxGroup("Description", false)] public TranslationVariable description = new TranslationVariable();

        [BoxGroup("Vars", false)] public Composed_Float mass;
        [BoxGroup("Vars", false)] public Composed_Vector2 centerOfMass;

        [BoxGroup("Refs", false), Required] public OrientationModule orientation;
        [BoxGroup("Refs", false), Required] public VariablesModule variablesModule;

        public Variants[] variants = Array.Empty<Variants>();
        public UsePartUnityEvent onPartUsed;
        
        // State
        [ReadOnly, NonSerialized] public BurnMark burnMark;

        // Injected
        public Rocket Rocket { get; set; }


        // Modules
        readonly Dictionary<string, object> modules = new Dictionary<string, object>();
        readonly Dictionary<string, int> moduleCount = new Dictionary<string, int>();
        //
        public T[] GetModules<T>()
        {
            string typeName = typeof(T).Name;

            if (!modules.ContainsKey(typeName))
                modules.Add(typeName, GetComponentsInChildren<T>(true));

            return (T[])modules[typeName];
        }
        public int GetModuleCount<T>()
        {
            string typeName = typeof(T).Name;

            if (!moduleCount.ContainsKey(typeName))
                moduleCount.Add(typeName, GetComponentsInChildren<T>(true).Length);

            return moduleCount[typeName];
        }
        public bool HasModule<T>()
        {
            return GetModuleCount<T>() > 0;
        }


        // Get
        public Field GetDisplayName()
        {
            //if (GetModules<EngineBase>().Any(engine => engine.vacuumEngine.Value))
            //return Loc.main.Engine_Vac.InjectField(displayName, "part_name");

            return displayName.Field;
        }

        public List<PolygonData> GetClickPolygons() => GetModules<PolygonData>().Where(x => x.Click).ToList();

        public (ConvexPolygon[], bool isFront) GetBuildColliderPolygons()
        {
            List<ConvexPolygon> polygons = new List<ConvexPolygon>();

            foreach (PolygonData module in GetModules<PolygonData>())
                if (module.BuildCollider)
                    polygons.AddRange(module.polygon.GetConvexPolygonsWorld(module.transform));
            
            return (polygons.ToArray(), IsFront());
        }
        public bool IsFront() => HasModule<FrontAttachModule>() && GetModules<FrontAttachModule>()[0].IsFront();
        public Line2[] GetAttachmentSurfacesWorld()
        {
            List<Line2> output = new List<Line2>();

            foreach (SurfaceData module in GetModules<SurfaceData>())
                if (module.Attachment)
                    foreach (Surfaces surface in module.surfaces)
                        output.AddRange(surface.GetSurfacesWorld());

            return output.ToArray();
        }



        public Vector2 Position
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }


        // Initializes parts
        public void InitializePart()
        {
            List<I_InitializePartModule> setups = new List<I_InitializePartModule>(GetComponentsInChildren<I_InitializePartModule>(true));
            setups.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            setups.ForEach(a => a.Initialize());
        }

        // Updates parts meshes
        public void SetSortingLayer(string sortingLayer)
        {
            foreach (BaseMesh meshModule in GetModules<BaseMesh>())
                meshModule.SetSortingLayer(sortingLayer);

            foreach (ModelSetup modelSetup in GetModules<ModelSetup>())
                modelSetup.SetSortingLayer(sortingLayer);
        }
        public void RegenerateMesh()
        {
            foreach (BaseMesh meshModule in GetModules<BaseMesh>())
                meshModule.GenerateMesh();

            foreach (ModelSetup modelSetup in GetModules<ModelSetup>())
                modelSetup.SetMesh();
        }


        // Heat implementation
        public float temperature = float.NegativeInfinity;
        string I_HeatModule.Name => GetDisplayName();
        bool I_HeatModule.IsHeatShield => false;
        float I_HeatModule.Temperature { get => temperature; set => temperature = value; }
        int I_HeatModule.LastAppliedIndex { get; set; } = -1;
        float I_HeatModule.ExposedSurface { get; set; } = 0;
        float I_HeatModule.HeatTolerance => AeroModule.GetHeatTolerance(HeatTolerance.Low);
        void I_HeatModule.OnOverheat(bool breakup) => OnOverheat(this, breakup);
        public void OnOverheat(I_HeatModule module, bool breakup)
        {
        }


        // Lifecycle
        public event Action<Part> onPartDestroyed;
        public void DestroyPart(bool createExplosion, DestructionReason reason)
        {
            Destroy(gameObject);
        }
        
        public bool IsSoft => HasModule<SoftAttach>();
        public bool ShouldDetach(I_JointNode x) => !x.IsSoft && (GetModules<SoftAttach>().FirstOrDefault(_ => true)?.DetachParts().Contains(x) ?? false);
    }

    [Serializable] public class UsePartUnityEvent : UnityEvent<UsePartData> { }

    public class UsePartData
    {
        public SharedData sharedData;
        public bool successfullyUsedPart = true;

        public UsePartData(SharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public class SharedData
        {
            public Action onPostPartsActivation;
            public bool hasToggledRCS = false;
        }
    }

    [Serializable]
    public class PartRegionalEvent
    {
        public List<PolygonData> regions;
        public UsePartUnityEvent onClick;
    }
}