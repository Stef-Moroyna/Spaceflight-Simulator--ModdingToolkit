using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Audio
{
    [CreateAssetMenu, HideMonoScript]
    public class SoundEffect : ScriptableObject
    {
        public List<Sound> sounds = new List<Sound>();
        
        public void Play(float volume = 1) { }

        public PlayingSoundEffect3D Play3D(Transform holder, float volume, bool loop) => null;
    }
}