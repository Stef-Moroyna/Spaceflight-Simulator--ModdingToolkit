using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using TriggerPoint = SFS.Parts.Modules.AdaptTriggerPoint;
using System;
using System.Linq;
using SFS.Builds;

namespace SFS.Parts.Modules
{
    public class AdaptModule : MonoBehaviour
    {
        public Point[] adaptPoints;
        
        public bool ignoreOccupied;
        [Space]
        public bool applySeparatorFix;
        public bool applyFairingConeFix;
        [ShowIf("applyFairingConeFix")] public Float_Reference original, width;
        
        [Serializable]
        public class Point
        {
            [BoxGroup("A", false)] public ReceiverType reciverType;
            [BoxGroup("A/Input", false)] public Composed_Vector2 position;
            [BoxGroup("A/Input", false)] public Vector2 normal;
            [BoxGroup("A/Input", false)] public MinMaxRange adaptRange;
            [BoxGroup("A/Input", false)] public TriggerType type;
            [BoxGroup("A/Input", false)] public ExtraType[] extraTypes;
            [BoxGroup("A/Input", false)] public PriorityType[] priority;
            //
            [BoxGroup("A/Area", false), ShowIf("reciverType", ReceiverType.Area), HideLabel] public Composed_Rect inputArea;
            //
            [BoxGroup("A/Output", false)] public Composed_Float defaultOutput;
            [BoxGroup("A/Output", false)] public float outputOffset;
            [BoxGroup("A/Output", false)] public Float_Reference output;
            [BoxGroup("A/Output", false)] public Bool_Reference isOccupied;
            [Space]
            [BoxGroup("A/Output", false), ShowIf("reciverType", ReceiverType.Area)] public Float_Reference differenceX;
            [BoxGroup("A/Output", false), ShowIf("reciverType", ReceiverType.Area)] public Float_Reference differenceY;

            // Temp
            [NonSerialized] public Vector2 worldPosition, worldNormal;
        }


        public enum TriggerType
        {
            Fuselage,
            Fairing,
        }
        public enum ReceiverType
        {
            Point,
            Area,
        }
        public enum PriorityType
        {
            MinDistance,
            MaxDistance,
            MinValue,
            MaxValue,
            ClosestValue,
        }
        
        [Serializable]
        public class ExtraType
        {
            public Bool_Reference apply;
            public TriggerType type;
            public float outputOffset;
        }
    }
}