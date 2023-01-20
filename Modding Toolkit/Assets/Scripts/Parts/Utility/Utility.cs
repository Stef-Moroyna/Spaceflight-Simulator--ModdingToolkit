using System;
using UnityEngine;


public static class Utility
{
    public static Vector2 Rotate_90(this Vector2 a)
    {
        return new Vector2(-a.y, a.x);
    }

    public static void DrawGizmosBox(Vector2 min, Vector2 max, Color color, bool border) 
    {
        Gizmos.color = color;
        Gizmos.DrawCube((min + max) / 2, max - min);

        if (!border)
            return;
        
        Debug.DrawLine(min, new Vector2(min.x, max.y), color);
        Debug.DrawLine(min, new Vector2(max.x, min.y), color);
        Debug.DrawLine(max, new Vector2(min.x, max.y), color);
        Debug.DrawLine(max, new Vector2(max.x, min.y), color);
    }
    
    public static void DrawArrow_Ray(Vector2 position, Vector2 size, Color c, float sizeM = 1)
    {
        Debug.DrawRay(position, size, c);

        Vector2 a = size.normalized * 0.15f;
        Debug.DrawRay(position + size, a * (-0.1f * sizeM) - a.Rotate_90() * (0.05f * sizeM), c);
        Debug.DrawRay(position + size, a * (-0.1f * sizeM) + a.Rotate_90() * (0.05f * sizeM), c);
    }
    public static void DrawArrow(Vector2 a, Vector2 b, Color c, float sizeM = 1)
    {
        DrawArrow_Ray(a, b - a, c, sizeM);
    }
}