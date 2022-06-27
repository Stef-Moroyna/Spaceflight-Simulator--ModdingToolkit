using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ShapeTexture : ScriptableObject
    {
        [BoxGroup("Shape Tex"), InlineProperty, HideLabel] public PartTexture shapeTex;
        [BoxGroup("Shadow Tex"), InlineProperty, HideLabel] public ShadowTexture shadowTex;

        public string[] tags;
    }
}