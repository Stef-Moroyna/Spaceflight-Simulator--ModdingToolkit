using System.Collections.Generic;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.World.Drag;
using Sirenix.OdinInspector;
using UnityEngine;

public class BurnManager : MonoBehaviour
{
    // Refs
    [Required] public PartHolder partHolder;

    // State
    [ShowInInspector, ReadOnly] float currentMarkOpacity = 1;
    
    
    public void ApplyBurnMarks(List<Surface> topSurfaces, float temperature, Matrix2x2 localToWorld, float velocityAngleRad, int frameIndex)
    {
        // Mark visibility (hides while below flames)
        float markOpacityRaw = 1 - Mathf.InverseLerp(800, 2500, temperature);
        float markOpacity = Mathf.Round(markOpacityRaw * 20) / 20;

        // Burn applying
        float markIntensity = AeroModule.GetIntensity(temperature - 400, 600) * 1.5f;
        if (markIntensity > 0.1f)
        {
            BurnMark burnMark = GetBurn(frameIndex, out bool newBurnMark);

            // Should apply
            bool applyBurn;
            if (newBurnMark || markIntensity - 0.05f > burnMark.burn.intensity)
            {
                // New apply // More intense
                applyBurn = true;
            }
            else if (markIntensity > burnMark.burn.intensity * 0.7f && Mathf.Abs(Mathf.DeltaAngle(burnMark.GetAngleRadWorld() * Mathf.Rad2Deg, velocityAngleRad * Mathf.Rad2Deg)) > 20)
            {
                // Update angle
                applyBurn = true;
                markIntensity = burnMark.burn.intensity;
            }
            else
                applyBurn = false;
            
        }

        // Hides updating
        if (markOpacity != currentMarkOpacity)
        {
            currentMarkOpacity = markOpacity;
            ApplyOpacity();
        }
    }

    public void ApplyOpacity()
    {
        foreach (Part part in partHolder.parts)
            if (part.burnMark != null)
                part.burnMark.SetOpacity(currentMarkOpacity, false);
    }
    
    BurnMark GetBurn(int frameIndex, out bool newBurnMark)
    {
        List<Part> parts = partHolder.parts;
        Part part = parts[frameIndex % parts.Count];

        newBurnMark = part.burnMark == null;
        if (newBurnMark)
            part.burnMark = part.gameObject.AddComponent<BurnMark>();
        
        return part.burnMark;
    }
}