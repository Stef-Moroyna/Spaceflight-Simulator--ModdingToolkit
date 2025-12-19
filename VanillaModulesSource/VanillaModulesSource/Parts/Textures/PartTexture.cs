using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UV;
using SFS.Parts.Modules;
using UnityEngine.Serialization;

namespace SFS.Parts
{
    [Serializable]
    public class PartTexture
    {
        [TableList] public PerValueTexture[] textures;
        [Space]
        public BorderData border_Bottom;
        public BorderData border_Top;
        [Space]
        [LabelText("Mode")] public CenterData center;
        [Space]
        public bool fixedWidth;
        [ShowIf("fixedWidth"), FormerlySerializedAs("width")] public float fixedWidthValue = 1;
        [Space]
        public bool flipToLight_X = true;
        public bool flipToLight_Y = true;
        [Space]
        public bool metalTexture;
        [Space]
        public Sprite icon;

        // Get
        public List<StartEnd_UV> Get_UV(Pipe shape, Line segment, float shapeWidth, Transform meshHolder, Vector2 lightDirection)
        {
            if (shape.points.Count < 2)
                return new List<StartEnd_UV>();

            // Prepares UV channel
            Texture2D texture = GetBestTexture(shapeWidth);
            Line2 texture_UV = Line2.StartSize(Vector2.zero, Vector2.one);

            // Adjusts to lighting
            if (flipToLight_X && Vector2.Angle(meshHolder.TransformVector(Vector2.left), lightDirection) > 90)
                texture_UV.FlipHorizontally();
            if (flipToLight_Y && Vector2.Angle(meshHolder.TransformVector(Vector2.up), lightDirection) > 90)
                texture_UV.FlipVertically();


            // Defines segment sizes
            Line bottom_Segment = new Line(segment.start, segment.start + GetBottomBorderSize(shape, segment, texture));
            Line top_Segment = new Line(segment.end - GetTopBorderSize(shape, segment, texture), segment.end);
            Line center_Segment = new Line(bottom_Segment.end, top_Segment.start);


            // Output
            List<StartEnd_UV> output = new List<StartEnd_UV>();
            void AddQuadClamped(Line cut, Line vertical_UV, Line2 uv, Line clamp, Texture2D tex, PartTexture data, float fixedWidthValue)
            {
                Line clampedCut = new Line(Mathf.Clamp(cut.start, clamp.start, clamp.end), Mathf.Clamp(cut.end, clamp.start, clamp.end));
                Line clampedUV = new Line(vertical_UV.Lerp(Mathf.InverseLerp(cut.start, cut.end, clampedCut.start)), vertical_UV.Lerp(Mathf.InverseLerp(cut.start, cut.end, clampedCut.end)));

                AddQuad(clampedCut, clampedUV, uv, tex, data, fixedWidthValue);
            }
            void AddQuad(Line cut, Line vertical_UV, Line2 uv, Texture2D tex, PartTexture data, float fixedWidthValue)
            {
                if (cut.Size > 0)
                    output.Add(new StartEnd_UV(cut, vertical_UV, uv, tex, data, fixedWidthValue));
            }
            
            
            // Bottom segment
            AddQuad(bottom_Segment, new Line(0, GetCenterUV().start), texture_UV, texture, this, fixedWidthValue);

            // Center segments
            if (center.mode == CenterData.CenterMode.Stretch)
            {
                AddQuad(center_Segment, GetCenterUV(), texture_UV, texture, this, fixedWidthValue);
            }
            else if (center.mode == CenterData.CenterMode.Logo)
            {
                float centerSize = GetCenterSize(center_Segment.Lerp(center.logoHeightPercent));
                float fixedWidthValue_Scaled = fixedWidthValue;

                if (center.scaleLogoToFit && centerSize > center_Segment.Size * 0.85f)
                {
                    float centerSize_Scaled = Mathf.Max(centerSize * 0.75f, center_Segment.Size * 0.85f);
                    fixedWidthValue_Scaled *= centerSize_Scaled / centerSize;
                    centerSize = centerSize_Scaled;
                }

                Line centerCut = Line.StartSize(Mathf.Lerp(center_Segment.start, center_Segment.end - centerSize, center.logoHeightPercent), centerSize);

                AddQuad(new Line(center_Segment.start, centerCut.start), Line.CenterSize(GetCenterUV().start, 0), texture_UV, texture, this, fixedWidthValue); // Bottom connection
                AddQuadClamped(centerCut, GetCenterUV(), texture_UV, center_Segment, texture, this, fixedWidthValue_Scaled); // Center logo quad
                AddQuad(new Line(centerCut.end, center_Segment.end), Line.CenterSize(GetCenterUV().end, 0), texture_UV, texture, this, fixedWidthValue); // Top connection
            }
            else if (center.mode == CenterData.CenterMode.Tile)
            {
                float centerSize = GetCenterSize(center_Segment.start);
                
                for (int i = 0; i < Mathf.Ceil(center_Segment.Size / centerSize); i++)
                    AddQuadClamped(Line.StartSize(center_Segment.start + centerSize * i, centerSize), GetCenterUV(), texture_UV, center_Segment, texture, this, fixedWidthValue);
            }
            //
            float GetCenterSize(float height) => center.sizeMode == VerticalSizeMode.Aspect? GetCenterUV().Size * GetAspectRatio(texture) * (fixedWidth? fixedWidthValue : shape.GetWidthAtHeight(height).magnitude) : center.size;

            // Top segment
            AddQuad(top_Segment, new Line(GetCenterUV().end, 1), texture_UV, texture, this, fixedWidthValue);

            return output;
        }
        
        public Texture2D GetBestTexture(float shapeWidth)
        {
            PerValueTexture best = textures[0];

            foreach (PerValueTexture texture in textures)
                if (Mathf.Abs(texture.ideal - shapeWidth) <= Mathf.Abs(best.ideal - shapeWidth))
                    best = texture;

            return best.texture;
        }
        
        float GetBottomBorderSize(Pipe shape, Line segment, Texture texture)
        {
            if (border_Bottom.uvSize == 0)
                return 0;

            float size = border_Bottom.sizeMode == VerticalSizeMode.Fixed ? border_Bottom.size : (border_Bottom.uvSize * GetAspectRatio(texture) * shape.GetWidthAtHeight(segment.start).magnitude);
            return Mathf.Min(size, segment.Size / 2);
        }
        float GetTopBorderSize(Pipe shape, Line segment, Texture texture)
        {
            if (border_Top.uvSize == 0)
                return 0;

            float size = border_Top.sizeMode == VerticalSizeMode.Fixed ? border_Top.size : (border_Top.uvSize * GetAspectRatio(texture) * shape.GetWidthAtHeight(segment.end).magnitude);
            return Mathf.Min(size, segment.Size / 2);
        }
        
        float GetAspectRatio(Texture texture)
        {
            return (float)texture.height / texture.width;
        }
        
        Line GetCenterUV()
        {
            return new Line(border_Bottom.uvSize, 1 - border_Top.uvSize);
        }
    }

    // Texture
    [Serializable]
    public class PerValueTexture
    {
        public Texture2D texture;
        public float ideal;    
    }

    // Vertical
    [Serializable, InlineProperty] public class BorderData
    {
        [HorizontalGroup, HideLabel, SuffixLabel("UV   ")] public float uvSize;

        [HorizontalGroup, HideLabel, ShowIf(nameof(Valid))] public VerticalSizeMode sizeMode;
        [HorizontalGroup, HideLabel, ShowIf(nameof(Valid)), ShowIf("sizeMode", VerticalSizeMode.Fixed), SuffixLabel("m   ")] public float size = 0.5f;

        bool Valid => uvSize > 0;
    }
    [Serializable, InlineProperty] public class CenterData
    {
        [HideLabel] public CenterMode mode;

        [HorizontalGroup, HideLabel, ShowIf(nameof(SizeOptions))] public VerticalSizeMode sizeMode;
        [HorizontalGroup, HideLabel, ShowIf(nameof(SizeOptions)), ShowIf(nameof(sizeMode), VerticalSizeMode.Fixed), SuffixLabel("m   ")] public float size = 0.5f;

        [ShowIf(nameof(mode), CenterMode.Logo), Range(0, 1)] public float logoHeightPercent = 0.5f;
        [ShowIf(nameof(mode), CenterMode.Logo)] public bool scaleLogoToFit;

        bool SizeOptions => mode != CenterMode.Stretch;

        public enum CenterMode
        {
            Stretch,
            Logo,
            Tile,
        }
    }
    public enum VerticalSizeMode
    {
        Aspect,
        Fixed,
    }
}