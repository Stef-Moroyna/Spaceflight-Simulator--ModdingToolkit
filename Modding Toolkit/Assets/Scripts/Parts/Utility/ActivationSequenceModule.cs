using SFS.Variables;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class ActivationSequenceModule : MonoBehaviour
    {
        public Float_Reference state;
        [Space]
        public UsePartUnityEvent[] steps;


        public void Activate(UsePartData data)
        {
            if (CanActivate)
            {
                steps[Index].Invoke(data);
                state.Value += 1;   
            }
        }

        bool CanActivate => steps.IsValidIndex(Index);
        int Index => Mathf.RoundToInt(state.Value);
    }
}