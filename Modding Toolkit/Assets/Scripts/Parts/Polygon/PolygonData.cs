using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public abstract class PolygonData : SurfaceData
    {
        [SerializeField, BoxGroup("", false), LabelText("Build Collider")] bool colliderArea = true; // Can the polygon collide
        [SerializeField, BoxGroup("", false), LabelText("Can Click (Build/Game)")] bool clickArea = true; // Can the polygon be clicked

        [HorizontalGroup, HideIf(nameof(isComposedDepth))] public float baseDepth = 0; // Used for raycast/rendering
        [HorizontalGroup, ShowIf(nameof(isComposedDepth))] public Composed_Float composedBaseDepth;
        [HorizontalGroup, HideLabel] public bool isComposedDepth;

        // Data
        public Polygon polygon;
        public Polygon polygonFast;


        public bool Click => clickArea && isActiveAndEnabled;
        public bool BuildCollider => colliderArea && isActiveAndEnabled;
        public bool BuildCollider_IncludeInactive => colliderArea;
        
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
            depth = baseDepth;
        }
    }
}