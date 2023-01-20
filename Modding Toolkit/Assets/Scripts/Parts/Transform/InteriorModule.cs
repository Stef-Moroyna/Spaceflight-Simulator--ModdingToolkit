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
        
        public enum LayerType
        {
            Interior,
            Exterior,
        }
    }
}