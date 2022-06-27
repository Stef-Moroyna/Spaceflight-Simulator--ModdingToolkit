using System;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable, InlineProperty]
public struct Double3
{
    [HorizontalGroup, LabelWidth(15)]
    public double x, y, z;

    public Double3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Double3(double x, double y)
    {
        this.x = x;
        this.y = y;
        z = 0;
    }

    public static Double3 zero => new Double3();

    public static Double3 Cross(Double3 a, Double3 b) => new Double3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
    public static Double3 Cross(Double2 a, Double2 b) => new Double3(0, 0, a.x * b.y - a.y * b.x);


    public static Double3 GetPerpendicular(Double3 point_A, Double3 point_B, Double3 point_C)
    {
        Double3 direction_1 = point_A - point_B;
        Double3 direction_2 = point_A - point_C;
        return Cross(direction_1, direction_2);
    }

    public static double Dot(Double3 a, Double3 b) => a.x * b.x + a.y * b.y + a.z * b.z;


    public static Double3 CosSin(double a) => new Double3(Math.Cos(a), Math.Sin(a));
    public double AngleRadians() => Math.Atan2(y, x);

    public double magnitude2d => Math.Sqrt(x * x + y * y);
    public double magnitude => Math.Sqrt(x * x + y * y + z * z);

    public double sqrMagnitude => x * x + y * y + z * z;

    public double sqrMagnitude2d => x * x + y * y;

    public Double3 normalized2d
    {
        get
        {
            double num = magnitude2d;

            if (num > 9.99999974737875E-06)
                return this / num;
            else
                return zero;
        }
    }

    public Double3 normalized
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

    public Double3 Rotate(double angleRadians)
    {
        double cs = Math.Cos(angleRadians);
        double sn = Math.Sin(angleRadians);
        return new Double3(x * cs - y * sn, x * sn + y * cs);
    }

    public static Double3 operator +(Double3 a, Double3 b) => new Double3(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Double3 operator +(Double3 a, Vector3 b) => new Double3( a.x + b.x, a.y + b.y, a.z + b.z);
    public static Double3 operator +(Double3 a, Vector2 b) => new Double3(a.x + b.x, a.y + b.y, a.z);

    public static Double3 operator - (Double3 a, Double3 b) => new Double3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static Double3 operator - (Double3 a, Vector3 b) => new Double3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static Double3 operator - (Double3 a, Vector2 b) => new Double3(a.x - b.x, a.y - b.y, a.z);
    public static Double3 operator - (Double3 a) => new Double3(-a.x, -a.y, -a.z);

    public static Double3 operator *(Double3 a, double b) => new Double3(a.x * b, a.y * b, a.z * b);
    public static Double3 operator *(double d, Double3 a) => new Double3(a.x * d, a.y * d, a.z * d);
    public static Double3 operator /(Double3 a, double b) => new Double3(a.x / b, a.y / b, a.z / b);

    public static bool operator == (Double3 a, Double3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
    public static bool operator != (Double3 a, Double3 b) => a.x != b.x || a.y != b.y || a.z != b.z;


    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }
    public override bool Equals(object other)
    {
        if (!(other is Double3))
            return false;
        else
            return Equals((Double3)other);
    }
    public bool Equals(Double3 other)
    {
        return x.Equals(other.x) && y.Equals(other.y);
    }


    public static implicit operator Vector2 (Double3 a) => new Vector2((float)a.x, (float)a.y);
    public static implicit operator Vector3 (Double3 a) => new Vector3((float)a.x, (float)a.y, (float)a.z);

    public static Double3 ToDouble3(Vector3 a) => new Double3(a.x, a.y, a.z);

    public static double GetClosestPointOnLine(Double3 p2, Double3 p3)
    {
        double px = p2.x;
        double py = p2.y;

        double temp = (px * px) + (py * py);

        double u = ((p3.x) * px + (p3.y) * py) / temp;

        // Clamps to 0-1
        if (u < 0)
            return 0;
        if (u > 1)
            return 1;

        return u;
    }

    public override string ToString()
    {
        return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
    }

    public static explicit operator Double3(Double2 a) => new Double3( a.x, a.y);
}