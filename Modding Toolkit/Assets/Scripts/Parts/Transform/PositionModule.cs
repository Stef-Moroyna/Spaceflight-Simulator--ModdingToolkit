using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class PositionModule : MonoBehaviour, I_InitializePartModule
    {
        public Composed_Vector2 position;

        public int Priority => 8;
        public void Initialize() => position.OnChange += Position;

        [Button]
        public void Position()
        {
            transform.localPosition = position.Value;
        }
    }
}