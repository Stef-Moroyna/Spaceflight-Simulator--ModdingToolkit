using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class PositionModule : MonoBehaviour
    {
        public Composed_Vector2 position;

        [Button]
        public void Position()
        {
            transform.localPosition = position.Value;
        }
    }
}