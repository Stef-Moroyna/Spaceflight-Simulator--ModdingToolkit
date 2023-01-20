using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using SFS.Parts.Modules;
using UnityEngine.Events;
using SFS.Translations;

namespace SFS.Parts
{
    [HideMonoScript]
    public class Part : MonoBehaviour
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

        public float temperature = float.NegativeInfinity;
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