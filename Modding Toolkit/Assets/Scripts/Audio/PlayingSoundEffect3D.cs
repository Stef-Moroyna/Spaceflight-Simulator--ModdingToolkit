using System;
using System.Linq;
using UnityEngine;

namespace SFS.Audio
{
    public class PlayingSoundEffect3D
    {
        Tuple<Sound, AudioSource>[] sources;
        float volume;
        
        public PlayingSoundEffect3D(Tuple<Sound, AudioSource>[] sources, float volume)
        {
            this.sources = sources;
            this.volume = volume;
        }

        public bool IsPlaying => sources.Any(a => a.Item2.isPlaying);
        
        public void Play()
        {
            Stop();
            
            foreach ((Sound sound, AudioSource audioSource) in sources)
            {
                if (sound.delay > 0.0f)
                    audioSource.PlayDelayed(sound.delay);
                else
                    audioSource.Play();
            }
        }
        public void Stop()
        {
            foreach (Tuple<Sound, AudioSource> item in sources)
                item.Item2.Stop();
        }
        public void SetVolume(float volume)
        {
            this.volume = volume;
            ApplyVolume();
        }
        public void ApplyVolume()
        {
        }
        public void SetPitch(float pitch)
        {
            foreach ((Sound sound, AudioSource audioSource) in sources)
                audioSource.pitch = sound.pitch * pitch;
        }
    }
}