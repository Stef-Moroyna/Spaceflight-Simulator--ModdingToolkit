using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct Line
{
    [BoxGroup] public float start, end;

    public float Size => end - start;
    public float Center => (start + end) / 2;

    // Constructors
    public Line(float start, float end)
    {
        this.start = start;
        this.end = end;
    }
    public static Line StartSize(float start, float size)
    {
        return new Line(start, start + size);
    }
    public static Line CenterSize(float center, float size)
    {
        return new Line(center - size / 2, center + size / 2);
    }

    public void Set(float start, float end)
    {
        this.start = start;
        this.end = end;
    }

    public float Lerp(float t)
    {
        return Mathf.Lerp(start, end, t);
    }
    public float InverseLerp(float a)
    {
        return Mathf.InverseLerp(start, end, a);
    }
}

[Serializable]
public struct Line2
{
    public Vector2 start, end;

    public Vector2 Size => end - start;
    public float SizeX => end.x - start.x;
    public float SizeY => end.y - start.y;
    
    // Constructors
    public Line2(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
    public static Line2 StartSize(Vector2 start, Vector2 size)
    {
        return new Line2(start, start + size);
    }

    public Vector2 Lerp(float t)
    {
        return Vector2.Lerp(start, end, t);
    }
    public Vector2 Lerp(float t_X, float t_Y)
    {
        return new Vector2(Mathf.Lerp(start.x, end.x, t_X), Mathf.Lerp(start.y, end.y, t_Y));
    }
    public Vector2 LerpUnclamped(float t)
    {
        return new Vector2(Mathf.LerpUnclamped(start.x, end.x, t), Mathf.LerpUnclamped(start.y, end.y, t));
    }
    public Vector2 LerpUnclamped(float t_X, float t_Y)
    {
        return new Vector2(Mathf.LerpUnclamped(start.x, end.x, t_X), Mathf.LerpUnclamped(start.y, end.y, t_Y));
    }

    public void Flip()
    {
        Vector2 temp = start;

        start = end;
        end = temp;
    }
    public void FlipHorizontally()
    {
        float temp = start.x;

        start.x = end.x;
        end.x = temp;
    }
    public void FlipVertically()
    {
        float temp = start.y;

        start.y = end.y;
        end.y = temp;
    }

    public Vector2 GetPositionAtX(float x)
    {
        float t = (x - start.x) / (end.x - start.x);

        if (t <= 0)
            return start;
        if (t >= 1)
            return end;

        return start + (end - start) * t;
    }
    public Vector2 GetPositionAtY(float y)
    {
        float t = (y - start.y) / (end.y - start.y);

        if (t <= 0)
            return start;
        if (t >= 1)
            return end;

        return start + (end - start) * t;
    }
    public Vector2 GetPositionAtX_Unclamped(float x)
    {
        float t = (x - start.x) / (end.x - start.x);
        return start + (end - start) * t;
    }
    public float GetHeightAtX(float x)
    {
        float t = (x - start.x) / (end.x - start.x);

        if (t <= 0)
            return start.y;
        if (t >= 1)
            return end.y;

        return start.y + (end.y - start.y) * t;
    }
    public float GetHeightAtX_Unclamped(float x)
    {
        float t = (x - start.x) / (end.x - start.x);
        return start.y + (end.y - start.y) * t;
    }

    public float GetSlope()
    {
        if (start.x == end.x)
        {
            Debug.LogError("Width 0");
            return 0;   
        }

        return (end.y - start.y) / (end.x - start.x);
    }
    public static float GetSlope_Abs(Vector2 start, Vector2 end)
    {
        return Mathf.Abs((end.y - start.y) / (end.x - start.x));
    }
    public static float GetSlope(Vector2 start, Vector2 end)
    {
        return (end.y - start.y) / (end.x - start.x);
    }
    
    public static bool FindIntersection_Unclamped(Line2 a, Line2 b, out Vector2 position)
    {
        float dx12 = a.end.x - a.start.x;
        float dy12 = a.end.y - a.start.y;
        float dx34 = b.end.x - b.start.x;
        float dy34 = b.end.y - b.start.y;
        
        float denominator = (dy12 * dx34 - dx12 * dy34);

        if (denominator == 0)
        {
            position = Vector2.zero;
            return false;
        }

        float t1 = ((a.start.x - b.start.x) * dy34 + (b.start.y - a.start.y) * dx34) / denominator;
        position = new Vector2(a.start.x + dx12 * t1, a.start.y + dy12 * t1);
        return true;
    }
}

[Serializable]
public struct Color2
{
    [BoxGroup] public Color start, end;

    public Color2(Color start, Color end)
    {
        this.start = start;
        this.end = end;
    }

    public Color Lerp(float t)
    {
        return Color.Lerp(start, end, t);
    }
}