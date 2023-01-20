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
    }
}