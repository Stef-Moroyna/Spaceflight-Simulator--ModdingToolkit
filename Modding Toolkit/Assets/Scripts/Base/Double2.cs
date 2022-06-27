using System;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable, InlineProperty]
public struct Double2
{
    [HorizontalGroup, HideLabel]
    public double x, y;

    public Double2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Double2 zero => new Double2();

    public static double Dot(Double2 a, Double2 b) => (a.x * b.x) + (a.y * b.y);

    public double AngleRadians => Math.Atan2(y, x);
    public double AngleDegrees => Math.Atan2(y, x) / (Math.PI * 2) * 360.0;

    public double magnitude => Math.Sqrt(x * x + y * y);

    public double sqrMagnitude => x * x + y * y;


    public bool Mag_MoreThan(double a) => sqrMagnitude > a * a;
    public bool Mag_LessThan(double a) => sqrMagnitude < a * a;

    public Double2 normalized
    {
        get
        {
            double num = magnitude;

            if (num > 9.99999974737875E-06)
                return this / num;
            else
                return zero;
        }
    }

    public Double2 Rotate(double angleRadians)
    {
        double cs = Math.Cos(angleRadians);
        double sn = Math.Sin(angleRadians);
        return new Double2(x * cs - y * sn, x * sn + y * cs);
    }

    public string ToParsableString()
    {
        return $"{x}:{y}";
    }

    public static Double2 operator +(Double2 a, Double2 b) => new Double2(a.x + b.x, a.y + b.y);
    public static Double2 operator +(Double2 a, Vector3 b) => new Double2( a.x + b.x, a.y + b.y);
    public static Double2 operator +(Double2 a, Vector2 b) => new Double2(a.x + b.x, a.y + b.y);

    public static Double2 operator -(Double2 a, Double2 b) => new Double2(a.x - b.x, a.y - b.y);
    public static Double2 operator -(Double2 a, Vector3 b) => new Double2(a.x - b.x, a.y - b.y);
    public static Double2 operator -(Double2 a, Vector2 b) => new Double2(a.x - b.x, a.y - b.y);
    public static Double2 operator -(Double2 a) => new Double2(-a.x, -a.y);

    public static Double2 operator *(Double2 a, Double2 b) => new Double2(a.x * b.x, a.y * b.y);
    public static Double2 operator *(Double2 a, Vector2 b) => new Double2(a.x * b.x, a.y * b.y);
    public static Double2 operator *(Double2 a, double b) => new Double2(a.x * b, a.y * b);
    public static Double2 operator *(double b, Double2 a) => new Double2(a.x * b, a.y * b);
    public static Double2 operator /(Double2 a, double b) => new Double2(a.x / b, a.y / b);

    public static bool operator == (Double2 a, Double2 b) => a.x == b.x && a.y == b.y;
    public static bool operator != (Double2 a, Double2 b) => a.x != b.x || a.y != b.y;

    public static implicit operator Vector2 (Double2 a) => new Vector2((float)a.x, (float)a.y);
    public static implicit operator Vector3 (Double2 a) => new Vector3((float)a.x, (float)a.y);


    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }
    public override bool Equals(object other)
    {
        if (!(other is Double2))
            return false;
        else
            return Equals((Double2)other);
    }
    public bool Equals(Double2 other)
    {
        return x.Equals(other.x) && y.Equals(other.y);
    }


    public Vector2 ToVector2 => new Vector2((float)x, (float)y);
    public Vector3 ToVector3 => new Vector3((float)x, (float)y, 0);

    public static Double2 ToDouble2(Vector2 a)
    {
        return new Double2(a.x, a.y);
    }
    public static Double2 ToDouble2(Vector3 a)
    {
        return new Double2(a.x, a.y);
    }

    public override string ToString()
    {
        return "(" + x.ToString() + ", " + y.ToString() + ")";
    }

    public static Double2 CosSin(double angleRadians)
    {
        return new Double2(Math.Cos(angleRadians), Math.Sin(angleRadians));
    }
    public static Double2 CosSin(double angleRadians, double radius)
    {
        return new Double2(Math.Cos(angleRadians) * radius, Math.Sin(angleRadians) * radius);
    }

    public static Double2 Reflect(Double2 inDirection, Double2 inNormal)
    {
        double factor = -2.0 * Dot(inNormal, inDirection);
        return new Double2(factor * inNormal.x + inDirection.x, factor * inNormal.y + inDirection.y);
    }

    public static explicit operator Double2(Vector2 a) => new Double2(a.x, a.y);
    public static explicit operator Double2(Double3 a) => new Double2(a.x, a.y);

    public static Double2 Lerp(Double2 a, Double2 b, double t)
    {
        return (1 - t) * a + t * b;
    }

    public static Double2 Parse(string text)
    {
        double x = double.Parse(text.Split(':')[1]);
        double y = double.Parse(text.Split(':')[2]);
        return new Double2(x, y);
    }
}