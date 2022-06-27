using System.Collections.Generic;
using System.Linq;
using SFS.Parts;
using UnityEngine;

namespace SFS.World.Drag
{
    public class HeatManager : MonoBehaviour
    {
        // Const
        const float AbsorptionRate = 0.02f;
        const float DissipationRate = 0.01f;
        
        // State
        List<I_HeatModule> heated = new List<I_HeatModule>();

        // Update
        public void ApplyHeat(List<Surface> exposedSurfaces, float temperature, int frameIndex)
        {
            float rate = AbsorptionRate;
            
            // Collects
            foreach (Surface surface in exposedSurfaces)
                surface.owner.ExposedSurface += surface.line.end.x - surface.line.start.x;
            
            // Applies heat
            foreach (Surface surface in exposedSurfaces)
            {
                I_HeatModule heatModule = surface.owner;
                if (heatModule.LastAppliedIndex == frameIndex) // Already applied heat this frame
                    continue;
                
                // Adds to heated list
                if (float.IsNegativeInfinity(heatModule.Temperature)) 
                {
                    heatModule.Temperature = 0;
                    heated.Add(heatModule);
                }
                
                // Is plasma hotter
                float diff = temperature - heatModule.Temperature;
                if (diff <= 0)
                    continue;
                
                // Applies heat
                float rateFromArea = 1 + Mathf.Log10(heatModule.ExposedSurface + 1);
                float rateFromDiff = diff < 1000? diff : (diff * diff / 1000);
                heatModule.Temperature += rateFromArea * rateFromDiff * rate;
                heatModule.LastAppliedIndex = frameIndex;
                
                // Destroy
                if (heatModule.Temperature > heatModule.HeatTolerance * 1.03f) ;
            }
            
            // Reset
            foreach (Surface surface in exposedSurfaces)
                surface.owner.ExposedSurface = 0;
        }
        public void DissipateHeat(int frameIndex)
        {
            float rate = DissipationRate;
            float fixedRate = 10;

            for (int i = heated.Count - 1; i >= 0; i--)
            {
                I_HeatModule module = heated[i];

                if (module.LastAppliedIndex == frameIndex) // Heat was applied this frame
                    continue;
                
                float temperature = module.Temperature;
                if (temperature >= 0)
                    module.Temperature -= fixedRate + temperature * rate;

                if (module.Temperature <= 0)
                {
                    module.Temperature = float.NegativeInfinity;
                    heated.RemoveAt(i);
                }
            }
        }
        
        // Parts setup
        public void OnSetParts(Part[] newParts)
        {
            heated.Clear();

            foreach (Part part in newParts)
            foreach (I_HeatModule module in part.GetModules<I_HeatModule>())
            {
                if (float.IsNegativeInfinity(module.Temperature))
                    continue; // Unheated

                if (module.Temperature < 0 || module.Temperature > 1000000)
                {
                    // Invalid
                    module.Temperature = float.NegativeInfinity;
                    continue;
                }

                heated.Add(module);
            }
        }
        
        // UI
        public List<I_HeatModule> GetMostHeatedModules(int count)
        {
            List<I_HeatModule> best = new List<I_HeatModule>();
            foreach (I_HeatModule module in heated)
            {
                int target = GetTargetIndex(module);
                
                if (target > count - 1)
                    continue;
                
                best.Insert(target, module);
            }
            return best.Take(Mathf.Min(best.Count, count)).ToList();
            
            int GetTargetIndex(I_HeatModule module)
            {
                float heatScore = GetHeatScore(module);
                
                int i = 0;
                for (; i < best.Count; i++)
                    if (heatScore - 0.01f > GetHeatScore(best[i]))
                        return i;
                
                return i;
            }
            
            float GetHeatScore(I_HeatModule module)
            {
                float percent = module.Temperature / module.HeatTolerance;
                return module.IsHeatShield? Mathf.Lerp(0.35f, 1f, percent) : percent; // Gives heat shields a higher score
            }
        }


        public void HeatPart(I_HeatModule a)
        {
            // Adds to heated list
            if (float.IsNegativeInfinity(a.Temperature)) 
            {
                a.Temperature = 0;
                heated.Add(a);
            }
            
            // Applies heat
            a.Temperature += 150 * Time.fixedDeltaTime;
            
            // Destroy
            if (a.Temperature > a.HeatTolerance * 1.03f) ;
        }
    }
}