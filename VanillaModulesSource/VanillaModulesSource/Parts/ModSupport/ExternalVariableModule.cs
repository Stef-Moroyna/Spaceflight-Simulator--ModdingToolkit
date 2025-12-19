using System;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using SFS.World;
using SFS.World.Drag;
using Dref = SFS.Variables.Double_Reference;

namespace SFS.Parts.Modules
{
	public class ExternalVariableModule : MonoBehaviour, Rocket.INJ_Rocket
	{
		[Title("Timewarp")] public Dref physicsTimeScale = new();
		public Dref railsTimewarp = new();
		public Dref effectiveTimewarp = new();
		public Bool_Reference physicsEnabled = new();

		[Space, Title("Rocket location")] public Dref rocketAltitude = new();
		public Dref currentAtmosphereDensity = new();
		public Dref rocketPositionAngleRadians = new();
		[Space] public Dref surfaceVelocityX = new();
		public Dref surfaceVelocityY = new();
		public Dref velocityMagnitude = new();

		[Space, Title("Rocket inputs")] public Dref rcsX = new();
		public Dref rcsY = new();
		public Dref turnInput = new();
		public Dref wheelsInput = new();
		public Bool_Reference throttleOn = new Bool_Reference();
		public Dref throttlePercent = new();
		public Dref throttleOutput = new();

		[Space, Title("Part values")] public Dref partTemperature = new();
		public Dref orientationX = new();
		public Dref orientationY = new();
		public Dref partRotation = new();
		public Dref partRotation_Normalized = new();
		public Dref globalRotation = new();

		OrientationModule ormod;
		HeatModuleBase hmb;
		Rocket rkt;

		public Rocket Rocket
		{
			set { rkt = value; }
		}

		void Awake()
		{
			ormod = GetComponentInParent<OrientationModule>();
			hmb = GetComponentInParent<HeatModuleBase>();
		}

		float NormalizeAngle(float angle) =>
			angle >= 0
				? angle % 360f
				: (360f + angle % 360f) % 360f;

		void Update()
		{
			if (WorldTime.main != null)
			{
				physicsTimeScale.Value = Time.timeScale;
				physicsEnabled.Value = WorldTime.main.realtimePhysics.Value;
				railsTimewarp.Value = physicsEnabled.Value ? 1 : WorldTime.main.timewarpSpeed;
				effectiveTimewarp.Value = physicsEnabled.Value ? Time.timeScale : WorldTime.main.timewarpSpeed;
			}

			if (ormod != null)
			{
				orientationX.Value = ormod.orientation.Value.x;
				orientationY.Value = ormod.orientation.Value.y;
				partRotation.Value = ormod.orientation.Value.z;
				partRotation_Normalized.Value = NormalizeAngle((float)partRotation.Value);

				globalRotation.Value = transform.eulerAngles.z;
			}

			if (rkt != null)
			{
				rocketAltitude.Value = rkt.physics.location.Height;
				currentAtmosphereDensity.Value =
					rkt.physics.location.planet.Value.GetAtmosphericDensity(rocketAltitude.Value);
				rocketPositionAngleRadians.Value = rkt.physics.location.position.Value.AngleRadians;

				velocityMagnitude.Value = rkt.physics.location.velocity.Value.magnitude;
				surfaceVelocityX.Value = rkt.physics.location.Value.velocity
					.Rotate(0.0 - (rkt.physics.location.position.Value.AngleRadians - Math.PI / 2.0)).x;
				surfaceVelocityY.Value = rkt.physics.location.Value.VerticalVelocity;

				turnInput.Value = rkt.output_TurnAxisTorque.Value;
				wheelsInput.Value = rkt.output_TurnAxisWheels.Value;
				rcsX.Value = rkt.output_DirectionalAxis.Value.x;
				rcsY.Value = rkt.output_DirectionalAxis.Value.y;

				throttleOn.Value = rkt.throttle.throttleOn.Value;
				throttlePercent.Value = rkt.throttle.throttlePercent.Value;
				throttleOutput.Value = rkt.throttle.output_Throttle.Value;
			}

			if (hmb != null)
			{
				partTemperature.Value = hmb.Temperature;
			}
		}
	}
}