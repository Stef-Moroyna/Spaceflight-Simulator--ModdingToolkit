public class Matrix2x2_Double
{
    public Double2 x, y;

    public static Matrix2x2_Double Angle(double angleRadians)
    {
        return new Matrix2x2_Double { x = new Double2(1, 0).Rotate(angleRadians), y = new Double2(0, 1).Rotate(angleRadians) };
    }

    public Double2 Apply(Double2 a)
    {
        return (a.x * x) + (a.y * y);
    }
    public Double3 Apply(Double3 a)
    {
        Double2 num = (a.x * x) + (a.y * y);
        return new Double3(num.x, num.y, a.z);
    }
}