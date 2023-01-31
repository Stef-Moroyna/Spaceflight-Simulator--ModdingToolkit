using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SFS.Parts.Modules
{
    [HideMonoScript, ExecuteInEditMode]
    public abstract class BaseMesh : MonoBehaviour
    {
        [FormerlySerializedAs("sortingLayerOffset")] public int renderQueueOffset = 0;

        // Generates mesh
        [Button] public abstract void GenerateMesh();

        static int baseDepthID = Shader.PropertyToID("_BaseDepth");
        static int depthMultiplierID = Shader.PropertyToID("_DepthMultiplier");

        // Creates a mesh based on the specified values
        protected void ApplyMeshData(List<Vector3> vertices, int[] indices, List<Vector3>[] UVs, Color[] colors, List<Vector3> shading, List<Vector3> depths, float baseDepth, float depthMultiplier, List<PartTex> textures, MeshTopology topology)
        {
            // Refs
            Mesh mesh = GetMesh();
            Renderer meshRenderer = this.GetOrAddComponent<MeshRenderer>();
            
            // Vertices
            mesh.SetVertices(vertices);

            // UVs
            for (int channel_Index = 0; channel_Index < UVs.Length; channel_Index++)
                mesh.SetUVs(channel_Index, UVs[channel_Index]);

            // Color
            mesh.colors = colors;
            
            // Shading
            mesh.SetUVs(5, shading);
            
            // Depth
            if (RenderSortingManager.main != null)
            {
                baseDepth = RenderSortingManager.main.GetGlobalDepth(0.5f, sortingLayer) + baseDepth * 0.02f * 1f / Mathf.Max(RenderSortingManager.main.layers.Count, 1);
                depthMultiplier = depthMultiplier * 0.02f * 1f / Mathf.Max(RenderSortingManager.main.layers.Count, 1);
            }
            else
            {
                baseDepth = 0.5f + baseDepth * 0.02f;
                depthMultiplier *= 0.02f;
            }
            //
            mesh.SetUVs(3, depths);

            
            // Indices, materials, property blocks
            if (textures.Count > 1)
                Set_TrySubmeshes();
            else
                Set_Basic();

            void Set_TrySubmeshes()
            {
                Dictionary<PartTex, List<int>> submeshes = new Dictionary<PartTex, List<int>>();
                for (int i = 0; i < textures.Count; i++)
                {
                    PartTex A = textures[i];
                    if (!submeshes.ContainsKey(A))
                        submeshes.Add(A, new List<int>());
                    
                    submeshes[A].Add(i);
                }
                
                int submeshCount = submeshes.Count;

                if (submeshCount > 1)
                    Set_Multiple(submeshes);
                else
                    Set_Basic();
            }
            void Set_Multiple(Dictionary<PartTex, List<int>> submeshes)
            {
                SetMaterials(submeshes.Count);
                mesh.subMeshCount = submeshes.Count;
                
                int submeshIndex = 0;
                foreach (List<int> pointers in submeshes.Values)
                {
                    // Indices
                    List<int> list = new List<int>(pointers.Count * 4);
                    foreach (int pointer in pointers)
                    {
                        list.Add(indices[pointer * 4]);
                        list.Add(indices[pointer * 4 + 1]);
                        list.Add(indices[pointer * 4 + 2]);
                        list.Add(indices[pointer * 4 + 3]);
                    }
                    
                    // Indices // Property block
                    SetPropertyBlock(textures[pointers[0]], submeshIndex);
                    mesh.SetIndices(list, topology, submeshIndex);
                    submeshIndex++;
                }
            }
            void Set_Basic()
            {
                mesh.SetIndices(indices, topology, 0);
                SetMaterials(1);

                // Property block
                if (textures.Count > 0)
                    SetPropertyBlock(textures[0], 0);
            }
            void SetMaterials(int count)
            {
                #if UNITY_EDITOR
                Material material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/Part 1.mat", typeof(Material));
                meshRenderer.sharedMaterials = new Material[count].Select(a => material).ToArray();
                #endif
            }
            void SetPropertyBlock(PartTex T, int index)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                if (meshRenderer.HasPropertyBlock())
                    meshRenderer.GetPropertyBlock(propertyBlock, index);
                
                // Tex
                propertyBlock.SetTexture(PartTex.ColorTexture, T.color);
                propertyBlock.SetTexture(PartTex.ShapeTexture, T.shape);
                propertyBlock.SetTexture(PartTex.ShadowTexture, T.shadow);
                
                // Depth
                propertyBlock.SetFloat(baseDepthID, baseDepth);
                propertyBlock.SetFloat(depthMultiplierID, depthMultiplier);
                
                meshRenderer.SetPropertyBlock(propertyBlock, index);
            }
            

            // Applies new mesh data
            mesh.RecalculateBounds();
        }

        // Gets mesh component or adds it if it doesn't exist
        Mesh GetMesh()
        {
            MeshFilter meshFilter = this.GetOrAddComponent<MeshFilter>();

            if (meshFilter.sharedMesh == null || !Application.isPlaying)
                meshFilter.sharedMesh = meshReference = new Mesh { name = "BaseMesh" };
            else
                meshFilter.sharedMesh.Clear();

            return meshFilter.sharedMesh;
        }

        // Changes depth layer and re-generates mesh
        string sortingLayer;
        public void SetSortingLayer(string sortingLayer)
        {
            if (this.sortingLayer == sortingLayer)
                return;
            
            this.sortingLayer = sortingLayer;
            GenerateMesh();
        }

        // Clears memory
        Mesh meshReference;

#if UNITY_EDITOR
        void OnDestroy() => DestroyImmediate(meshReference);
#else
        void OnDestroy() => Destroy(meshReference);
#endif

        // Auto regenerate mesh
        #if UNITY_EDITOR
        protected void OnValidate() => hasGeneratedMeshForEditing = true;
        bool hasGeneratedMeshForEditing;
        void Update()
        {
            if (!Application.isPlaying && hasGeneratedMeshForEditing)
            {
                hasGeneratedMeshForEditing = false;
                GenerateMesh();
            }
        }
        #endif
    }
    
    public struct PartTex
    {
        public Texture2D color, shape, shadow;
        
        public static readonly int ColorTexture = Shader.PropertyToID("_ColorTexture");
        public static readonly int ShapeTexture = Shader.PropertyToID("_ShapeTexture");
        public static readonly int ShadowTexture = Shader.PropertyToID("_ShadowTexture");
    }
}