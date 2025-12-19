using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class ScaleModule : MonoBehaviour
    {
        public Composed_Vector2 scale;

        void Start()
        {
            scale.OnChange += Scale;
        }

        [Button(ButtonSizes.Medium)]
        public void Scale()
        {
            transform.localScale = scale.Value;
        }
    }
}