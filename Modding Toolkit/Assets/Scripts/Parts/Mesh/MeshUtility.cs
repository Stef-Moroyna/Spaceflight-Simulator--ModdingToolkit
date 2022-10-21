using System.Collections.Generic;
using SFS.Parts;
using UnityEngine;

namespace UV
{
    // UV channel quad
    public class StartEnd_UV
    {
        // Variables
        public Line height;
        public Line vertical_UV;
        public Line2 texture_UV;
        public Texture2D texture;
        public PartTexture data;
        public float fixedWidthValue;
        
        // Constructor
        public StartEnd_UV(Line height, Line vertical_UV, Line2 texture_UV, Texture2D texture, PartTexture data, float fixedWidthValue)
        {
            this.height = height;
            this.vertical_UV = vertical_UV;
            this.texture_UV = texture_UV;
            this.texture = texture;
            this.data = data;
            this.fixedWidthValue = fixedWidthValue;
        }

        public StartEnd_UV Cut(Line cut)
        {
            Line time = new Line(height.InverseLerp(cut.start), height.InverseLerp(cut.end));
            Line uv = new Line(vertical_UV.Lerp(time.start), vertical_UV.Lerp(time.end));
            return new StartEnd_UV(cut, uv, texture_UV, texture, data, fixedWidthValue);
        }
    }

    // Color channel
    public class Color_Channel
    {
        public List<StartEnd_Color> elements;
        public Color_Channel(List<StartEnd_Color> elements) => this.elements = elements;
    }

    // Color quad
    public class StartEnd_Color
    {
        public Color2 color_Edge;
        public Line height;

        public StartEnd_Color(Color2 color_Edge, Line height)
        {
            this.color_Edge = color_Edge;
            this.height = height;
        }

        public StartEnd_Color Cut(Line cut)
        {
            Line time = new Line(height.InverseLerp(cut.start), height.InverseLerp(cut.end));
            Color2 color = new Color2(color_Edge.Lerp(time.start), color_Edge.Lerp(time.end));
            return new StartEnd_Color(color, cut);
        }
    }
}