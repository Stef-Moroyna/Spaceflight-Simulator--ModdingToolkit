using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using SFS.Builds;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class AdaptTriggerModule : MonoBehaviour, I_InitializePartModule
    {
        public AdaptTriggerPoint[] points;

        [NonSerialized] public Dictionary<int, AdaptModule.Point> occupied = new Dictionary<int, AdaptModule.Point>();

        bool initialized;
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            foreach (AdaptTriggerPoint point in points)
                point.owner = this;
            
            if (BuildManager.main != null)
                foreach (AdaptTriggerPoint point in points)
                    point.enabled.OnChange += () => AdaptModule.OnCanAdaptChange(transform, initialized);
            
            initialized = true;
        }
    }
    
    [Serializable]
    public class AdaptTriggerPoint
    {
        public bool toggle;
        [ShowIf("toggle")] public Bool_Reference enabled;
        
        [BoxGroup] public Composed_Vector2 position;
        [BoxGroup] public Vector2 normal;
        [BoxGroup] public AdaptModule.TriggerType type;
        [BoxGroup] public Composed_Float output;
        [BoxGroup] public float outputOffset;

        [HorizontalGroup] public bool shareIsOccupied;
        [HorizontalGroup, HideLabel, ShowIf("shareIsOccupied")] public int shareIndex;
        
        // State
        [NonSerialized] public AdaptTriggerModule owner;
        [NonSerialized] AdaptModule.Point occupied = null;
        public AdaptModule.Point Occupied
        {
            get => shareIsOccupied ? owner.occupied[shareIndex] : occupied;
            set
            {
                if (shareIsOccupied)
                    owner.occupied[shareIndex] = value;
                else
                    occupied = value;
            }
        }
        
        // Cache
        [NonSerialized] public Vector2 worldPosition, worldNormal;
    }
}