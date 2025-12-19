using UnityEngine;
using SFS.Variables;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SeparatorColor : ColorModule
    {
        public Color buildColor = Color.white, worldColor = Color.white;
        public Float_Reference height;
        
        public override Color GetColor() => !Application.isPlaying || (InteriorManager.main.interiorView.Value && height.Value > 1)? buildColor : worldColor;
        
        // Updates interior toggle
        public UnityEvent onToggleInterior;
        bool initialized;
        void Awake()
        {
            InteriorManager.main.interiorView.OnChange += OnToggleInterior;
            initialized = true;
        }
        void OnDestroy() => InteriorManager.main.interiorView.OnChange -= OnToggleInterior;
        void OnToggleInterior()
        {
            if (initialized)
                onToggleInterior.Invoke();
        }
    }
}