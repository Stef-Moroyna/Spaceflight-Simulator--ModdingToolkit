using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class LinkModule : MonoBehaviour
    {
        public Composed_Float input;
        public Float_Reference output;


        void Start()
        {
            input.OnChange += RecalculateOutput;
        }

        [Button(ButtonSizes.Medium)]
        public void RecalculateOutput()
        {
            output.Value = input.Value;
        }
    }
}