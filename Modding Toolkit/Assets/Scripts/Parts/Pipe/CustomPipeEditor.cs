#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(CustomPipe))]
    public class CustomPipeEditor : OdinEditor
    {
        CustomPipe shape;

        void OnSceneGUI()
        {
            shape = target as CustomPipe;

            if (shape == null)
                return;
            
            shape.Output();
            Pipe pipe = shape.pipe;

            if (pipe == null || pipe.points == null || pipe.points.Count == 0)
                return;

            if (!shape.edit && !shape.view)
                return;
            
            // Lines
            DrawHorizontalLines(pipe, shape.edit);
            DrawVerticalLines(pipe);

            if (!shape.edit)
                return;

            // Move
            DrawHandles(pipe);

            // Add/Remove
            DrawAddButtons(pipe);

            if (pipe.points.Count > 2)
                DrawRemoveButtons(pipe);
        }

        // Editor drawing
        void DrawHorizontalLines(Pipe pipe, bool drawBetweenLines)
        {
            // Draw Point
            for (int i = 0; i < pipe.points.Count; i++)
            {
                PipePoint a = pipe.points[i];

                // Horizontal line
                if (i == 0 || i == pipe.points.Count - 1)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(shape.transform.TransformPoint(a.Left), shape.transform.TransformPoint(a.Right));
                }
                else if (drawBetweenLines)
                {
                    Handles.color = new Color(1, 1, 1, 0.5f);
                    Handles.DrawDottedLine(shape.transform.TransformPoint(a.Left), shape.transform.TransformPoint(a.Right), 4);
                }
            }
        }
        void DrawVerticalLines(Pipe pipe)
        {
            for (int i = 0; i < pipe.points.Count - 1; i++)
            {
                PipePoint a = pipe.points[i];
                PipePoint b = pipe.points[i + 1];

                // Vertical lines
                Handles.color = new Color(1, 1, 1, 0.5f);

                Handles.DrawDottedLine(shape.transform.TransformPoint(a.position), shape.transform.TransformPoint(b.position), 5);
                Handles.color = Color.white;
                Handles.DrawLine(shape.transform.TransformPoint(a.Left), shape.transform.TransformPoint(b.Left));
                Handles.DrawLine(shape.transform.TransformPoint(a.Right), shape.transform.TransformPoint(b.Right));
            }
        }
        //
        void DrawHandles(Pipe pipe)
        {
            for (int i = 0; i < pipe.points.Count; i++)
            {
                Vector2 offsetPos = Round(MyHandles.DrawHandle(shape.transform, pipe.points[i].position, Color.blue));   
                if (offsetPos.sqrMagnitude != 0)
                    shape.composedShape.points[i].position.Offset(offsetPos);
                 
                Vector2 offsetWidth = Round(MyHandles.DrawHandle(shape.transform, pipe.points[i].Right, Color.blue));
                if (offsetWidth.sqrMagnitude != 0)
                    shape.composedShape.points[i].width.Offset(offsetWidth);
            }
        }
        //
        void DrawAddButtons(Pipe pipe)
        {
            float size = HandleUtility.GetHandleSize(Vector2.zero);

            // Add at start/end buttons
            if (MyHandles.DrawButton(shape.transform, pipe.points.First().position + Vector2.down * 0.2f * size, 0.2f, Color.green))
                AddPoint(0, pipe.points.First().position + Vector2.down * shape.gridSize, pipe.points.First().width);
            if (MyHandles.DrawButton(shape.transform, pipe.points[pipe.points.Count - 1].position + Vector2.up * 0.2f * size, 0.2f, Color.green))
                AddPoint(pipe.points.Count, pipe.points.Last().position + Vector2.up * shape.gridSize, pipe.points.Last().width);
        
            // Add between buttons
            for (int i = 0; i < pipe.points.Count - 1; i++)
            {
                PipePoint a = pipe.points[i];
                PipePoint b = pipe.points[i + 1];

                Vector2 position = ((a.Left + b.Left) / 2) - ((a.width / 2 + b.width / 2).normalized * size * 0.2f);
                bool show = (a.position - b.position).magnitude > shape.gridSize;

                if (MyHandles.DrawButton(this.shape.transform, position, 0.2f, show? Color.green : Color.clear) && show)
                    AddPoint(i + 1, (a.position + b.position) / 2, (a.width + b.width) / 2);
            }
        }
        void DrawRemoveButtons(Pipe pipe)
        {
            for (int i = 0; i < pipe.points.Count; i++)
                if (MyHandles.DrawButton(shape.transform, pipe.points[i].Left + (-pipe.points[i].width.normalized * 0.2f * HandleUtility.GetHandleSize(Vector2.zero)), 0.2f, Color.red))
                {
                    shape.composedShape.points.RemoveAt(i);
                    break;
                }
        }


        // Functions
        void AddPoint(int index, Vector2 position, Vector2 width)
        {
            position = Round(position);
            width = Round(width);

            Composed_ShapePoint newPoint = new Composed_ShapePoint(position, width);
            shape.composedShape.points.Insert(index, newPoint);
        }


        // Utility
        Vector2 Round(Vector2 a)
        {
            if (shape.gridSize != 0)
                return new Vector2(Mathf.Round(a.x / shape.gridSize) * shape.gridSize, Mathf.Round(a.y / shape.gridSize) * shape.gridSize);
            
            return a;
        }
    }
}
#endif