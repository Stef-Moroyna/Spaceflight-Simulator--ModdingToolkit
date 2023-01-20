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
        
        
        [ShowInInspector] public Dictionary<(Material, int), Material> partMaterials = new Dictionary<(Material, int), Material>();

        const int Transparent_Queue = 3000;
        
        Material GetMaterial(int renderQueue, Material prefab)
        {
            (Material prefab, int renderQueue) key = (prefab, renderQueue);
            if (!partMaterials.ContainsKey(key))
            {
                Material mat = Instantiate(prefab);
                mat.name = "PartModel " + (renderQueue - 3000) + " " + mat.GetInstanceID();
                mat.renderQueue = renderQueue;
                partMaterials.Add(key, mat);
            }
            
            return partMaterials[key];
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
        /*public List<float> GetGlobalDepths(List<float> localDepths, string layer)
        {
            if (!layers.Contains(layer))
                return localDepths;

            int index = layers.Count - 1 - layers.IndexOf(layer);
            float range = 1.0f / layers.Count;
            float start = index * range;

            for (int i = 0; i < localDepths.Count; i++)
                localDepths[i] = Mathf.Lerp(start, start + range, localDepths[i]);

            return localDepths;
        }*/
        public int GetRenderQueue(string layer)
        {
            if (!layers.Contains(layer))
                return Transparent_Queue;

            return Transparent_Queue + GetLayerIndex(layer) * 50;
        }
        int GetLayerIndex(string layer) => layers.Count - 1 - layers.IndexOf(layer);
    }
}