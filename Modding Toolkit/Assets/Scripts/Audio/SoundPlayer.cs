using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        public static SoundPlayer main;
        void Awake() => main = this;

        
        // Resources
        [Required] public SoundEffect clickSound, pickupSound, dropSound, denySound;
        
        // State
        Dictionary<int, AudioSource> sources = new Dictionary<int, AudioSource>();
        int lastID = -1;

        List<PlayingSoundEffect2D> playing_2D = new List<PlayingSoundEffect2D>();
        List<PlayingSoundEffect3D> playing_3D = new List<PlayingSoundEffect3D>();

        
        void OnVolumeChange()
        {
            foreach (PlayingSoundEffect2D sound in playing_2D)
                sound.ApplyVolume();
            foreach (PlayingSoundEffect3D sound in playing_3D)
                sound.ApplyVolume();
        }

        public PlayingSoundEffect3D PlaySound_3D(SoundEffect soundEffect, Transform holder, float volume, bool loop)
        {
            Tuple<Sound, AudioSource>[] soundSources = new Tuple<Sound, AudioSource>[soundEffect.sounds.Count];

            for (int i = 0; i < soundEffect.sounds.Count; i++)
            {
                Sound sound = soundEffect.sounds[i];

                AudioSource source = holder.gameObject.AddComponent<AudioSource>();
                source.loop = loop;
                source.clip = sound.clip;
                source.pitch = sound.pitch;
                source.dopplerLevel = 0.0f;
                source.playOnAwake = false;
                source.maxDistance = sound.range;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.minDistance = 1.0f;
                source.spatialBlend = 1.0f;
                source.panStereo = 0.0f;
                
                soundSources[i] = new Tuple<Sound, AudioSource>(sound, source);
            }

            PlayingSoundEffect3D output = new PlayingSoundEffect3D(soundSources, volume);
            output.Play();
            output.ApplyVolume();
            
            playing_3D.Add(output);
            return output;
        }
        public void Clear3DSounds()
        {
            playing_3D.Clear();
        }

        public void PlaySound_2D(SoundEffect soundEffect, float volume)
        {
            Tuple<Sound, AudioSource>[] soundSources = new Tuple<Sound, AudioSource>[soundEffect.sounds.Count];

            for (int i = 0; i < soundEffect.sounds.Count; i++)
            {
                Sound sound = soundEffect.sounds[i];
                AudioSource source = GetFreeSource(out int id);

                // Update values and play
                source.Stop();
                source.clip = sound.clip;
                source.pitch = sound.pitch;
                source.dopplerLevel = 0.0f;
                source.playOnAwake = false;

                if (sound.delay > 0)
                    source.PlayDelayed(sound.delay);
                else
                    source.Play();
                
                soundSources[i] = new Tuple<Sound, AudioSource>(sound, source);
            }

            PlayingSoundEffect2D output = new PlayingSoundEffect2D(soundSources, volume);
            output.ApplyVolume();
            
            playing_2D.Add(output);
        }
        AudioSource GetFreeSource(out int id)
        {
            AudioSource source = null;
            id = -1;

            // Loop through sources to find a source to reuse
            foreach (KeyValuePair<int, AudioSource> pair in sources)
            {
                AudioSource potentialSource = pair.Value;

                if (!potentialSource.isPlaying)
                {
                    potentialSource.clip = null;
                    if (source == null)
                    {
                        source = potentialSource;
                        id = pair.Key;
                    }
                }
            }

            // If there are no available sources, add a new one, otherwise remove the chosen source to be re-added with a new key
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
            else
                sources.Remove(id);

            // Increment id to insure uniqueness
            lastID += 1;
            id = lastID;

            // Add source to dictionary
            sources.Add(id, source);

            return source;
        }
    }
}