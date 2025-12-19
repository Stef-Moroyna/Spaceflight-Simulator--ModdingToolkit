using System.Collections.Generic;
using SFS.Builds;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public abstract class SeparatorBase : MonoBehaviour
    {
        public abstract bool ShowDescription { get; }
        public abstract void Draw(List<SeparatorBase> modules, StatsMenu drawer, PartDrawSettings settings);
        
        protected static void SetForcePercent(float newValue, List<SeparatorBase> modules, bool touchStart)
        {
            Undo.main.RecordStatChangeStep(modules, () =>
            {
                foreach (SeparatorBase module in modules)
                    switch (module)
                    {
                        case DetachModule detachModule:

                            if (detachModule.showForceMultiplier)
                                detachModule.forceMultiplier.Value = newValue;

                            break;

                        case SplitModule splitModule:
                        {
                            if (splitModule.showForceMultiplier)
                                splitModule.forceMultiplier.Value = newValue;

                            break;
                        }
                    }
            }, touchStart);
        }
    }   
}
