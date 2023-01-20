using UnityEngine;
using SFS.Variables;
using SFS.Translations;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class TorqueModule : MonoBehaviour
    {
        [LabelText("Torque enabled")] public new Bool_Reference enabled = new Bool_Reference() { Value = true };
        public Composed_Float torque;
        
        public bool showDescription = true;
    }
}