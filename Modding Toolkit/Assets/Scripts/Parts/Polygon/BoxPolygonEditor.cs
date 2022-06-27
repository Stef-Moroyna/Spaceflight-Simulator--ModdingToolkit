#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(BoxPolygon))]
    public class BoxPolygonEditor : OdinEditor
    {
        BoxPolygon shape;

        void OnSceneGUI()
        {
            shape = target as BoxPolygon;

            if (shape == null)
                return;
            
            shape.Output();
            
            // Draw lines between points
            DrawLines();

            if (!shape.edit)
                return;

            // Draw handles to move points
            DrawHandles(shape);
        }
        
        void DrawLines()
        {
            Vector2[] points = shape.polygon.GetVerticesWorld(shape.transform);

            Handles.color = Color.white;
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[1], points[2]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawLine(points[3], points[0]);
        }
        void DrawHandles(BoxPolygon shape)
        {
            shape.point_A += Round(MyHandles.DrawHandle(shape.transform, shape.point_A, Color.blue));
            shape.point_B += Round(MyHandles.DrawHandle(shape.transform, shape.point_B, Color.blue));
        }
        

        // Clamps a to grid based on gridSize
        Vector2 Round(Vector2 a)
        {
            return shape.gridSize != 0 ? new Vector2(Mathf.Round(a.x / shape.gridSize) * shape.gridSize, Mathf.Round(a.y / shape.gridSize) * shape.gridSize) : a;
        }
    }
}
#endif