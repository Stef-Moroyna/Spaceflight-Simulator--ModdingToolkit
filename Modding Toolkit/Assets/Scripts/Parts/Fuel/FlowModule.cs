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
            [BoxGroup("Flow", false), HideInInspector] public Double_Reference flowRate;
            //
            [HideInEditorMode] public State_Local state = new State_Local();
            [HideInEditorMode] public ResourceModule[] sources = new ResourceModule[0];
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