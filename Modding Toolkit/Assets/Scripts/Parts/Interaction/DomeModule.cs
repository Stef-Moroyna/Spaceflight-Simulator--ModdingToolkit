using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    // Used to hide domes to fix render issues
    public class DomeModule : MonoBehaviour
    {
        [SerializeField] Trigger[] triggers;
        [SerializeField] Point[] points;
        
        [Serializable]
        public class Trigger
        {
            public Composed_Vector2 position;
            public Composed_Vector2 normal;
        }
        
        [Serializable]
        public class Point
        {
            public Composed_Vector2 position;
            public Composed_Vector2 normal;
            public MeshRenderer dome;
        }
    }
}