using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ShapeTexture : TextureAssetBase
    {
        [HideIf(nameof(multiple)), BoxGroup, InlineProperty, HideLabel] public PartTexture shapeTex;
        [Space]
        
        [BoxGroup("Shadow Tex"), InlineProperty, HideLabel] public ShadowTexture shadowTex;
        
        public string[] tags;
        public bool pack_Redstone_Atlas;
        
        protected override PartTexture Texture => shapeTex;
    }
}