using SFS.Translations;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class IsCoveredModule : MonoBehaviour
    {
        public SurfaceData surface;
        public TranslationVariable onCoveredMsg;
        public UsePartUnityEvent onPartUsed;
        
        public void Activate(UsePartData a)
        {
        }
    }
}