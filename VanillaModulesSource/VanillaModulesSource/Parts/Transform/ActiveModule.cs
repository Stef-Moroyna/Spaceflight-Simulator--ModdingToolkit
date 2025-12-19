using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class ActiveModule : MonoBehaviour, I_InitializePartModule
    {
        public Bool_Reference active;
        public bool invert;
        
        public int Priority => 5;
        public void Initialize() => active.OnChange += UpdateActive;

        void UpdateActive()
        {
            bool _active = active.Value;

            if (invert)
                _active = !_active;

            if (gameObject.activeSelf != _active)
                gameObject.SetActive(_active);
        }
    }
}