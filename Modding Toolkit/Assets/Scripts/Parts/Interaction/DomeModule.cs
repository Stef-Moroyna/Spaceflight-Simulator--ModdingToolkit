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
        
        [SerializeField, ShowIf(nameof(HasTriggers))] bool activeInBuild = true;
        bool HasTriggers => triggers.Length > 0;
        
        public static void UpdateInteraction(params Part[] parts)
        {
        }
        void Adapt(List<(Vector3 position, Vector2 normal)> triggerWorldPositions)
        {
            foreach (Point point in points)
            {
                Vector3 pointPosition = transform.TransformPoint(point.position.Value);
                Vector2 pointNormal = transform.TransformVectorUnscaled(point.normal.Value);
                point.dome.enabled = !triggerWorldPositions.Any(trigger => (trigger.position - pointPosition).sqrMagnitude < 0.01f && (pointNormal + trigger.normal).sqrMagnitude < 0.01f);   
            }
        }
        
        [Serializable]
        public class Trigger
        {
            public Composed_Vector2 position;
            public Composed_Vector2 normal;

            public (Vector3, Vector2) GetWorld(DomeModule owner)
            {
                return (owner.transform.TransformPoint(position.Value), owner.transform.TransformVectorUnscaled(normal.Value));
            }
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