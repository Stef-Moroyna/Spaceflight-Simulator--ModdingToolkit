using SFS.Translations;
using SFS.Variables;
using SFS.World.Drag;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class HeatModule : MonoBehaviour, I_HeatModule
    {
        // Refs
        public HeatTolerance heatTolerance;
        public bool isHeatShield;
        [Space]
        public bool useCustomName;
        [ShowIf(nameof(useCustomName))] public TranslationVariable customName;
        
        // State
        [Space]
        public Float_Reference temperature = new Float_Reference { localValue = float.NegativeInfinity };
        
        
        Part part;
        void Start() => part = transform.GetComponentInParentTree<Part>();

        
        // Implementation
        string I_HeatModule.Name
        {
            get
            {
                if (useCustomName)
                    return customName.Field;
                
                if (part == null)
                    part = transform.GetComponentInParentTree<Part>();
                    
                return part.GetDisplayName();
            }
        }
        bool I_HeatModule.IsHeatShield => isHeatShield;
        float I_HeatModule.Temperature { get => temperature.Value; set => temperature.Value = value; }
        int I_HeatModule.LastAppliedIndex { get; set; } = -1;
        float I_HeatModule.ExposedSurface { get; set; } = 0;
        float I_HeatModule.HeatTolerance => AeroModule.GetHeatTolerance(heatTolerance);
        void I_HeatModule.OnOverheat(bool breakup) => part.OnOverheat(this, breakup);
    }
}