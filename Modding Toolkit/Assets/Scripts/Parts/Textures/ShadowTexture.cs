using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ShadowTexture : TextureAssetBase
    {
        [HideIf(nameof(multiple)), BoxGroup, InlineProperty, HideLabel] public PartTexture texture;

        protected override PartTexture Texture => texture;
    }
}