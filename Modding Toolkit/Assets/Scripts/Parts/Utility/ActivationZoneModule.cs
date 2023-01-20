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
            if (overwriteOnStage && data.sharedData.fromStaging)
            {
                onPartUsed_Staging.Invoke(data);
                return;
            }
            
            foreach (Zone zone in activationZones)
            foreach (PolygonData polygon in zone.polygons)
                if (polygon == data.clickPolygon)
                {
                    zone.onPartUsed.Invoke(data);
                    return;
                }
            
            onPartUsed_Default.Invoke(data);
        }
        
        [Serializable]
        public class Zone
        {
            public PolygonData[] polygons;
            public UsePartUnityEvent onPartUsed;
        }
    }
}