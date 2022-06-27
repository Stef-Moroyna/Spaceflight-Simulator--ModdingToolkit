#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using SFS.Variables;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(CustomSurfaces))]
    public class CustomSurfacesEditor : OdinEditor
    {
        CustomSurfaces surface;

        const float HandleSize = 0.1f;
        const float AddButtonSize = 0.15f;

        void OnSceneGUI()
        {
            surface = target as CustomSurfaces;
            
            if (surface == null)
                return;

            if (!(surface.edit || surface.view))
                return;
            
            surface.Output();
            DrawLines();

            if (!surface.edit)
                return;
            
            DrawHandles();
            DrawAddButtons();
            DrawRemoveButtons();
        }


        void DrawLines()
        {
            foreach (Surfaces a in surface.surfaces)
            foreach (Line2 line in a.GetSurfacesWorld())
            {
                Handles.color = Color.white;
                Handles.DrawLine(line.start, line.end);
            }
        }
        void DrawHandles()
        {
            foreach (ComposedSurfaces composedSurfaces in surface.pointsArray)
            foreach (Composed_Vector2 point in composedSurfaces.points)
                point.Offset(Round(MyHandles.DrawHandle(surface.transform, point.Value, Color.blue)));
        }
        void DrawAddButtons()
        {
            foreach (ComposedSurfaces a in surface.pointsArray)
            {
                for (int i = 0; i < a.points.Length + (a.loop? 0 : -1); i++)
                {
                    Vector2 p0 = a.points[i].Value;
                    Vector2 p1 = a.points[(i + 1) % a.points.Length].Value;

                    Vector2 position = Vector2.Lerp(p0, p1, 0.5f);

                    float distance = Vector2.Distance(p0, p1);

                    bool show = distance > surface.gridSize && distance > AddButtonSize * HandleUtility.GetHandleSize(Vector2.zero);
                    if (!show)
                        continue;

                    if (MyHandles.DrawButton(surface.transform, position, AddButtonSize, Color.green))
                    {
                        List<Composed_Vector2> list = a.points.ToList();
                        list.Insert((i + 1) % a.points.Length, new Composed_Vector2(Round(position)));
                        a.points = list.ToArray();
                    }
                }
            }
        }
        void DrawRemoveButtons()
        {
            foreach (ComposedSurfaces a in surface.pointsArray)
            {
                if (a.points.Length <= 2)
                    return;

                Event e = Event.current;

                if (e.type == EventType.MouseDown && e.button == 1)
                {
                    Vector2 mousePos = surface.transform.InverseTransformPoint(HandleUtility.GUIPointToWorldRay(e.mousePosition).origin);
                    float range = HandleUtility.GetHandleSize(Vector2.zero) * HandleSize * 1.5f;

                    foreach (Composed_Vector2 point in a.points)
                        if (Vector2.Distance(point.Value, mousePos) <= range)
                        {
                            List<Composed_Vector2> list = a.points.ToList();
                            list.Remove(point);
                            a.points = list.ToArray();
                            break;
                        }
                }   
            }
        }


        Vector2 Round(Vector2 a)
        {
            return surface.gridSize != 0 ? new Vector2(Mathf.Round(a.x / surface.gridSize) * surface.gridSize, Mathf.Round(a.y / surface.gridSize) * surface.gridSize) : a;
        }
    }
}
#endif