using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class DockingPortTrigger : MonoBehaviour
    {
        [Required] public DockingPortModule dockingPort;

        Rigidbody2D rb2d;
        CircleCollider2D trigger;
        int layer;

        void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            rb2d.isKinematic = true;

            trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = dockingPort.pullDistance;

            layer = LayerMask.NameToLayer("Docking Trigger");
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == layer)
                dockingPort.AddPort(other.GetComponent<DockingPortTrigger>().dockingPort);
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == layer)
                dockingPort.RemovePort(other.GetComponent<DockingPortTrigger>().dockingPort);
        }
    }
}