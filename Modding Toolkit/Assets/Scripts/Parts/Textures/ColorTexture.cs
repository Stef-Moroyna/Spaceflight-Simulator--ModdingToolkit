using System;
using System.Collections.Generic;
using SFS.Parts.Modules;
using Sirenix.OdinInspector;
using UnityEngine;
using UV;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class ColorTexture : TextureAssetBase
    {
        [HideIf(nameof(multiple)), BoxGroup, InlineProperty, HideLabel] public PartTexture colorTex;
        [Space]
        public string[] tags;
        public bool pack_Redstone_Atlas;
        
        protected override PartTexture Texture => colorTex;
    }

    [Serializable]
    public class Segment
    {
        [BoxGroup] public float height;
        [Space]
        [BoxGroup, InlineProperty, HideLabel] public PartTexture texture;
    }

    public abstract class TextureAssetBase : ScriptableObject
    {
        public bool multiple;
        [ShowIf(nameof(multiple)), Space] public Segment[] segments;
        
        public List<StartEnd_UV> Get_UV(Pipe shape, Line segment, float shapeWidth, Transform meshHolder, Vector2 lightDirection)
        {
            if (multiple)
            {
                List<StartEnd_UV> output = new List<StartEnd_UV>();

                float start = segment.start;
                foreach (Segment a in segments)
                {
                    float end = a.height > 0? segment.start + a.height : segment.end + a.height;
                    Line localSegment = new Line(start, end);
                    start = end;
                    
                    output.AddRange(a.texture.Get_UV(shape, localSegment, shapeWidth, meshHolder, lightDirection));
                }
                
                return output;
            }

            return Texture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection);
        }

        public PartTexture GetTexID() => multiple ? segments[0].texture : Texture;
        protected abstract PartTexture Texture { get; }
    }
}