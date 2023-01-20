using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class SeparatorPanel : MonoBehaviour
    {
        public Float_Reference height;
        public Transform panel, arrow;
        public PipeMesh pipeMesh;
        public ColorTexture defaultTexture;

        [Button]
        void Position()
        {
            float h = height.Value > 0.7f? height.Value : 0;
            panel.localPosition = new Vector2(0, 0.25f + h);
            arrow.localPosition = new Vector2(0, h);   
        }
        void Enable()
        {
            bool active = pipeMesh.textures.texture.colorTexture == defaultTexture;
            panel.gameObject.SetActive(active);
            arrow.gameObject.SetActive(active);
        }
        
        [Button]
        public void FlipToLight()
        {
            Vector2 lightDirection = GetLightDirection();
            float scaleX = Vector2.Angle(transform.TransformVector(Vector2.left), lightDirection) < 90? -1 : 1;
            float scaleY = Vector2.Angle(transform.TransformVector(Vector2.up), lightDirection) > 90? -1 : 1;
            panel.localScale = new Vector2(scaleX, scaleY);
        }
        Vector2 GetLightDirection()
        {
            Vector2 a = new Vector2(-1, 1);

            return a;
        }
    }
}