using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class DockingPortModule : MonoBehaviour
    {
        [Required] public DockingPortTrigger trigger;
        [Required] public SurfaceData occupationSurface;

        public float dockDistance, pullDistance, pullForce;
        public Float_Reference forceMultiplier;

        public Bool_Local isOccupied;
        public Bool_Local isOnCooldown;
        public Bool_Local isDockable;
    }
}