using System;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public abstract class SurfaceData : MonoBehaviour
    {
        [SerializeField, BoxGroup("", false)] bool attachmentSurfaces = true; // Is the surface used for attachment
        [SerializeField, BoxGroup("", false)] bool dragSurfaces = true; // Is the surface used for drag


        // Data
        public List<Surfaces> surfaces;
        public List<Surfaces> surfacesFast;
        [NonSerialized] public Event_Local onChange = new Event_Local();


        public bool Attachment => attachmentSurfaces && isActiveAndEnabled;
        public bool Drag => dragSurfaces && isActiveAndEnabled;


        public abstract void Output();
        protected void SetData(List<Surfaces> surfaces) => SetData(surfaces, surfaces);
        protected void SetData(List<Surfaces> surfaces, List<Surfaces> surfacesFast)
        {
            this.surfaces = surfaces;
            this.surfacesFast = surfacesFast;
            onChange.Invoke();
        }
    }

    public class Surfaces
    {
        public readonly Vector2[] points;
        public readonly Transform owner;
        public readonly bool loop;

        // Constructor
        public Surfaces(Vector2[] points, bool loop, Transform owner)
        {
            this.owner = owner;
            this.points = points;
            this.loop = loop;
        }

        public Line2[] GetSurfacesWorld()
        {
            if (points.Length < 2)
                return new Line2[]{};
            
            // Converts to world
            Vector2[] pointsWorld = points.Select(p => (Vector2)owner.TransformPoint(p)).ToArray();

            // Creates surfaces from points
            Line2[] surfaces = new Line2[loop? pointsWorld.Length : pointsWorld.Length - 1];
            for (int i = 0; i < pointsWorld.Length - 1; i++)
                surfaces[i] = new Line2(pointsWorld[i], pointsWorld[i + 1]);

            // Closes loop
            if (loop)
                surfaces[surfaces.Length - 1] = new Line2(pointsWorld[pointsWorld.Length - 1], pointsWorld[0]);

            return surfaces;
        }
    }
}