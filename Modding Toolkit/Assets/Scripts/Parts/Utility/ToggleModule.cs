using SFS.Builds;
using SFS.Translations;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    public class ToggleModule : MonoBehaviour, I_PartMenu
    {
        public TranslationVariable label;
        [Required] public MoveModule state;

        void Start()
        {
                state.animationTime = 0.5f;
        }
        
    }
}