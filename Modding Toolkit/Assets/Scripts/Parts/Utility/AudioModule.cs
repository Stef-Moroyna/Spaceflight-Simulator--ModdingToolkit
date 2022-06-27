using SFS.Audio;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class AudioModule : MonoBehaviour
    {
        [BoxGroup, Required] public SoundEffect soundEffect;

        [BoxGroup("Settings", false)] public Composed_Float volume = new Composed_Float("1"), pitch = new Composed_Float("1");
        [BoxGroup("Settings", false)] public bool playOnStart;
        [BoxGroup("Settings", false)] public bool loop;

        [ShowInInspector, ReadOnly] bool init;
        [ShowInInspector, ReadOnly] PlayingSoundEffect3D playingSoundEffect;


        void Start()
        {
            volume.OnChange += OnVolumeChange;
            pitch.OnChange += OnPitchChange;

            init = true;

            if (playOnStart)
                Play();
        }

        void OnVolumeChange()
        {
            if (!init)
                return;

            if (volume.Value > 0)
                Play();
            else
                Stop();

            playingSoundEffect?.SetVolume(volume.Value);
        }
        void OnPitchChange()
        {
            playingSoundEffect?.SetPitch(pitch.Value);
        }

        [Button]
        void Play()
        {
            if (playingSoundEffect != null)
            {
                if (!playingSoundEffect.IsPlaying)
                    playingSoundEffect.Play();
            }
            else
                playingSoundEffect = soundEffect.Play3D(transform, volume.Value, loop);
        }
        void Stop()
        {
            playingSoundEffect?.Stop();
        }
    }
}