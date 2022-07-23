using Sirenix.OdinInspector;
using UnityEngine;

namespace ModLoader
{
    [CreateAssetMenu]
    public class PackData : ScriptableObject
    {
        public string DisplayName;
        public string Version;
        public string Description;
        public string Author;
        public bool ShowIcon;
        [ShowIf(nameof(ShowIcon))]public Texture2D Icon;
    }
}