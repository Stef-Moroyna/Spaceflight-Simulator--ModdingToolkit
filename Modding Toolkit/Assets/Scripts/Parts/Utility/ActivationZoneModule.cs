using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class ActivationZoneModule : MonoBehaviour
    {
        public Zone[] activationZones;
        public UsePartUnityEvent onPartUsed_Default;

        public bool overwriteOnStage;
        [ShowIf("overwriteOnStage")] public UsePartUnityEvent onPartUsed_Staging;
        
        public void OnPartUsed(UsePartData data)
        {
        }
        
        [Serializable]
        public class Zone
        {
            public PolygonData[] polygons;
            public UsePartUnityEvent onPartUsed;
        }
    }
}