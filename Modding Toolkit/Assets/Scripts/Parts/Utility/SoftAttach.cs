using System.Collections.Generic;
using System.Linq;
using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class SoftAttach : MonoBehaviour
    {
        public SurfaceData detach;

        public Rocket Rocket { private get; set; }

        public List<Part> DetachParts()
        {
            Part part = transform.GetComponentInParentTree<Part>();
            Line2[] surfacesWorld = detach.surfaces.SelectMany(x => x.GetSurfacesWorld()).ToArray();

            return null;
        }
    }
}