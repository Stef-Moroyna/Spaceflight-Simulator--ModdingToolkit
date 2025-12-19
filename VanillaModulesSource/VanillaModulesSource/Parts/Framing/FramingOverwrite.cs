using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class FramingOverwrite : MonoBehaviour
    {
        public Vector2 a, b;
        public Rect GetBounds_WorldSpace() => new Rect(transform.TransformPoint(a), transform.TransformVector(b - a));
    }
}