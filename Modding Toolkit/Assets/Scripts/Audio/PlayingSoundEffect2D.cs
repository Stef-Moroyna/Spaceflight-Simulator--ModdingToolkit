using System;
using UnityEngine;

namespace SFS.Audio
{
    public class PlayingSoundEffect2D
    {
        Tuple<Sound, AudioSource>[] sources;
        float volume;

        public PlayingSoundEffect2D(Tuple<Sound, AudioSource>[] sources, float volume)
        {
            this.sources = sources;
            this.volume = volume;
        }
        
        public void ApplyVolume()
        {
        }
    }
}