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


        void UpdateActive()
        {
            switch (layerType)
            {
            }
        }
        
        public enum LayerType
        {
            Interior,
            Exterior,
        }
    }
}

//bool active = (location == SceneType.Build && BuildManager.main != null) || (location == SceneType.World && GameManager.main != null);