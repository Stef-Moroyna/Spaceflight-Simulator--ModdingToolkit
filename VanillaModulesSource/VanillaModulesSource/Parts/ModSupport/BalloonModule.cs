using System;
using SFS.Translations;
using SFS.UI;
using SFS.Variables;
using SFS.World;
using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace InterplanetaryModule
{
    public class BalloonModule : MonoBehaviour, Rocket.INJ_Location, Rocket.INJ_Physics, I_PartMenu
    {
        public Location Location { private get; set; }
        public Rigidbody2D Rb2d { get; set; }

        public Float_Reference radius;
        public double dragConstant = 1;
        public double areaToVolume = 20;
        public double buoyantMultiplier = 0.4;
        public double maxDeployVelocity;
        public Transform balloon;
        public OrientationModule orientation;

        [Space]
        public Float_Reference state;
        public Float_Reference targetState;
        public Float_Reference maxSpeed;

        Double2 oldPosition;

        [Space]
        public UnityEvent onDeploy;

        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            // drawer.DrawStat(40, Loc.main.Info_Parachute_Max_Height.Inject(maxDeployHeight.ToDistanceString(true), "height"), null);
        }

        public void DeployBalloon(UsePartData data)
        {
            bool flag = false;
            double num = Location.planet.HasAtmospherePhysics ? Location.planet.data.atmospherePhysics.parachuteMultiplier : 1.0;
            if (targetState.Value == 0f && state.Value == 0f)
            {
                if (!Location.planet.HasAtmospherePhysics || Location.Height > Location.planet.AtmosphereHeightPhysics * 0.9)
                    MsgDrawer.main.Log("Cannot inflate balloon in vacuum");
                else if (Location.velocity.magnitude > maxDeployVelocity * num)
                    MsgDrawer.main.Log(
                        Loc.main.Msg_Cannot_Deploy_Parachute_While_Faster.Inject((maxDeployVelocity * num).ToVelocityString(false, false), "velocity"));
                else
                {
                    MsgDrawer.main.Log("Balloon inflated");
                    targetState.Value = 2f;
                    onDeploy.Invoke();
                    flag = true;
                }
            }
            else if (targetState.Value == 2f && state.Value == 2f)
            {
                MsgDrawer.main.Log("Balloon deflated");
                targetState.Value = 0f;
                state.Value = 0f;
                flag = true;
            }
            else if (targetState.Value == 3f)
                flag = true;
            if (!flag)
                data.successfullyUsedPart = false;
        }

        void Start()
        {
            if (GameManager.main == null)
            {
                enabled = false;
                return;
            }
            targetState.OnChange += UpdateEnabled;
        }

        void UpdateEnabled()
        {
            enabled = (targetState.Value == 1f || targetState.Value == 2f);
        }

        void FixedUpdate()
        {
            if (state.Value == 0f) return;
            double scaledRadius = radius.Value * (state.Value / 2);
            double height = Location.Height;
            double density = Location.planet.GetAtmosphericDensity(height);
            double gravity = Location.planet.GetGravity(Location.planet.Radius + height);

            double volume = areaToVolume * Math.PI * scaledRadius * scaledRadius;
            double buoyantMagnitude = density * volume * gravity * buoyantMultiplier;

            if (maxSpeed.Value > 0.1 && Location.velocity.Mag_MoreThan(0.1) && Location.VerticalVelocity > maxSpeed.Value) 
                buoyantMagnitude *= maxSpeed.Value * maxSpeed.Value / Location.velocity.sqrMagnitude;
            
            Vector2 upDirection = Location.position.normalized;
            float upAngle = Mathf.Atan2(upDirection.y, upDirection.x);
            float rocketAngle = Rb2d.rotation * Mathf.Deg2Rad;
            float orientationAngle = orientation.orientation.Value.z + (orientation.orientation.Value.y < 0 ? 180f : 0);
            orientationAngle *= Mathf.Deg2Rad;
            Vector2 direction = new(
                (float)Math.Cos(upAngle - rocketAngle - orientationAngle),
                (float)Math.Sin(upAngle - rocketAngle - orientationAngle)
            );

            Vector2 force = transform.TransformVector(direction * (float)buoyantMagnitude);
            Vector2 relativePoint = Rb2d.GetRelativePoint(
                Transform_Utility.LocalToLocalPoint(base.transform, Rb2d, new Vector2(0, 3f))
            );
            Rb2d.AddForceAtPosition(force, relativePoint, ForceMode2D.Force);
        }

        void LateUpdate()
        {
            if (GameManager.main == null || Location.planet == null)
                return;
            
            if (oldPosition is { x: 0.0, y: 0.0 }) 
                oldPosition = WorldView.ToGlobalPosition(base.transform.position) - Location.velocity;
            
            AngleToOldPosition();
        }

        void AngleToOldPosition()
        {
            Vector2 upDirection = Location.position.normalized;
            float upAngle = Mathf.Atan2(upDirection.y, upDirection.x);
            float rocketAngle = Rb2d.rotation * Mathf.Deg2Rad;
            float orientationAngle = orientation.orientation.Value.z + (orientation.orientation.Value.y < 0 ? 180f : 0);
            orientationAngle *= Mathf.Deg2Rad;
            float angle = upAngle - rocketAngle - orientationAngle;
            angle *= Mathf.Rad2Deg;
            angle = 90f + angle;
            
            balloon.localEulerAngles = new Vector3(
                0f,
                0f,
                angle + Mathf.Sin(Time.time) * 3f * balloon.parent.lossyScale.x * balloon.parent.lossyScale.y
            );
        }
    }
}
