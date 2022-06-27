using UnityEngine;

namespace SFS
{
    public static class UV_Utility
    {
        // Returns the uv coords of a point x y in a quad (counter-clockwise)
        public static Vector2 UV(Vector2 point, Vector2[] bounds)
        {
            float u = Get_UV_InQuad(point, bounds[1], bounds[0], bounds[3], bounds[2]).x;
            float v = Get_UV_InQuad(point, bounds[0], bounds[1], bounds[2], bounds[3]).y;
            return new Vector2(u, 1 - v);
        }

        // Returns the uv coords of a point x y in a quad (clockwise)
        public static Vector2 Get_UV_InQuad(Vector2 point, Vector2 p1, Vector2 p4, Vector2 p3, Vector2 p2)
        {
            return UV(p1.x, p1.y, p2.x, p2.y, p3.x, p3.y, p4.x, p4.y, point.x, point.y);
        }

        // Returns the uv coords of a point x y in a quad
        static Vector2 UV(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float x, float y)
        {
            float ar = (x3 - x4) * (y1 - y2) - (x1 - x2) * (y3 - y4);
            float ar2 = (x1 - x4) * (y2 - y3) - (x2 - x3) * (y1 - y4);

            float uA = -1 / (2 * ar);
            float uB = -x2 * y + x3 * y - x4 * y - x * y1 - x3 * y1 + 2 * x4 * y1 + x * y2 - x4 * y2 - x * y3 + x1 * (y + y3 - 2 * y4) + x * y4 + x2 * y4;

            float C = Mathf.Sqrt(4 * ar * (x4 * (-y + y1) + x1 * (y - y4) + x * (-y1 + y4)) + Mathf.Pow(x3 * y - x4 * y - x3 * y1 + 2 * x4 * y1 - x4 * y2 + x1 * (y + y3 - 2 * y4) + x2 * (-y + y4) + x * (-y1 + y2 - y3 + y4), 2));

            float vA = 1 / (2 * ar2);
            float vB = x2 * y - x3 * y + x4 * y + x * y1 - 2 * x2 * y1 + x3 * y1 - x * y2 - x4 * y2 + x * y3 - x1 * (y - 2 * y2 + y3) - x * y4 + x2 * y4;

            float u = uA * (uB + C);
            float v = 1 - vA * (vB + C);

            if (ar == 0 && ar2 == 0)
            {
                // Rectangle
                u = Math_Utility.InverseLerpUnclamped(x1, x2, x);
                v = Math_Utility.InverseLerpUnclamped(y4, y1, y);
            }
            else if (ar == 0)
            {
                // Top and bottom are parallel
                Vector2 left = Vector2.Lerp(new Vector2(x4, y4), new Vector2(x1, y1), v);
                Vector2 right = Vector2.Lerp(new Vector2(x3, y3), new Vector2(x2, y2), v);
                u = Math_Utility.InverseLerpUnclamped(left.x, right.x, x);
            }
            else if (ar2 == 0)
            {
                // Left and right are parallel
                Vector2 top = Vector2.Lerp(new Vector2(x4, y4), new Vector2(x3, y3), u);
                Vector2 bottom = Vector2.Lerp(new Vector2(x1, y1), new Vector2(x2, y2), u);
                v = Math_Utility.InverseLerpUnclamped(top.y, bottom.y, y);
            }

            return new Vector2(u, v);
        }

        // M
        public static float[] GetQuadM(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            Vector2 center = Math_Utility.GetLineIntersection(topLeft, bottomRight, topRight, bottomLeft, out bool parallel);

            if (parallel)
                return new[] { 1f, 1f, 1f, 1f };

            float d_TopLeft = Vector2.Distance(topLeft, center);
            float d_TopRight = Vector2.Distance(topRight, center);
            float d_BottomRight = Vector2.Distance(bottomRight, center);
            float d_BottomLeft = Vector2.Distance(bottomLeft, center);

            float d_TopLeft_To_BottomRight = d_TopLeft + d_BottomRight;
            float d_TopRight_To_BottomLeft = d_TopRight + d_BottomLeft;

            float[] M = new float[4];
            M[0] = d_TopLeft_To_BottomRight / d_BottomRight; // Top left
            M[1] = d_TopRight_To_BottomLeft / d_BottomLeft; // Top right
            M[2] = d_TopLeft_To_BottomRight / d_TopLeft; // Bottom right
            M[3] = d_TopRight_To_BottomLeft / d_TopRight; // Bottom left
            return M;
        }
    }
}

// Debug.DrawLine(topLeft, bottomLeft);
// Debug.DrawLine(topLeft, bottomRight, Color.green);
// Debug.DrawLine(topRight, bottomLeft, Color.red);
// Debug.DrawRay(center, Vector3.up * 0.1f);