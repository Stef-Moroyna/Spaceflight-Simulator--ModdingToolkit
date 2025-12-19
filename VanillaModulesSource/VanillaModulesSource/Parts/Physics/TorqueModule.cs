using UnityEngine;
using SFS.Variables;
using SFS.Translations;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class TorqueModule : MonoBehaviour, I_PartMenu
    {
        [LabelText("Torque enabled")] public new Bool_Reference enabled = new Bool_Reference() { Value = true };
        public Composed_Float torque;
        
        public bool showDescription = true;

        // Description
        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            if (showDescription)
                drawer.DrawStat(80, Loc.main.Torque_Module_Torque.Inject(torque.Value.ToString(), "value"), null);
        }
    }
}