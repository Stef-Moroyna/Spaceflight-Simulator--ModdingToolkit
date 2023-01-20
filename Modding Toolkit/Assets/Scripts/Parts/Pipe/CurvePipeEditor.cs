#if UNITY_EDITOR
using System.Linq;
using SFS.Variables;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(CurvePipe))]
    public class CurvePipeEditor : OdinEditor
    {
        CurvePipe shape;

        void OnSceneGUI()
        {
            shape = target as CurvePipe;
            if (shape == null)
                return;
            
            if (!shape.edit && !shape.view)
                return;

            // Shape
            foreach (int side in new[] { -1, 1 })
            {
                Vector3[] points = shape.GetPoints().Select(p => shape.transform.TransformPoint(new Vector2(p.x * side, p.y))).ToArray();
            
                for (int i = 0; i < points.Length - 1; i++)
                    Handles.DrawLine(points[i], points[i + 1]);   
            }
            
            if (!shape.edit)
                return;

            // Lines
            for (int i = 0; i < shape.points.Count - 1; i++)
                Handles.DrawDottedLine(shape.transform.TransformPoint(shape.points[i].Value), shape.transform.TransformPoint(shape.points[i + 1].Value), 2);
            
            // Move
            foreach (Composed_Vector2 point in shape.points)
            {
                Vector2 offsetPos = Round(MyHandles.DrawHandle(shape.transform, point.Value, Color.blue));
                
                if (offsetPos.sqrMagnitude != 0)
                    point.Offset(offsetPos);
            }
        }
        Vector2 Round(Vector2 a)
        {
            if (shape.gridSize != 0)
                return new Vector2(Mathf.Round(a.x / shape.gridSize) * shape.gridSize, Mathf.Round(a.y / shape.gridSize) * shape.gridSize);
            
            return a;
        }
    }
}
#endif