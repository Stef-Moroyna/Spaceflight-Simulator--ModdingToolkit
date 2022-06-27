using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ColorTexture : ScriptableObject
    {
        [BoxGroup("Color Tex"), InlineProperty, HideLabel] public PartTexture colorTex;
        public string[] tags;
    }
}