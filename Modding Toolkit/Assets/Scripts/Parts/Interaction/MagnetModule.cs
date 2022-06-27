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

        // Delete after transfer
        [HideInInspector] public Composed_Vector2[] snapPoints;
        void OnValidate()
        {
            if ((points == null || points.Length == 0) && snapPoints != null && snapPoints.Length > 0)
                points = snapPoints.Select(a => new Point { position = a }).ToArray();
        }

        public Vector2[] GetSnapPointsWorld()
        {
            return points.Select(p => (Vector2)transform.TransformPoint(p.position.Value)).ToArray();
        }
        
        
        // Delete after new build
        public static List<Vector2> GetSnapOffsets(MagnetModule A, MagnetModule B, float snapDistance)
        {
            List<Vector2> offsets = new List<Vector2>();

            foreach (Vector2 point_A in A.GetSnapPoints())
            foreach (Vector2 point_B in B.GetSnapPoints())
            {
                Vector2 offset = point_B - point_A;

                if (offset.sqrMagnitude < snapDistance * snapDistance)
                    offsets.Add(offset);
            }

            return offsets;
        }
        Vector2[] GetSnapPoints()
        {
            return points.Select(a => (Vector2)transform.TransformPoint(a.position.Value)).ToArray();
        }
        
        
        [Serializable]
        public class Point
        {
            public Composed_Vector2 position;
        }
    }
}