using System.Collections.Generic;
using SFS.Parts;
using UnityEngine;

namespace UV
{
    // UV channel
    public class UV_Channel
    {
        // Variables
        public List<StartEnd_UV> elements = new List<StartEnd_UV>();

        public void AddQuadClamped(Line cut, Line vertical_UV, Line2 texture_UV, Line clamp, Texture2D texture, PartTexture data)
        {
            Line clampedCut = new Line(Mathf.Clamp(cut.start, clamp.start, clamp.end), Mathf.Clamp(cut.end, clamp.start, clamp.end));
            Line clampedUV = new Line(vertical_UV.Lerp(Mathf.InverseLerp(cut.start, cut.end, clampedCut.start)), vertical_UV.Lerp(Mathf.InverseLerp(cut.start, cut.end, clampedCut.end)));

            AddQuad(clampedCut, clampedUV, texture_UV, texture, data);
        }
        public void AddQuad(Line cut, Line vertical_UV, Line2 texture_UV, Texture2D texture, PartTexture data)
        {
            if (cut.Size > 0)
                elements.Add(new StartEnd_UV(cut, vertical_UV, texture_UV, texture, data));
        }
    }

    // UV channel quad
    public class StartEnd_UV
    {
        // Variables
        public Line height;
        public Line vertical_UV;
        public Line2 texture_UV;
        public Texture2D texture;
        public PartTexture data;

        // Constructor
        public StartEnd_UV(Line height, Line vertical_UV, Line2 texture_UV, Texture2D texture, PartTexture data)
        {
            this.height = height;
            this.vertical_UV = vertical_UV;
            this.texture_UV = texture_UV;
            this.texture = texture;
            this.data = data;
        }

        public StartEnd_UV Cut(Line cut)
        {
            Line time = new Line(height.InverseLerp(cut.start), height.InverseLerp(cut.end));
            Line output_uv = new Line(vertical_UV.Lerp(time.start), vertical_UV.Lerp(time.end));
            return new StartEnd_UV(cut, output_uv, texture_UV, texture, data);
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
        //public Color2 color_Center;
        public Line height;

        public StartEnd_Color(Color2 color_Edge, /*Color2 color_Center,*/ Line height)
        {
            this.color_Edge = color_Edge;
            //this.color_Center = color_Center;
            this.height = height;
        }

        public StartEnd_Color Cut(Line cut)
        {
            Line time = new Line(height.InverseLerp(cut.start), height.InverseLerp(cut.end));

            Color2 output_color_A = new Color2(color_Edge.Lerp(time.start), color_Edge.Lerp(time.end));
            //Color2 output_color_B = new Color2(color_Center.Lerp(time.start), color_Center.Lerp(time.end));

            return new StartEnd_Color(output_color_A, /*output_color_B,*/ cut);
        }
    }
}