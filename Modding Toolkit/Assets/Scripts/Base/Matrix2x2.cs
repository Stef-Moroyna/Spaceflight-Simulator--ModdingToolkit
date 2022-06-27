using UnityEngine;

public class Matrix2x2
{
    Vector2 x;
    Vector2 y;

    public static Matrix2x2 Angle(float angleRadians)
    {
        float cs = Mathf.Cos(angleRadians);
        float sn = Mathf.Sin(angleRadians);
        return new Matrix2x2 { x = new Vector2(cs, sn), y = new Vector2(-sn, cs) };
    }

    public static Vector2 operator *(Matrix2x2 matrix, Vector2 b)
    {
        return new Vector2(matrix.x.x * b.x + matrix.y.x * b.y, matrix.x.y * b.x + matrix.y.y * b.y);
    }
    public static Vector2 operator *(Vector2 b, Matrix2x2 matrix)
    {
        return new Vector2(matrix.x.x * b.x + matrix.y.x * b.y, matrix.x.y * b.x + matrix.y.y * b.y);
    }
    public static Vector2 operator *(Matrix2x2 matrix, Vector3 b)
    {
        return new Vector2(matrix.x.x * b.x + matrix.y.x * b.y, matrix.x.y * b.x + matrix.y.y * b.y);
    }
    public static Vector2 operator *(Vector3 b, Matrix2x2 matrix)
    {
        return new Vector2(matrix.x.x * b.x + matrix.y.x * b.y, matrix.x.y * b.x + matrix.y.y * b.y);
    }

    public float GetX(Vector2 b)
    {
        return x.x * b.x + y.x * b.y;
    }
}