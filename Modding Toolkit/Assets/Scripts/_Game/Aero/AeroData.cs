using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AeroData : MonoBehaviour
{
    // Test
    public bool testShock;
    public float shockOpacity;
    public bool testReentry;
    public float reentryPercent;
    
    [BoxGroup("s", false)] public StraightMesh shock_Edge;
    [BoxGroup("s", false)] public StraightMesh shock_Outer;
    
    [BoxGroup("r", false)] public StraightMesh reentry_Edge;
    [BoxGroup("r", false)] public CurvedMesh reentry_Outer;

    public TemperatureTest formulaHolder;
    public AeroFormula Formula => formulaHolder.formula;
}

[Serializable]
public class CurvedMesh : BasicMeshData
{
    [BoxGroup] public float tail_InitialSlope, tail_InitialVelocity, tail_Acceleration, tail_Scale = 1;

    public List<Vector2> GetCurveData()
    {
        Vector2 tail_InitialVelocityVector = new Vector2(1, tail_InitialSlope).normalized * tail_InitialVelocity;
        
        return new Vector2[tail_Resolution].Select((_, i) =>
        {
            float t = i / (float)(tail_Resolution - 1);
            return new Vector2(tail_InitialVelocityVector.x * t, tail_InitialVelocityVector.y * t + (tail_Acceleration * t * t)) * tail_Scale;
        }).ToList();
    }
    
    public Func<float, float> SampleCurve()
    {
        Vector2 tail_InitialVelocityVector = new Vector2(1, tail_InitialSlope).normalized * tail_InitialVelocity;

        return x =>
        {
            float t = x / (tail_InitialVelocityVector.x * tail_Scale);
            return (tail_InitialVelocityVector.y * t + (tail_Acceleration * t * t)) * tail_Scale;  
        };
    }
}

[Serializable]
public class StraightMesh : BasicMeshData
{
    public Vector2 size;
    public List<Vector2> GetCurveData() => new List<Vector2> { Vector2.zero, size };
}

[Serializable]
public class BasicMeshData
{
    [BoxGroup("a", false)] public float top_Move, top_Fade;
    [BoxGroup("b", false)] public bool extend;
    [BoxGroup("b", false)] public float side_FadeX, side_FadeM;
    
    [BoxGroup("c", false)] public float textureWidth;
    [BoxGroup("c", false)] public bool randomizeHorizontalUV;
    [BoxGroup("c", false)] public bool startTexAfterTopFade, skipSurfaces, reduceIfBelow;
    [BoxGroup("c", false)] public int tail_Resolution;
}