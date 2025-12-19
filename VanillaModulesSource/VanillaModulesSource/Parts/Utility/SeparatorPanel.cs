using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class SeparatorPanel : MonoBehaviour, I_InitializePartModule
    {
        public Float_Reference height;
        public Transform panel, arrow;
        public PipeMesh pipeMesh;
        public ColorTexture defaultTexture;

        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            height.OnChange += Position;
            pipeMesh.onSetColorTexture += Enable;
            FlipToLight();
        }
        
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
            
            if (GameManager.main != null && transform.root.childCount > 0 && transform.root.GetChild(0).name == "Parts Holder")
                return transform.root.GetChild(0).TransformDirection(a);

            return a;
        }
    }
}