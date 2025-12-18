using SFS.Parts.Modules;
using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace InterplanetaryModule
{
    public class AtmosphericFlameModule : MonoBehaviour, Rocket.INJ_Location
    {
        public float maxPressure = 0.005f;
        public MoveModule moveModule;
        Location location;

        Location Rocket.INJ_Location.Location
        {
            set
            {
                try { location = value; } catch { }
            }
        }

        void Update()
        {
            if (location == null) return;
            float pressure = (float) location.planet.GetAtmosphericDensity(location.Height);
            moveModule.time.Value = Mathf.Clamp01(pressure / maxPressure);
        }
    }
}
