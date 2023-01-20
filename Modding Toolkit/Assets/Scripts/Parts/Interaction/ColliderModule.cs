using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class ColliderModule : MonoBehaviour
    {
        public ImpactTolerance impactTolerance = ImpactTolerance.Medium;
        public Collider2D ownEngineNozzle;
        
        public enum ImpactTolerance
        {
            Low,
            Medium,
            High,
            Wheel,
        }
    }
}