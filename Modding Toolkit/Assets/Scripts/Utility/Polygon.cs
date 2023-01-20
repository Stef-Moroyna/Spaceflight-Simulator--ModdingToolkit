using UnityEngine;

public class Polygon
{
    public readonly ConvexPolygon[] convexPolygons;
    public readonly Vector2[] vertices;

    public Polygon(params Vector2[] vertices)
    {
        this.vertices = vertices;
        convexPolygons = PolygonPartioner.Partion(vertices).ToArray();
    }


    // Polygons - Polygons
    public static bool Intersect(ConvexPolygon[] A, ConvexPolygon[] B, float overlapThreshold)
    {
        foreach (ConvexPolygon convexPolygon_A in A)
            foreach (ConvexPolygon convexPolygon_B in B)
            {
                //#warning FUTURE / Find the source of the problem
                if (convexPolygon_A.points.Length == 0 || convexPolygon_B.points.Length == 0)
                {
                    Debug.LogError("Length 0");
                    continue;   
                }

                if (ConvexPolygon.Intersect(convexPolygon_A, convexPolygon_B, overlapThreshold))
                    return true;
            }

        return false;
    }
    // Line - Polygons
    public static bool Intersect(Vector2 position, Vector2 ray, ConvexPolygon[] B, float overlapThreshold)
    {
        foreach (ConvexPolygon convexPolygon in B)
            if (convexPolygon.Intersects(position, position + ray, overlapThreshold))
                return true;

        return false;
    }
    // Point - Polygons
    public static float GetDistanceToPolygons(Vector2 point, params ConvexPolygon[] polygons)
    {
        float distance = float.PositiveInfinity;

        foreach (ConvexPolygon convexPolygon in polygons)
        {
            float newDistance = convexPolygon.GetDistanceToPolygon(point);

            if (newDistance < distance)
                distance = newDistance;
        }

        return distance;
    }

    
    // Get
    public Vector2[] GetVerticesWorld(Transform transform)
    {
        Vector2[] output = new Vector2[vertices.Length];
        for (int i = 0; i < output.Length; i++)
            output[i] = transform.TransformPoint(vertices[i]);

        return output;
    }
    public ConvexPolygon[] GetConvexPolygonsWorld(Transform transform)
    {
        ConvexPolygon[] output = new ConvexPolygon[convexPolygons.Length];

        for (int polygonIndex = 0; polygonIndex < output.Length; polygonIndex++)
        {
            ConvexPolygon polygon = convexPolygons[polygonIndex];

            Vector2[] points = new Vector2[polygon.points.Length];
            for (int i = 0; i < points.Length; i++)
                points[i] = transform.TransformPoint(polygon.points[i]);

            output[polygonIndex] = new ConvexPolygon(points);
        }

        return output;
    }
}

public class ConvexPolygon
{
    public readonly Vector2[] points;
    public readonly Vector2[] surfaces;

    public ConvexPolygon(Vector2[] points)
    {
        this.points = points;

        surfaces = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
            surfaces[i] = points[i] - points[(i + 1) % points.Length];
    }


    // Polygon 
    public static bool Intersect(ConvexPolygon A, ConvexPolygon B, float overlapThreshold)
    {
        // Loop through all the edges of A
        foreach (Vector2 surface_A in A.surfaces)
            if (Intersect(surface_A, A, B, overlapThreshold))
                return false;

        // Loop through all the edges of B
        foreach (Vector2 surface_B in B.surfaces)
            if (Intersect(surface_B, A, B, overlapThreshold))
                return false;

        return true;
    }
    static bool Intersect(Vector2 edge, ConvexPolygon A, ConvexPolygon B, float overlapThreshold)
    {
        // Find the axis perpendicular to the current edge
        Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

        // Find the projection of the polygon on the current axis
        A.ProjectPolygon(axis, out float minA, out float maxA);
        B.ProjectPolygon(axis, out float minB, out float maxB);

        // Checks if axis does not intersect
        return IntervalDistance(minA, maxA, minB, maxB) > overlapThreshold;
    }
    void ProjectPolygon(Vector2 axis, out float min, out float max)
    {
        min = max = Vector2.Dot(points[0], axis);

        foreach (Vector2 point in points)
        {
            // Projects point to axis
            float dot = Vector2.Dot(point, axis);

            if (dot < min)
                min = dot;
            else if (dot > max)
                max = dot;
        }
    }

    // Line
    public bool Intersects(Vector2 point_A, Vector2 point_B, float overlapThreshold)
    {
        // Loop through all the edges
        for (int edgeIndex = 0; edgeIndex < surfaces.Length; edgeIndex++)
            if (Intersects(surfaces[edgeIndex], point_A, point_B, overlapThreshold))
                return false;

        // Checks surface edge
        if (Intersects(point_A - point_B, point_A, point_B, overlapThreshold))
            return false;

        return true;
    }
    bool Intersects(Vector2 edge, Vector2 point_A, Vector2 point_B, float overlapThreshold)
    {
        // Find the axis perpendicular to the current edge
        Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

        // Find the projection of the polygon on the current axis
        ProjectPolygon(axis, out float minA, out float maxA);
        ProjectSurface(point_A, point_B, axis, out float minB, out float maxB);

        // Checks if axis does not intersect
        return IntervalDistance(minA, maxA, minB, maxB) > overlapThreshold;
    }
    static void ProjectSurface(Vector2 point_A, Vector2 point_B, Vector2 axis, out float min, out float max)
    {
        // Projects points to axis
        float dot_A = Vector2.Dot(point_A, axis);
        float dot_B = Vector2.Dot(point_B, axis);
        min = Mathf.Min(dot_A, dot_B);
        max = Mathf.Max(dot_A, dot_B);
    }

    // Point
    public float GetDistanceToPolygon(Vector2 point)
    {
        if (surfaces.Length == 0)
            return float.PositiveInfinity;

        float distance = float.NegativeInfinity;

        // Loop through edges
        foreach (Vector2 surface in surfaces)
        {
            // Find the axis perpendicular to the current edge
            Vector2 axis = new Vector2(-surface.y, surface.x).normalized;

            // Find the projection of the polygon on the current axis
            ProjectPolygon(axis, out float minB, out float maxB);
            float projectedPoint = Vector2.Dot(point, axis);

            // Distance
            float newDistance = Mathf.Max(minB - projectedPoint, projectedPoint - maxB);

            if (newDistance > distance)
                distance = newDistance;
        }

        return distance;
    }


    // Calculate the distance between minA/maxA and minB/maxB // The distance will be negative if the intervals overlap
    static float IntervalDistance(float minA, float maxA, float minB, float maxB)
    {
        if (minA < minB)
            return minB - maxA;
        else
            return minA - maxB;
    }
}