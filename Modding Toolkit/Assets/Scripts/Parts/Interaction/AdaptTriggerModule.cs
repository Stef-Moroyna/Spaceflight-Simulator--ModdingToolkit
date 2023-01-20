using Sirenix.OdinInspector;
using System;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class AdaptTriggerModule : MonoBehaviour
    {
        public AdaptTriggerPoint[] points;
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
    }
}