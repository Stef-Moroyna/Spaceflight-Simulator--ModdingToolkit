using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using TriggerPoint = System.Tuple<SFS.Parts.Modules.AdaptTriggerModule, SFS.Parts.Modules.AdaptTriggerPoint>;
using System;
using System.Linq;

namespace SFS.Parts.Modules
{
    public class AdaptModule : MonoBehaviour
    {
        // Data
        public Point[] adaptPoints;
        public bool ignoreOccupied;
        
        // State
        [NonSerialized] public bool disableAdaptation;
        

        // Adapt to triggers
        public static void UpdateAdaptation(params Part[] parts)
        {
            AdaptTriggerModule[] triggers = Part_Utility.GetModules<AdaptTriggerModule>(parts).ToArray();
            
            // Resets triggers
            foreach (AdaptTriggerModule trigger in triggers)
            foreach (AdaptTriggerPoint triggerPoint in trigger.points)
                triggerPoint.occupied = null;

            // Marks triggers as occupied
            foreach (AdaptModule adaptModule in Part_Utility.GetModules<AdaptModule>(parts))
                adaptModule.Adapt(triggers, true);
            
            // Adapts (Loops in reverse so new adapts to old)
            AdaptModule[] adaptModules_Reverse = Part_Utility.GetModules<AdaptModule>(parts.Reverse()).ToArray();
            foreach (AdaptModule adaptModule in adaptModules_Reverse)
                adaptModule.Adapt(triggers, false);
            foreach (AdaptModule adaptModule in adaptModules_Reverse)
                adaptModule.Adapt(triggers, false);
        }
        void Adapt(AdaptTriggerModule[] triggers, bool justMarkTriggerAsOccupied)
        {
            if (disableAdaptation)
                return;

            // Loops trough each of the points
            foreach (Point point in adaptPoints)
            {
                List<TriggerPoint> acceptedTriggers = new List<TriggerPoint>();

                // Loops trough all the triggers to find the accepted ones
                foreach (AdaptTriggerModule triggerModule in triggers)
                foreach (AdaptTriggerPoint triggerPoint in triggerModule.points)
                {
                    TriggerPoint trigger = new TriggerPoint(triggerModule, triggerPoint);
                    if (Accepted(point, trigger))
                        acceptedTriggers.Add(trigger);
                }

                bool hasTrigger = acceptedTriggers.Count > 0;
                TriggerPoint bestTrigger = hasTrigger ? GetBestPoint(point, acceptedTriggers) : null;

                float output = hasTrigger ? bestTrigger.Item2.output.Value + point.outputOffset : point.defaultOutput.Value;
                float differenceX = point.reciverType == ReceiverType.Area && hasTrigger ? GetPositionDifference(point, bestTrigger).x : 0;
                float differenceY = point.reciverType == ReceiverType.Area && hasTrigger ? GetPositionDifference(point, bestTrigger).y : 0;
                
                if (justMarkTriggerAsOccupied)
                {
                    if (hasTrigger && !ignoreOccupied)
                        if (point.output.Value == output && point.differenceX.Value == differenceX && point.differenceY.Value == differenceY) // Points current state is already target state
                            bestTrigger.Item2.occupied = point;
                }
                else
                {
                    point.isOccupied.Value = hasTrigger;
                    point.output.Value = output;
                    point.differenceX.Value = differenceX;
                    point.differenceY.Value = differenceY;
                
                    if (hasTrigger && !ignoreOccupied)
                        bestTrigger.Item2.occupied = point;
                }
            }
        }

        // Is trigger accepted
        bool Accepted(Point receiver, TriggerPoint trigger)
        {
            bool occupied = ignoreOccupied || trigger.Item2.occupied == null || trigger.Item2.occupied == receiver;
            return occupied && MatchingType(receiver, trigger) && InAdaptRange(receiver, trigger) && InPositionRange(receiver, trigger) && MatchingNormals(receiver, trigger);
        }
        //
        public static bool MatchingType(Point receiver, TriggerPoint trigger)
        {
            return receiver.type == trigger.Item2.type;
        }
        public static bool InAdaptRange(Point receiver, TriggerPoint trigger)
        {
            return trigger.Item2.output.Value > receiver.adaptRange.min.Value - 0.001f && trigger.Item2.output.Value < receiver.adaptRange.max.Value + 0.001f;
        }
        bool InPositionRange(Point receiver, TriggerPoint trigger)
        {
            Vector2 triggerPoint = Transform_Utility.LocalToLocalPoint(trigger.Item1, this, trigger.Item2.position.Value);

            if (receiver.reciverType == ReceiverType.Area)
                return Math_Utility.InArea(receiver.inputArea.Value, triggerPoint, 0.05f);
            else
                return (triggerPoint - receiver.position.Value).sqrMagnitude < 0.05f * 0.05f;
        }
        bool MatchingNormals(Point receiver, TriggerPoint trigger)
        {
            Vector2 triggerNormal = trigger.Item1.transform.TransformVectorUnscaled(trigger.Item2.normal);
            Vector2 receiverNormal = transform.TransformVectorUnscaled(receiver.normal);
            return (receiverNormal + triggerNormal).sqrMagnitude < 0.01f; // Max difference of 0.1
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

            foreach (TriggerPoint pointToCheck in pointsToCheck)
            {
                float score = 0;
                switch (priorityType)
                {
                    case PriorityType.MinDistance: score = -GetPositionDifference(adaptPoint, pointToCheck).sqrMagnitude; break;
                    case PriorityType.MaxDistance: score = GetPositionDifference(adaptPoint, pointToCheck).sqrMagnitude; break;
                    case PriorityType.MinValue: score = -pointToCheck.Item2.output.Value; break;
                    case PriorityType.MaxValue: score = pointToCheck.Item2.output.Value; break;
                    case PriorityType.ClosestValue: score = -Mathf.Abs(adaptPoint.defaultOutput.Value - pointToCheck.Item2.output.Value); break;
                }

                if (score > bestScore)
                    bestPoints.Clear();

                if (score >= bestScore)
                {
                    bestPoints.Add(pointToCheck);
                    bestScore = score;
                }
            }
        
            return bestPoints.ToArray();
        }
        Vector2 GetPositionDifference(Point adaptPoint, TriggerPoint trigger)
        {
            return Transform_Utility.LocalToLocalPoint(trigger.Item1, this, trigger.Item2.position.Value) - adaptPoint.position.Value;
        }


        [Serializable]
        public class Point
        {
            [BoxGroup("A", false)] public ReceiverType reciverType;
            [BoxGroup("A/Input", false)] public Composed_Vector2 position;
            [BoxGroup("A/Input", false)] public Vector2 normal;
            [BoxGroup("A/Input", false)] public MinMaxRange adaptRange;
            [BoxGroup("A/Input", false)] public TriggerType type;
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
    }
}