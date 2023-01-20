using System.Collections.Generic;
using SFS.Builds;
using SFS.Translations;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    public class ToggleModule : MonoBehaviour
    {
        public TranslationVariable label;
        [Required] public MoveModule state;
    }
}