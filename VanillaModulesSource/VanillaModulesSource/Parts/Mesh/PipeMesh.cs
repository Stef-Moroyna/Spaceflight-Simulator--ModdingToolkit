using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Career;
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
        [BoxGroup("material", false), Space] public Colors colors;
        [FoldoutGroup("Specific")] public bool leftCover, centerCover, rightCover, separatorRing, smoothShading;
        
        // Setup
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            if (textures.widthMode == Textures.WidthMode.Composed)
                textures.width.OnChange += GenerateMesh;
            
            pipeData.onChange += GenerateMesh;
            pipeData.SubscribeToComposedDepth(GenerateMesh);

            initialized = true;
            GenerateMesh();
        }
        

        // Mesh generation
        bool initialized;
        public override void GenerateMesh()
        {
            bool testScene = GameManager.main == null && BuildManager.main == null && HubManager.main == null;
            
            if (!initialized && Application.isPlaying && !testScene) // Prevents repeated generations on startup
                return;

            if (!Application.isPlaying || testScene)
                pipeData.Output();

            // Mesh
            Points_Splittable points = new Points_Splittable(pipeData.pipe.points);
            //
            UV_Splittable[] uv_Channels = Get_UV_Channels(pipeData.pipe).Select(x => new UV_Splittable(x)).ToArray();
            Color_Splittable color_Channel = new Color_Splittable(colors.GetOutput());

            // Splitting
            List<Splittable> a = new List<Splittable>();
            a.Add(points);
            a.AddRange(uv_Channels);
            a.Add(color_Channel);
            Splittable.Split(points.GetSegment(), a.ToArray());
            
            // Generates quads
            MeshData data = new MeshData();
            GetMeshQuads(points.elements, uv_Channels.Select(x => x.element).ToArray(), color_Channel.element, GetSlopeShading(points.elements), data);

            // Generates mesh
            ApplyMeshData(data.vertices, GetQuadIndices(points.elements), data.UVs, data.colors.ToArray(), data.shading, data.depths, pipeData.BaseDepth, pipeData.depthMultiplier, data.textures, MeshTopology.Quads);
        }
        void GetMeshQuads(List<PipePoint> points, List<StartEnd_UV>[] vertical_UVs, Color_Channel color_Channel, List<Line> slopeShading, MeshData data)
        {
            // Loops trough each point 
            for (int quadIndex = 0; quadIndex < points.Count - 1; quadIndex++)
            {
                PipePoint point_A = points[quadIndex];
                PipePoint point_B = points[quadIndex + 1];
                float width_A = point_A.width.magnitude;
                float width_B = point_B.width.magnitude;
                
                // Vertices
                Vector3[] vertices =
                {
                    point_A.GetPosition(point_A.cutLeft * 2 - 1),
                    point_B.GetPosition(point_B.cutLeft * 2 - 1),
                    point_B.GetPosition(point_B.cutRight * 2 - 1),
                    point_A.GetPosition(point_A.cutRight * 2 - 1)
                };
                
                // M for uv  // Needs to calculate it without cutting
                float[] M = UV_Utility.GetQuadM(point_A.GetPosition(-1), point_B.GetPosition(-1), point_B.GetPosition(1), point_A.GetPosition(1));
                
                // Calculates UVs for each channel
                Vector3[][] UVs = new Vector3[vertical_UVs.Length][];
                for (int uv_Index = 0; uv_Index < vertical_UVs.Length; uv_Index++)
                {
                    // Calculates UV and applies M at the end
                    StartEnd_UV tex = vertical_UVs[uv_Index][quadIndex];
                    if (!tex.data.metalTexture)
                    {
                        if (leftCover && uv_Index == 0)
                            tex.texture_UV.end.x = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.05f);
                        
                        if (centerCover && uv_Index == 0)
                        {
                            float start = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.4f);
                            float end = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.6f);
                            tex.texture_UV.start.x = start;
                            tex.texture_UV.end.x = end;
                        }
                        
                        if (rightCover && uv_Index == 0)
                            tex.texture_UV.start.x = Mathf.Lerp(tex.texture_UV.start.x, tex.texture_UV.end.x, 0.95f);   
                    }
                    
                    if (separatorRing && uv_Index == 0)
                        tex.texture_UV.end.y = tex.texture_UV.start.y;
                    
                    float leftBottom = tex.data.fixedWidth? ((point_A.cutLeft - 0.5f) * point_A.width.magnitude / tex.fixedWidthValue + 0.5f) : point_A.cutLeft;
                    float leftTop = tex.data.fixedWidth? ((point_B.cutLeft - 0.5f) * point_B.width.magnitude / tex.fixedWidthValue + 0.5f) : point_B.cutLeft;
                    float rightTop = tex.data.fixedWidth? ((point_B.cutRight - 0.5f) * point_B.width.magnitude / tex.fixedWidthValue + 0.5f) : point_B.cutRight;
                    float rightBottom = tex.data.fixedWidth? ((point_A.cutRight - 0.5f) * point_A.width.magnitude / tex.fixedWidthValue + 0.5f) : point_A.cutRight;
                   
                    Line2 texture_UV = tex.texture_UV;
                    Line vertical = tex.vertical_UV;
                    float[] m = tex.data.fixedWidth? new float[]{ 1, 1, 1, 1 } : M;

                    UVs[uv_Index] = new []
                    {
                        texture_UV.LerpUnclamped(leftBottom, vertical.start).ToVector3(1) * m[0],
                        texture_UV.LerpUnclamped(leftTop, vertical.end).ToVector3(1) * m[1],
                        texture_UV.LerpUnclamped(rightTop, vertical.end).ToVector3(1) * m[2],
                        texture_UV.LerpUnclamped(rightBottom, vertical.start).ToVector3(1) * m[3]
                    };
                }

                // Colors
                StartEnd_Color quad = color_Channel.elements[quadIndex];
                Color[] colors = { quad.color_Edge.start, quad.color_Edge.end, quad.color_Edge.end, quad.color_Edge.start };

                // Shading
                Line s = slopeShading[quadIndex];
                Vector3[] shading = new[]
                {
                    new Vector3(s.start, 0, 1f) * M[0],
                    new Vector3(s.end, 0, 1f) * M[1],
                    new Vector3(s.end, 0, 1f) * M[2],
                    new Vector3(s.start, 0, 1f) * M[3],
                };
                
                // Depths
                Vector3[] depths =
                {
                    // (uv position, point width, m offset)
                    new Vector3(point_A.cutLeft, width_A, 1f) * M[0],
                    new Vector3(point_B.cutLeft, width_B, 1f) * M[1],
                    new Vector3(point_B.cutRight, width_B, 1f) * M[2],
                    new Vector3(point_A.cutRight, width_A, 1f) * M[3],
                };
                
                
                // Adds mesh data
                data.vertices.AddRange(vertices);
                data.UVs[0].AddRange(UVs[0]);
                data.UVs[1].AddRange(UVs[1]);
                data.UVs[2].AddRange(UVs[2]);
                data.colors.AddRange(colors);
                data.shading.AddRange(shading);
                data.depths.AddRange(depths);
                data.textures.Add(new PartTex { color = vertical_UVs[0][quadIndex].texture, shape = vertical_UVs[1][quadIndex].texture, shadow = vertical_UVs[2][quadIndex].texture });
            }
        }

        // Generates indices for quads
        int[] GetQuadIndices(List<PipePoint> points)
        {
            if (points.Count == 0)
                return new int[0];

            int[] indices = new int[(points.Count - 1) * 4];

            for (int i = 0; i < indices.Length; i++)
                indices[i] = i;

            return indices;
        }

        // Used by skin module
        public Event_Local onSetColorTexture;
        public void SetColorTexture(ColorTexture colorTexture)
        {
            if (textures.texture.colorTexture == colorTexture)
                return;

            textures.texture.colorTexture = colorTexture;
            GenerateMesh();
            
            onSetColorTexture?.Invoke();
        }
        public void SetShapeTexture(ShapeTexture shapeTexture)
        {
            if (textures.texture.shapeTexture == shapeTexture)
                return;

            textures.texture.shapeTexture = shapeTexture;
            GenerateMesh();
        }

        // Get
        List<StartEnd_UV>[] Get_UV_Channels(Pipe shape) => textures.GetOutput(shape, transform, GetLightDirection());
        Vector2 GetLightDirection()
        {
            Vector2 a = new Vector2(-1 * pipeData.depthMultiplier, 1);
            
            if (GameManager.main != null && transform.root.childCount > 0 && transform.root.GetChild(0).name == "Parts Holder")
                return transform.root.GetChild(0).TransformDirection(a);

            return a;
        }
        
        List<Line> GetSlopeShading(List<PipePoint> points)
        {
            float[] shade = new float[points.Count - 1];
            for (int i = 0; i < points.Count - 1; i++)
                shade[i] = GetSlopeShade(points[i], points[i + 1]);
            
            List<Line> lines = new List<Line>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                float start = shade[smoothShading && i > 0? i - 1 : i];
                float end = shade[i];
                lines.Add(new Line(start, end));
            }
            
            return lines;
        }
        float GetSlopeShade(PipePoint point_A, PipePoint point_B)
        {
            float slope = Mathf.Clamp((point_B.width.magnitude - point_A.width.magnitude) / (point_B.height - point_A.height), -0.8f, 0.8f);

            if (Vector2.Angle(transform.TransformVector(Vector2.up), new Vector2(-1, 1)) < 90)
                slope = -slope;

            float shade = slope * 0.2f * pipeData.depthMultiplier;
            return shade;
        }
    }

    // Input data
    [Serializable, InlineProperty, HideLabel]
    public class Textures
    {
        public Mode textureMode;
        [ShowIf("textureMode", Mode.Single)] public TextureSelector texture;
        [ShowIf("textureMode", Mode.Multiple)] public TextureKey[] textures;
        [Space]
        public WidthMode widthMode;
        [ShowIf("widthMode", WidthMode.Composed)] public Composed_Float width;


        public List<StartEnd_UV>[] GetOutput(Pipe shape, Transform meshHolder, Vector2 lightDirection)
        {
            float shapeWidth = GetShapeWidth(shape);

            if (textureMode == Mode.Single)
            {
                Line segment = new Line(0, shape.points.Last().height);
                return new []
                {
                    texture.colorTexture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection),
                    texture.shapeTexture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection),
                    texture.shapeTexture.shadowTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection)
                };
            }
            else
            {
                List<StartEnd_UV>[] output = { new List<StartEnd_UV>(), new List<StartEnd_UV>(), new List<StartEnd_UV>() };

                for (int i = 0; i < textures.Length; i++)
                {
                    Line segment = new Line(ConvertHeight(i > 0? textures[i - 1].height : 0), ConvertHeight(textures[i].height));
                    float ConvertHeight(float height) => height >= 0? height : shape.points.Last().height + height;
                    
                    output[0].AddRange(textures[i].texture.colorTexture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection));
                    output[1].AddRange(textures[i].texture.shapeTexture.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection));
                    output[2].AddRange(textures[i].texture.shapeTexture.shadowTex.Get_UV(shape, segment, shapeWidth, meshHolder, lightDirection));
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
        [ShowIf("mode", Mode.Single)] public ColorSelector color;
        [ShowIf("mode", Mode.Multiple)] public ColorKey[] colors;

        public Color_Channel GetOutput()
        {
            if (mode == Mode.Single)
            {
                Color c = color.GetColor();
                return new Color_Channel(new List<StartEnd_Color>() { new StartEnd_Color(new Color2(c, c), /*new Color2(c, c),*/ new Line(0, 0)) });
            }
            else
            {
                Color_Channel output = new Color_Channel(new List<StartEnd_Color>());

                for (int index = 0; index < colors.Length; index++)
                {
                    Color2 c = new Color2(colors[index].color.GetColor(), colors[index].color.GetColor());
                    Line cut = new Line(index == 0 ? 0 : colors[index - 1].height, colors[index].height);
                
                    output.elements.Add(new StartEnd_Color(c, /*c,*/ cut));
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
            
            public Color GetColor() => type == Type.Local ? colorBasic : colorModule.GetColor();

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
        Single,
        Multiple,
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
        public List<StartEnd_UV> element;
        public UV_Splittable(List<StartEnd_UV> element) => this.element = element;

        // Implementation
        protected override float[] GetSplits()
        {
            if (element.Count == 0)
                return new float[0];

            List<float> heights = new List<float>() { element[0].height.start };
            
            heights.AddRange(element.Select(uv => uv.height.end));

            return heights.ToArray();
        }
        protected override void Split(float[] splits)
        {
            if (element.Count == 0)
                return;

            List<StartEnd_UV> output = new List<StartEnd_UV>();

            // Creates cut elements
            int elementIndex = 0;
            for (int splitIndex = 0; splitIndex < splits.Length - 1; splitIndex++)
            {
                if (splits[splitIndex] >= element[elementIndex].height.end && elementIndex < element.Count - 1)
                    elementIndex++;

                // Cuts segment
                Line cut = new Line(splits[splitIndex], splits[splitIndex + 1]);
                output.Add(element[elementIndex].Cut(cut));
            }

            element = output;
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
        public List<Vector3> shading = new List<Vector3>();
        public List<Vector3> depths = new List<Vector3>();
        public List<PartTex> textures = new List<PartTex>();
    }
}