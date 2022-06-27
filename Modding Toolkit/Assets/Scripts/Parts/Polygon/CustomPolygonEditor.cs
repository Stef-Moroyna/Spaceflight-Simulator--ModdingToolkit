#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(CustomPolygon))]
    public class CustomPolygonEditor : OdinEditor
    {
        CustomPolygon shape;

        const float HandleSize = 0.1f;
        const float AddButtonSize = 0.15f;

        void OnSceneGUI()
        {
            shape = target as CustomPolygon;
            
            if (shape == null)
                return;

            if (!(shape.edit || shape.view))
                return;
            
            shape.Output();
            DrawLines();

            if (!shape.edit)
                return;
            
            DrawHandles();
            DrawAddButtons();
            DrawRemoveButtons();
        }


        void DrawLines()
        {
            foreach (Line2 line in shape.GetLines(true))
            {
                Handles.color = Color.white;
                Handles.DrawLine(line.start, line.end);
            }
        }
        void DrawHandles()
        {
            for (int i = 0; i < shape.polygonVertices.Count; i++)
                shape.polygonVertices[i] += Round(MyHandles.DrawHandle(shape.transform, shape.polygonVertices[i], Color.blue));
        }
        void DrawAddButtons()
        {
            for (int i = 0; i < shape.polygonVertices.Count; i++)
            {
                Vector2 p0 = shape.polygonVertices[i];
                Vector2 p1 = shape.polygonVertices[(i + 1) % shape.polygonVertices.Count];

                Vector2 position = Vector2.Lerp(p0, p1, 0.5f);

                float distance = Vector2.Distance(p0, p1);

                bool show = distance > shape.gridSize && distance > AddButtonSize * HandleUtility.GetHandleSize(Vector2.zero);
                if (!show)
                    continue;
                
                if (MyHandles.DrawButton(shape.transform, position, AddButtonSize, Color.green))
                    shape.polygonVertices.Insert((i + 1) % shape.polygonVertices.Count, Round(position));
            }
        }
        void DrawRemoveButtons()
        {
            if (shape.polygonVertices.Count <= 3)
                return;

            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                Vector2 mousePos = shape.transform.InverseTransformPoint(HandleUtility.GUIPointToWorldRay(e.mousePosition).origin);
                float range = HandleUtility.GetHandleSize(Vector2.zero) * HandleSize * 1.5f;

                foreach (Vector2 point in shape.polygonVertices)
                {
                    if (Vector2.Distance(point, mousePos) <= range)
                    {
                        shape.polygonVertices.Remove(point);
                        break;
                    }
                }
            }
        }


        Vector2 Round(Vector2 a)
        {
            return shape.gridSize != 0 ? new Vector2(Mathf.Round(a.x / shape.gridSize) * shape.gridSize, Mathf.Round(a.y / shape.gridSize) * shape.gridSize) : a;
        }
    }
}
#endif