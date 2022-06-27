using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class PolygonMesh : BaseMesh, I_InitializePartModule
    {
        [Required] public PolygonData polygonModule;
        public UVOptions UV_Mode = UVOptions.Auto;
        [HideInInspector]public Vector2[] bounds;

        [BoxGroup, Required] public BasicTexture texture;
        [BoxGroup, LabelText("Color [Optional]")] public ColorModule colorModule;
        

        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            polygonModule.onChange += GenerateMesh;
            
            if (polygonModule.isComposedDepth)
                polygonModule.composedBaseDepth.OnChange += GenerateMesh;

            initialized = true;
            GenerateMesh();
        }

        bool initialized;
        public override void GenerateMesh()
        {
            if (!initialized && Application.isPlaying)
                return;
            
            if (polygonModule.isComposedDepth)
                polygonModule.baseDepth = polygonModule.composedBaseDepth.Value;
            
            if (!Application.isPlaying)
                polygonModule.Output();
            
            // Data
            List<Vector2> points = new List<Vector2>(polygonModule.polygon.vertices);
            Line2[] uv_Channels = Get_UV_Channels();

            // Output
            Vector3[] vertices = new Vector3[points.Count];
            List<int> indices = new List<int>();
            Vector2[] uv = new Vector2[points.Count];
            
            foreach (ConvexPolygon shape in PolygonPartioner.Partion(points.ToArray()))
            {
                int origin = points.IndexOf(shape.points[0]);

                for (int i = 1; i < shape.points.Length - 1; i++)
                {
                    indices.Add(origin);
                    indices.Add(points.IndexOf(shape.points[i]));
                    indices.Add(points.IndexOf(shape.points[i + 1]));
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 vertice = points[i];
                vertices[i] = vertice;
                uv[i] = UV_Utility.UV(vertice, bounds);
            }
            
            List<Vector3>[] uvs = new List<Vector3>[uv_Channels.Length];
            for (int uv_Channel_Index = 0; uv_Channel_Index < uv_Channels.Length; uv_Channel_Index++)
            {
                uvs[uv_Channel_Index] = new List<Vector3>();
                
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Applies texture rect
                    Vector2 atlasUV = uv_Channels[uv_Channel_Index].LerpUnclamped(uv[i].x, uv[i].y);
                    uvs[uv_Channel_Index].Add(new Vector3(atlasUV.x, atlasUV.y, 1));
                }
            }
            
            List<PartTex> textures = new List<PartTex> { new PartTex { color = texture.colorTex.texture, shape = texture.shapeTex.texture, shadow = texture.shadowTex.texture } };
            ApplyMeshData(vertices.ToList(), indices.ToArray(), uvs, GetColors(vertices.Length), GetDepths(vertices.Length, 0.5f + polygonModule.baseDepth * 0.02f), textures, MeshTopology.Triangles);
        }
        Line2[] Get_UV_Channels()
        {
            return new []
            {
                texture.colorTex.Get_Rect(transform),
                texture.shapeTex.Get_Rect(transform),
                texture.shadowTex.Get_Rect(transform)
            };
        }
        Color[] GetColors(int verticeCount)
        {
            Color color = colorModule != null ? colorModule.GetColor() : Color.white;
            Color[] output = new Color[verticeCount];

            for (int i = 0; i < output.Length; i++)
                output[i] = color;

            return output;
        }
        static List<float> GetDepths(int vertexCount, float depth)
        {
            List<float> depths = new List<float>(vertexCount);

            for (int i = 0; i < vertexCount; i++)
                depths.Add(depth);

            return depths;
        }


        public enum UVOptions
        {
            Auto,
            Two_Points,
            Four_Points,
        }
    }
}