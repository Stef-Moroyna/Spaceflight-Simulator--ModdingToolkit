using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ShadowTexture : ScriptableObject
    {
        [BoxGroup("Shadow Tex"), InlineProperty, HideLabel] public PartTexture texture;
    }
}