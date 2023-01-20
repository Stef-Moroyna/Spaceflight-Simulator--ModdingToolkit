using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class ParachuteModule : MonoBehaviour
    {
        // Reference
        public double maxDeployHeight;
        public double maxDeployVelocity;
        [Space]
        public AnimationCurve drag;
        [Required] public Transform parachute;
        
        // State
        [Space]
        public Float_Reference state;
        public Float_Reference targetState;
        
        // Output
        [Space]
        public UnityEvent onDeploy;
        
        public void DeployParachute(UsePartData data)
        {
        }
    }
}