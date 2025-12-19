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
    public class AdaptModule : MonoBehaviour, I_InitializePartModule
    {
        const float Range = 0.05f;
        
        // Data
        public Point[] adaptPoints;
        
        public bool ignoreOccupied;
        [Space]
        public bool applySeparatorFix;
        public bool applyFairingConeFix;
        [ShowIf("applyFairingConeFix")] public Float_Reference original, width;

        
        public int Priority => 8;
        public void Initialize()
        {
            if (applyFairingConeFix && original.Value == -1)
                original.Value = width.Value;
        }

        bool initialized;
        void Start()
        {
            if (BuildManager.main != null)
                foreach (Point point in adaptPoints)
                foreach (ExtraType extraType in point.extraTypes)
                    extraType.apply.OnChange += () => OnCanAdaptChange(transform, initialized);
            
            initialized = true;
        }
        public static void OnCanAdaptChange(Transform owner, bool initialized)
        {
            if (initialized)
                UpdateAdaptation(owner.GetComponentInParentTree<PartHolder>().GetArray());
        }

        // Adapt to triggers
        public static void UpdateAdaptation(params Part[] parts)
        {
            // Builds dictionary
            Dictionary<Vector2Int, List<TriggerPoint>> triggers = new Dictionary<Vector2Int, List<TriggerPoint>>();
            foreach (AdaptTriggerModule trigger in Part_Utility.GetModules<AdaptTriggerModule>(parts))
                foreach (AdaptTriggerPoint point in trigger.points)
                {
                    // Cache
                    point.worldPosition = trigger.transform.TransformPoint(point.position.Value);
                    point.worldNormal = trigger.transform.TransformVectorUnscaled(point.normal);
                    
                    Vector2 pointWorld = point.worldPosition;
                    Vector2Int pointPosition = new Vector2Int(Mathf.FloorToInt(pointWorld.x / Range), Mathf.FloorToInt(pointWorld.y / Range));
                    
                    if (triggers.ContainsKey(pointPosition))
                        triggers[pointPosition].Add(point);
                    else
                        triggers[pointPosition] = new List<TriggerPoint> { point };
                }

            // Cache
            AdaptModule[] adaptModules = Part_Utility.GetModules<AdaptModule>(parts).ToArray();
            foreach (AdaptModule adaptModule in adaptModules)
            foreach (Point point in adaptModule.adaptPoints)
            {
                point.worldPosition = adaptModule.transform.TransformPoint(point.position.Value);
                point.worldNormal = adaptModule.transform.TransformVectorUnscaled(point.normal);
            }

            Adapt();
            Adapt();
            void Adapt()
            {
                // Resets occupied
                foreach (AdaptTriggerModule trigger in Part_Utility.GetModules<AdaptTriggerModule>(parts))
                foreach (AdaptTriggerPoint point in trigger.points)
                    point.Occupied = null;
                
                // Marks triggers as occupied
                foreach (AdaptModule adaptModule in adaptModules)
                    adaptModule.Adapt(triggers, true);

                // Adapts (Loops in reverse so new adapts to old)
                AdaptModule[] adaptModules_Reverse = Part_Utility.GetModules<AdaptModule>(parts.Reverse()).ToArray();
                foreach (AdaptModule adaptModule in adaptModules_Reverse)
                    adaptModule.Adapt(triggers, false);   
            }
        }

        void Adapt(Dictionary<Vector2Int, List<TriggerPoint>> triggers, bool justMarkTriggerAsOccupied)
        {
            foreach (Point point in adaptPoints)
            {
                List<TriggerPoint> acceptedTriggers = new List<TriggerPoint>();

                // Checking is it area or point, to not make full iteration if it is point
                switch (point.reciverType)
                {
                    case ReceiverType.Point:
                        
                        foreach (Vector2Int key in MagnetModule.GetDictionaryKeys(point.worldPosition, Range))
                            if (triggers.TryGetValue(key, out List<TriggerPoint> triggerPoints))
                                foreach (TriggerPoint triggerPoint in triggerPoints)
                                    if (Accepted(point, triggerPoint))
                                        if (!acceptedTriggers.Contains(triggerPoint))
                                            acceptedTriggers.Add(triggerPoint);

                        break;

                    case ReceiverType.Area:
                        
                        foreach (List<TriggerPoint> triggersList in triggers.Values)
                        foreach (TriggerPoint triggerPoint in triggersList)
                            if (Accepted(point, triggerPoint))
                                acceptedTriggers.Add(triggerPoint);
                        
                        break;
                }

                bool hasTrigger = acceptedTriggers.Count > 0;
                TriggerPoint bestTrigger = hasTrigger ? GetBestPoint(point, acceptedTriggers) : null;

                float output = hasTrigger ? bestTrigger.output.Value + GetOutputOffset() + bestTrigger.outputOffset : point.defaultOutput.Value;
                float differenceX = point.reciverType == ReceiverType.Area && hasTrigger ? GetPositionDifference_Local().x : 0;
                float differenceY = point.reciverType == ReceiverType.Area && hasTrigger ? GetPositionDifference_Local().y : 0;

                Vector2 GetPositionDifference_Local() => (Vector2)transform.InverseTransformPoint(bestTrigger.worldPosition) - point.position.Value;

                if (justMarkTriggerAsOccupied)
                {
                    if (hasTrigger && !ignoreOccupied)
                        if (point.output.Value == output && point.differenceX.Value == differenceX && point.differenceY.Value == differenceY) // Points current state is already target state
                            bestTrigger.Occupied = point;
                }
                else
                {
                    point.isOccupied.Value = hasTrigger;
                    point.output.Value = output;
                    point.differenceX.Value = differenceX;
                    point.differenceY.Value = differenceY;
                        
                    if (hasTrigger && !ignoreOccupied)
                        bestTrigger.Occupied = point;
                }

                float GetOutputOffset()
                {
                    if (bestTrigger.type == point.type)
                        return point.outputOffset;

                    foreach (ExtraType extraType in point.extraTypes)
                        if (extraType.apply.Value && extraType.type == bestTrigger.type)
                            return extraType.outputOffset;
                    
                    return 0;
                }
            }
        }

        // Is trigger accepted
        bool Accepted(Point receiver, TriggerPoint trigger)
        {
            bool free = trigger.Occupied == null || trigger.Occupied == receiver;
            bool occupied = ignoreOccupied || free;

            if (applySeparatorFix && !free && trigger.outputOffset == 0.4f)
                return false;

            return occupied && (!trigger.toggle || trigger.enabled.Value) && MatchingType(receiver, trigger) && InPositionRange(receiver, trigger) && MatchingNormals(receiver, trigger) && InAdaptRange(receiver, trigger);
        }
        //
        static bool MatchingType(Point receiver, TriggerPoint trigger)
        {
            if (receiver.type == trigger.type)
                return true;
            
            foreach (ExtraType extraType in receiver.extraTypes)
                if (extraType.apply.Value && extraType.type == trigger.type)
                    return true;
            
            return false;
        }
        bool InAdaptRange(Point receiver, TriggerPoint trigger)
        {
            if (applySeparatorFix && trigger.outputOffset == 0.4f)
            {
                float differenceY = ((Vector2)transform.InverseTransformPoint(trigger.worldPosition) - receiver.position.Value).y;
                if (differenceY < 0.001f)
                    return false;
            }
            
            return trigger.output.Value > receiver.adaptRange.min.Value - 0.001f && trigger.output.Value < receiver.adaptRange.max.Value + 0.001f;
        }
        bool InPositionRange(Point receiver, TriggerPoint trigger)
        {
            if (receiver.reciverType == ReceiverType.Area)
            {
                Vector2 triggerPoint = transform.InverseTransformPoint(trigger.worldPosition);
                return Math_Utility.InArea(receiver.inputArea.Value, triggerPoint, 0.05f);
            }

            return (trigger.worldPosition - receiver.worldPosition).sqrMagnitude < 0.05f * 0.05f;
        }
        bool MatchingNormals(Point receiver, TriggerPoint trigger)
        {
            return (receiver.worldNormal + trigger.worldNormal).sqrMagnitude < 0.01f; // Max difference of 0.1
        }

        // Get
        TriggerPoint GetBestPoint(Point adaptPoint, List<TriggerPoint> points)
        {
            TriggerPoint[] bestPoints = points.ToArray();

            foreach (PriorityType priorityType in adaptPoint.priority)
            {
                bestPoints = GetBestPoints(adaptPoint, bestPoints, priorityType);

                if (bestPoints.Length == 1)
                    break;
            }

            return bestPoints[0];
        }
        TriggerPoint[] GetBestPoints(Point adaptPoint, TriggerPoint[] pointsToCheck, PriorityType priorityType)
        {
            List<TriggerPoint> bestPoints = new List<TriggerPoint>();
            float bestScore = float.NegativeInfinity;

            foreach (TriggerPoint trigger in pointsToCheck)
            {
                float score = 0;
                switch (priorityType)
                {
                    case PriorityType.MinDistance: score = -GetPositionDifference(adaptPoint, trigger).sqrMagnitude; break;
                    case PriorityType.MaxDistance: score = GetPositionDifference(adaptPoint, trigger).sqrMagnitude; break;
                    case PriorityType.MinValue: score = -trigger.output.Value; break;
                    case PriorityType.MaxValue: score = trigger.output.Value; break;
                    case PriorityType.ClosestValue: score = -Mathf.Abs(adaptPoint.defaultOutput.Value - (trigger.output.Value + trigger.outputOffset)); break;
                }

                if (score > bestScore)
                    bestPoints.Clear();

                if (score >= bestScore)
                {
                    bestPoints.Add(trigger);
                    bestScore = score;
                }
            }

            return bestPoints.ToArray();
        }
        Vector2 GetPositionDifference(Point adaptPoint, TriggerPoint trigger)
        {
            return trigger.worldPosition - adaptPoint.worldPosition;
        }


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