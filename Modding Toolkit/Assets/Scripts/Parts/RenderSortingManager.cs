using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SFS
{
    public class RenderSortingManager : MonoBehaviour
    {
        public static RenderSortingManager main;
        void Awake() => main = this;

        public List<string> layers = new List<string>();
        
        
        [ShowInInspector] public Dictionary<int, Material> partMaterials = new Dictionary<int, Material>();
        [ShowInInspector] public Dictionary<int, Material> partModelMaterials = new Dictionary<int, Material>();

        const int Transparent_Queue = 3000;
        

        public Material GetPartMaterial(int renderQueue)
        {
            return null;
        }
        public Material GetPartModelMaterial(int renderQueue, bool normals)
        {
            return null;
        }

        public float GetGlobalDepth(float localDepth, string layer)
        {
            if (!layers.Contains(layer))
                return localDepth;

            int index = GetLayerIndex(layer);
            float range = 1.0f / layers.Count;
            float start = index * range;

            return Mathf.Lerp(start, start + range, localDepth);
        }
        public List<float> GetGlobalDepths(List<float> localDepths, string layer)
        {
            if (!layers.Contains(layer))
                return localDepths;

            int index = layers.Count - 1 - layers.IndexOf(layer);
            float range = 1.0f / layers.Count;
            float start = index * range;

            for (int i = 0; i < localDepths.Count; i++)
                localDepths[i] = Mathf.Lerp(start, start + range, localDepths[i]);

            return localDepths;
        }
        public int GetRenderQueue(string layer)
        {
            if (!layers.Contains(layer))
                return Transparent_Queue;

            return Transparent_Queue + GetLayerIndex(layer) * 50;
        }
        int GetLayerIndex(string layer) => layers.Count - 1 - layers.IndexOf(layer);
    }
}