using SFS.Parts.Modules;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.World;
using SFS.Variables;

namespace SFS.Parts
{
    public class WheelModule : MonoBehaviour
    {
        // Stats
        [BoxGroup("Stats", false)] public float power;
        [BoxGroup("Stats", false)] public float traction;
        [BoxGroup("Stats", false)] public float maxAngularVelocity;
        [BoxGroup("Stats", false)] public float wheelSize;

        // State
        [BoxGroup("State", false)] public float angularVelocity;
        [BoxGroup("State", false)] public Bool_Reference on;

        public void ToggleEnabled()
        {
        }
    }
}