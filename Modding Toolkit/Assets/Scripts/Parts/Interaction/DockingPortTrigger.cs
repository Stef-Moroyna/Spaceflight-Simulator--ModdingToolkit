using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class DockingPortTrigger : MonoBehaviour
    {
        [Required] public DockingPortModule dockingPort;
    }
}