using SFS.Parts.Modules;
using SFS.Translations;
using SFS.UI;
using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class DisableOnNoFlow : MonoBehaviour
    {
        public FlowModule flowModule;
        public Bool_Reference on;
        public bool logOutOfResource;

        bool previousFrameWasOn;

        void Update()
        {
            if (GameManager.main == null || !on.Value)
            {
                previousFrameWasOn = on.Value;
                return;
            }

            foreach (var source in flowModule.sources)
            {
                if (source.state.Value is FlowModule.FlowState.IsFlowing or FlowModule.FlowState.CanFlow)
                    continue;

                if (logOutOfResource)
                    MsgDrawer.main.Log(previousFrameWasOn
                        ? Loc.main.Msg_No_Resource_Left.InjectField(source.resourceType.displayName, "resource")
                        : Loc.main.Msg_No_Resource_Source.InjectField(source.resourceType.displayName, "resource")
                    );

                on.Value = false;
                break;
            }

            previousFrameWasOn = on.Value;
        }
    }
}