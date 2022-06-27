using System;
using UnityEngine;

public static class Math_Utility
{
    public static int GetFitsTime(double value, double a, double b)
    {
        return (int)((b - a) / value);
    }
    public static bool IsInsideRange(double value, double a, double b)
    {
        return value >= Math.Min(a, b) && value <= Math.Max(a, b);
    }

    public static bool InArea(Rect area, Vector2 position, float extra)
    {
        Vector2 min = area.min - (Vector2.one * extra);
        Vector2 max = area.max + (Vector2.one * extra);

        return (position.x > min.x && position.y > min.y) && (position.x < max.x && position.y < max.y);
    }

    public static double Floor_ForNavigation(double floor, double level, double a)
    {
        if (a < floor)
            return floor;

        return floor + Math.Floor((a - floor) / level) * level;
    }
    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }
    public static double Clamp01(double value)
    {
        if (value < 0)
            return 0;

        if (value > 1)
            return 1;

        return value;
    }
    public static double Lerp(double a, double b, double t)
    {
        return (1 - t) * a + t * b;
    }
    public static double InverseLerp(double a, double b, double value)
    {
        if (a == b)
            return 0;

        return (value - a) / (b - a);
    }


    public static float InverseLerpUnclamped(float a, float b, float value)
    {
        return (value - a) / (b - a);
    }
    
    
    // How much does A overreach area
    public static void GetRectOverreach(Rect A, Rect area, bool clampNegative, out Vector2 leftDown, out Vector2 rightUp)
    {
        leftDown = area.min - A.min;
        rightUp = A.max - area.max;

        if (!clampNegative)
            return;

        leftDown = new Vector2(Mathf.Max(0, leftDown.x), Mathf.Max(0, leftDown.y));
        rightUp = new Vector2(Mathf.Max(0, rightUp.x), Mathf.Max(0, rightUp.y));
    }

    public static float Round(this float A, float range)
    {
        return Mathf.Round(A / range) * range;
    }
    public static float Round_Anchor(this float A, float range, float anchor)
    {
        return Mathf.Round((A - anchor) / range) * range + anchor;
    }
    public static double Round(this double A, double range)
    {
        return Math.Round(A / range) * range;
    }
    //
    public static Vector2 Round(this Vector2 A, float range)
    {
        return new Vector2(Mathf.Round(A.x / range) * range, Mathf.Round(A.y / range) * range);
    }
    public static Vector2 Round(this Vector2 A, Vector2 range)
    {
        return new Vector2(Mathf.Round(A.x / range.x) * range.y, Mathf.Round(A.y / range.y) * range.y);
    }
    
    public static float Floor(this float A, float range)
    {
        return Mathf.Floor(A / range) * range;
    }
    
    // Vector2
    public static float AngleDegrees(this Vector2 a)
    {
        return a.AngleRadians() * Mathf.Rad2Deg;
    }
    public static float AngleRadians(this Vector2 a)
    {
        return Mathf.Atan2(a.y, a.x);
    }
    
    
    // Other
    public static Vector2 GetLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out bool parallel)
    {
        Vector2 d12 = a2 - a1;
        Vector2 d34 = b2 - b1;

        float denominator = (d12.y * d34.x - d12.x * d34.y);
        
        parallel = denominator == 0;
        if (parallel)
            return default;

        float t = ((a1.x - b1.x) * d34.y + (b1.y - a1.y) * d34.x) / denominator;
        return new Vector2(a1.x + d12.x * t, a1.y + d12.y * t);
    }
    
    public static float GetClosestPointOnLine(Vector2 p1, Vector2 p2, Vector2 point)
    {
        Vector2 size = p2 - p1;
        return Vector2.Dot(point - p1, size) / size.sqrMagnitude;
    }
    public static double GetClosestPointOnLine(Double2 p1, Double2 p2, Double2 point)
    {
        Double2 size = p2 - p1;
        return Double2.Dot(point - p1, size) / size.sqrMagnitude;
    }
    public static bool IsClockwise(Vector2 a, Vector2 b, Vector2 c) 
    {
        return (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y) >= 0;
    }

    public static Vector3 ToVector3(this Vector2 a, float z)
    {
        return new Vector3(a.x, a.y, z);
    }
    public static double NormalizeAngleDegrees(double angle)
    {
        while (angle > 180)
            angle -= 360;

        while (angle < -180)
            angle += 360;

        return angle;
    }
    public static double NormalizePositiveAngleDegrees(double angle)
    {
        while (angle > 360)
            angle -= 360;

        while (angle < 0)
            angle += 360;

        return angle;
    }
    public static double NormalizePositiveAngleRadians(double angle)
    {
        while (angle > Math.PI)
            angle -= Math.PI;

        while (angle < 0)
            angle += Math.PI;

        return angle;
    }
}
