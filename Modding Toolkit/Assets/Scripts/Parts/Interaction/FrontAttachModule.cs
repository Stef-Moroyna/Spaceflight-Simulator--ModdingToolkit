using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;
using SFS.World;
using UnityEngine.Events;
using PartJoint = SFS.World.Joint<SFS.Parts.Part>;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class FrontAttachModule : MonoBehaviour
    {
        // Data
        public Type type;
        public Point[] points;
        [ShowIf("type", Type.Holder_Back)] public Composed_Float depthPosition_Output;
        [ShowIf("type", Type.Attacher_Front)] public ModelSetup[] depthOffsetTargets;
        [ShowIf("type", Type.Attacher_Front)] public GameObject[] applyLayerTargets;
        
        // State
        [Space]
        [ShowIf("type", Type.Attacher_Front)] public Float_Reference depthPosition = new Float_Reference { localValue = -1 };

        // Out
        [Space]
        public Float_Reference boosterCount;
        public UnityEvent onDetach;
        
        
        // Injection
        public Rocket Rocket { private get; set; }
        
        
        void Start()
        {
            depthPosition.OnChange += UpdateDepthPosition;
        }
        void UpdateDepthPosition()
        {
            if (type != Type.Attacher_Front)
                return;

            boosterCount.Value = IsFront()? 2 : 1;
            
            foreach (ModelSetup depthOffsetTarget in depthOffsetTargets)
                depthOffsetTarget.SetDepthOffset(IsFront()? depthPosition.Value : 0);
            
            foreach (GameObject layerTarget in applyLayerTargets)
                layerTarget.layer = LayerMask.NameToLayer(IsFront()? "Front Parts" : "Parts");
        }


        // Functionality
        public void DetachFromBack()
        {
            onDetach?.Invoke();
        }
        //
        public static bool ResetIsAttached(Part[] heldParts)
        {
            FrontAttachModule[] modules = Part_Utility.GetModules<FrontAttachModule>(heldParts.ToArray()).ToArray();
            bool wasFront = modules.Any(a => a.IsFront());
            modules.ForEach(a => a.depthPosition.Value = -1);
            return wasFront;
        }
        public static void TryAttach(Part[] heldParts, Part[] gridParts)
        {
            FrontAttachModule[] attaches_Front = Part_Utility.GetModules<FrontAttachModule>(heldParts.ToArray()).Where(a => a.type == Type.Attacher_Front).ToArray();
            FrontAttachModule[] attaches_Back = Part_Utility.GetModules<FrontAttachModule>(gridParts.ToArray()).Where(a => a.type == Type.Holder_Back).ToArray();

            // Finds connections
            foreach (FrontAttachModule attach_Front in attaches_Front)
            foreach (FrontAttachModule attach_Back in attaches_Back)
                if (Connect(attach_Front, attach_Back))
                    attach_Front.depthPosition.Value = attach_Back.depthPosition_Output.Value;
        }
        public static bool Connect(FrontAttachModule attach_Front, FrontAttachModule attach_Back)
        {
            foreach (Vector2 point_Front in attach_Front.GetSnapPointsWorld())
            foreach (Vector2 point_Back in attach_Back.GetSnapPointsWorld())
                if (Vector2.Distance(point_Front, point_Back) < 0.05f)
                    return true;
            
            return false;
        }
        
        // Get
        public bool IsFront()
        {
            return depthPosition.Value != -1;
        }
        public Vector2[] GetSnapPointsWorld()
        {
            return points.Select(p => (Vector2)transform.TransformPoint(p.position.Value)).ToArray();
        }
        
        
        [Serializable]
        public class Point
        {
            public Composed_Vector2 position;
        }
        public enum Type
        {
            Holder_Back,
            Attacher_Front,
        }
    }
}