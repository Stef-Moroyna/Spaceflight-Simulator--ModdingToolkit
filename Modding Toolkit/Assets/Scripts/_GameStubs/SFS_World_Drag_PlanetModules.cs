// Auto-generated compile-only dummy stubs for the public modding toolkit. No implementation, no anti-hack.
using UnityEngine;

namespace SFS.World.Drag
{
    // NOTE: HeatModuleBase and Valid are NOT defined here.
    // They already exist in the toolkit at Parts/Utility/HeatModuleBase.cs.

    public enum HeatTolerance
    {
        Low,
        Mid,
        High,
    }

    public abstract class AeroModule : MonoBehaviour
    {
        public static float GetHeatTolerance(HeatTolerance a) { return default; }
    }
}

namespace SFS.World.PlanetModules
{
    public class PostProcessingModule
    {
        public Key Evaluate(float height) { return default; }

        public class Key
        {
            public float height;
            public float shadowIntensity;
            public float starIntensity;
            public float hueShift;
            public float saturation;
            public float contrast;
            public float red;
            public float green;
            public float blue;
        }
    }
}
