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
    public class ResourceModule : MonoBehaviour, I_InitializePartModule, Rocket.INJ_Rocket, ResourceDrawer.I_Resource
    {
        // Refs
        [BoxGroup] public ResourceType resourceType;
        [BoxGroup] public Double_Reference wetMass = new Double_Reference();
        [BoxGroup] public Double_Reference dryMassPercent = new Double_Reference();
        [BoxGroup] public Double_Reference resourcePercent = new Double_Reference();
        [BoxGroup] public bool setMass = true;
        [BoxGroup, ShowIf(nameof(setMass))] public Double_Reference mass = new Double_Reference();
        [BoxGroup, ShowIf(nameof(setMass))] public bool setDensity = true;

        // State
        [HideInInspector] public ResourceModule parent;
        [HideInInspector] public List<ResourceModule> children;
        //
        [HideInInspector] public List<FlowModule.Flow> flowModules = new List<FlowModule.Flow>();



        // Injected
        public Rocket Rocket { private get; set; }
        // Ref
        Part part;

        // Description
        public bool showDescription = true;
        public void Draw(List<ResourceModule> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            if (settings.build || settings.game)
                drawer.DrawSlider(0, GetLabelAndValue, MaxSize, () => (float)resourcePercent.Value, SetResource, Register, Unregister); // Slider
            else
                drawer.DrawStat(0, GetLabelAndValue, MaxSize, Register, Unregister); // Stat
            
            string GetLabelAndValue() => Loc.main.Info_Resource_Amount.InjectField(resourceType.displayName, "resource", true).Inject(ResourceAmount.ToString(2, false) + resourceType.resourceUnit.Field, "amount");
            string MaxSize() => Loc.main.Info_Resource_Amount.InjectField(resourceType.displayName, "resource", true).Inject(TotalResourceCapacity.ToString(2, false) + resourceType.resourceUnit.Field, "amount");
            
            void Register(Action update) => resourcePercent.OnChange += update;
            void Unregister(Action update) => resourcePercent.OnChange -= update;

            void SetResource(float newValue, bool touchStart)
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    if (BuildManager.main != null)
                        foreach (ResourceModule module in modules)
                            module.resourcePercent.Value = newValue;
                }, touchStart);
            }
        }


        // Get
        double DryMassMultiplier => Application.isPlaying && Base.worldBase.insideWorld.Value? Base.worldBase.settings.difficulty.DryMassMultiplier : 1;
        double DryMassPercent => dryMassPercent.Value * DryMassMultiplier;
        public double TotalResourceCapacity => (1 - DryMassPercent) * wetMass.Value;
        public double ResourceAmount => TotalResourceCapacity * resourcePercent.Value;
        public double ResourceSpace => TotalResourceCapacity * (1 - resourcePercent.Value);


        // Initialize
        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize()
        {
            part = transform.GetComponentInParentTree<Part>();

            if (setMass)
            {
                wetMass.OnChange += RecalculateMass;
                dryMassPercent.OnChange += RecalculateMass;
                resourcePercent.OnChange += RecalculateMass;
            }

            resourcePercent.OnChange += UpdateFlowModules;
        }
        
        void RecalculateMass()
        {
            mass.Value = DryMassPercent * wetMass.Value + ResourceAmount * resourceType.resourceMass;
            
            if (setDensity)
                part.density = Mathf.Lerp((float)DryMassPercent, 1, (float)resourcePercent.Value) * resourceType.density;
        }

        // On change
        void UpdateFlowModules(double oldAmount, double newAmount)
        {
            // No longer full  // No longer empty
            if (oldAmount is 1 or 0)
                foreach (FlowModule.Flow flowModule in flowModules)
                    flowModule.UpdateState();
        }


        // Add / Take
        public void TakeResource(double takeAmount)
        {
            if (resourcePercent.Value == 0)
                return;

            takeAmount = Math.Min(takeAmount, ResourceAmount);

            TakeFromParent(takeAmount);
            TakeFromChildren(1 - takeAmount / ResourceAmount);
            
            resourcePercent.Value -= takeAmount / TotalResourceCapacity;
        }
        void TakeFromChildren(double leftoverPercent)
        {
            foreach (ResourceModule child in children)
            {
                child.resourcePercent.Value *= leftoverPercent; // Sets child resource
                child.TakeFromChildren(leftoverPercent); // Propagates down the tree
            }
        }
        void TakeFromParent(double amount)
        {
            if (parent == null)
                return;

            parent.resourcePercent.Value -= amount / parent.TotalResourceCapacity; // Takes resource
            parent.TakeFromParent(amount); // Propagates up the tree
        }
        //
        public void AddResource(double amount)
        {
            if (resourcePercent.Value == 1)
                return;

            amount = Math.Min(amount, TotalResourceCapacity - ResourceAmount);

            AddToParent(amount);
            AddToChildren(amount / (TotalResourceCapacity - ResourceAmount));

            resourcePercent.Value += amount / TotalResourceCapacity;
        }
        void AddToChildren(double addPercent) // Percent of space that gets filled
        {
            foreach (ResourceModule child in children)
            {
                child.resourcePercent.Value += (1 - child.resourcePercent.Value) * addPercent; // Adds resource
                child.AddToChildren(addPercent); // Propagates down the tree
            }
        }
        void AddToParent(double amount)
        {
            if (parent == null)
                return;

            parent.resourcePercent.Value += amount / parent.TotalResourceCapacity; // Adds resource
            parent.AddToParent(amount); // Propagates up the tree
        }

        
        // Part action
        public void ToggleTransfer()
        {
            Rocket.resources.ToggleTransfer(part, parent);
        }

        
        // Creation
        public static ResourceModule CreateGroup(List<ResourceModule> resources, GameObject holder)
        {
            ResourceModule a = holder.AddComponent<ResourceModule>();
            a.setDensity = false;
            
            a.resourceType = resources[0].resourceType;
            a.SetChildren(resources);
            ((I_InitializePartModule)a).Initialize();

            return a;
        }
        void SetChildren(List<ResourceModule> newChildren)
        {
            children = newChildren;

            double space = 0;
            double amount = 0;

            foreach (ResourceModule newChild in newChildren)
            {
                space += newChild.TotalResourceCapacity;
                amount += newChild.ResourceAmount;

                // Binds child
                newChild.parent = this;
            }

            wetMass.Value = space;
            dryMassPercent.Value = 0;
            resourcePercent.Value = amount / space;
        }

        // Implementation
        ResourceType ResourceDrawer.I_Resource.ResourceType => resourceType;
        float ResourceDrawer.I_Resource.WetMass => (float)wetMass.Value;
        Double_Reference ResourceDrawer.I_Resource.ResourcePercent => resourcePercent;
    }
}