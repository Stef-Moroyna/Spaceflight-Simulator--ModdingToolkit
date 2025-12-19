using SFS.World;
using SFS.Variables;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class DetachModule : SeparatorBase, Rocket.INJ_Rocket
    {
        // Refs
        [Required] public SurfaceData separationSurface;
        public Composed_Vector2 separationForce;
        [Space]
        // Description
        public bool showDescription = true;
        [Space]
        public bool showForceMultiplier = false;
        [HideIf("showForceMultiplier")] public bool useForceMultiplierEvenIfNotShown;
        public Float_Reference forceMultiplier;
        
        // Extra
        [Space]
        public bool cannotDetachIfSurfaceCovered;
        [ShowIf("cannotDetachIfSurfaceCovered"), Required] public SurfaceData surfaceForCover;
        [Space]
        public bool activatedByLES;
        
        // Out
        [Space]
        public UnityEvent onDetach;
        
        
        // Injection
        public Rocket Rocket { private get; set; }
        
        // Get
        bool Use => showForceMultiplier || useForceMultiplierEvenIfNotShown;
        float ForceMultiplier => Use? forceMultiplier.Value * 2 : 1;
        
        // Description
        public override bool ShowDescription => showDescription;
        public override void Draw(List<SeparatorBase> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            float force = separationForce.Value.magnitude;
            string GetForce() => (force * ForceMultiplier).ToSeparationForceString(false);
            string GetMaxForce() => (force * 2).ToSeparationForceString(showForceMultiplier);
            
            if (settings.build && showForceMultiplier)
                drawer.DrawSlider(70, GetForce, GetMaxForce, () => forceMultiplier.Value, (newValue, touchStart) => SetForcePercent(newValue, modules, touchStart), update => forceMultiplier.OnChange += update, update => forceMultiplier.OnChange -= update);
            else
                drawer.DrawStat(70, GetForce());
        }

        // Functions
        public void Detach(UsePartData data)
        {
            if (cannotDetachIfSurfaceCovered && SurfaceData.IsSurfaceCovered(surfaceForCover))
                return;

            List<DetachData> toDetach = GetJointsToDetach();

            if (toDetach.Count == 0)
                return;
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Rocket.location.velocity.Value.magnitude < 10)
            {
                float totalSeparationSurface = separationSurface.surfaces.Select(x => Vector2.Distance(x.points[0], x.points[1])).Aggregate((x, y) => x + y);
            
                foreach (DetachData detachData in toDetach)
                {
                    Rocket rocketOld = Rocket;
                    Double2 centerOfMassOld = WorldView.ToGlobalPosition(Rocket.rb2d.worldCenterOfMass + PlayerController.main.cameraOffset);
                    JointGroup.DestroyJoint(detachData.joint, Rocket, out bool split, out Rocket rocketNew); // Detaches parts

                    if (!split)
                        continue;
            
                    Vector3 force = transform.TransformVectorUnscaled(separationForce.Value * ForceMultiplier) * (detachData.overlapSurface / totalSeparationSurface);
                    Part otherPart = detachData.joint.GetOtherPart(transform.GetComponentInParentTree<Part>());
            
                    data.sharedData.onPostPartsActivation += () =>
                    {
                        if (otherPart != null)
                            otherPart.Rocket.rb2d.AddForceAtPosition(force, detachData.connectionPosition, ForceMode2D.Impulse);
                
                        Rocket.rb2d.AddForceAtPosition(-force, detachData.connectionPosition, ForceMode2D.Impulse);
                        PlayerController.main.CreateShakeEffect(force.magnitude / 50, 1.2f, 500, Rocket.rb2d.position);
                    };

                    rocketOld.EnableCollisionImmunity(0.1f);
                    rocketNew.EnableCollisionImmunity(0.1f);

                    if (rocketOld.isPlayer)
                    {
                        Rocket.SetPlayerToBestControllable(rocketOld, rocketNew);
                        Rocket playerRocket = (Rocket)PlayerController.main.player.Value;
                        playerRocket.location.position.Value = WorldView.ToGlobalPosition(playerRocket.physics.PhysicsObject.LocalPosition);
                        PlayerController.main.SetOffset(WorldView.ToLocalPosition(centerOfMassOld) - (playerRocket.rb2d.worldCenterOfMass + PlayerController.main.cameraOffset), 4);
                    }
                }

                onDetach?.Invoke();
            }
        }
        List<DetachData> GetJointsToDetach() // Gets all joints connected to the separation surface
        {
            Part part = transform.GetComponentInParentTree<Part>();
            Line2[] surfaces = separationSurface.surfaces.SelectMany(x => x.GetSurfacesWorld()).ToArray();
            
            List<DetachData> toDetach = new List<DetachData>();
            foreach (PartJoint joint in Rocket.jointsGroup.GetConnectedJoints(part))
            {
                Part otherPart = joint.GetOtherPart(part);
                Line2[] otherSurfaces = otherPart.GetAttachmentSurfacesWorld();
                
                bool connected = false;
                float totalOverlap = 0;
                Vector2 totalCenter = Vector2.zero;
                
                foreach (Line2 surface in surfaces)
                foreach (Line2 otherSurface in otherSurfaces)
                    if (SurfaceUtility.SurfacesConnect(surface, otherSurface, out float overlap, out Vector2 center))
                    {
                        connected = true;
                        totalOverlap += overlap;
                        totalCenter += center * overlap;
                    }

                if (connected)
                {
                    totalCenter /= totalOverlap;
                    toDetach.Add(new DetachData { joint = joint, overlapSurface = totalOverlap, connectionPosition = totalCenter });   
                }
            }

            return toDetach;
        }

        struct DetachData
        {
            public PartJoint joint;
            public float overlapSurface;
            public Vector2 connectionPosition;
        }
    }
}