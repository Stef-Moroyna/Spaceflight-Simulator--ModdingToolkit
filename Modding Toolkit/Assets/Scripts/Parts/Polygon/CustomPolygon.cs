using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class CustomPolygon : PolygonData, I_InitializePartModule
    {
        public List<Vector2> polygonVertices = new List<Vector2>() { Vector2.zero, Vector2.up, Vector2.right };

        int I_InitializePartModule.Priority => 10;
        void I_InitializePartModule.Initialize() => Output();

        public override void Output()
        {
            SetData(new Polygon(polygonVertices.ToArray()));
        }

        #if UNITY_EDITOR
        [BoxGroup("edit", false), HorizontalGroup("edit/h")] public bool edit, view;
        [BoxGroup("edit", false), ShowIf("edit")] public float gridSize = 0.1f;

        public List<Line2> GetLines(bool worldSpace)
        {
            List<Line2> output = new List<Line2>();

            for (int i = 0; i < polygonVertices.Count - 1; i++)
                output.Add(GetLine(polygonVertices[i], polygonVertices[i + 1], worldSpace));

            output.Add(GetLine(polygonVertices[polygonVertices.Count - 1], polygonVertices[0], worldSpace));

            return output;
        }
        Line2 GetLine(Vector2 p1, Vector2 p2, bool worldSpace)
        {
            return worldSpace ? new Line2(transform.TransformPoint(p1), transform.TransformPoint(p2)) : new Line2(p1, p2);
        }
        
        [Button] void Reverse() => polygonVertices.Reverse();
        #endif
    }
}