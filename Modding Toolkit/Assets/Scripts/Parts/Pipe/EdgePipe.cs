using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class EdgePipe : PipeData, I_InitializePartModule
    {
        [Required] public SurfaceData surfaceData;
        public float width = 1;

        public bool flatStart, flatEnd;
        
        int I_InitializePartModule.Priority => 8;
        void I_InitializePartModule.Initialize() => surfaceData.onChange += Output;

        public override void Output()
        {
            if (!Application.isPlaying)
                surfaceData.Output();
            if (surfaceData.surfaces == null)
                surfaceData.Output();
            
            
            Vector2[] points = surfaceData.surfaces[0].points;
            Vector2[] normals = new Vector2[points.Length - 1];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = (points[i + 1] - points[i]).normalized;
            
            Pipe output = new Pipe();
            output.AddPoint_SideAnchor(points[0], flatStart? A(points[0], Vector2.right, normals[0]) : normals[0] * width);
            
            for (int i = 1; i < points.Length - 1; i++)
                output.AddPoint_SideAnchor(points[i], GetWidth(points[i], normals[i - 1], normals[i]));

            output.AddPoint_SideAnchor(points.Last(), flatEnd? A(points.Last(), Vector2.left, normals.Last()) : normals.Last() * width);
            SetData(output);
            
            
            Vector2 GetWidth(Vector2 p, Vector2 normal_A, Vector2 normal_B)
            {
                Vector2 a = p + normal_A * width;
                Vector2 b = p + normal_B * width;
                Vector2 widthPoint = Math_Utility.GetLineIntersection(a, a + normal_A, b, b + normal_B, out bool parallel);
                return (parallel? a : widthPoint) - p;
            }

            Vector2 A(Vector2 p, Vector2 line, Vector2 normal_A)
            {
                Vector2 a = p + normal_A * width;
                Vector2 widthPoint = Math_Utility.GetLineIntersection(p, p + line, a, a + normal_A, out bool parallel);
                return (parallel? a : widthPoint) - p;
            }
        }
    }
}