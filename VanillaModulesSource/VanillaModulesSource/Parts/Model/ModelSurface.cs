using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class ModelSurface : SurfaceData
    {
        [HideInInspector] public MeshFilter[] meshes = new MeshFilter[0];
        public List<Surface> points = new List<Surface> { new Surface { points = new List<Vector2>() }};

        [Button, HorizontalGroup] void GetMesh() => meshes = new [] { GetComponent<MeshFilter>() };
        [Button, HorizontalGroup] void GetAllMeshes() => meshes = transform.root.GetComponentsInChildren<MeshFilter>();

        public override void Output() => SetData(points.Select(surface => new Surfaces(surface.points.ToArray(), surface.loop, transform)).ToList());
        
        [Serializable]
        public class Surface
        {
            public List<Vector2> points = new List<Vector2>();
            public bool loop;
        }
    }
}