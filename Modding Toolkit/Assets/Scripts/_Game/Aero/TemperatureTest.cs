using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class TemperatureTest : MonoBehaviour
{
    public ReentryData[] recordings;
    public AeroFormula formula;
    
    void Update()
    {
        if (!Application.isEditor)
            return;

        for (int recordingIndex = 0; recordingIndex < recordings.Length; recordingIndex++)
        {
            Vector3 pos = transform.position + Vector3.down * (recordingIndex * 10);
            ReentryData recording = recordings[recordingIndex];

            // Base
            //Draw(recording.points.Select(a => (float)a.height * 1000), Color.magenta);
            Draw(recording.points.Select(a => (float)a.density * 1000 / 2), Color.blue);


            // Targets
            (float Q, float shockOpacity, float temperature, float pure)[] values = recording.points.Select(a =>
            {
                formula.GetEverything(a.velocity, a.velocity_Y, a.density, (float)recording.startHeatingVelocityMultiplier, recording.shockwaveM, out float Q, out float shockOpacity, out float temperature, out float pure);
                return (Q, shockOpacity, temperature, pure);
            }).ToArray();
            
            Draw(values.Select(a => a.Q / 20), Color.cyan);
            //Draw(values.Select(a => a.shockOpacity), Color.white);
            //Draw(values.Select(a => a.pure), Color.green);
            Draw(values.Select(a => a.temperature / 1000), Color.red);
            

            void Draw(IEnumerable<float> a, Color c)
            {
                float[] points = a.ToArray();
                for (int ii = 0; ii < points.Length - 1; ii++)
                    Debug.DrawLine(pos + new Vector3(ii / 30f, points[ii]), pos + new Vector3((ii + 1) / 30f, points[ii + 1]), c);
            }
        }
    }
}

[Serializable]
public struct ReentryData
{
    public string name;
    public float shockwaveM;
    public double startHeatingVelocityMultiplier;
    public double atmosphereHeight;
    [TableList] public List<Point> points;
}
[Serializable]
public struct Point
{
    public double height, velocity, velocity_Y, density;
}

// double max = 0;
// foreach (ReentryData data in recordings)
// foreach (double t in data.points.Select(a => temp.GetTemp(a.velocity, a.velocity_Y, a.density, 1)))
//     max = Math.Max(t, max);
// temp.m = (float) max / 6;