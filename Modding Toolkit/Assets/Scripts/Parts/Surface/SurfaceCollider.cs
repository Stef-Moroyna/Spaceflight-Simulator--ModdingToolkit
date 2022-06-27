using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class SurfaceCollider : ColliderModule, I_InitializePartModule
    {
        [Required] public SurfaceData surfaces;

        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize() => surfaces.onChange += CreateSurfaceColliders;

        void CreateSurfaceColliders()
        {
            // Creates new surfaces
            List<Surfaces> surfacesList = surfaces.surfaces;
            
            // Creates
            List<EdgeCollider2D> edges = this.GetOrAddComponents<EdgeCollider2D>(surfacesList.Count);
            for (int i = 0; i < surfacesList.Count; i++)
                if (i < surfacesList.Count)
                    edges[i].points = surfacesList[i].points;
                else
                    Destroy(edges[i]);
        }
    }
}