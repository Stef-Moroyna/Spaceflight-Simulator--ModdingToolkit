using SFS.Translations;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [CreateAssetMenu]
    public class ResourceType : ScriptableObject
    {
        public TranslationVariable displayName;
        public TranslationVariable resourceUnit;
        //
        public double resourceMass;
        public double transferRate;
    }
}