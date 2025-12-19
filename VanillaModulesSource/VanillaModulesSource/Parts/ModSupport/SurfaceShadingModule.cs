using SFS.World;
using SFS.World.PlanetModules;
using UnityEngine;

namespace InterplanetaryModule
{
    public class SurfaceShadingModule : MonoBehaviour, Rocket.INJ_Location
    {
        public Texture2D shadowTexture;
        public SpriteRenderer spriteRenderer;
        public Transform rotationTransform;
        public Color baseColor = Color.white;

        Location location;
        Location Rocket.INJ_Location.Location
        {
            set => location = value;
        }

        void Update()
        {
            float height = (float)(location.position.magnitude - location.planet.Radius);
            PostProcessingModule.Key keys = location.planet.data.postProcessing.Evaluate(height);

            float initialAngle = transform.localEulerAngles.y;
            float currentAngle = 0;
            
            if (rotationTransform != null)
                currentAngle = rotationTransform.localEulerAngles.y;
            
            float t = ((currentAngle + initialAngle) / 360f) % 1;
            t = t < 0 ? t + 1 : t;
            if (0.25 <= t && t < 0.5) t += 0.5f;
            if (0.5 <= t && t < 0.75) t -= 0.5f;
            Color color = new(0, 0, 0, 0);
            if ((0 <= t && t < 0.25) || (0.75 <= t && t < 1))
            {
                if (t > 0.5) t -= 1;
                t = (-t + 0.25f) * 2;
                float shadow = 1 - shadowTexture.GetPixel((int)(t * shadowTexture.width), 0).r;
                shadow = 1 - Mathf.Clamp01(shadow * keys.shadowIntensity);
                color = new Color(baseColor.r * shadow, baseColor.g * shadow, baseColor.b * shadow);
            }
            spriteRenderer.color = color;
            // Debug.Log(initialAngle.Value + " -> " + sign * rotationTransform.localEulerAngles.y + ", " + t);
        }
    }
}