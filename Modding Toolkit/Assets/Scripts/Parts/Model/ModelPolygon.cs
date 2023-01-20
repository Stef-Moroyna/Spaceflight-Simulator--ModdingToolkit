using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class ModelPolygon : PolygonData
    {
        [HideInInspector] public MeshFilter[] meshes = Array.Empty<MeshFilter>();
        public List<Vector2> points;

        [HorizontalGroup("A")] public bool edit, view;
        
        [Button, HorizontalGroup] void GetMesh() => meshes = new [] { GetComponent<MeshFilter>() };
        [Button, HorizontalGroup] void GetAllMeshes() => meshes = transform.GetComponentsInChildren<MeshFilter>();
        [Button, HorizontalGroup] void GetAllMeshesRoot() => meshes = transform.root.GetComponentsInChildren<MeshFilter>();
        [Button, HorizontalGroup] void ClearPoints() => points.Clear();
        public override void Output() => SetData(new Polygon(points.ToArray()));
    }
}