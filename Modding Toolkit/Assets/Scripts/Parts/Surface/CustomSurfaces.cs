using System;
using UnityEngine;
using SFS.Variables;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public sealed class CustomSurfaces : SurfaceData, I_InitializePartModule
    {
        // Legacy
        [HideInInspector, SerializeField] Composed_Vector2[] points;
        [HideInInspector, SerializeField] bool loop;
        void OnValidate()
        {
            if (points != null && points.Length > 0)
            {
                pointsArray = new List<ComposedSurfaces> { new ComposedSurfaces { points = points, loop = loop }};
                points = null;
            }
        }


        [Space]
        public List<ComposedSurfaces> pointsArray = new List<ComposedSurfaces> { new ComposedSurfaces() };

        [BoxGroup("edit", false), HorizontalGroup("edit/h")] public bool edit = false, view = false;
        [BoxGroup("edit", false), ShowIf("edit")] public float gridSize = 0.1f;
        
        
        int I_InitializePartModule.Priority => 10;
        void I_InitializePartModule.Initialize() => Output();
        
        
        public override void Output()
        {
            List<Surfaces> output = pointsArray.Select(a => new Surfaces(a.points.Select(p => p.Value).ToArray(), a.loop, transform)).ToList();
            SetData(output, output);
        }
    }

    [Serializable]
    public class ComposedSurfaces
    {
        public Composed_Vector2[] points = new Composed_Vector2[2];
        [HorizontalGroup] public bool loop;
        [HorizontalGroup] [Button] void Reverse() => points = points.Reverse().ToArray();
    }
}