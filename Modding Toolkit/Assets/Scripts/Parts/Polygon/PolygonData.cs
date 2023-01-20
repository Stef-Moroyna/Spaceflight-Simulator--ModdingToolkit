using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using SFS.World;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public abstract class PolygonData : SurfaceData
    {
        [SerializeField, BoxGroup("", false), LabelText("Build Collider")] bool colliderArea = true; // Can the polygon collide
        [SerializeField, BoxGroup("", false), LabelText("Attach By Overlap"), ShowIf("colliderArea")] bool attachByOverlap = true; // Attach by overlapping colliders
        [SerializeField, BoxGroup("", false), LabelText("Physics Collider")] bool physicsCollider = false; // Can the polygon be clicked
        [SerializeField, BoxGroup("", false), LabelText("Can Click (Build/Game)")] bool clickArea = true; // Can the polygon be clicked

        [BoxGroup("Depth", false), HorizontalGroup("Depth/H"), HideIf(nameof(isComposedDepth)), SerializeField] float baseDepth = 0; // Used for raycast/rendering
        [BoxGroup("Depth", false), HorizontalGroup("Depth/H"), ShowIf(nameof(isComposedDepth)), SerializeField] Composed_Float composedBaseDepth;
        [BoxGroup("Depth", false), HorizontalGroup("Depth/H"), HideLabel, SerializeField] bool isComposedDepth;

        // Data
        public Polygon polygon;
        public Polygon polygonFast;

        void Reset()
        {
            physicsCollider = true;
        }
        
        public bool Click => clickArea && isActiveAndEnabled;
        public bool BuildCollider => colliderArea && isActiveAndEnabled;
        public bool BuildCollider_IncludeInactive => colliderArea;
        public bool PhysicsCollider_IncludeInactive => physicsCollider;
        public bool AttachByOverlap => attachByOverlap;
        
        public float BaseDepth => isComposedDepth ? composedBaseDepth.Value : baseDepth;
        public void SubscribeToComposedDepth(Action a)
        {
            if (isComposedDepth)
                composedBaseDepth.OnChange += a;
        }
        
        protected void SetData(Polygon polygon, Polygon polygonFast)
        {
            this.polygon = polygon;
            this.polygonFast = polygonFast;

            SetData(new List<Surfaces> { new Surfaces(polygon.vertices, true, transform) }, new List<Surfaces>() { new Surfaces(polygonFast.vertices, true, transform) });
        }
        protected void SetData(Polygon polygon)
        {
            this.polygon = polygon;
            polygonFast = polygon;
            
            List<Surfaces> newSurfaces = new List<Surfaces> { new Surfaces(polygon.vertices, true, transform) };
            SetData(newSurfaces, newSurfaces);
        }
        
        public virtual void Raycast(Vector2 point, out float depth)
        {
            depth = BaseDepth;
        }
    }
}