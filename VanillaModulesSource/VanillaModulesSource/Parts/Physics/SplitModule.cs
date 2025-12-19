using UnityEngine;
using System;
using SFS.Variables;
using SFS.World;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Translations;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SplitModule : SeparatorBase, Rocket.INJ_Rocket, I_InitializePartModule
    {
        // Refs
        public SplitModule prefab;
        public Fragment[] fragments;
        [Space]
        // Description
        public bool showDescription = true;
        //
        [Space]
        public bool showForceMultiplier = false;
        public Float_Reference forceMultiplier;
        //
        [Space]
        public bool fairing;
        [ShowIf("fairing")] public Bool_Reference detachEdges;

        // State
        [Space]
        public String_Reference fragmentName;

        // Out
        [Space]
        public UnityEvent onDeploy;
        
        // State
        Part part;

        // Injected variables
        public Rocket Rocket { private get; set; }

        // Get
        float ForceMultiplier => showForceMultiplier? forceMultiplier.Value * 2 : 1;
        // Description
        public override bool ShowDescription => showDescription;
        public override void Draw(List<SeparatorBase> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            float force = fragments.Sum(f => f.separationForce.Value.magnitude);
            string GetForce() => (force * ForceMultiplier).ToSeparationForceString(false);
            string GetMaxForce() => (force * 2).ToSeparationForceString(showForceMultiplier);
            
            if (settings.build && showForceMultiplier)
                drawer.DrawSlider(0, GetForce, GetMaxForce, () => forceMultiplier.Value, (newValue, touchStart) => SetForcePercent(newValue, modules, touchStart), update => forceMultiplier.OnChange += update, update => forceMultiplier.OnChange -= update);
            else
                drawer.DrawStat(70, GetForce());
            
            if (fairing)
                if (settings.build || settings.game)
                    drawer.DrawToggle(0, () => Loc.main.Detach_Edges_Label, ToggleDetachEdges, () => detachEdges.Value, update => detachEdges.OnChange += update, update => detachEdges.OnChange -= update);

            void ToggleDetachEdges()
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    bool newValue = !detachEdges.Value;
                    foreach (SeparatorBase module in modules)
                        if (module is SplitModule splitModule && splitModule.fairing)
                            splitModule.detachEdges.Value = newValue;
                });
            }
        }
        
        
        // Initialize
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            part = transform.GetComponentInParentTree<Part>();
            
            if (!string.IsNullOrEmpty(fragmentName.Value))
                ApplyFragment();
        }

        // Button action
        public void Split(UsePartData data)
        {
            // Return if already split
            if (!string.IsNullOrEmpty(fragmentName.Value))
                return;

            bool wasPlayer = Rocket.isPlayer;
            Double2 centerOfMassOld = WorldView.ToGlobalPosition(Rocket.rb2d.worldCenterOfMass + PlayerController.main.cameraOffset);

            // List of parts to push
            List<SplitModule> partsToPush = new List<SplitModule>();

            // If fairing deploy all connected fairings
            if (fairing)
                foreach (SplitModule fairingModule in Rocket.jointsGroup.GetConnectedFairings(part, this))
                    fairingModule.Deploy(ref partsToPush);
            else
                Deploy(ref partsToPush);
            
            // Update groups
            JointGroup.RecreateRockets(Rocket, out List<Rocket> childRockets);

            // Rocket control
            List<Rocket> rockets = childRockets.ToList();
            rockets.Add(Rocket);
            if (wasPlayer)
            {
                Rocket.SetPlayerToBestControllable(rockets.ToArray());
                Rocket playerRocket = (Rocket)PlayerController.main.player.Value;
                playerRocket.location.position.Value = WorldView.ToGlobalPosition(playerRocket.physics.PhysicsObject.LocalPosition);
                PlayerController.main.SetOffset(WorldView.ToLocalPosition(centerOfMassOld) - (playerRocket.rb2d.worldCenterOfMass + PlayerController.main.cameraOffset), 4);
            }
            
            // Apply separation force to parts
            foreach (SplitModule partToPush in partsToPush)
                data.sharedData.onPostPartsActivation += partToPush.ApplySeparationForce;
        }
        void Deploy(ref List<SplitModule> partsToPush)
        {
            // Marks it as used
            fragmentName.Value = "!";
            onDeploy?.Invoke();
            Rocket.EnableCollisionImmunity(0.1f);
            
            JointGroup group = Rocket.jointsGroup;
            
            // Joints copy
            List<PartJoint> connections = group.GetConnectedJoints(part);

            // Remove part from this group
            group.RemovePartAndItsJoints(part);

            // Loop through fragments assigning appropriate rockets and creating joints
            for (int i = 0; i < fragments.Length; i++)
            {
                // Create fragment
                SplitModule fragment = CreateFragment(i);
                group.parts.Add(fragment.GetComponent<Part>());
                partsToPush.Add(fragment);

                // Loop trough previously connected parts
                foreach (PartJoint connection in connections)
                {
                    Part connectedPart = connection.GetOtherPart(part);

                    if (!ArePartsConnected(fragment.fragments[i], connectedPart))
                        continue;
                    
                    if (fairing)
                        if (detachEdges.Value && !(connectedPart.HasModule<SplitModule>() && connectedPart.GetModules<SplitModule>()[0].fairing))
                            continue; // Keeps fairing edge detached from non fairings

                    // Create the joint
                    group.AddJoint(new PartJoint(fragment.part, connectedPart, connection.GetRelativeAnchor(part)));
                }
            }
            
            // Destroy split part
            part.DestroyPart(false, false, DestructionReason.Intentional);
        }
        SplitModule CreateFragment(int index)
        {
            Part fragmentPart = PartsLoader.DuplicateParts(null, part)[0];
            
            SplitModule fragment = fragmentPart.GetModules<SplitModule>()[0];
            fragment.name = prefab.name;
            
            Transform a = fragment.part.transform;
            Transform b = part.transform;
            a.parent = b.parent;
            a.localPosition = b.localPosition;
            a.localRotation = b.localRotation;
            

            // Transfers burn mark
            if (part.burnMark != null)
            {
                fragment.part.burnMark = fragment.part.gameObject.AddComponent<BurnMark>();
                fragment.part.burnMark.Initialize();
                
                fragment.part.burnMark.burn = part.burnMark.burn.GetCopy();
                fragment.part.burnMark.ApplyEverything();
            }

            // Applies fragment
            fragment.fragmentName.Value = fragments[index].fragmentName;
            fragment.ApplyFragment();

            return fragment;
        }
        void ApplyFragment()
        {
            foreach (Fragment fragment in fragments)
                if (fragmentName.Value == fragment.fragmentName)
                {
                    foreach (GameObject a in fragment.toEnable)
                        a.SetActive(true);
                    foreach (GameObject a in fragment.toDisable)
                        a.SetActive(false);
                    
                    break;
                }
        }
        static bool ArePartsConnected(Fragment fragment, Part other)
        {
            Line2[] otherSurfaces = other.GetAttachmentSurfacesWorld();

            // If joint can be reconnected to this fragment
            foreach (SurfaceData surface in fragment.attachmentSurfaces)
                foreach (Surfaces chain in surface.surfaces)
                    if (SurfaceUtility.SurfacesConnect(otherSurfaces, chain.GetSurfacesWorld(), out _, out Vector2 center))
                        return true;

            return false;
        }

        void ApplySeparationForce()
        {
            Fragment fragment = fragments.Single(p => p.fragmentName == fragmentName.Value);
            Vector2 force = fragment.separationForce.Value * ForceMultiplier;
            Rocket.rb2d.AddForceAtPosition(transform.TransformVectorUnscaled(force), transform.TransformPoint(part.centerOfMass.Value), ForceMode2D.Impulse);
            PlayerController.main.CreateShakeEffect(force.magnitude / 200, 1.2f, 500, Rocket.rb2d.position);
        }

        [Serializable]
        public class Fragment
        {
            public string fragmentName;
            public GameObject[] toEnable, toDisable;
            public SurfaceData[] attachmentSurfaces; // Used to transfer joints
            public Composed_Vector2 separationForce;
        }
    }
}