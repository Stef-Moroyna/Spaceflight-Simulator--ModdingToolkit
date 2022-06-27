using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using UV;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class PipeMesh : BaseMesh, I_InitializePartModule
    {
        [Required] public PipeData pipeData;
        [BoxGroup("material", false)] public Textures textures;
        [BoxGroup("material", false)] public Colors colors;
        public bool leftCover, centerCover, rightCover, separatorRing;

        // Setup
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            textures.width.OnChange += GenerateMesh;
            pipeData.onChange += GenerateMesh;
            
            if (pipeData.isComposedDepth)
                pipeData.composedBaseDepth.OnChange += GenerateMesh;

            initialized = true;
            GenerateMesh();
        }
        

        // Mesh generation
        bool initialized;
        public override void GenerateMesh()
        {
            if (!initialized && Application.isPlaying)
                return;
            
            if (pipeData.isComposedDepth)
                pipeData.baseDepth = pipeData.composedBaseDepth.Value;
            
            if (!Application.isPlaying)
                pipeData.Output();

            // Mesh
            Points_Splittable points = new Points_Splittable(pipeData.pipe.points);
            UV_Splittable[] uv_Channels = Get_UV_Channels(pipeData.pipe).Select(x => new UV_Splittable(x)).ToArray();
            Color_Splittable[] color_Channels = Get_Color_Channels(pipeData.pipe).Select(x => new Color_Splittable(x)).ToArray();

            // Splitting
            List<Splittable> a = new List<Splittable>();
            a.Add(points);
            a.AddRange(uv_Channels);
            a.AddRange(color_Channels);
            Splittable.Split(points.GetSegment(), a.ToArray());
            
            // Generates quads
            UV_Channel[] vertical_UVs = uv_Channels.Select(x => x.element).ToArray();
            Color_Channel[] vertical_Colors = color_Channels.Select(x => x.element).ToArray();
            MeshData data = new MeshData();
            // Left
            if (pipeData.cut != 1)
                GetMeshQuads(points.elements, vertical_UVs, vertical_Colors, Mathf.Clamp(pipeData.cut * 0.5f, 0, 0.5f), 0.5f, data);
            // Right
            if (pipeData.cut != -1)
                GetMeshQuads(points.elements, vertical_UVs, vertical_Colors, 0.5f, Mathf.Clamp(1.0f - pipeData.cut * -0.5f, 0.5f, 1), data);
            
            // Generates mesh
            ApplyMeshData(data.vertices, GetQuadIndices(points.elements), data.UVs, data.colors.ToArray(), data.depths, data.textures, MeshTopology.Quads);
        }
        void GetMeshQuads(List<PipePoint> points, UV_Channel[] vertical_UVs, Color_Channel[] vertical_Colors, float xLeft, float xRight, MeshData data)
        {
            // Loops trough each point 
            for (int quadIndex = 0; quadIndex < points.Count - 1; quadIndex++)
            {
                PipePoint point_A = points[quadIndex];
                PipePoint point_B = points[quadIndex + 1];

                // Vertices
                Vector3[] vertices =
                {
                    point_A.GetPosition(xLeft * 2 - 1),
                    point_B.GetPosition(xLeft * 2 - 1),
                    point_B.GetPosition(xRight * 2 - 1),
                    point_A.GetPosition(xRight * 2 - 1)
                };

                // Calculates UVs for each channel
                Vector3[][] UVs = new Vector3[vertical_UVs.Length][];
                float[] M = UV_Utility.GetQuadM(vertices[0], vertices[1], vertices[2], vertices[3]);
                for (int uv_Index = 0; uv_Index < vertical_UVs.Length; uv_Index++)
                {
                    StartEnd_UV tex = vertical_UVs[uv_Index].elements[quadIndex];

                    if (!tex.data.metalTexture)
                    {
                        if (leftCover && uv_Index == 0)
                            tex.texture_UV.end.x = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.2f);
                        if (centerCover && uv_Index == 0)
                        {
                            float start = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.3f);
                            float end = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.7f);
                            tex.texture_UV.start.x = start;
                            tex.texture_UV.end.x = end;
                        }
                        if (rightCover && uv_Index == 0)
                            tex.texture_UV.start.x = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.8f);   
                    }
                    if (separatorRing && uv_Index == 0)
                        tex.texture_UV.end.y = Mathf.Lerp(tex.texture_UV.start.y, tex.texture_UV.end.y, 0.2f);
                    

                    Line2 texture_UV = tex.texture_UV;
                    Line vertical = tex.vertical_UV;

                    float[] m = tex.data.fixedWidth? new float[]{ 1, 1, 1, 1 } : M;

                    float leftBottom = tex.data.fixedWidth? ((xLeft - 0.5f) * point_A.width.magnitude / tex.data.width + 0.5f) : xLeft;
                    float leftTop = tex.data.fixedWidth? ((xLeft - 0.5f) * point_B.width.magnitude / tex.data.width + 0.5f) : xLeft;
                    float rightTop = tex.data.fixedWidth? ((xRight - 0.5f) * point_B.width.magnitude / tex.data.width + 0.5f) : xRight;
                    float rightBottom = tex.data.fixedWidth? ((xRight - 0.5f) * point_A.width.magnitude / tex.data.width + 0.5f) : xRight;
                    
                    UVs[uv_Index] = new []
                    {
                        texture_UV.LerpUnclamped(leftBottom, vertical.start).ToVector3(1) * m[0],
                        texture_UV.LerpUnclamped(leftTop, vertical.end).ToVector3(1) * m[1],
                        texture_UV.LerpUnclamped(rightTop, vertical.end).ToVector3(1) * m[2],
                        texture_UV.LerpUnclamped(rightBottom, vertical.start).ToVector3(1) * m[3]
                    };
                }

                // Colors
                Color[] colors = { Color.white, Color.white, Color.white, Color.white };
                foreach (Color_Channel color_Channel in vertical_Colors)
                {
                    StartEnd_Color quad = color_Channel.elements[quadIndex];

                    colors[0] *= Color.Lerp(quad.color_Center.start, quad.color_Edge.start, Mathf.Abs(xLeft * 2 - 1));
                    colors[1] *= Color.Lerp(quad.color_Center.end, quad.color_Edge.end, Mathf.Abs(xLeft * 2 - 1));
                    colors[2] *= Color.Lerp(quad.color_Center.end, quad.color_Edge.end, Mathf.Abs(xRight * 2 - 1));
                    colors[3] *= Color.Lerp(quad.color_Center.start, quad.color_Edge.start, Mathf.Abs(xRight * 2 - 1));
                }

                // Depths
                float width_A = point_A.width.magnitude;
                float width_B = point_B.width.magnitude;
                float[] depths =
                {
                    0.5f + (pipeData.baseDepth + width_A * (1 - Mathf.Abs(xLeft * 2 - 1)) * pipeData.depthMultiplier) * 0.02f,
                    0.5f + (pipeData.baseDepth + width_B * (1 - Mathf.Abs(xLeft * 2 - 1)) * pipeData.depthMultiplier) * 0.02f,
                    0.5f + (pipeData.baseDepth + width_B * (1 - Mathf.Abs(xRight * 2 - 1)) * pipeData.depthMultiplier) * 0.02f,
                    0.5f + (pipeData.baseDepth + width_A * (1 - Mathf.Abs(xRight * 2 - 1)) * pipeData.depthMultiplier) * 0.02f
                };
                
                
                // Adds mesh data
                data.vertices.AddRange(vertices);
                data.UVs[0].AddRange(UVs[0]);
                data.UVs[1].AddRange(UVs[1]);
                data.UVs[2].AddRange(UVs[2]);
                data.colors.AddRange(colors);
                data.depths.AddRange(depths);
                data.textures.Add(new PartTex { color = vertical_UVs[0].elements[quadIndex].texture, shape = vertical_UVs[1].elements[quadIndex].texture, shadow = vertical_UVs[2].elements[quadIndex].texture });
            }
        }

        // Generates indices for quads
        int[] GetQuadIndices(List<PipePoint> points)
        {
            if (points.Count == 0)
                return new int[0];

            int[] indices = new int[(points.Count - 1) * (pipeData.cut == 1 || pipeData.cut == -1? 4 : 8)];

            for (int i = 0; i < indices.Length; i++)
                indices[i] = i;

            return indices;
        }

        
        // Used by skin module
        public void SetColorTexture(ColorTexture colorTexture)
        {
            if (textures.texture.colorTexture == colorTexture)
                return;

            textures.texture.colorTexture = colorTexture;
            GenerateMesh();
        }
        public void SetShapeTexture(ShapeTexture shapeTexture)
        {
            if (textures.texture.shapeTexture == shapeTexture)
                return;

            textures.texture.shapeTexture = shapeTexture;
            GenerateMesh();
        }

        // Get
        UV_Channel[] Get_UV_Channels(Pipe shape) => textures.GetOutput(shape, transform, GetLightDirection());
        Vector2 GetLightDirection()
        {

            return new Vector2(-1, 1);
        }
        //
        Color_Channel[] Get_Color_Channels(Pipe shape) => new[] { GetSlopeShading(shape), colors.GetOutput() };
        Color_Channel GetSlopeShading(Pipe shape)
        {
            List<StartEnd_Color> output = new List<StartEnd_Color>();

            for (int i = 0; i < shape.points.Count - 1; i++)
            {
                PipePoint point_A = shape.points[i];
                PipePoint point_B = shape.points[i + 1];

                Color slopeColor = GetSlopeShade(point_A, point_B);
                Color centerColor = Color.Lerp(Color.white, slopeColor * Color.white, 0.25f);

                output.Add(new StartEnd_Color(new Color2(slopeColor, slopeColor), new Color2(centerColor, centerColor), new Line(point_A.height, point_B.height)));
            }

            return new Color_Channel(output);
        }
        Color GetSlopeShade(PipePoint point_A, PipePoint point_B)
        {
            float slope = Mathf.Clamp((point_B.width.magnitude - point_A.width.magnitude) / (point_B.height - point_A.height), -0.8f, 0.8f);

            if (Vector2.Angle(transform.TransformVector(Vector2.up), new Vector2(-1, 1)) < 90)
                slope = -slope;

            float shade = 1 + slope * 0.2f * pipeData.depthMultiplier;
            return new Color(shade, shade, shade, 1);
        }
    }

    // Input data
    [Serializable, InlineProperty, HideLabel]
    public class Textures
    {
        public Mode textureMode;
        [ShowIf("textureMode", Mode.Basic)] public TextureSelector texture;
        [ShowIf("textureMode", Mode.Advanced)] public TextureKey[] textures;
        [Space]
        public WidthMode widthMode;
        [ShowIf("widthMode", WidthMode.Composed)] public Composed_Float width;


        public UV_Channel[] GetOutput(Pipe shape, Transform meshHolder, Vector2 lightDirection)
        {
            float shapeWidth = GetShapeWidth(shape);

            if (textureMode == Mode.Basic)
            {
                Line segment = new Line(0, shape.points.Last().height);

                return new []
                {
                    texture.colorTexture.colorTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection),
                    texture.shapeTexture.shapeTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection),
                    texture.shapeTexture.shadowTex.texture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection)
                };
            }
            else
            {
                UV_Channel[] output = { new UV_Channel(), new UV_Channel(), new UV_Channel() };

                for (int i = 0; i < textures.Length; i++)
                {
                    Line segment = new Line(i > 0? textures[i - 1].height : 0, textures[i].height);

                    output[0].elements.AddRange(textures[i].texture.colorTexture.colorTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection).elements);
                    output[1].elements.AddRange(textures[i].texture.shapeTexture.shapeTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection).elements);
                    output[2].elements.AddRange(textures[i].texture.shapeTexture.shadowTex.texture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection).elements);
                }

                return output;
            }
        }

        public float GetShapeWidth(Pipe shape)
        {
            if (widthMode == WidthMode.Standard)
            {
                float width_a = shape.points.First().width.magnitude;
                float width_b = shape.points.Last().width.magnitude;

                float average = (width_a + width_b) * 0.5f;
                float max = Mathf.Max(width_a, width_b);
                return (average + max) * 0.5f;
            }

            if (widthMode == WidthMode.Composed)
                return width.Value;

            throw new Exception("Width mode is not supported: " + widthMode);
        }

        [Serializable, HideLabel]
        public class TextureKey
        {
            public TextureSelector texture;
            public float height;
        }

        [Serializable, HideLabel, InlineProperty]
        public class TextureSelector
        {
            [Required] public ColorTexture colorTexture;
            [Required] public ShapeTexture shapeTexture;
        }

        public enum WidthMode
        {
            Standard,
            Composed,
        }
    }
    //
    [Serializable, InlineProperty, HideLabel]
    public class Colors
    {
        [LabelText("Color Mode")] public Mode mode;
        [ShowIf("mode", Mode.Basic)] public ColorSelector color;
        [ShowIf("mode", Mode.Advanced)] public ColorKey[] colors;

        public Color_Channel GetOutput()
        {
            if (mode == Mode.Basic)
            {
                Color c = color.GetColor();
                return new Color_Channel(new List<StartEnd_Color>() { new StartEnd_Color(new Color2(c, c), new Color2(c, c), new Line(0, 0)) });
            }
            else
            {
                Color_Channel output = new Color_Channel(new List<StartEnd_Color>());

                for (int index = 0; index < colors.Length; index++)
                {
                    Color2 c = new Color2(colors[index].color.GetColor(), colors[index].color.GetColor());
                    Line cut = new Line(index == 0 ? 0 : colors[index - 1].height, colors[index].height);
                
                    output.elements.Add(new StartEnd_Color(c, c, cut));
                }

                return output;
            }
        }

        [Serializable]
        public class ColorKey
        {
            public ColorSelector color;
            public float height;
        }

        [Serializable, InlineProperty]
        public class ColorSelector
        {
            [HorizontalGroup, HideLabel] public Type type;
            [HorizontalGroup, HideLabel, ShowIf("type", Type.Local)] public Color colorBasic = Color.white;
            [HorizontalGroup, HideLabel, ShowIf("type", Type.Module), Required] public ColorModule colorModule;
            
            public Color GetColor()
            {
                if (type == Type.Local)
                    return colorBasic;
                else
                    return colorModule.GetColor();
            }

            public enum Type
            {
                Local,
                Module,
            }
        }
    }
    //
    public enum Mode
    {
        Basic,
        Advanced,
    }
    
    // Process data
    public abstract class Splittable
    {
        // Function
        public static void Split(Line segment, params Splittable[] A)
        {
            float[] splits = GetSplits(segment, A);

            foreach (Splittable a in A)
                a.Split(splits);
        }
        static float[] GetSplits(Line cut, Splittable[] A)
        {
            List<float> splits = new List<float>();

            foreach (Splittable a in A)
                foreach (float split in a.GetSplits())
                    if (split >= cut.start && split <= cut.end)
                        if (!splits.Contains(split))
                            splits.Add(split);

            splits.Sort();

            return splits.ToArray();
        }

        // Abstract
        protected abstract float[] GetSplits();
        protected abstract void Split(float[] splits);
    }
    public class Points_Splittable : Splittable
    {
        public List<PipePoint> elements;
        public Points_Splittable(List<PipePoint> elements) => this.elements = elements;

        public Line GetSegment()
        {
            return new Line(0, elements.Last().height);
        }

        // Implementation
        protected override float[] GetSplits()
        {
            return elements.Select(x => x.height).ToArray();
        }
        protected override void Split(float[] splits)
        {
            if (elements.Count == 0)
                return;

            List<PipePoint> output = new List<PipePoint>();

            // Creates cut elements
            int elementIndex = 0;
            foreach (float split in splits)
            {
                if (split >= elements[elementIndex + 1].height && elementIndex < elements.Count - 2)
                    elementIndex++;

                // Cuts segment
                PipePoint lastElement = elements[elementIndex];
                PipePoint nextElement = elements[elementIndex + 1];
                output.Add(PipePoint.LerpByHeight(lastElement, nextElement, split));
            }

            elements = output;
        }
    }
    public class UV_Splittable : Splittable
    {
        public UV_Channel element;
        public UV_Splittable(UV_Channel element) => this.element = element;

        // Implementation
        protected override float[] GetSplits()
        {
            if (element.elements.Count == 0)
                return new float[0];

            List<float> heights = new List<float>() { element.elements[0].height.start };
            
            heights.AddRange(element.elements.Select(uv => uv.height.end));

            return heights.ToArray();
        }
        protected override void Split(float[] splits)
        {
            if (element.elements.Count == 0)
                return;

            List<StartEnd_UV> output = new List<StartEnd_UV>();

            // Creates cut elements
            int elementIndex = 0;
            for (int splitIndex = 0; splitIndex < splits.Length - 1; splitIndex++)
            {
                if (splits[splitIndex] >= element.elements[elementIndex].height.end && elementIndex < element.elements.Count - 1)
                    elementIndex++;

                // Cuts segment
                Line cut = new Line(splits[splitIndex], splits[splitIndex + 1]);
                output.Add(element.elements[elementIndex].Cut(cut));
            }

            element.elements = output;
        }
    }
    public class Color_Splittable : Splittable
    {
        public Color_Channel element;
        public Color_Splittable(Color_Channel element) => this.element = element;

        // Implementation
        protected override float[] GetSplits()
        {
            if (element.elements.Count == 0)
                return new float[0];

            List<float> heights = new List<float>() { element.elements[0].height.start };
            
            heights.AddRange(element.elements.Select(UV => UV.height.end));

            return heights.ToArray();
        }
        protected override void Split(float[] splits)
        {
            if (element.elements.Count == 0)
                return;

            List<StartEnd_Color> output = new List<StartEnd_Color>();

            // Creates cut elements
            int elementIndex = 0;
            for (int splitIndex = 0; splitIndex < splits.Length - 1; splitIndex++)
            {
                if (splits[splitIndex] >= element.elements[elementIndex].height.end && elementIndex < element.elements.Count - 1)
                    elementIndex++;

                // Cuts segment
                Line cut = new Line(splits[splitIndex], splits[splitIndex + 1]);
                output.Add(element.elements[elementIndex].Cut(cut));
            }

            element.elements = output;
        }
    }
    //
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3>[] UVs = { new List<Vector3>(), new List<Vector3>(), new List<Vector3>() };
        public List<Color> colors = new List<Color>();
        public List<float> depths = new List<float>();
        public List<PartTex> textures = new List<PartTex>();
    }
}