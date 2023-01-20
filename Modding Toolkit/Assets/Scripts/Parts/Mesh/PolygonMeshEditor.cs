#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(PolygonMesh))]
    public class PolygonMeshEditor : OdinEditor
    {
        PolygonMesh mesh;

        void OnSceneGUI()
        {
            mesh = target as PolygonMesh;

            if (mesh != null && mesh.polygonModule != null)
            {
                switch (mesh.UV_Mode)
                {
                    case PolygonMesh.UVOptions.Auto:
                        Vector2 min = new Vector2(mesh.polygonModule.polygon.vertices.Select(a => a.x).Min(), mesh.polygonModule.polygon.vertices.Select(a => a.y).Min());
                        Vector2 max = new Vector2(mesh.polygonModule.polygon.vertices.Select(a => a.x).Max(), mesh.polygonModule.polygon.vertices.Select(a => a.y).Max());
                        mesh.bounds = new [] { new Vector2(min.x, min.y), new Vector2(min.x, max.y), new Vector2(max.x, max.y), new Vector2(max.x, min.y) };
                        break;
                    
                    case PolygonMesh.UVOptions.Two_Points:
                        DrawBounds();
                        DrawTwoHandles();
                        break;
                    
                    case PolygonMesh.UVOptions.Four_Points:
                        DrawBounds();
                        DrawFourHandles();
                        break;
                }
            }
        }

        // Draws texture bounds
        void DrawBounds()
        {
            Vector2[] bounds = mesh.bounds;
            
            MyHandles.DrawLine(mesh.transform, bounds[0], bounds[1]);
            MyHandles.DrawLine(mesh.transform, bounds[1], bounds[2]);
            MyHandles.DrawLine(mesh.transform, bounds[2], bounds[3]);
            MyHandles.DrawLine(mesh.transform, bounds[3], bounds[0]);
        }
        void DrawTwoHandles()
        {
            Vector2[] bounds = mesh.bounds;

            bounds[0] += Round(MyHandles.DrawHandle(mesh.transform, bounds[0], Color.red), mesh.grid);
            bounds[2] += Round(MyHandles.DrawHandle(mesh.transform, bounds[2], Color.red), mesh.grid);

            bounds[1] = new Vector2(bounds[0].x, bounds[2].y);
            bounds[3] = new Vector2(bounds[2].x, bounds[0].y);

            mesh.bounds = bounds;
        }
        void DrawFourHandles()
        {
            Vector2[] bounds = mesh.bounds;

            bounds[0] += Round(MyHandles.DrawHandle(mesh.transform, bounds[0], Color.red), mesh.grid);
            bounds[1] += Round(MyHandles.DrawHandle(mesh.transform, bounds[1], Color.red), mesh.grid);
            bounds[2] += Round(MyHandles.DrawHandle(mesh.transform, bounds[2], Color.red), mesh.grid);
            bounds[3] += Round(MyHandles.DrawHandle(mesh.transform, bounds[3], Color.red), mesh.grid);

            mesh.bounds = bounds;
        }

        // Utility
        Vector2 Round(Vector2 a, float value)
        {
            return value != 0 ? a.Round(value) : a;
        }
    }
}
#endif