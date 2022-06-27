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
        [BoxGroup] public Composed_Vector2 position;
        [BoxGroup] public Vector2 normal;
        [BoxGroup] public AdaptModule.TriggerType type;
        [BoxGroup] public Composed_Float output;
        
        // State
        [NonSerialized] public AdaptModule.Point occupied = null;
        [ShowInInspector] bool Occupied => occupied != null;
    }
}