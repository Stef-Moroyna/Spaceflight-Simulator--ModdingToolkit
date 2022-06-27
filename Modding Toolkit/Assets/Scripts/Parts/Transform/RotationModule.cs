using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class RotationModule : MonoBehaviour
    {
        public Composed_Float rotation;

        void Start()
        {
            rotation.OnChange += Rotate;
        }

        [Button(ButtonSizes.Medium)]
        public void Rotate()
        {
            transform.localEulerAngles = new Vector3(0, 0, rotation.Value);
        }
    }
}