using UnityEngine;
using Sirenix.OdinInspector;
using SFS.Variables;
using System;
using System.Collections.Generic;
using SFS.Builds;
using SFS.World;
using SFS.Translations;

namespace SFS.Parts.Modules
{
    [ExecuteInEditMode]
    public class ResourceModule : MonoBehaviour
    {
        // Refs
        [BoxGroup] public ResourceType resourceType;
        [BoxGroup] public Double_Reference wetMass = new Double_Reference();
        [BoxGroup] public Double_Reference dryMassPercent = new Double_Reference();
        [BoxGroup] public Double_Reference resourcePercent = new Double_Reference();
        [BoxGroup] public bool setMass;
        [BoxGroup, ShowIf(nameof(setMass))] public Double_Reference mass = new Double_Reference();

        // State
        [HideInInspector] public ResourceModule parent;
        [HideInInspector] public List<ResourceModule> children;
        //
        [HideInInspector] public List<FlowModule.Flow> flowModules = new List<FlowModule.Flow>();
        
        public bool showDescription = true;
        
        public void ToggleTransfer()
        {
        }
    }
}