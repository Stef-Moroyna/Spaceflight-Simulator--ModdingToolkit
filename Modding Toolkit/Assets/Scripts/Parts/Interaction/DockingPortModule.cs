using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class DockingPortModule : MonoBehaviour
    {
        [Required] public DockingPortTrigger trigger;
        [Required] public SurfaceData occupationSurface;

        public float dockDistance, pullDistance, pullForce;

        public Bool_Local isOccupied;
        public Bool_Local isOnCooldown;
        public Bool_Local isDockable;

        List<DockingPortModule> portsInRange = new List<DockingPortModule>();
        Part part;
        
        // Injected
        public Rocket Rocket { get; set; }

        void Awake()
        {
            part = transform.GetComponentInParentTree<Part>();
        }
        void Start()
        {
            isOccupied.OnChange += OnIsOccupiedChange;
        
            isOccupied.OnChange += UpdateIsDockable;
            isOnCooldown.OnChange += UpdateIsDockable;
            
            isDockable.OnChange += OnIsDockableChange;
        }

        public void UpdateOccupied()
        {
        }
        void OnIsOccupiedChange()
        {
            if (!isOccupied)
            {
                if (IsInvoking(nameof(EndCooldown)))
                    CancelInvoke(nameof(EndCooldown));

                isOnCooldown.Value = true;
                Invoke(nameof(EndCooldown), 2);
            }
        }
        void EndCooldown()
        {
            isOnCooldown.Value = false;
        }

        void UpdateIsDockable()
        {
            isDockable.Value = !isOccupied && !isOnCooldown;
        }
        void OnIsDockableChange(bool newValue)
        {
            if (newValue == false)
                portsInRange.Clear();
             
            trigger.gameObject.SetActive(newValue);
        }

        void FixedUpdate()
        {
            if (!isDockable)
                return;

            foreach (DockingPortModule otherPort in portsInRange)
            {
                if (!otherPort.isDockable)
                    continue;

                float distance = Vector2.Distance(transform.position, otherPort.transform.position);

                if (distance <= dockDistance)
                {
                    Dock(otherPort);
                    break;
                }

                // Pull
                Vector3 direction = (otherPort.transform.position - transform.position).normalized;
                Rocket.rb2d.AddForceAtPosition(pullForce * direction, transform.position);
            }
        }

        void Dock(DockingPortModule otherPort)
        {
        }

        public void AddPort(DockingPortModule port)
        {
            if (IsValidPort(port) && !portsInRange.Contains(port))
                portsInRange.Add(port);
        }
        public void RemovePort(DockingPortModule port)
        {
            if (IsValidPort(port) && portsInRange.Contains(port))
                portsInRange.Remove(port);
        }
        bool IsValidPort(DockingPortModule port)
        {
            return port != this && port.Rocket != null && port.Rocket != Rocket;
        }

        void OnDestroy()
        {
            foreach (DockingPortModule port in portsInRange)
                port.RemovePort(this);
        }
    }
}