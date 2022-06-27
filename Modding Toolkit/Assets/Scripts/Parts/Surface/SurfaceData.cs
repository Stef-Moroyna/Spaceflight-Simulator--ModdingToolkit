using System;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using SFS.World.Drag;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public abstract class SurfaceData : MonoBehaviour
    {
        [SerializeField, BoxGroup("", false)] bool attachmentSurfaces = true; // Is the surface used for attachment
        [SerializeField, BoxGroup("", false)] bool dragSurfaces = true; // Is the surface used for drag


        // Data
        public List<Surfaces> surfaces;
        public List<Surfaces> surfacesFast;
        [NonSerialized] public Event_Local onChange = new Event_Local();


        public bool Attachment => attachmentSurfaces && isActiveAndEnabled;
        public bool Drag => dragSurfaces && isActiveAndEnabled;


        public abstract void Output();
        protected void SetData(List<Surfaces> surfaces) => SetData(surfaces, surfaces);
        protected void SetData(List<Surfaces> surfaces, List<Surfaces> surfacesFast)
        {
            this.surfaces = surfaces;
            this.surfacesFast = surfacesFast;
            onChange.Invoke();
        }
        
        
        // Heat
        [NonSerialized] public I_HeatModule heatModule;
        protected void Start() => heatModule = transform.GetComponentInParentTree<I_HeatModule>();
    }

    public class Surfaces
    {
        public readonly Vector2[] points;
        public readonly Transform owner;
        public readonly bool loop;

        // Constructor
        public Surfaces(Vector2[] points, bool loop, Transform owner)
        {
            this.owner = owner;
            this.points = points;
            this.loop = loop;
        }

        public Line2[] GetSurfacesWorld()
        {
            if (points.Length < 2)
                return new Line2[]{};
            
            // Converts to world
            Vector2[] pointsWorld = points.Select(p => (Vector2)owner.TransformPoint(p)).ToArray();

            // Creates surfaces from points
            Line2[] surfaces = new Line2[loop? pointsWorld.Length : pointsWorld.Length - 1];
            for (int i = 0; i < pointsWorld.Length - 1; i++)
                surfaces[i] = new Line2(pointsWorld[i], pointsWorld[i + 1]);

            // Closes loop
            if (loop)
                surfaces[surfaces.Length - 1] = new Line2(pointsWorld[pointsWorld.Length - 1], pointsWorld[0]);

            return surfaces;
        }
    }
    
    // Utility
    public static class SurfaceUtility
    {
        const float MaxDistance = 0.05f;
        const float MinOverlap = 0.05f;
        const int MaxAngleDiff = 5;
    
        // Connection
        public static bool SurfacesConnect(Part A, Part B, out float overlap, out Vector2 center)
        {
            return SurfacesConnect(A.GetAttachmentSurfacesWorld(), B.GetAttachmentSurfacesWorld(), out overlap, out center);
        }
        public static bool SurfacesConnect(Part A, SurfaceData B, out float overlap, out Vector2 center)
        {
            List<Line2> surfaces = new List<Line2>();

            foreach (Surfaces chain in B.surfaces)
                surfaces.AddRange(chain.GetSurfacesWorld());
        
            return SurfacesConnect(surfaces.ToArray(), A.GetAttachmentSurfacesWorld(), out overlap, out center);
        }
        public static bool SurfacesConnect(Part A, Line2[] B, out float overlap, out Vector2 center)
        {
            return SurfacesConnect(A.GetAttachmentSurfacesWorld(), B, out overlap, out center);
        }
        //
        public static bool SurfacesConnect(SurfaceData A, SurfaceData B, out float overlap, out Vector2 center)
        {
            return SurfacesConnect(A.surfaces.Select(a => a.GetSurfacesWorld()).Collapse().ToArray(), B.surfaces.Select(b => b.GetSurfacesWorld()).Collapse().ToArray(), out overlap, out center);
        }
        public static bool SurfacesConnect(Line2[] surfaces_A, Line2[] surfaces_B, out float overlap, out Vector2 center)
        {
            overlap = 0f;
            center = Vector2.zero;

            foreach (Line2 surface_A in surfaces_A)
                foreach (Line2 surface_B in surfaces_B)
                    if (SurfacesConnect(surface_A, surface_B, MaxAngleDiff, MaxDistance, MinOverlap, out overlap, out center))
                        return true;

            return false;
        }
        public static bool SurfacesConnect(Line2 A, Line2 B, float maxAngleDiff, float maxDistance, float minOverlap, out float overlap, out Vector2 center)
        {
            overlap = BiggestOverlap(A, B, out center);

            // Get the angle between the two lines
            float angleDiff = Vector2.Angle(A.end - A.start, B.end - B.start);

            // If angle is outside of threshold then surfaces dont connect
            if (angleDiff > maxAngleDiff && angleDiff < 180 - maxAngleDiff)
                return false;

            // If the lines don't intersect and the shortest distance between them is outside of threshold then surfaces dont connect
            if (!Intersect(A, B) && ShortestDistance(A, B) > maxDistance)
                return false;

            // If the biggest overlap between the two lines is smaller than the minimum required overlap then surfaces dont connect
            if (overlap < minOverlap)
                return false;

            // None of the checks rejected the lines so then they connect
            return true;
        }
        
        // Returns the biggest overlap (x or y axis) between two lines
        static float BiggestOverlap(Line2 A, Line2 B, out Vector2 center)
        {
            // Store biggest overlap value to check for higher values later
            float biggestOverlap = 0.0f;

            // Split line A and B into their x axis and y axis components and reorder their points
            Line xA = A.start.x < A.end.x ? new Line(A.start.x, A.end.x) : new Line(A.end.x, A.start.x);
            Line xB = B.start.x < B.end.x ? new Line(B.start.x, B.end.x) : new Line(B.end.x, B.start.x);
            Line yA = A.start.y < A.end.y ? new Line(A.start.y, A.end.y) : new Line(A.end.y, A.start.y);
            Line yB = B.start.y < B.end.y ? new Line(B.start.y, B.end.y) : new Line(B.end.y, B.start.y);

            // Get the potential x axis overlap by using the biggest start and smallest end
            float startX = Mathf.Max(xA.start, xB.start);
            float endX = Mathf.Min(xA.end, xB.end);

            // If the start is lower than the end then there's an overlap so we update the overlap value
            if (startX < endX)
                biggestOverlap = Mathf.Max(biggestOverlap, endX - startX);

            // Get the potential y axis overlap by using the biggest start and smallest end
            float startY = Mathf.Max(yA.start, yB.start);
            float endY = Mathf.Min(yA.end, yB.end);

            // If the start is lower than the end then there's an overlap so we update the overlap value
            if (startY < endY)
                biggestOverlap = Mathf.Max(biggestOverlap, endY - startY);

            center = new Vector2((startX + endX) * 0.5f, (startY + endY) * 0.5f);

            return biggestOverlap;
        }
        // Returns shortest distance between two lines
        static float ShortestDistance(Line2 A, Line2 B)
        {
            // Start with the highest possible value
            float shortestDist = float.MaxValue;

            // Check all the distance between each of the four points and the opposite segment and update shortest distance
            float dist = DistanceToSegment(A, B.start);

            if (dist < shortestDist)
                shortestDist = dist;

            dist = DistanceToSegment(A, B.end);

            if (dist < shortestDist)
                shortestDist = dist;

            dist = DistanceToSegment(B, A.start);

            if (dist < shortestDist)
                shortestDist = dist;

            dist = DistanceToSegment(B, A.end);

            if (dist < shortestDist)
                shortestDist = dist;

            return shortestDist;
        }
        // Returns the distance between a line segment and a point
        static float DistanceToSegment(Line2 segment, Vector2 point)
        {
            float a = point.x - segment.start.x;
            float b = point.y - segment.start.y;
            float c = segment.end.x - segment.start.x;
            float d = segment.end.y - segment.start.y;
            float dot = a * c + b * d;
            float lengthSquared = c * c + d * d;

            float param = lengthSquared != 0 ? dot / lengthSquared : -1;

            float xx, yy;

            if (param < 0)
            {
                xx = segment.start.x;
                yy = segment.start.y;
            }
            else if (param > 1)
            {
                xx = segment.end.x;
                yy = segment.end.y;
            }
            else
            {
                xx = segment.start.x + param * c;
                yy = segment.start.y + param * d;
            }

            var dx = point.x - xx;
            var dy = point.y - yy;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
        // Returns true if two lines intersect
        static bool Intersect(Line2 A, Line2 B)
        {
            int startBToA = Direction(A.start, A.end, B.start);
            int endBToA = Direction(A.start, A.end, B.end);
            int startAToB = Direction(B.start, B.end, A.start);
            int endAToB = Direction(B.start, B.end, A.end);

            if (startBToA != endBToA && startAToB != endAToB)
                return true; //they are intersecting

            if (startBToA == 0 && OnLine(A, B.start)) //when end of B intersects A
                return true;

            if (endBToA == 0 && OnLine(A, B.end)) //when start of B intersects A
                return true;

            if (startAToB == 0 && OnLine(B, A.start)) //when end of A intersects B
                return true;

            if (endAToB == 0 && OnLine(B, A.end)) //when start of A intersects B
                return true;

            return false;
        }
        // Returns the direction relation between a point and a line segment
        static int Direction(Vector2 start, Vector2 end, Vector2 point)
        {
            float val = (end.y - start.y) * (point.x - end.x) - (end.x - start.x) * (point.y - end.y);

            if (val == 0.0f)
                return 0;     // colinear
            if (val < 0.0f)
                return 2;    // anti-clockwise direction

            return 1;    // clockwise direction
        }
        // Returns true if point is on line
        static bool OnLine(Line2 line, Vector2 point)
        {
            return point.x <= Mathf.Max(line.start.x, line.end.x) && point.x <= Mathf.Min(line.start.x, line.end.x) && point.y <= Mathf.Max(line.start.y, line.end.y) && point.y <= Mathf.Min(line.start.y, line.end.y);
        }
    }
}