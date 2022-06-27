using System;
using SFS;
using SFS.World.Drag;
using UnityEngine;

[Serializable]
public struct AeroFormula
{
    public float velPow;
    public float densityPow;
    public float tempOffset;
    public float m;

    public void GetEverything(double velocity, double velocity_Y, double density, float startHeatingVelocityMultiplier, float shockwaveM, out float Q, out float shockOpacity, out float temperature, out float pure)
    {
        Q = GetQ(velocity, density);
        temperature = GetTemperature(velocity, velocity_Y, density, startHeatingVelocityMultiplier);
        shockOpacity = GetShockOpacity(Q, (float)velocity, (float)density, shockwaveM, temperature, out pure);
    }
    
    float GetShockOpacity(float Q, float velocity, float density, float shockwaveM, float temperature, out float pure)
    {
        float drag = Q / 50;
        pure = Apply();
        
        // Clamps
        float minDensity = density * 2000 - 0.2f;
        float minVelocity = Mathf.Max((velocity - 100) / 80, 0);
        float min_Temperature = temperature > 0? temperature * -0.006f : temperature * -0.0006f;
        
        foreach (float max in new [] { minDensity, minVelocity, min_Temperature })
            drag = Mathf.Min(drag, Mathf.Max(max, 0));

        return Apply() * shockwaveM;
        float Apply() => AeroModule.GetIntensity(drag, 3) * 2f;
    }
    float GetTemperature(double velocity, double velocity_Y, double density, float minHeatVelocity)
    {
            
        // Reduces ascend temp in central atmosphere
        if (velocity_Y > 0)
            velocity -= Math.Min(velocity_Y * 2.5f, velocity * 0.5);

        // Calculates temp
        float temp = (float)(Math.Pow(velocity, velPow) * Math.Pow(density, 1 / densityPow)) / m;
        
        // Reduces temp in lower and ascend temp in upper atmosphere
        temp += tempOffset * (float)(0.2 + (velocity_Y > 0? Math.Min(velocity_Y / velocity * 2, 0.4) : 0));

        // Min velocity for heat
        float maxTemp = ((float)velocity - minHeatVelocity) * 6;
        if (temp > maxTemp)
            temp = maxTemp;

        if (temp > 2000)
            temp = 2000 + (temp - 2000) / 1.5f;
        
        return temp > 0? temp : 0;
    }
    float GetQ(double velocity, double density)
    {
        return (float)(velocity * velocity * density);
    }
}