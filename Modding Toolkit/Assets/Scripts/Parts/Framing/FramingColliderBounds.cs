using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class FramingColliderBounds : MonoBehaviour
    {
        [Required] public PolygonData shape;
    }
}