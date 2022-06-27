using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public abstract class PipeData : PolygonData
    {
        [BoxGroup("Depth", false)] public float depthMultiplier = 1;
        [Range(-1, 1)] public float cut;
        public bool reduceResolution = true;

        // Data
        public Pipe pipe;
        
        
        protected void SetData(Pipe pipe)
        {
            this.pipe = pipe;

            List<PipePoint> shapePoints = pipe.points;
            Vector2[] points = new Vector2[shapePoints.Count * 2];

            for (int i = 0; i < shapePoints.Count; i++)
            {
                points[i] = shapePoints[i].GetPosition(Mathf.Clamp(cut - 1, -1, 0));
                points[points.Length - 1 - i] = shapePoints[i].GetPosition(Mathf.Clamp(cut + 1, 0, 1));
            }

            if (reduceResolution)
                SetData(new Polygon(points), new Polygon(ToFastPoints(points, 0.05f)));
            else
                SetData(new Polygon(points));
        }
        
        // Resolution reduction
        static Vector2[] ToFastPoints(Vector2[] points, float tolerance)
        {
            if (points.Length < 3)
                return points;
            
            int lastPoint = points.Length - 1;

            // Add the first and last index to the keepers
            List<int> pointIndexesToKeep = new List<int> { 0, lastPoint };

            // The first and the last point cannot be the same
            while (points[0] == points[lastPoint])
                lastPoint--;

            ToFastPoints(points, 0, lastPoint, tolerance, ref pointIndexesToKeep);
            pointIndexesToKeep.Sort();

            Vector2[] output = new Vector2[pointIndexesToKeep.Count];

            for (int i = 0; i < output.Length; i++)
                output[i] = points[pointIndexesToKeep[i]];

            return output;
        }
        static void ToFastPoints(Vector2[] points, int firstPoint, int lastPoint, float tolerance, ref List<int> pointIndexesToKeep)
        {
            float maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                float distance = GetDistanceSqrt(points[firstPoint], points[lastPoint], points[index]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                // Add the largest point that exceeds the tolerance
                pointIndexesToKeep.Add(indexFarthest);
                ToFastPoints(points, firstPoint, indexFarthest, tolerance, ref pointIndexesToKeep);
                ToFastPoints(points, indexFarthest, lastPoint, tolerance, ref pointIndexesToKeep);
            }
        }
        static float GetDistanceSqrt(Vector2 point_A, Vector2 point_B, Vector2 point)
        {
            Vector2 line = point_B - point_A;

            // Calculate the t that minimizes the distance
            float t = ((point.x - point_A.x) * line.x + (point.y - point_A.y) * line.y) / (line.x * line.x + line.y * line.y);

            if (t < 0)
                return (point - point_A).sqrMagnitude;

            if (t > 1)
                return (point - point_B).sqrMagnitude;

            return (point - (point_A + line * t)).sqrMagnitude;
        }


        public override void Raycast(Vector2 point, out float depth)
        {
            for (int i = 0; i < pipe.points.Count - 1; i++)
                if (Depth(pipe.points[i], pipe.points[i + 1], point, out depth))
                {
                    depth = baseDepth + depth * depthMultiplier;
                    return;
                }

            depth = 0f;
        }
        static bool Depth(PipePoint p0, PipePoint p1, Vector2 point, out float depth)
        {
            return Depth(p1.position, p1.width, p0.position, p0.width, point, out depth);
        }
        static bool Depth(Vector2 bottom, Vector2 bottomWidth, Vector2 top, Vector2 topWidth, Vector2 point, out float depth)
        {
            depth = 0f;

            Vector2 bottomLeft = bottom - bottomWidth / 2;
            Vector2 topLeft = top - topWidth / 2;
            Vector2 topRight = top + topWidth / 2;
            Vector2 bottomRight = bottom + bottomWidth / 2;

            if (new Polygon(bottomLeft, topLeft, topRight, bottomRight).convexPolygons[0].GetDistanceToPolygon(point) > 0)
                return false;
            
            Vector2 uv = UV_Utility.Get_UV_InQuad(point, bottomLeft, topLeft, topRight, bottomRight);
            uv.x = uv.x > 0.5f ? (1 - uv.x) * 2 : uv.x * 2;
            depth = Mathf.Lerp(0, Mathf.Lerp(bottomWidth.x / 2, topWidth.x / 2, 1 - uv.y), uv.x);
            return true;
        }
    }

    // Output
    public class Pipe
    {
        public List<PipePoint> points;

        public Pipe()
        {
            points = new List<PipePoint>();
        }

        public void AddPoint_SideAnchor(Vector2 position, Vector2 width)
        {
            AddPoint(position - width / 2, width);
        }
        public void AddPoint(Vector2 position, Vector2 width)
        {
            points.Add(new PipePoint(position, width, points.Count > 0 ? points.Last().height + (points.Last().position - position).magnitude : 0));
        }

        // Get
        public Vector2 GetWidthAtHeight(float height)
        {
            for (int i = 0; i < points.Count - 1; i++)
                if (points[i + 1].height > height)
                    return PipePoint.LerpByHeight(points[i], points[i + 1], height).width;
        
            return points.Last().width;
        }
    }
    public class PipePoint
    {
        public Vector2 position;
        public Vector2 width;
        public float height;
    
        public Vector2 Left => position - width / 2;
        public Vector2 Right => position + width / 2;

        public PipePoint(Vector2 position, Vector2 width, float height)
        {
            this.position = position;
            this.width = width;
            this.height = height;
        }

        public Vector2 GetPosition(float t)
        {
            return position + width * (t * 0.5f);
        }

        public static PipePoint Lerp(PipePoint a, PipePoint b, float t)
        {
            return new PipePoint(Vector2.Lerp(a.position, b.position, t), Vector2.Lerp(a.width, b.width, t), Mathf.Lerp(a.height, b.height, t));
        }

        public static PipePoint LerpByHeight(PipePoint a, PipePoint b, float height)
        {
            return Lerp(a, b, Mathf.InverseLerp(a.height, b.height, height));
        }
    }
}