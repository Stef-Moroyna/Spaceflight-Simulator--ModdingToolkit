using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PartJoint = SFS.World.Joint<SFS.Parts.Part>;

namespace SFS.Parts.Modules
{
    public class DetachModule : MonoBehaviour, I_PartMenu
    {
        // Refs
        [Required] public SurfaceData separationSurface;
        public Composed_Vector2 separationForce;
        [Space]
        public bool cannotDetachIfSurfaceCovered;
        [ShowIf("cannotDetachIfSurfaceCovered")] public SurfaceData surfaceForCover;
        [Space]
        // Description
        public bool showDescription = true;
        public bool showForceMultiplier = false;
        public bool useForceMultiplierEvenIfNotShown;
        public Float_Reference forceMultiplier;
        
        // Out
        [Space]
        public UnityEvent onDetach;
        
        
        // Injection
        public Rocket Rocket { private get; set; }
        
        // Get
        float ForceMultiplier => showForceMultiplier || useForceMultiplierEvenIfNotShown? forceMultiplier.Value * 2 : 1;

        // Functions
        public void Detach(UsePartData data)
        {
            onDetach?.Invoke();
        }
        
        

        struct DetachData
        {
            public PartJoint joint;
            public float overlapSurface;
            public Vector2 connectionPosition;
        }
    }
}