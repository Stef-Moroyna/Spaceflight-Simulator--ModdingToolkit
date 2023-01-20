using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class MagnetModule : MonoBehaviour
    {
        public Point[] points;
        
        public Vector2[] GetSnapPointsWorld()
        {
            return points.Select(p => (Vector2)transform.TransformPoint(p.position.Value)).ToArray();
        }

        [Serializable]
        public class Point
        {
            public Composed_Vector2 position;
        }
    }
}