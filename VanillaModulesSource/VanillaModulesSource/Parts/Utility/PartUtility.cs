using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFS.Parts.Modules;
using SFS.Platform;


namespace SFS.Parts
{
    public static class Part_Utility
    {
        // Point casting
        public static bool RaycastParts(Part[] parts, Vector2 worldPoint, float threshold, out PartHit hit)
        {
            if (PlatformManager.current == PlatformType.PC)
                threshold *= 0.1f;
            
            // Get parts in range
            PartHit[] hits = parts.SelectMany(part => part.GetClickPolygons().Where(polygonData => Polygon.GetDistanceToPolygons(worldPoint, polygonData.polygon.GetConvexPolygonsWorld(polygonData.transform)) <= threshold).Select(x => new PartHit(part, x))).ToArray();
            
            if (hits.Length > 0)
            {
                if (hits.Length == 1)
                {
                    hit = hits[0];
                    return true;
                }

                // Get parts with highest depth value
                hits = GetHighestDepthAtPoint(worldPoint, hits, threshold);

                if (hits.Length == 1)
                {
                    hit = hits[0];
                    return true;
                }

                // Get closest part
                GetClosestPartToClick(worldPoint, hits, out float bestDistance, out PartHit bestHit);

                if (bestDistance < threshold)
                {
                    hit = bestHit;
                    return true;
                }
            }

            hit = null;
            return false;
        }
        static PartHit[] GetHighestDepthAtPoint(Vector2 worldPoint, PartHit[] hits, float threshold)
        {
            List<PartHit> highestDepthPolygons = new List<PartHit>();

            float bestDepth = float.NegativeInfinity;

            foreach (PartHit hit in hits)
            {
                foreach (PolygonData shape in hit.part.GetModules<PolygonData>())
                {
                    if (!hit.polygon.Click)
                        continue;
                    if (Polygon.GetDistanceToPolygons(worldPoint, shape.polygon.GetConvexPolygonsWorld(shape.transform)) > threshold)
                        continue;
                    
                    if (!hit.polygon.Raycast(shape, shape.transform.InverseTransformPoint(worldPoint), out float depth))
                        depth = float.NegativeInfinity;

                    if (depth < bestDepth)
                        continue;

                    if (depth > bestDepth)
                        highestDepthPolygons.Clear();

                    highestDepthPolygons.Add(hit);

                    bestDepth = depth;
                }
            }

            return highestDepthPolygons.ToArray();
        }
        public static void GetClosestPartToClick(Vector2 point, PartHit[] hits, out float bestDistance, out PartHit bestHit)
        {
            bestDistance = float.PositiveInfinity;
            bestHit = null;

            foreach (PartHit hit in hits)
            {
                float newDistance = Polygon.GetDistanceToPolygons(point, hit.polygon.polygon.GetConvexPolygonsWorld(hit.polygon.transform));

                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    bestHit = hit;
                }
            }
        }


        // Collider intersection
        public static bool CollidersIntersect(Part part_A, Part part_B)
        {
            // Is on same layer and intersects
            (ConvexPolygon[] polygons_A, bool isFront_A) = part_A.GetBuildColliderPolygons();
            (ConvexPolygon[] polygons_B, bool isFront_B) = part_B.GetBuildColliderPolygons();
            return isFront_A == isFront_B && Polygon.Intersect(polygons_A, polygons_B, -0.08f);
        }


        // Get collider
        public static (ConvexPolygon[] normal, ConvexPolygon[] front) GetBuildColliderPolygons(Part[] parts)
        {
            List<ConvexPolygon> polygons = new List<ConvexPolygon>();
            List<ConvexPolygon> polygons_Front = new List<ConvexPolygon>();

            foreach (Part part in parts)
            {
                (ConvexPolygon[] a, bool isFront) = part.GetBuildColliderPolygons();
                (isFront? polygons_Front : polygons).AddRange(a);
            }

            return (polygons.ToArray(), polygons_Front.ToArray());
        }
        public static (Part part, (ConvexPolygon[], bool isFront))[] GetBuildColliderPolygons_WithPart(Part[] parts)
        {
            return parts.Select(part => (part, part.GetBuildColliderPolygons())).ToArray();
        }


        // Part position
        public static void PositionParts(Vector2 position, Vector2 pivot, bool round, bool useLaunchBounds, params Part[] parts)
        {
            if (!GetBuildColliderBounds_WorldSpace(out Rect rect, useLaunchBounds, parts))
                return;

            Vector2 currentPosition = rect.position + rect.size * pivot;
            Vector2 offset = position - currentPosition;

            OffsetPartPosition(offset, round, parts);
        }
        public static void OffsetPartPosition(Vector2 offset, bool round, params Part[] parts)
        {
            if (offset.magnitude < 0.001f)
                return;
            
            if (round)
                offset = offset.Round(0.5f);

            foreach (Part part in parts)
                part.transform.localPosition += (Vector3)offset;
        }
        public static void CenterParts(Part[] parts, Vector2 boxSize)
        {
            if (!GetBuildColliderBounds_WorldSpace(out Rect partBounds, true, parts))
                return;
            
            Vector2 offset = (boxSize / 2) - partBounds.center;
            OffsetPartPosition(offset, true, parts);
        }

        // Orientation
        public static void ApplyOrientationChange(Orientation change, Vector2 pivot, IEnumerable<Part> parts)
        {
            foreach (Part part in parts)
            {
                part.orientation.orientation.Value += part.orientation.orientation.Value.InversedAxis() ? new Orientation(change.y, change.x, change.z) : change;
                part.transform.localPosition = ((Vector2)part.transform.localPosition - pivot) * change + pivot;
                part.RegenerateMesh();
            }
        }

        // Framing bounds
        public static bool GetFramingBounds_WorldSpace(out Rect bounds, Part[] parts)
        {
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;
        
            foreach (Part part in parts)
                if (GetFramingBounds_WorldSpace(out Rect bound, part))
                    ExpandToFitPoint(ref min, ref max, bound.min, bound.max);
            
            bounds = new Rect(min, max - min);
            return !Double.IsPositiveInfinity(min.x);
        }
        public static bool GetFramingBounds_WorldSpace(out Rect bounds, Part part)
        {
            // Overwrites framing
            if (part.HasModule<FramingOverwrite>())
            {
                bounds = part.GetModules<FramingOverwrite>()[0].GetBounds_WorldSpace();
                return true;
            }
            
            // Get bounds using build colliders and framing collider bounds
            return GetBounds_WorldSpace(out bounds, part.GetModules<PolygonData>().Where(x => x.BuildCollider || x.Click)
                .Concat(part.GetModules<FramingColliderBounds>().Select(bounds => bounds.shape)).ToArray());
        }
        
        // Collider bounds
        public static bool GetBuildColliderBounds_WorldSpace(out Rect bounds, bool useLaunchBounds, params Part[] parts)
        {
            return GetBounds_WorldSpace(out bounds, x => x.BuildCollider || x.Click, useLaunchBounds, false, parts);
        }

        public static bool GetBounds_WorldSpace(out Rect bounds, IEnumerable<PolygonData> polygons)
        {
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;
            
            polygons.ForEach(poly => ExpandToFitPoint(ref min, ref max, poly.polygon.GetVerticesWorld(poly.transform)));
            
            bounds = new Rect(min, max - min);
            return !double.IsPositiveInfinity(min.x);
        }
        
        public static bool GetBounds_WorldSpace(out Rect bounds, Func<PolygonData, bool> usePolygon, bool useLaunchBounds, bool useFramingBounds, params Part[] parts)
        {
            List<PolygonData> polygons = GetModules<PolygonData>(parts).Where(usePolygon).ToList();
        
            if (useLaunchBounds)
                polygons.AddRange(GetModules<LaunchColliderBounds>(parts).Select(x => x.shape));
            
            if (useFramingBounds)
                polygons.AddRange(GetModules<FramingColliderBounds>(parts).Select(x => x.shape));
            
            return GetBounds_WorldSpace(out bounds, polygons);
        }

        public static void ExpandToFitPoint(ref Vector2 min, ref Vector2 max, params Vector2[] points)
        {
            foreach (Vector2 point in points)
            {
                if (point.x < min.x)
                    min.x = point.x;
                if (point.y < min.y)
                    min.y = point.y;
                
                if (point.x > max.x)
                    max.x = point.x;
                if (point.y > max.y)
                    max.y = point.y;   
            }
        }
        
        public static IEnumerable<T> GetModules<T>(IEnumerable<Part> parts) => parts.SelectMany(x => x.GetModules<T>());
    }

    public class PartHit
    {
        public Part part;
        public PolygonData polygon;
        
        public PartHit(Part part, PolygonData polygon)
        {
            this.part = part;
            this.polygon = polygon;
        }
    }
}