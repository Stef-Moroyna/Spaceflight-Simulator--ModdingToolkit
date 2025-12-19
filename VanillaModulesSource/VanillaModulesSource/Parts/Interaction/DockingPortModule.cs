using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class DockingPortModule : MonoBehaviour, Rocket.INJ_Rocket
    {
        [Required] public DockingPortTrigger trigger;
        [Required] public SurfaceData occupationSurface;

        public float dockDistance, pullDistance, pullForce;
        public Float_Reference forceMultiplier;

        public Bool_Local isOccupied;
        public Bool_Local isOnCooldown;
        public Bool_Local isDockable;

        // Injected
        Part part;
        public Rocket Rocket { get; set; }
        
        // State
        List<DockingPortModule> portsInRange = new List<DockingPortModule>();

        
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
            Line2[] mySurfaces = occupationSurface.surfaces[0].GetSurfacesWorld();
            isOccupied.Value = Rocket.jointsGroup.GetConnectedJoints(part).Any(joint => SurfaceUtility.SurfacesConnect(joint.GetOtherPart(part), mySurfaces, out _, out _));
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
                Rocket.rb2d.AddForceAtPosition(pullForce * forceMultiplier.Value * 2 * direction, transform.position);
            }
        }
        void Dock(DockingPortModule otherPort)
        {
            if (otherPort.Rocket.isPlayer)
                return;

            Vector2 normal_A = Vector2.up * part.orientation;
            Vector2 normal_B = Vector2.up * otherPort.part.orientation;

            Orientation diff = new Orientation(1, 1, (int)(Mathf.Atan2(normal_A.y, normal_A.x) * Mathf.Rad2Deg - Mathf.Atan2(normal_B.y, normal_B.x) * Mathf.Rad2Deg) + 180);
            diff.z = Mathf.RoundToInt(diff.z / 90f) * 90;

            foreach (Part rocketPart in otherPort.Rocket.partHolder.parts)
                rocketPart.orientation.orientation.Value += diff;
            foreach (PartJoint joint in otherPort.Rocket.jointsGroup.joints)
                joint.anchor *= diff;
            
            Vector2 centerOfMassOld = Rocket.rb2d.worldCenterOfMass;
            RocketManager.MergeRockets(Rocket, part, otherPort.Rocket, otherPort.part, Vector2.zero);

            if (Rocket.isPlayer)
                PlayerController.main.SetOffset(centerOfMassOld - Rocket.rb2d.worldCenterOfMass, 1);
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

        public void Draw(List<DockingPortModule> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            string GetForce() => (pullForce * forceMultiplier.Value * 2).ToMagnetForceString(true);
            string GetMaxForce() => (pullForce).ToMagnetForceString(true);
            
            if (settings.build)
                drawer.DrawSlider(71, GetForce, GetMaxForce, () => forceMultiplier.Value, SetForce, x => forceMultiplier.OnChange += x, x => forceMultiplier.OnChange -= x);
            
            void SetForce(float value, bool touchStart)
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    foreach (DockingPortModule module in modules)
                        module.forceMultiplier.Value = value;
                }, touchStart);
            }
        }
    }
}