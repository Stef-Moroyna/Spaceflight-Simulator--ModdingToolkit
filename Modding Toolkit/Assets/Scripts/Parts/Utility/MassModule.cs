using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class MassModule : MonoBehaviour, I_InitializePartModule
    {
        // Input variables
        [Required, BoxGroup("Input", false)] public PolygonData polygonModule;
        [BoxGroup("Input", false)] public float density = 1.25f;
        // Output variables
        [BoxGroup("Output", false)] public Float_Reference mass;

        // Initialization
        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize()
        {
            polygonModule.onChange += RecalculateMass;
        }

        // On change
        [Button] void RecalculateMass()
        {
            float newMass = (GetArea() * density).Round(0.01f);

            if (float.IsNaN(newMass) || newMass == 0)
                newMass = 0.01f;

            mass.Value = newMass;
        }
        float GetArea()
        {
            Vector2[] points = polygonModule.polygon.vertices;

            float area = 0;

            for (int i = 0; i < points.Length - 1; i++)
                area += (points[i + 1].x - points[i].x) * (points[i + 1].y + points[i].y) / 2;

            area += (points[0].x - points[points.Length - 1].x) * (points[0].y + points[points.Length - 1].y) / 2;

            return Mathf.Abs(area);
        }
    }
}