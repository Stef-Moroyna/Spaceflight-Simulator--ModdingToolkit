#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(ModelPolygon))]
    public class ModelPolygonEditor : OdinEditor
    {
        ModelPolygon modelPolygon;

        void OnSceneGUI()
        {
            modelPolygon = target as ModelPolygon;

            if (modelPolygon != null && modelPolygon.isActiveAndEnabled)
            {
                if (modelPolygon.view || modelPolygon.edit)
                    DrawLines(modelPolygon.points, true, modelPolygon.transform, Color.white);
                
                if (modelPolygon.edit)
                    DrawAddButtons(modelPolygon.meshes, modelPolygon.points, modelPolygon.transform);
            }
        }

        public static void DrawAddButtons(MeshFilter[] meshes, List<Vector2> points, Transform transform)
        {
            foreach (MeshFilter meshFilter in meshes)
            foreach (Vector3 vertice in meshFilter.sharedMesh.vertices)
                if (meshFilter.transform.TransformPoint(vertice).z > -0.01f)
                    if (MyHandles.DrawButton(meshFilter.transform, vertice, 0.1f, Color.green))
                        points.Add(transform.InverseTransformPoint(meshFilter.transform.TransformPoint(vertice)));
        }
        public static void DrawLines(List<Vector2> points, bool loop, Transform transform, Color c)
        {
            for (int i = 0; i < points.Count - (loop ? 0 : 1); i++)
            {
                Handles.color = c;
                Handles.DrawLine(transform.TransformPoint(points[i]), transform.TransformPoint(points[(i + 1) % points.Count]));
            }
        }
    }
}
#endif