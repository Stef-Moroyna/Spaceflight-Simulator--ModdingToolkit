using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS.Career;
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
    public class Part : HeatModuleBase, Rocket.INJ_Rocket
    {
        // Refs
        [BoxGroup("Description", false)] public TranslationVariable displayName = new TranslationVariable();
        [BoxGroup("Description", false)] public TranslationVariable pickCategoryName = new TranslationVariable();
        [BoxGroup("Description", false)] public TranslationVariable description = new TranslationVariable();

        [BoxGroup("Vars", false)] public Composed_Float mass;
        [BoxGroup("Vars", false)] public Composed_Vector2 centerOfMass;
        [BoxGroup("Vars", false)] public float density = 0.5f;

        [BoxGroup("Refs", false), Required] public OrientationModule orientation;
        [BoxGroup("Refs", false), Required] public VariablesModule variablesModule;

        public Variants[] variants = Array.Empty<Variants>();
        public UsePartUnityEvent onPartUsed;
        
        // State
        [ReadOnly, NonSerialized] public BurnMark burnMark;
        //[ReadOnly, NonSerialized] public float area;
        [ReadOnly, NonSerialized] public int frameIndex_WaterDamage = -10;

        // Injected
        public Rocket Rocket { get; set; }


        // Description
        public void DrawPartStats(Part[] allParts, StatsMenu drawer, PartDrawSettings settings)
        {
            // Title
            if (settings.showTitle)
                drawer.DrawTitle(GetDisplayName());

            // Draws only crew module??
            if ((settings.build || settings.game) && HasModule<CrewModule>() && !DevSettings.DisableAstronauts)
            {
                ((I_PartMenu)GetModules<CrewModule>()[0]).Draw(drawer, settings);
                return;
            }

            // Draws mass
            drawer.DrawStat(100, () => mass.Value.ToMassString(false),
            null,
            update => mass.OnChange += update,
            update => mass.OnChange -= update);
            
            // Density
            drawer.DrawStat(100, Loc.main.Density.Inject(density.ToString(1, true), "value"));
            

            // Draws module stats
            foreach (I_PartMenu partMenu in GetModules<I_PartMenu>().ToArray())
                partMenu.Draw(drawer, settings);
            
            
            // Resources
            DrawMultiple<ResourceModule>(x => x.showDescription, x => x.resourceType.GetInstanceID().ToString(), 
                (a, b, c, d) => a.Draw(b, c, d));
            // Toggle
            DrawMultiple<ToggleModule>(x => true, x => x.label.Field, 
                (a, b, c, d) => a.Draw(b, c, d));
            // Engines
            DrawMultiple<EngineModule>(x => true, x => "", 
                (a, b, c, d) => a.Draw(b, c, d));
            // Separators (split and detach)
            DrawMultiple<SeparatorBase>(x => x.ShowDescription, x => "", 
                (a, b, c, d) => a.Draw(b, c, d));
            // Wheel
            DrawMultiple<WheelModule>(x => true, x => "", 
                (a, b, c, d) => a.Draw(b, c, d));
            // Docking port
            DrawMultiple<DockingPortModule>(x => true, x => "", 
                (a, b, c, d) => a.Draw(b, c, d));
            // LES
            DrawMultiple<LES_Module>(x => true, x => "", 
                (a, b, c, d) => a.Draw(b, c, d));
            // Variables drawer
            DrawMultiple<VariablesDrawer>(x => true, x =>
                {
                    StringBuilder id = new StringBuilder();
                    
                    foreach (VariablesDrawer.DrawElement element in x.elements)
                    {
                        id.Append(element.variableType.ToString());
                        id.Append(element.label.Field);
;                    }
                    
                    return id.ToString();
                }, 
                (a, b, c, d) => a.Draw(b, c, d));
            
            void DrawMultiple<T>(Func<T, bool> drawn, Func<T, string> getID, Action<T, List<T>, StatsMenu, PartDrawSettings> draw)
            {
                Dictionary<string, List<T>> variable = new Dictionary<string, List<T>>();
                T[] resourceModules = GetModules<T>().Where(drawn.Invoke).ToArray();
                
                if (resourceModules.Length == 0)
                    return;
                
                for (int i = 0; i < resourceModules.Length; i++)
                {
                    T module = resourceModules[i];
                    variable.Add(i + getID.Invoke(module), new List<T>());
                }
                foreach (Part part in allParts)
                {
                    T[] partModules = part.GetModules<T>().Where(drawn.Invoke).ToArray();
                    for (int i = 0; i < partModules.Length; i++)
                    {
                        T module = partModules[i];
                        string id = i + getID.Invoke(module);
                    
                        if (variable.ContainsKey(id))
                            variable[id].Add(module);
                    }
                }
                foreach (List<T> list in variable.Values)
                    draw.Invoke(list[0], list, drawer, settings);
            }
            
            // Description
            if (settings.showDescription)
            {
                string text = description.Field;
                
                if (!string.IsNullOrEmpty(text))
                    drawer.DrawText(-1000, text);
            }
        }


        // Modules
        Dictionary<string, object> modules = new Dictionary<string, object>();
        Dictionary<string, int> moduleCount = new Dictionary<string, int>();
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

        public (ConvexPolygon[], bool isFront) GetBuildColliderPolygons(bool forAttach = false)
        {
            List<ConvexPolygon> polygons = new List<ConvexPolygon>();

            foreach (PolygonData module in GetModules<PolygonData>())
                if (module.BuildCollider && (!forAttach || module.AttachByOverlap))
                    polygons.AddRange(module.polygon.GetConvexPolygonsWorld(module.transform));
            
            return (polygons.ToArray(), IsFront());
        }
        public bool IsFront() => false; // was super bad for performance  //HasModule<FrontAttachModule>() && GetModules<FrontAttachModule>()[0].IsFront();
        public Line2[] GetAttachmentSurfacesWorld()
        {
            List<Line2> output = new List<Line2>();

            foreach (SurfaceData module in GetModules<SurfaceData>())
                if (module.Attachment)
                    foreach (Surfaces surface in module.surfaces)
                        output.AddRange(surface.GetSurfacesWorld());

            return output.ToArray();
        }
        
        // Ownership
        public OwnershipState GetOwnershipState()
        {
            OwnModule[] ownModules = GetModules<OwnModule>();

            // Ownership
            if (!ownModules.All(a => a.IsOwned || !a.IsPremium))
                return OwnershipState.NotOwned;
            
            // Unlock
            if (!CareerState.main.HasPart(this))
                return OwnershipState.NotUnlocked;

            // Has part
            return OwnershipState.OwnedAndUnlocked;
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
            
            // Hacky fix for now
            foreach (SeparatorPanel separator in GetComponentsInChildren<SeparatorPanel>())
                separator.FlipToLight();
        }


        // Heat implementation
        public float temperature = float.NegativeInfinity;
        public override string Name => GetDisplayName();
        public override bool IsHeatShield => false;
        public override float Temperature { get => temperature; set => temperature = value; }
        public override int LastAppliedIndex { get; set; } = -1;
        public override float ExposedSurface { get; set; } = 0;
        public override float HeatTolerance => AeroModule.GetHeatTolerance(World.Drag.HeatTolerance.Low);
        public override void OnOverheat(bool breakup) => OnOverheat(this, breakup);
        public void OnOverheat(HeatModuleBase module, bool breakup)
        {
            List<PartJoint> connectedJoints = Rocket.jointsGroup.GetConnectedJoints(this);

            if (breakup && connectedJoints.Count > 0 && !module.IsHeatShield)
            {
                Rocket rocketOld = Rocket;
                JointGroup.DestroyJoint(connectedJoints[0], Rocket, out bool split, out Rocket rocketNew);
                EffectManager.CreatePartOverheatEffect(transform.TransformPoint(centerOfMass.Value), mass.Value * 2f + 0.5f);

                if (split)
                {
                    rocketOld.EnableCollisionImmunity(1.5f);
                    rocketNew.EnableCollisionImmunity(1.5f);

                    if (rocketOld.isPlayer)
                        Rocket.SetPlayerToBestControllable(rocketOld, rocketNew);

                    module.Temperature *= 0.8f;
                }
            }
            else
            {
                EffectManager.CreatePartOverheatEffect(transform.TransformPoint(centerOfMass.Value), mass.Value * 2f + 0.5f);
                DestroyPart(!breakup, true, DestructionReason.Overheat);
            }
        }


        // Build undo utility
        public Action<Part> aboutToDestroy;
        
        // Lifecycle
        public Action<Part> onPartDestroyed;
        public void DestroyPart(bool createExplosion, bool updateJoints, DestructionReason reason)
        {
            if (createExplosion)
                EffectManager.CreateExplosion(transform.TransformPoint(centerOfMass.Value), mass.Value * 2f + 0.5f);

            // Prevents modification while invoking
            Action<Part> action = onPartDestroyed;
            onPartDestroyed = null;
            action?.Invoke(this);

            if (updateJoints)
                JointGroup.OnPartDestroyed(this, Rocket, reason);

            Destroy(gameObject);
        }
    }

    [Serializable] public class UsePartUnityEvent : UnityEvent<UsePartData> { }

    public class UsePartData
    {
        public SharedData sharedData;
        public bool successfullyUsedPart = true;
        public PolygonData clickPolygon;

        public UsePartData(SharedData sharedData, PolygonData clickPolygon)
        {
            this.sharedData = sharedData;
            this.clickPolygon = clickPolygon;
        }

        public class SharedData
        {
            public readonly bool fromStaging;
            
            public Action onPostPartsActivation;
            public bool hasToggledRCS = false;

            public SharedData(bool fromStaging) => this.fromStaging = fromStaging;
        }
    }
}