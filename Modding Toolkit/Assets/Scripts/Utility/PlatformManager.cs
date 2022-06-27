using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Platform
{
    // Very important to put PlatformManager as the first loading script in Script Execution Order
    public class PlatformManager : MonoBehaviour
    {
        #if UNITY_STANDALONE
        public static PlatformType current = PlatformType.PC;
        #else
        public static PlatformType current = PlatformType.Mobile;
        #endif

        public List<PlatformSpecific> allPlatformSpecific;

        void Awake()
        {
            // Enables per platform GameObjects
            foreach (PlatformSpecific platformSpecific in allPlatformSpecific)
            {
                bool active = platformSpecific.platformType == current;
                
                foreach (GameObject a in platformSpecific.affiliatedGameObject)
                    a.SetActive(active);
            }
        }
    }
    
    [Serializable]
    public class PlatformSpecific
    {
        public PlatformType platformType;
        public List<GameObject> affiliatedGameObject;
    }
    
    [Serializable]
    public enum PlatformType
    {
        Mobile,
        PC
    }
}