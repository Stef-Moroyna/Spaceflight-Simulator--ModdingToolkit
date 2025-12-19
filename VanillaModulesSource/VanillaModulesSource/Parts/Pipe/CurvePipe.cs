using System.Collections.Generic;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class CurvePipe : PipeData, I_InitializePartModule
    {
        public List<Composed_Vector2> points;
        public int[] pointCount;
        public Composed_Vector2 scale = new Composed_Vector2(Vector2.one);

        [BoxGroup("edit", false), HorizontalGroup("edit/a")] public bool edit = true, view = true;
        [BoxGroup("edit", false), ShowIf("edit")] public float gridSize = 0.1f;
        
        int I_InitializePartModule.Priority => 10;
        void I_InitializePartModule.Initialize()
        {
            foreach (Composed_Vector2 point in points)
                point.OnChange += Output;

            scale.OnChange += Output;
                
            Output();
        }

        public override void Output()
        {
            Pipe output = new Pipe();
            
            foreach (Vector2 point in GetPoints())
                output.AddPoint(new Vector2(0, point.y), new Vector2(point.x * 2, 0));
            
            SetData(output);
        }
        public Vector2[] GetPoints()
        {
            List<Vector2> _points = new List<Vector2>();
            Vector2 _scale = scale.Value;

            for (int ii = 0; ii < points.Count - 1; ii += 2)
            {
                Vector2[] verts = { points[ii].Value, points[ii + 1].Value, points[ii + 2].Value };

                int _pointCount = pointCount[ii / 2];
                for (int i = ii == 0? 0 : 1; i < _pointCount; i++)
                {
                    float t = i / (float)(_pointCount - 1);

                    _points.Add(Reduce(verts)[0] * _scale);
                    Vector2[] Reduce(Vector2[] input)
                    {
                        while (input.Length > 1)
                        {
                            Vector2[] temp = new Vector2[input.Length - 1];
                    
                            for (int j = 0; j < input.Length - 1; j++)
                                temp[j] = Vector2.Lerp(input[j], input[j + 1], t);

                            input = temp;
                        }
                        return input;
                    }
                }
            }

            return _points.ToArray();
        }
    }
}