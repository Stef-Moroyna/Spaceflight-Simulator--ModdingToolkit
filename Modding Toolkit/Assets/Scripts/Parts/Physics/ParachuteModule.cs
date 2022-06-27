using SFS.Translations;
using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class ParachuteModule : MonoBehaviour
    {
        // Reference
        [Required] public Transform parachute;
        public AnimationCurve drag;
        public UnityEvent onDeploy;
        //
        public double maxDeployHeight;
        public double maxDeployVelocity;
        
        // State
        public Float_Reference state;
        public Float_Reference targetState;
        Double2 oldPosition;


        public void DeployParachute(UsePartData data)
        {
        }


        void Start()
        {
            targetState.OnChange += UpdateEnabled;
        }
        void UpdateEnabled()
        {
            // Only enables to check stuff when half or fully deployed
            enabled = targetState.Value == 1 || targetState.Value == 2;
        }
    }
}