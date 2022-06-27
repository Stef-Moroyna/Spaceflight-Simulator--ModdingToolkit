using SFS.World;
using SFS.Translations;
using SFS.Variables;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class FlowModule : MonoBehaviour
    {
        public Flow[] sources;
        public event Action onStateChange;


        void Start()
        {
            foreach (Flow source in sources)
            {
                source.Initialize();
                source.state.OnChange += () => onStateChange?.Invoke();   
            }

            onStateChange += UpdateEnabled;
        }


        // Flow
        double massFlow = double.NaN; // Small optimization
        public void SetMassFlow(double newMassFlow)
        {
            if (newMassFlow == massFlow)
                return;
            
            massFlow = newMassFlow;

            double massFlowPerUnit = GetMassFlowPerUnit();
            double totalResourceFlow = massFlowPerUnit > 0 ? newMassFlow / massFlowPerUnit : 1;

            // Sets output
            foreach (Flow source in sources)
                source.flowRate.Value = totalResourceFlow * source.flowPercent;

            UpdateEnabled();
        }
        double GetMassFlowPerUnit()
        {
            return sources.Sum(source => source.resourceType.resourceMass * source.flowPercent);
        }

        // Source
        public void FindSources(Rocket rocket)
        {
            Part part = transform.GetComponentInParentTree<Part>();
            foreach (Flow source in sources)
                source.FindSources(rocket, part);
        }


        // Flow update
        void UpdateEnabled()
        {
            sources.ForEach(s => s.UpdateState());
            enabled = sources.Any(source => source.state.Value == FlowState.IsFlowing);
        }

        void FixedUpdate()
        {
            foreach (Flow source in sources)
                source.OnFixedUpdate();
        }


        // Flow module
        [Serializable]
        public class Flow
        {
            // Variables
            [BoxGroup] public ResourceType resourceType;
            [BoxGroup] public float flowPercent = 1;
            //
            [BoxGroup("Search", false)] public SourceMode sourceSearchMode;
            [BoxGroup("Search", false), ShowIf(nameof(sourceSearchMode), SourceMode.Surfaces), Required] public SurfaceData surface;
            //
            [BoxGroup("Flow", false)] public FlowType flowType;
            [BoxGroup("Flow", false)] public Double_Reference flowRate;
            //
            [HideInEditorMode] public State_Local state = new State_Local();
            [HideInEditorMode] public ResourceModule[] sources = new ResourceModule[0];

            // Source fetching
            public void FindSources(Rocket rocket, Part part)
            {
                // Gets sources
                sources = GetSources(rocket, part);

                // Register
                foreach (ResourceModule source in sources)
                    source.flowModules.Add(this);

                UpdateState();
            }
            ResourceModule[] GetSources(Rocket rocket, Part part)
            {
                if (sourceSearchMode == SourceMode.Global)
                    return GetGlobally(rocket);

                if (sourceSearchMode == SourceMode.Surfaces)
                    return GetBySurfaces(rocket, part);

                if (sourceSearchMode == SourceMode.Local)
                    return GetLocally(part);

                throw new Exception();
            }
            ResourceModule[] GetGlobally(Rocket rocket)
            {
                return null;
            }
            ResourceModule[] GetBySurfaces(Rocket rocket, Part part)
            {
                List<ResourceModule> output = new List<ResourceModule>();

                return output.ToArray();
            }
            ResourceModule[] GetLocally(Part part)
            {
                return part.GetModules<ResourceModule>().Where(a => a.resourceType == resourceType).ToArray();
            }

            public void Initialize()
            {
                flowRate.OnChange += UpdateState;
            }

            // State
            public void UpdateState()
            {
                
            }

            // Flow
            public void OnFixedUpdate()
            {
                //#warning FUTURE / We could optimize this a lot if we took all resources at once
                if (flowType == FlowType.Negative)
                    FlowNegative();
                else if (flowType == FlowType.Positive)
                    FlowPositive();
            }
            void FlowNegative()
            {
               
                double sourcesResourceAmount = GetSourcesResourceAmount();
                double removeAmount = flowRate.Value * Time.fixedDeltaTime;
                double removePercent = Math.Min(removeAmount / sourcesResourceAmount, 1);
                
                foreach (ResourceModule source in sources)
                    source.TakeResource(source.ResourceAmount * removePercent);
                
                // Can no longer flow
                if (removePercent == 1)
                    UpdateState();
            }
            void FlowPositive()
            {
                double resourceSpace = GetSourcesResourceSpace();
                double addAmount = flowRate.Value * Time.fixedDeltaTime;
                double fillPercent = Math.Min(addAmount / resourceSpace, 1);

                foreach (ResourceModule source in sources)
                    source.AddResource(source.ResourceSpace * fillPercent);
                
                // Can no longer flow
                if (fillPercent == 1)
                    UpdateState();
            }

            // Get
            double GetSourcesResourceAmount()
            {
                return sources.Sum(source => source.ResourceAmount);
            }
            double GetSourcesResourceSpace()
            {
                return sources.Sum(source => source.ResourceSpace);
            }
        }


        // State of the flow module
        [Serializable, InlineProperty, PropertySpace(1, 1)]
        public class State_Local : Obs<FlowState>
        {
            protected override bool IsEqual(FlowState a, FlowState b) => a == b;
        }

        
        
        // Type of source gathering
        public enum SourceMode
        {
            Global,
            Surfaces,
            Local,
        }
        // Type of flow
        public enum FlowType
        {
            Negative,
            Positive
        }
        // State of the flow
        public enum FlowState
        {
            NoSource,
            NoResource,
            NoSpace,
            CanFlow,
            IsFlowing,
        }
    }
}