using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class PolygonCollider : ColliderModule, I_InitializePartModule
    {
        [Required] public PolygonData polygon;

        [SerializeField, HideInInspector] PolygonCollider2D collider_Polygon;
        [SerializeField, HideInInspector] BoxCollider2D collider_Box;

        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize()
        {
            polygon.onChange += BuildCollider;
        }

        void BuildCollider()
        {
            Vector2[] points = polygon.polygonFast.vertices;

            if (points.Length == 4 && points[0].x == points[1].x && points[1].y == points[2].y && points[2].x == points[3].x && points[3].y == points[0].y)
            {
                if (collider_Polygon != null)
                    Destroy(collider_Polygon);
                
                if (collider_Box == null)
                    collider_Box = gameObject.AddComponent<BoxCollider2D>();

                collider_Box.size = new Vector2(Mathf.Abs(points[2].x - points[0].x), Mathf.Abs(points[2].y - points[0].y));
                collider_Box.offset = (points[0] + points[2]) * 0.5f;
            }
            else
            {
                if (collider_Box != null)
                    Destroy(collider_Polygon);
                
                if (collider_Polygon == null)
                    collider_Polygon = gameObject.AddComponent<PolygonCollider2D>();

                collider_Polygon.points = points;
            }
        }
    }
}