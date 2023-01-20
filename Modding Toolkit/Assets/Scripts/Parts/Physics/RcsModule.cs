using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class RcsModule : MonoBehaviour
    {
        public float directionAngleThreshold, torqueAngleThreshold, thrust, ISP;
        public List<Thruster> thrusters = new List<Thruster>();
        [Required] public FlowModule source;

        public Vector2 thrustPosition;
        
        // Activate part
        public void ToggleRCS(UsePartData data)
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