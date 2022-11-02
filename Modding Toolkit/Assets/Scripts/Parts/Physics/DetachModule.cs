using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class DetachModule : MonoBehaviour
    {
        // Refs
        [Required] public SurfaceData separationSurface;
        public Composed_Vector2 separationForce;
        [Space]
        // Description
        public bool showDescription = true;
        [Space]
        public bool showForceMultiplier = false;
        [HideIf("showForceMultiplier")] public bool useForceMultiplierEvenIfNotShown;
        public Float_Reference forceMultiplier;
        
        // Extra
        [Space]
        public bool cannotDetachIfSurfaceCovered;
        [ShowIf("cannotDetachIfSurfaceCovered"), Required] public SurfaceData surfaceForCover;
        [Space]
        public bool activatedByLES;
        
        // Out
        [Space]
        public UnityEvent onDetach;

        // Functions
        public void Detach(UsePartData data)
        {
        }
    }
}