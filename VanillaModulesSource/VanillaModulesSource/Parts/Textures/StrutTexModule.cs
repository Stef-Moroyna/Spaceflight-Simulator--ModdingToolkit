using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class StrutTexModule : MonoBehaviour, I_InitializePartModule
    {
        public ColorTexture reference;

        public Float_Reference input;
        public Float_Reference output;

        public int Priority => 12;
        public void Initialize() => input.OnChange += Calculate;
        
        [Button] void Calculate()
        {
            PartTexture tex = reference.colorTex;
            float aspectRatio = (float)tex.textures[0].texture.height / tex.textures[0].texture.width;
            
            // m -> uv
            float totalHeight = input.Value / aspectRatio;
            
            float borderSize = (tex.border_Bottom.uvSize + tex.border_Top.uvSize);
            float centerSize = 1 - borderSize;
            float currentCenterSize = totalHeight - borderSize;
            int segmentCount = Mathf.RoundToInt(currentCenterSize / centerSize + 0.25f);
            
            output.Value = ((segmentCount * centerSize) + borderSize) * aspectRatio; // uv -> m
            transform.localScale = new Vector2(1, input.Value / output.Value); // Scales to compensate
        }
    }
}