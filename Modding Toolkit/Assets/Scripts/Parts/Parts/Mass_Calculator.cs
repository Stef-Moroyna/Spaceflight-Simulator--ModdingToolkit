using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts
{
    public class Mass_Calculator : MonoBehaviour
    {
        [Required] public PartHolder partHolder;
        

        void Start()
        {
            partHolder.TrackParts(
            addedPart =>
            {
                addedPart.mass.OnChange += MarkDirty;
                addedPart.centerOfMass.OnChange += MarkDirty;
            },
            removedPart =>
            {
                removedPart.mass.OnChange -= MarkDirty;
                removedPart.centerOfMass.OnChange -= MarkDirty;
            },
            MarkDirty);
        }
        void MarkDirty()
        {
            dirty = true;
        }

        // Get
        public float GetMass()
        {
            Calculate();
            return mass;
        }
        public Vector2 GetCenterOfMass()
        {
            Calculate();
            return centerOfMass;
        }
        
        // Calculate
        bool dirty = true;
        float mass;
        Vector2 centerOfMass;
        void Calculate()
        {
            if (!dirty)
                return;
            
            mass = 0;
            centerOfMass = Vector2.zero;

            foreach (Part part in partHolder.parts)
            {
                mass += part.mass.Value;
                centerOfMass += (part.Position + part.centerOfMass.Value * part.orientation) * part.mass.Value;
            }

            centerOfMass /= mass;
            dirty = false;
        }
    }
}