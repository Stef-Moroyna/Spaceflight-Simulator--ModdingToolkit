using UnityEngine;
using System;
using SFS.Variables;
using SFS.World;
using System.Collections.Generic;
using System.Linq;
using SFS.Translations;
using Sirenix.OdinInspector;
using Joint = SFS.World.Joint<SFS.Parts.Part>;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class SplitModule : MonoBehaviour, I_PartMenu, I_InitializePartModule
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
        SplitModule[] preloadedFragments;

        
        // Injected variables
        public Rocket Rocket { private get; set; }

        // Get
        float ForceMultiplier => showForceMultiplier? forceMultiplier.Value * 2 : 1;


        // Initialize
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            part = transform.GetComponentInParentTree<Part>();
            
            // Prevents infinite loop
            if (fragmentName.Value == "PRELOADED")
                return;

            if (string.IsNullOrEmpty(fragmentName.Value))
            {
                // Will fragment, so it preloads
            }
            else
                ApplyFragment();
        }
        void OnDestroy()
        {
            if (preloadedFragments != null)
                foreach (SplitModule fragment in preloadedFragments)
                    if (fragment != null)
                        Destroy(fragment.gameObject);
        }

        // Button action
        public void Split(UsePartData data)
        {
            // Return if already split
            if (!string.IsNullOrEmpty(fragmentName.Value))
                return;

            // List of parts to push
            List<SplitModule> partsToPush = new List<SplitModule>();

            // If fairing deploy all connected fairings otherwise only deploy this module
            if (fairing)
                foreach (SplitModule fairingModule in GetConnectedFairings())
                    fairingModule.Deploy(ref partsToPush);
            else
                Deploy(ref partsToPush);

            // Apply separation force to parts
            foreach (SplitModule partToPush in partsToPush)
                data.sharedData.onPostPartsActivation += partToPush.ApplySeparationForce;
        }
        void Deploy(ref List<SplitModule> partsToPush)
        {
        }
        

        SplitModule CreateFragment(int index)
        {
            SplitModule fragment = preloadedFragments[index];
            preloadedFragments[index] = null;

            fragment.gameObject.SetActive(true);
            fragment.name = prefab.name;
            fragment.part.transform.parent = part.transform.parent;
            fragment.part.transform.position = part.transform.position;
            fragment.part.transform.rotation = part.transform.rotation;
            

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

        SplitModule[] GetConnectedFairings()
        {
            return null;
        }

        void ApplySeparationForce()
        {
            Fragment fragment = fragments.Single(p => p.fragmentName == fragmentName.Value);
            Rocket.rb2d.AddForceAtPosition(transform.TransformVectorUnscaled(fragment.separationForce.Value * ForceMultiplier), transform.TransformPoint(part.centerOfMass.Value), ForceMode2D.Impulse);
        }


        
        [Serializable]
        public class Fragment
        {
            public string fragmentName;
            public GameObject[] toEnable, toDisable;
            public SurfaceData[] attachmentSurfaces; // Used to transfer joints
            public Composed_Vector2 separationForce;

            //public Composed_Vector2 centerOfMass;
            //public Composed_Float newMass;
        }
    }
}