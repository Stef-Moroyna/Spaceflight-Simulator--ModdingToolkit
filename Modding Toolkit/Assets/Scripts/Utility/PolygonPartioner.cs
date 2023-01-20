using System.Collections.Generic;
using UnityEngine;

public static class PolygonPartioner
{
    public static List<ConvexPolygon> Partion(Vector2[] points)
    {
        VertexChain v = new VertexChain(points);
        List<VertexChain> polys = ConvexPartition(v);
        List<ConvexPolygon> output = new List<ConvexPolygon>();

        for (int i = 0; i < polys.Count; i++)
        {
            VertexChain polygon = polys[i];
            Vector2[] polygonPoints = new Vector2[polygon.Count];

            for (int j = 0; j < polygon.Count; j++)
                polygonPoints[j] = polygon[j];

            output.Add(new ConvexPolygon(polygonPoints));
        }

        return output;
    }

    static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c) => a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);

    static Vector2 At(int position, VertexChain vertices)
    {
        int count = vertices.Count;
        return vertices[position < 0 ? count - (-position % count) : position % count];
    }

    static bool CanSee(int i, int j, VertexChain vertices)
    {
        Vector2 prev = At(i - 1, vertices);
        Vector2 on = At(i, vertices);
        Vector2 next = At(i + 1, vertices);

        if (Reflex(prev, on, next))
        {
            if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) &&
                RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)))
                return false;
        }
        else
        {
            if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) ||
                LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)))
                return false;
        }

        Vector2 prevj = At(j - 1, vertices);
        Vector2 onj = At(j, vertices);
        Vector2 nextj = At(j + 1, vertices);

        if (Reflex(prevj, onj, nextj))
        {
            if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) &&
                RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)))
                return false;
        }
        else
        {
            if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) ||
                LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)))
                return false;
        }
        for (int k = 0; k < vertices.Count; ++k)
        {
            Vector2 nullvec = new Vector2();
            Vector2 intPoint = new Vector2();
            Vector2 p1 = At(i, vertices);
            Vector2 p2 = At(j, vertices);
            Vector2 q1 = At(k, vertices);
            Vector2 q2 = At(k + 1, vertices);

            // Ignore incident edges
            if (p1.Equals(q1) || p1.Equals(q2) || p2.Equals(q1) || p2.Equals(q2))
                continue;

            // Segment intersection
            bool result = LineIntersect(ref p1, ref p2, ref q1, ref q2, true, true, out intPoint);

            if (intPoint != nullvec)
            {
                // If intPoint is not any of the j line then false, else continue. Intersection has to be interior to qualify as false from here
                if ((!intPoint.Equals(At(k, vertices))) || (!intPoint.Equals(At(k + 1, vertices))))
                    return false;
            }
        }
        return true;
    }

    static VertexChain CollinearSimplify(VertexChain vertices, float collinearityTolerance = 0)
    {
        if (vertices.Count <= 3)
            return vertices;

        VertexChain simplified = new VertexChain(vertices.Count);

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 prev = vertices.PreviousVertex(i);
            Vector2 current = vertices[i];
            Vector2 next = vertices.NextVertex(i);

            if (IsCollinear(ref prev, ref current, ref next, collinearityTolerance))
                continue;

            simplified.Add(current);
        }

        return simplified;
    }

    static List<VertexChain> ConvexPartition(VertexChain vertices)
    {
        // We force CCW as it is a precondition to this algorithm.
        vertices.ForceCounterClockWise();

        List<VertexChain> list = new List<VertexChain>();

        if (vertices.Count < 3)
            return list;

        double d = 0.0, lowerDist = 0.0, upperDist = 0.0;
        Vector2 p;
        Vector2 lowerInt = new Vector2();
        Vector2 upperInt = new Vector2();
        int lowerIndex = 0, upperIndex = 0;
        VertexChain lowerPoly, upperPoly;

        for (int i = 0; i < vertices.Count; ++i)
        {
            Vector2 prev = At(i - 1, vertices);
            Vector2 on = At(i, vertices);
            Vector2 next = At(i + 1, vertices);

            if (Reflex(prev, on, next))
            {
                lowerDist = upperDist = double.MaxValue;
                for (int j = 0; j < vertices.Count; ++j)
                {
                    if ((i == j) || (i == Index(j - 1, vertices.Count)) || (i == Index(j + 1, vertices.Count)))
                        continue;

                    Vector2 iPrev = At(i - 1, vertices);
                    Vector2 iSelf = At(i, vertices);
                    Vector2 jSelf = At(j, vertices);
                    Vector2 jPrev = At(j - 1, vertices);

                    bool leftOK = Left(iPrev, iSelf, jSelf);
                    bool rightOK = Right(iPrev, iSelf, jPrev);

                    bool leftOnOK = IsCollinear(ref iPrev, ref iSelf, ref jSelf);
                    bool rightOnOK = IsCollinear(ref iPrev, ref iSelf, ref jPrev);

                    if (leftOnOK || rightOnOK)
                    {
                        d = SquareDist(iSelf, jSelf);

                        if (d < lowerDist)
                        {
                            lowerDist = d;
                            lowerInt = jSelf;
                            lowerIndex = j - 1;
                        }

                        d = SquareDist(iSelf, jPrev);

                        if (d < lowerDist)
                        {
                            lowerDist = d;
                            lowerInt = jPrev;
                            lowerIndex = j;
                        }
                    }
                    else if (leftOK && rightOK)
                    {
                        p = LineIntersect(At(i - 1, vertices), At(i, vertices), At(j, vertices),
                                                    At(j - 1, vertices));

                        if (Right(At(i + 1, vertices), At(i, vertices), p))
                        {

                            d = SquareDist(At(i, vertices), p);

                            if (d < lowerDist)
                            {
                                lowerDist = d;
                                lowerInt = p;
                                lowerIndex = j;
                            }
                        }
                    }

                    Vector2 iNext = At(i + 1, vertices);
                    Vector2 jNext = At(j + 1, vertices);

                    bool leftOKn = Left(iNext, iSelf, jNext);
                    bool rightOKn = Right(iNext, iSelf, jSelf);

                    bool leftOnOKn = IsCollinear(ref iNext, ref iSelf, ref jNext);
                    bool rightOnOKn = IsCollinear(ref iNext, ref iSelf, ref jSelf);

                    if (leftOnOKn || rightOnOKn)
                    {
                        d = SquareDist(iSelf, jNext);

                        if (d < upperDist)
                        {
                            upperDist = d;
                            upperInt = jNext;
                            upperIndex = j + 1;
                        }

                        d = SquareDist(At(i, vertices), At(j, vertices));

                        if (d < upperDist)
                        {
                            upperDist = d;
                            upperInt = jSelf;
                            upperIndex = j;
                        }
                    }
                    else if (leftOKn && rightOKn)
                    {
                        p = LineIntersect(At(i + 1, vertices), At(i, vertices), At(j, vertices),
                                                    At(j + 1, vertices));
                        if (Left(At(i - 1, vertices), At(i, vertices), p))
                        {
                            d = SquareDist(At(i, vertices), p);
                            if (d < upperDist)
                            {
                                upperDist = d;
                                upperIndex = j;
                                upperInt = p;
                            }
                        }
                    }
                }

                if (lowerIndex == (upperIndex + 1) % vertices.Count)
                {
                    Vector2 sp = ((lowerInt + upperInt) / 2);

                    lowerPoly = Copy(i, upperIndex, vertices);
                    lowerPoly.Add(sp);
                    upperPoly = Copy(lowerIndex, i, vertices);
                    upperPoly.Add(sp);
                }
                else
                {
                    double highestScore = 0, bestIndex = lowerIndex;
                    while (upperIndex < lowerIndex)
                        upperIndex += vertices.Count;

                    for (int j = lowerIndex; j <= upperIndex; ++j)
                    {
                        if (CanSee(i, j, vertices))
                        {
                            double score = 1 / (SquareDist(At(i, vertices), At(j, vertices)) + 1);

                            Vector2 prevj = At(j - 1, vertices);
                            Vector2 onj = At(j, vertices);
                            Vector2 nextj = At(j + 1, vertices);

                            if (Reflex(prevj, onj, nextj))
                            {
                                if (RightOn(At(j - 1, vertices), At(j, vertices), At(i, vertices)) &&
                                    LeftOn(At(j + 1, vertices), At(j, vertices), At(i, vertices)))
                                    score += 3;
                                else
                                    score += 2;
                            }
                            else
                                score += 1;
                            if (score > highestScore)
                            {
                                bestIndex = j;
                                highestScore = score;
                            }
                        }
                    }

                    lowerPoly = Copy(i, (int)bestIndex, vertices);
                    upperPoly = Copy((int)bestIndex, i, vertices);
                }

                if (lowerPoly.Count < upperPoly.Count)
                {
                    list.AddRange(ConvexPartition(lowerPoly));
                    list.AddRange(ConvexPartition(upperPoly));
                }
                else
                {
                    list.AddRange(ConvexPartition(upperPoly));
                    list.AddRange(ConvexPartition(lowerPoly));
                }

                return list;
            }
        }

        list.Add(vertices);

        for (int i = 0; i < list.Count; i++)
            list[i] = CollinearSimplify(list[i], 0.00001f);

        return list;
    }

    static VertexChain Copy(int i, int j, VertexChain vertices)
    {
        VertexChain p = new VertexChain();

        while (j < i)
            j += vertices.Count;

        for (; i <= j; ++i)
            p.Add(At(i, vertices));

        return p;
    }

    static bool FloatEquals(float value1, float value2) => Mathf.Abs(value1 - value2) <= Mathf.Epsilon;

    static bool FloatInRange(float value, float min, float max) => (value >= min && value <= max);

    static int Index(int i, int size) => i < 0 ? size - (-i % size) : i % size;

    static bool IsCollinear(ref Vector2 a, ref Vector2 b, ref Vector2 c, float tolerance = 0) => FloatInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);

    static bool Left(Vector2 a, Vector2 b, Vector2 c) => Area(ref a, ref b, ref c) > 0;

    static bool LeftOn(Vector2 a, Vector2 b, Vector2 c) => Area(ref a, ref b, ref c) >= 0;

    static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, bool firstIsSegment, bool secondIsSegment, out Vector2 point)
    {
        point = new Vector2();

        float a = point4.y - point3.y;
        float b = point2.x - point1.x;
        float c = point4.x - point3.x;
        float d = point2.y - point1.y;

        // Denominator to solution of linear system
        float denom = (a * b) - (c * d);

        // If denominator is 0, then lines are parallel
        if (!(denom >= -Mathf.Epsilon && denom <= Mathf.Epsilon))
        {
            float e = point1.y - point3.y;
            float f = point1.x - point3.x;
            float oneOverDenom = 1.0f / denom;

            // Numerator of first equation
            float ua = (c * e) - (a * f);
            ua *= oneOverDenom;

            // Check if intersection point of the two lines is on line segment 1
            if (!firstIsSegment || ua >= 0.0f && ua <= 1.0f)
            {
                // Numerator of second equation
                float ub = (b * e) - (d * f);
                ub *= oneOverDenom;

                // Check if intersection point of the two lines is on line segment 2
                // That means that the line segments intersect, since we know it is on segment 1 as well.
                if (!secondIsSegment || ub >= 0.0f && ub <= 1.0f)
                {
                    // Check if they are coincident (no collision in this case)
                    if (ua != 0f || ub != 0f)
                    {
                        // There is an intersection
                        point.x = point1.x + ua * b;
                        point.y = point1.y + ua * d;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        Vector2 i = Vector2.zero;
        float a1 = p2.y - p1.y;
        float b1 = p1.x - p2.x;
        float c1 = a1 * p1.x + b1 * p1.y;
        float a2 = q2.y - q1.y;
        float b2 = q1.x - q2.x;
        float c2 = a2 * q1.x + b2 * q1.y;
        float det = a1 * b2 - a2 * b1;

        if (!FloatEquals(det, 0))
        {
            // lines are not parallel
            i.x = (b2 * c1 - b1 * c2) / det;
            i.y = (a1 * c2 - a2 * c1) / det;
        }
        return i;
    }

    static bool Reflex(Vector2 prev, Vector2 on, Vector2 next)
    {
        if (IsCollinear(ref prev, ref on, ref next))
            return false;

        return Right(prev, on, next);
    }
    static bool Right(Vector2 a, Vector2 b, Vector2 c) => Area(ref a, ref b, ref c) < 0;

    static bool RightOn(Vector2 a, Vector2 b, Vector2 c) => Area(ref a, ref b, ref c) <= 0;

    static float SquareDist(Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        return dx * dx + dy * dy;
    }
}

public class VertexChain : List<Vector2>
{
    public VertexChain() { }

    public VertexChain(int capacity) : base(capacity) { }

    public VertexChain(IEnumerable<Vector2> vertices) => AddRange(vertices);

    public void ForceCounterClockWise()
    {
        if (Count < 3)
            return;

        if (!IsCounterClockWise())
            Reverse();
    }

    public float GetSignedArea()
    {
        if (Count < 3)
            return 0;

        int i;

        float area = 0;

        for (i = 0; i < Count; i++)
        {
            int j = (i + 1) % Count;

            Vector2 vi = this[i];
            Vector2 vj = this[j];

            area += vi.x * vj.y;
            area -= vi.y * vj.x;
        }
        area /= 2.0f;
        return area;
    }

    public bool IsCounterClockWise()
    {
        if (Count < 3)
            return false;

        return (GetSignedArea() > 0.0f);
    }

    public int NextIndex(int index) => (index + 1 > Count - 1) ? 0 : index + 1;

    public Vector2 NextVertex(int index) => this[NextIndex(index)];

    public int PreviousIndex(int index) => index - 1 < 0 ? Count - 1 : index - 1;

    public Vector2 PreviousVertex(int index) => this[PreviousIndex(index)];
}