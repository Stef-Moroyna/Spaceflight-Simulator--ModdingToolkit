using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class InteriorModule : MonoBehaviour
    {
        [FormerlySerializedAs("location")]
        public LayerType layerType;

        void Awake() => InteriorManager.main.interiorView.OnChange += UpdateActive;
        void OnDestroy()
        {
            if (!Base.sceneLoader.isUnloading)
                InteriorManager.main.interiorView.OnChange -= UpdateActive;
        }

        void UpdateActive()
        {
            bool interior = InteriorManager.main.interiorView.Value;
            
            switch (layerType)
            {
                case LayerType.Interior: gameObject.SetActive(interior); break;
                case LayerType.Exterior: gameObject.SetActive(!interior); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public enum LayerType
        {
            Interior,
            Exterior,
        }
    }
}