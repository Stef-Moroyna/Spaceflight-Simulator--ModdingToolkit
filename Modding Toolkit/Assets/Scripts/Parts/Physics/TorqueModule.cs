using UnityEngine;
using SFS.Variables;
using SFS.Translations;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class TorqueModule : MonoBehaviour, I_PartMenu
    {
        public Composed_Float torque;
        public bool showDescription = true;

    }
}