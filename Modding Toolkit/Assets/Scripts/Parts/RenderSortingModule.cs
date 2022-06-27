using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SFS
{
    public class RenderSortingModule : MonoBehaviour
    {
        // Modes
        public enum SortingMode { DepthOnly, AbsoluteDepth_AndQueue, RelativeDepth_AndQueue, QueueOnly }
        public SortingMode sortingMode = SortingMode.DepthOnly;
        
        // Depth (always used)
        [HideIf("sortingMode", SortingMode.QueueOnly), Range(0.0f, 1.0f)] public float depth;
        // Depth relative to layers
        [ShowIf("sortingMode", SortingMode.RelativeDepth_AndQueue), ValueDropdown(nameof(GetLayers))] public string selectedLayer;
        [ShowIf("sortingMode", SortingMode.RelativeDepth_AndQueue)] public RenderSortingManager manager;
        List<string> GetLayers() => manager != null? manager.layers : new List<string>();
        
        // Queue
        [FormerlySerializedAs("layer"), HideIf("sortingMode", SortingMode.DepthOnly)] public int renderQueue;

        
        public bool setSharedMaterial;

        
        void Start()
        {
            switch (sortingMode)
            {
                case SortingMode.DepthOnly:
                    SetDepth(gameObject, depth);
                    break;
                case SortingMode.AbsoluteDepth_AndQueue:
                    SetRenderQueue(gameObject, renderQueue);
                    SetDepth(gameObject, depth);
                    break;
                case SortingMode.RelativeDepth_AndQueue:
                    SetRenderQueue(gameObject, manager.GetRenderQueue(selectedLayer));
                    SetDepth(gameObject, manager.GetGlobalDepth(depth, selectedLayer));
                    break;
                case SortingMode.QueueOnly:
                    SetRenderQueue(gameObject, renderQueue);
                    break;
            }
        }

        void SetDepth(GameObject gameObject, float depth)
        {
            Renderer component = gameObject.GetComponent<Renderer>();
            (setSharedMaterial? component.sharedMaterial : component.material).SetFloat("_Depth", depth);
        }
        void SetRenderQueue(GameObject gameObject, int renderQueue)
        {
            Renderer component = gameObject.GetComponent<Renderer>();
            (setSharedMaterial? component.sharedMaterial : component.material).renderQueue = renderQueue;
        }
    }
}