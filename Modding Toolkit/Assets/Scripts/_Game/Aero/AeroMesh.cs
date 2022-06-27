using System;
using System.Collections.Generic;
using System.Linq;
using SFS;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using Surface = SFS.World.Drag.Surface;

public class AeroMesh : MonoBehaviour
{
    static readonly int Temperature = Shader.PropertyToID("_Temperature");
    static readonly int Opacity = Shader.PropertyToID("_Opacity");
    const float SlopeReduction = 0.8f;

    // Refs
    [Required] public MeshFilter meshFilter;
    [Required] public MeshRenderer meshRenderer;
    
    public bool _drawDebug;

    
    void Start()
    {
        meshFilter.mesh = new Mesh();
        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 10;
    }
    void OnDestroy() => Destroy(meshFilter.mesh);


    public void SetShockOpacity(float shockOpacity)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetFloat(Opacity, shockOpacity);
        meshRenderer.SetPropertyBlock(block);
    }
    public void SetTemperature(float strength)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetFloat(Temperature, strength);
        meshRenderer.SetPropertyBlock(block);
    }


    public void GenerateMesh(Data A, List<Surface> exposedSurfaces)
    {
        if (!meshRenderer.enabled)
            return;
        
        List<Vector2> curveData = A.curveData;
        BasicMeshData data = A.data;
        
        // Inserts vertice for vertical gradient
        for (int i = 0; i < curveData.Count - 1; i++)
            if (curveData[i + 1].y > data.top_Fade)
            {
                curveData.Insert(i + 1, new Line2(curveData[i], curveData[i + 1]).GetPositionAtY(data.top_Fade));
                break;
            }
        
        // Data
        List<List<Vector2>> pointGroups = GetPoints(exposedSurfaces);

        // Curve M
        List<float>[] mGroups = GetM(pointGroups, A.sampleCurve, Line2.GetSlope_Abs(curveData[0], curveData[1]), A.data);


        // Insert side
        (float, float)[] fade = SideFade_Insert(pointGroups, mGroups, data);

        // Vertices
        Vector2[][][] vertices = GetVertices(curveData, pointGroups, mGroups);

        // Create mesh
        GenerateMesh(A, vertices, fade);
    }

    void GenerateMesh(Data A, Vector2[][][] verticeGroups, (float, float)[] fadeXGroups)
    {
        int levelCount = A.curveData.Count;

        float uv_RandomOffset = A.data.randomizeHorizontalUV? Random.value : 0;
        float height = A.curveData.Last().y - A.data.top_Fade;
        float[] levels_UV_Y = new float[levelCount].Select(Get_UV_Y).ToArray();
        float Get_UV_Y(float none, int i)
        {
            float y = A.curveData[i].y - (A.data.startTexAfterTopFade? A.data.top_Fade : 0);
            return y / height;
        }

        int verticeCount = (verticeGroups.Sum(points => points[0].Length - 1) * (levelCount - 1)) * 4;
        Vector3[] vertices = new Vector3[verticeCount];
        Color[] colors = new Color[verticeCount];
        Vector3[] uv = new Vector3[verticeCount];
        int[] indices = new int[verticeCount];

        // Main quads
        int ii = 0;
        for (int groupIndex = 0; groupIndex < verticeGroups.Length; groupIndex++)
        {
            Vector2[][] verticesRaw = verticeGroups[groupIndex];
            float[] uv_X = verticesRaw[levelCount / 2].Select(p => p.x / A.data.textureWidth + uv_RandomOffset).ToArray();

            // Fade side
            Vector2[] pointGroup = verticesRaw[0];
            (float fadeX_Left, float fadeX_Right) = fadeXGroups[groupIndex];
            float[] alphaX = verticesRaw.First().Select(p =>
            {
                float left = Mathf.InverseLerp(pointGroup.First().x, fadeX_Left, p.x);
                float right = Mathf.InverseLerp(pointGroup.Last().x, fadeX_Right, p.x);
                return Mathf.Min(left, right);
            }).ToArray();

            // Levels loop
            for (int levelIndex = 0; levelIndex < verticesRaw.Length - 1; levelIndex++)
            {
                Vector2[] points_LevelA = verticesRaw[levelIndex];
                Vector2[] points_LevelB = verticesRaw[levelIndex + 1];
                float uv_Y_A = levels_UV_Y[levelIndex];
                float uv_Y_B = levels_UV_Y[levelIndex + 1];

                // Front fade
                float alphaY_A = Mathf.Clamp01(A.curveData[levelIndex].y / A.data.top_Fade);
                float alphaY_B = Mathf.Clamp01(A.curveData[levelIndex + 1].y / A.data.top_Fade);

                // Points loop
                for (int i = 0; i < points_LevelA.Length - 1; i++)
                {
                    vertices[ii + 0] = points_LevelA[i];
                    vertices[ii + 1] = points_LevelA[i + 1];
                    vertices[ii + 2] = points_LevelB[i + 1];
                    vertices[ii + 3] = points_LevelB[i];

                    float[] uv_M = UV_Utility.GetQuadM(vertices[ii + 0], vertices[ii + 1], vertices[ii + 2], vertices[ii + 3]);

                    uv[ii + 0] = new Vector3(uv_X[i], uv_Y_A, 1) * uv_M[0];
                    uv[ii + 1] = new Vector3(uv_X[i + 1], uv_Y_A, 1) * uv_M[1];
                    uv[ii + 2] = new Vector3(uv_X[i + 1], uv_Y_B, 1) * uv_M[2];
                    uv[ii + 3] = new Vector3(uv_X[i], uv_Y_B, 1) * uv_M[3];

                    colors[ii + 0] = new Color(0, 0, alphaX[i], alphaY_A);
                    colors[ii + 1] = new Color(0, 0, alphaX[i + 1], alphaY_A);
                    colors[ii + 2] = new Color(0, 0, alphaX[i + 1], alphaY_B);
                    colors[ii + 3] = new Color(0, 0, alphaX[i], alphaY_B);

                    // Next quad
                    ii += 4;
                }
            }
        }

        // Indices
        for (int i = 0; i < verticeCount; i++)
            indices[i] = i;

        // Apply mesh
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Quads, 0);
        mesh.SetColors(colors);
        mesh.SetUVs(0, uv);
        mesh.RecalculateBounds();

        // Positions/rotates mesh to apply LocalToWorld
        transform.position = new Vector2(0, A.data.top_Move) * A.localToWorld;
        transform.eulerAngles = new Vector3(0, 0, A.velocityAngle_Rad * Mathf.Rad2Deg);
    }
    
    static List<List<Vector2>> GetPoints(List<Surface> exposedSurfaces)
    {
        if (exposedSurfaces.Count == 0)
            return new List<List<Vector2>>();
        
        List<List<Vector2>> output = new List<List<Vector2>> { new List<Vector2> { exposedSurfaces[0].line.start, exposedSurfaces[0].line.end }};

        int listIndex = 0;
        for (int i = 1; i < exposedSurfaces.Count; i++)
        {
            Line2 line = exposedSurfaces[i].line;

            const float thresholdSqrt = 0.03f * 0.03f;
            if ((output[listIndex].Last() - line.start).sqrMagnitude < thresholdSqrt) // Is connected
            {
                // Adds point
                output[listIndex].Add(line.end);
            }
            else
            {
                // Starts a new list
                output.Add(new List<Vector2> {line.start, line.end});
                listIndex++;
            }
        }

        return output;
    }
    List<float>[] GetM(List<List<Vector2>> pointGroups, Func<float, float> sampleCurve, float curveSlope, BasicMeshData data)
    {
        List<float>[] mGroups = new List<float>[pointGroups.Count];
        for (int groupIndex = 0; groupIndex < pointGroups.Count; groupIndex++)
        {
            List<Vector2> points = pointGroups[groupIndex];

            // Clamp
            int pointsCountMinusOne = points.Count - 1;
            List<(float, float)> clampM = new List<(float, float)>(new (float, float)[pointsCountMinusOne]);
            for (int i = 0; i < pointsCountMinusOne; i++)
            {
                float slope = -Line2.GetSlope(points[i], points[i + 1]);
                float maxM = curveSlope / slope * SlopeReduction;
                clampM[i] = (maxM < 0 ? maxM : -1, maxM > 0 ? maxM : 1);
            }

            // Skip surfaces
            if (data.skipSurfaces)
            {
                if (_drawDebug)
                    for (int i = 0; i < points.Count - 1; i++)
                        Debug.DrawLine(points[i], points[i + 1], new Color(0, 0, 0, 0.5f));

                // Left (goes right to left)
                for (int checkingIndex = points.Count - 2; checkingIndex >= 1; checkingIndex--)
                {
                    Vector2 checkingPoint = points[checkingIndex];
                    float m = Mathf.Clamp01(-clampM[checkingIndex].Item1 - 0.2f);

                    // Is it blocking
                    bool cannotSkip = false;
                    for (int i = checkingIndex - 1; i >= 0; i--)
                    {
                        Vector2 point = points[i];
                        if (point.y > checkingPoint.y - sampleCurve.Invoke((checkingPoint.x - point.x) / m))
                        {
                            cannotSkip = true;
                            break;
                        }
                    }

                    if (_drawDebug)
                        for (int i = 0; i < 50; i++)
                            Debug.DrawLine(checkingPoint + new Vector2(-i / 10f * m, -sampleCurve.Invoke(i / 10f)), checkingPoint + new Vector2(-(i + 1) / 10f * m, -sampleCurve.Invoke((i + 1) / 10f)), cannotSkip ? Color.red : Color.yellow);

                    // Removes start
                    if (!cannotSkip)
                    {
                        points.RemoveRange(0, checkingIndex);
                        clampM.RemoveRange(0, checkingIndex);
                        break;
                    }
                }

                // Right (goes left to right)
                int pointsCount = points.Count;
                for (int checkingIndex = 1; checkingIndex < pointsCount - 1; checkingIndex++)
                {
                    Vector2 checkingPoint = points[checkingIndex];
                    float m = Mathf.Clamp01(clampM[checkingIndex - 1].Item2 - 0.2f);

                    // Is it blocking
                    bool cannotSkip = false;
                    for (int i = checkingIndex + 1; i < pointsCount; i++)
                    {
                        Vector2 point = points[i];
                        if (point.y > checkingPoint.y - sampleCurve.Invoke((point.x - checkingPoint.x) / m))
                        {
                            cannotSkip = true;
                            break;
                        }
                    }

                    if (_drawDebug)
                        for (int i = 0; i < 50; i++)
                            Debug.DrawLine(checkingPoint + new Vector2(i / 10f * m, -sampleCurve.Invoke(i / 10f)), checkingPoint + new Vector2((i + 1) / 10f * m, -sampleCurve.Invoke((i + 1) / 10f)), cannotSkip ? Color.red : Color.yellow);

                    // Removes end
                    if (!cannotSkip)
                    {
                        int removeCount = pointsCount - checkingIndex - 1;
                        points.RemoveRange(checkingIndex + 1, removeCount);
                        clampM.RemoveRange(checkingIndex, removeCount);
                        break;
                    }
                }

                if (_drawDebug)
                    for (int i = 0; i < points.Count - 1; i++)
                        Debug.DrawLine(points[i], points[i + 1]);
            }

            // Carry clamp
            int clampMCount = clampM.Count;
            for (int i = 1; i < clampMCount; i++)
                clampM[i] = (Mathf.Max(clampM[i].Item1, clampM[i - 1].Item1), clampM[i].Item2); // Clamps left going left to right
            for (int i = clampM.Count - 2; i >= 0; i--)
                clampM[i] = (clampM[i].Item1, Mathf.Min(clampM[i].Item2, clampM[i + 1].Item2)); // Clamps right, going right to left

            // Calculate M
            float start = points.First().x;
            float end = points.Last().x;
            float m_Left = Mathf.Max(!data.reduceIfBelow || groupIndex == 0 || pointGroups[groupIndex - 1].Last().y < points.First().y ? -1f : -0.3f, clampM.First().Item1);
            float m_Right = Mathf.Min(!data.reduceIfBelow || groupIndex == pointGroups.Count - 1 || pointGroups[groupIndex + 1].Last().y < points.First().y ? 1f : 0.3f, clampM.Last().Item2);
            float GetM(float x) => Mathf.Lerp(m_Left, m_Right, Mathf.InverseLerp(start, end, x));
            
            int pointCount = points.Count;
            List<float> mGroup = new List<float>(new float[pointCount]);
            for (int i = 0; i < pointCount; i++)
            {
                float m = GetM(points[i].x);
                mGroup[i] = Mathf.Clamp(m, i < pointCount - 1 ? clampM[i].Item1 : -1, i > 0 ? clampM[i - 1].Item2 : 1);
            }

            mGroups[groupIndex] = mGroup;
        }

        return mGroups;
    }
    static (float, float)[] SideFade_Insert(List<List<Vector2>> pointGroups, List<float>[] mGroups, BasicMeshData data)
    {
        (float, float)[] fade = new (float, float)[pointGroups.Count];
        for (int groupIndex = 0; groupIndex < pointGroups.Count; groupIndex++)
        {
            List<Vector2> points = pointGroups[groupIndex];
            List<float> Ms = mGroups[groupIndex];

            if (data.extend)
            {
                fade[groupIndex] = (points[0].x, points[points.Count - 1].x);
                
                Vector2 sizeA = points[1] - points[0];
                points.Insert(0, points[0] - new Vector2(sizeA.x, Mathf.Max(sizeA.y, 0)) / sizeA.magnitude * data.side_FadeX);
                Ms.Insert(0, Mathf.LerpUnclamped(Ms[0], Ms[1], 0 - Mathf.Min(data.side_FadeX / sizeA.magnitude, 0.5f)));

                Vector2 sizeB = points[points.Count - 1] - points[points.Count - 2];
                points.Add(points[points.Count - 1] + new Vector2(sizeB.x, Mathf.Min(sizeB.y, 0)) / sizeB.magnitude * data.side_FadeX);
                Ms.Add(Mathf.LerpUnclamped(Ms[Ms.Count - 2], Ms[Ms.Count - 1], 1 + Mathf.Min(data.side_FadeX / sizeB.magnitude, 0.5f)));
            }
            else
            {
                float m_Left = Ms.First() + data.side_FadeM; // max M allowed
                float x_Left = points.First().x + data.side_FadeX; // max X allowed
                for (int i = 0; i < points.Count - 1; i++)
                    if (Ms[i + 1] > m_Left || points[i + 1].x > x_Left)
                    {
                        float tm = Mathf.InverseLerp(Ms[i], Ms[i + 1], m_Left);
                        float tx = Mathf.InverseLerp(points[i].x, points[i + 1].x, x_Left);
                        float t = Mathf.Min(tm, tx);
                        Vector2 point = Vector2.Lerp(points[i], points[i + 1], t);
                        points.Insert(i + 1, point);
                        Ms.Insert(i + 1, Mathf.Lerp(Ms[i], Ms[i + 1], t));
                        fade[groupIndex].Item1 = point.x;
                        break;
                    }

                float m_Right = Ms.Last() - data.side_FadeM; // min M allowed
                float x_Right = points.Last().x - data.side_FadeX; // min X allowed
                for (int i = points.Count - 1; i >= 1; i--)
                    if (Ms[i - 1] < m_Right || points[i - 1].x < x_Right)
                    {
                        float tm = Mathf.InverseLerp(Ms[i - 1], Ms[i], m_Right);
                        float tx = Mathf.InverseLerp(points[i - 1].x, points[i].x, x_Right);
                        float t = Mathf.Max(tm, tx);
                        Vector2 point = Vector2.Lerp(points[i - 1], points[i], t);
                        points.Insert(i, point);
                        Ms.Insert(i, Mathf.Lerp(Ms[i - 1], Ms[i], t));
                        fade[groupIndex].Item2 = point.x;
                        break;
                    }   
            }
        }

        return fade;
    }

    static Vector2[][][] GetVertices(List<Vector2> curveData, List<List<Vector2>> pointGroups, List<float>[] M)
    {
        Vector2[][][] levelGroups = new Vector2[pointGroups.Count][][];

        for (int groupIndex = 0; groupIndex < pointGroups.Count; groupIndex++)
        {
            // Levels loop
            List<Vector2> points = pointGroups[groupIndex];
            List<float> m = M[groupIndex];
            int pointCount = points.Count;
            int levelCount = curveData.Count;
            
            Vector2[][] levels = new Vector2[levelCount][];
            for (int y = 0; y < levelCount; y++)
            {
                Vector2 curvePoint = curveData[y];
                
                Vector2[] level = new Vector2[pointCount];
                
                for (int x = 0; x < pointCount; x++)
                    level[x] = points[x] + new Vector2(curvePoint.x * m[x], -curvePoint.y);

                levels[y] = level;
            }

            levelGroups[groupIndex] = levels;
        }

        return levelGroups;
    }

    public struct Data
    {
        public float velocityAngle_Rad;
        public Matrix2x2 localToWorld;
        
        public BasicMeshData data;
        public List<Vector2> curveData;
        public Func<float, float> sampleCurve;
    }
}