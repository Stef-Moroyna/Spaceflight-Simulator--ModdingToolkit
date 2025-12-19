using SFS.World;
using SFS.Translations;
using SFS.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using SFS.Input;
using SFS.Platform;

namespace SFS.Parts.Modules
{
    public class RcsModule : MonoBehaviour, Rocket.INJ_Rocket, Rocket.INJ_IsPlayer, Rocket.INJ_DirectionalAxis, Rocket.INJ_TurnAxisTorque, I_PartMenu
    {
        public float directionAngleThreshold, torqueAngleThreshold, thrust, ISP;
        public List<Thruster> thrusters = new List<Thruster>();
        [Required] public FlowModule source;

        public Vector2 thrustPosition;

        // Injection
        public Rocket Rocket { get; set; }
        public bool IsPlayer { get; set; }
        public float TurnAxis { get; set; }
        public Vector2 DirectionalAxis { get; set; }
        bool RCS_On { get => Rocket.arrowkeys.rcs.Value; set => Rocket.arrowkeys.rcs.Value = value; }


        // Description
        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            drawer.DrawStat(60, thrust.ToThrustString(), null);
            drawer.DrawStat(50, ISP.ToEfficiencyString(), null);
        }


        void Start()
        {
            source.onStateChange += Update_RCS_On;
        }
        void Update_RCS_On()
        {
            if (!RCS_On)
                return;
            
            I_MsgLogger logger = IsPlayer ? (I_MsgLogger)MsgDrawer.main : new MsgNone();

            if (!source.CanFlow(logger))
                ToggleRCS(new UsePartData(new UsePartData.SharedData(false), null), false);
        }

        
        // Activate part
        public void ToggleRCS(UsePartData data) => ToggleRCS(data, true);
        void ToggleRCS(UsePartData data, bool showMsg)
        {
            if (data.sharedData.hasToggledRCS) // Ensures toggle when activating multiple RCS at once
                return;
            data.sharedData.hasToggledRCS = true;

            I_MsgLogger logger = showMsg && IsPlayer ? (I_MsgLogger)MsgDrawer.main : new MsgNone();

            if (!RCS_On && !source.CanFlow(logger))
                return; // Cannot enable RCS

            RCS_On = !RCS_On;

            if (RCS_On && PlatformManager.current == PlatformType.PC)
                logger.Log(Loc.main.Msg_RCS_Module_State.InjectField(RCS_On.State_ToOnOff(), "state") + "\n\n" + $"Use {KeybindingsPC.keys.Turn_Rocket.ToKeysString()} to turn, {KeybindingsPC.keys.Move_Rocket_Using_RCS.ToKeysString()} to move");
            else
                logger.Log(Loc.main.Msg_RCS_Module_State.InjectField(RCS_On.State_ToOnOff(), "state"));
        }


        void FixedUpdate()
        {
            if (Rocket == null || !RCS_On || Mathf.Abs(TurnAxis) < 0.01f && DirectionalAxis.sqrMagnitude < 0.01)
            {
                foreach (Thruster thruster in thrusters)
                    thruster.effect.targetTime.Value = 0;

                source.SetMassFlow(0);
                return;
            }

            Vector2 thrustPositionRelative = Rocket.rb2d.worldCenterOfMass - (Vector2)transform.TransformPoint(thrustPosition);
            float throttle = 0;
            Vector2 force = Vector2.zero;

            foreach (Thruster thruster in thrusters)
            {
                Vector2 thrustDirectionWorld = transform.TransformVectorUnscaled(thruster.thrustNormal);

                bool torqueThrust = TorqueThrust(thrustDirectionWorld, thrustPositionRelative);
                bool directionThrust = DirectionThrust(thrustDirectionWorld);

                if (torqueThrust || directionThrust)
                {
                    force += thrustDirectionWorld;
                    throttle += 1;
                    thruster.effect.targetTime.Value = 1;
                }
                else
                {
                    thruster.effect.targetTime.Value = 0;
                }
            }

            if (force != Vector2.zero)
                Rocket.rb2d.AddForceAtPosition(force * (thrust * throttle * 9.8f), transform.TransformPoint(thrustPosition));

            source.SetMassFlow(thrust * throttle / ISP);
        }

        // Returns true if the angle from the thrust direction ± 90 is not bigger than the maximum torque angle delta
        bool TorqueThrust(Vector2 thrustDirection, Vector2 positionToCenterOfMass)
        {
            if (thrustDirection == Vector2.zero || positionToCenterOfMass == Vector2.zero)
                return false;
            if (Mathf.Abs(TurnAxis) < 0.95f && Mathf.Abs(Rocket.rb2d.angularVelocity) < 2)
                return false;

            float rightAngleDelta = Vector2.Angle(thrustDirection, Quaternion.Euler(0f, 0f, 90f) * positionToCenterOfMass);
            float leftAngleDelta = Vector2.Angle(thrustDirection, Quaternion.Euler(0f, 0f, -90f) * positionToCenterOfMass);

            return TurnAxis > 0.1f && rightAngleDelta <= torqueAngleThreshold || TurnAxis < -0.1f && leftAngleDelta <= torqueAngleThreshold;
        }
        // Returns true if the angle from thrust direction to input direction is not bigger than the maximum direction angle delta
        bool DirectionThrust(Vector2 thrustDirection)
        {
            if (DirectionalAxis == Vector2.zero)
                return false;

            return Vector2.Angle(thrustDirection, DirectionalAxis) <= directionAngleThreshold;
        }

        [Serializable]
        public class Thruster
        {
            public Vector2 thrustNormal;
            [Required] public MoveModule effect;
        }
    }
}