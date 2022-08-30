using System;
using UnityEngine;

public class OptionalDelegate<T>
{
    event Action emptyDelegate;
    event Action<T> singleDelegate;

    public int CallCount { get; private set; }

    public static OptionalDelegate<T> operator +(OptionalDelegate<T> a, Action b)
    {
        a.emptyDelegate += b;
        a.CallCount++;
        return a;
    }
    public static OptionalDelegate<T> operator +(OptionalDelegate<T> a, Action<T> b)
    {
        a.singleDelegate += b;
        a.CallCount++;
        return a;
    }

    public static OptionalDelegate<T> operator -(OptionalDelegate<T> a, Action b)
    {
        a.emptyDelegate -= b;
        a.CallCount--;
        return a;
    }
    public static OptionalDelegate<T> operator -(OptionalDelegate<T> a, Action<T> b)
    {
        a.singleDelegate -= b;
        a.CallCount--;
        return a;
    }

    public void Invoke(T data)
    {
        emptyDelegate?.Invoke();
        singleDelegate?.Invoke(data);
    }

    public void Clear()
    {
        emptyDelegate = null;
        singleDelegate = null;
        CallCount = 0;
    }

    public bool IsEmpty => CallCount < 1;
}

public static class Utility
{
    public static float GetDeltaV(float isp, float fullMass, float dryMass)
    {
        return isp * 9.8f * Mathf.Log(fullMass / dryMass);
    }
    
    public static Vector2 Rotate_Radians(this Vector2 a, float b)
    {
        float cs = Mathf.Cos(b);
        float sn = Mathf.Sin(b);
        return new Vector2(a.x * cs - a.y * sn, a.x * sn + a.y * cs); // Rotates velocity by arg before returning it
    }
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