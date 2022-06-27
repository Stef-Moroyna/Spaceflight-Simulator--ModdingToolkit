using UnityEngine;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SeparatorColor : ColorModule
    {
        [Required] public Part part;

        public Color buildColor = Color.white;
        public Float_Reference height;
        
        public override Color GetColor()
        {
            return Color.white;
        }

        
        // Updates interior toggle
        public UnityEvent onToggleInterior;
        bool initialized;
        void Awake()
        {
            initialized = true;
        }
        void OnToggleInterior()
        {
            if (initialized)
                onToggleInterior.Invoke();
        }
    }
}