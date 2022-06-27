using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace SFS.Audio
{
    [InlineProperty, Serializable]
    public class Sound
    {
        public AudioClip clip;

        [Range(0.0f, 1.0f)]
        public float volume = 1;

        [Range(0f, 3.0f)]
        public float pitch = 1;

        [Range(0.0f, 10.0f)]
        public float delay = 0;

        [Tooltip("Range is only used if the sound effect is played in 3D")]
        [Range(0.0f, 5000.0f)]
        public int range = 5000;
    }
}