using SFS.World;
using SFS.Translations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using SFS.Platform;

namespace SFS.Parts.Modules
{
    public class RcsModule : MonoBehaviour
    {
        public float directionAngleThreshold, torqueAngleThreshold, thrust, ISP;
        public List<Thruster> thrusters = new List<Thruster>();
        [Required] public FlowModule source;

        public Vector2 thrustPosition;

        // Injection
        public Rocket Rocket { get; set; }
        public bool IsPlayer { get; set; }
        public float TurnAxis { get; set; }
        public Vector2 DirectionalAxis { get; set; }

        void Start()
        {
            source.onStateChange += Update_RCS_On;
        }
        void Update_RCS_On()
        {
           
            ToggleRCS(new UsePartData(new UsePartData.SharedData()), false);
        }

        
        // Activate part
        public void ToggleRCS(UsePartData data) => ToggleRCS(data, true);
        void ToggleRCS(UsePartData data, bool showMsg)
        {
        }

        [Serializable]
        public class Thruster
        {
            public Vector2 thrustNormal;
            [Required] public MoveModule effect;
        }
    }
}