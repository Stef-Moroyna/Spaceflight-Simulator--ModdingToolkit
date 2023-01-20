using UnityEngine;
using System;
using SFS.Variables;
using SFS.World;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Translations;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SplitModule : MonoBehaviour
    {
        // Refs
        public SplitModule prefab;
        public Fragment[] fragments;
        [Space]
        // Description
        public bool showDescription = true;
        //
        [Space]
        public bool showForceMultiplier = false;
        public Float_Reference forceMultiplier;
        //
        [Space]
        public bool fairing;
        [ShowIf("fairing")] public Bool_Reference detachEdges;

        // State
        [Space]
        public String_Reference fragmentName;

        // Out
        [Space]
        public UnityEvent onDeploy;
        
    
        public void Split(UsePartData data)
        {
        }
        
        [Serializable]
        public class Fragment
        {
            public string fragmentName;
            public GameObject[] toEnable, toDisable;
            public SurfaceData[] attachmentSurfaces; // Used to transfer joints
            public Composed_Vector2 separationForce;
        }
    }
}