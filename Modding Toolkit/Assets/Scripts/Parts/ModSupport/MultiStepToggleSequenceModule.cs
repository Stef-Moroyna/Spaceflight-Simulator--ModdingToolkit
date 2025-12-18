using UnityEngine;
using SFS.Variables;

/* It is used to Allow For repeatable Toggle Sequences for a part
 e.g. When you want an engine to only be on when the solar Array Is Retracted or vice versa and in a way that it is repeatable */
namespace SFS.Parts.Modules
{
    public class MultiStepToggleSequenceModule : MonoBehaviour
    {
        public Float_Reference state;
        public UsePartUnityEvent[] steps;

        public void Activate(UsePartData data)
        {
            if (CanActivate)
            {
                // Run or "Invoke" the current step
                steps[Index].Invoke(data);

                // Increment and wrap around automatically based on thy variable steps.Length
                state.Value = (state.Value + 1) % steps.Length;
            }
        }
        bool CanActivate => steps.IsValidIndex(Index);
        int Index => Mathf.RoundToInt(state.Value);
    }
}