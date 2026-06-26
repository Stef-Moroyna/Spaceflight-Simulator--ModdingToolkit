// Auto-generated compile-only dummy stubs for the public modding toolkit. No implementation, no anti-hack.

using System;
using UnityEngine;

namespace Beebyte.Obfuscator
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class SkipAttribute : Attribute { }
}

namespace SFS.Platform
{
    public enum PlatformType
    {
        Mobile,
        PC
    }

    public class PlatformManager : MonoBehaviour
    {
        public static readonly PlatformType current = PlatformType.PC;
    }
}

namespace SFS.Stats
{
    public class StatsRecorder : MonoBehaviour
    {
        public static void OnSplit(StatsRecorder A, StatsRecorder B) { }
        public static void OnMerge(StatsRecorder A, StatsRecorder B) { }
        public void OnCrash(float impactVelocity) { }
        public void OnLeaveCapsule(string astronautName) { }
    }
}

namespace SFS.Input
{
    public class TouchPosition
    {
        public Vector2 pixel;
        public Vector2 World(float positionZ) { return default; }
    }

    public class OnInputEndData
    {
        public TouchPosition position;
        public bool click;
        public bool LeftClick { get { return default; } }
    }
}

namespace SFS.Audio
{
    public class SoundEffect : ScriptableObject
    {
        public PlayingSoundEffect3D Play3D(Transform holder, float volume, bool loop) { return default; }
    }

    public class PlayingSoundEffect3D
    {
        public bool IsPlaying { get { return default; } }
        public void Play() { }
        public void Stop() { }
        public void SetVolume(float volume) { }
        public void SetPitch(float pitch) { }
    }

    public class AudioSettings : MonoBehaviour
    {
        public static AudioSettings main;
        public float soundVolume;
    }
}
