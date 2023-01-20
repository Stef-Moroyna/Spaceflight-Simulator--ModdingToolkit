using UnityEngine;
using SFS.Variables;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SeparatorColor : ColorModule
    {
        public Color buildColor = Color.white, worldColor = Color.white;
        public Float_Reference height;
        
        public override Color GetColor() => buildColor;
        
        public UnityEvent onToggleInterior;
    }
}