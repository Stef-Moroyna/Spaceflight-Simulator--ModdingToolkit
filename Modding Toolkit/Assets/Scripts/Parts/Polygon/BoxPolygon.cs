using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class BoxPolygon : PolygonData
    {
        public Vector2 point_A = Vector2.zero;
        public Vector2 point_B = Vector2.one;

        public override void Output()
        {
            Polygon newVertices = GetVertices();
            SetData(newVertices, newVertices);
        }
        
        public Polygon GetVertices()
        {
            return new Polygon(new [] { point_A, new Vector2(point_A.x, point_B.y), point_B, new Vector2(point_B.x, point_A.y) });
        }
        
        
        #if UNITY_EDITOR
        [BoxGroup("edit", false)] public bool edit = true;
        [BoxGroup("edit", false), ShowIf("edit")] public float gridSize = 0.2f;
        #endif
    }
}