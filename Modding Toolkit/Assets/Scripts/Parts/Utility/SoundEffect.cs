using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Audio
{
    [CreateAssetMenu, HideMonoScript]
    public class SoundEffect : ScriptableObject
    {
        public List<Sound> sounds = new List<Sound>();
    }
}