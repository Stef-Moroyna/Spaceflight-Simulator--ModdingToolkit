using SFS.Translations;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class HeatModule : MonoBehaviour
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
    }
    
    public enum HeatTolerance
    {
        Low,
        Mid,
        High,
    }
}