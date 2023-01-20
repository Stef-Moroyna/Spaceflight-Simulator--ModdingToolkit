using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class PolygonCollider : ColliderModule
    {
        [Required] public PolygonData polygon;
    }
}