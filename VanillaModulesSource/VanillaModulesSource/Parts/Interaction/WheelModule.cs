using System.Collections.Generic;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.World;
using SFS.Variables;
using SFS.UI;
using SFS.Translations;

namespace SFS.Parts
{
    public class WheelModule : MonoBehaviour, Rocket.INJ_TurnAxisWheels, Rocket.INJ_Physics
    {
        // Stats
        [BoxGroup("Stats", false)] public float power;
        [BoxGroup("Stats", false)] public float traction;
        [BoxGroup("Stats", false)] public float maxAngularVelocity;
        [BoxGroup("Stats", false)] public float wheelSize;

        // State
        [BoxGroup("State", false)] public float angularVelocity;
        [BoxGroup("State", false)] public Bool_Reference on;

        
        public void Draw(List<WheelModule> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            if (settings.build || settings.game)
                drawer.DrawToggle(-1, () => Loc.main.Wheel_On_Label, Toggle, () => on.Value, update => on.OnChange += update, update => on.OnChange -= update);

            void Toggle()
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    bool value = !on.Value;
                    foreach (WheelModule wheel in modules)
                        wheel.on.Value = value;
                });
            }
        }
        public void ToggleEnabled()
        {
            on.Value = !on.Value;
            MsgDrawer.main.Log(Loc.main.Wheel_Module_State.InjectField(on.Value.State_ToOnOff(), "state"));
        }

        // Injection
        public float TurnAxis { get; set; }
        public Rigidbody2D Rb2d { get; set; }


        // Applies wheel physics
        void OnCollisionStay2D(Collision2D collision)
        {
            // Wheel surface velocity
            float wheelSurfaceVelocity = angularVelocity * Mathf.Deg2Rad * wheelSize;
            Vector2 wheelSurfaceVelocityNormal = Quaternion.Euler(0, 0, -90) * collision.contacts[0].normal;

            // Relative velocity
            Vector2 relativeVelocity = collision.contacts[0].relativeVelocity - (wheelSurfaceVelocityNormal * wheelSurfaceVelocity);
            float relativeVelocityMagnitude = relativeVelocity.magnitude;

            float multiplier = 1;

            multiplier = multiplier * 0.1f * (float)WorldView.main.ViewLocation.planet.data.basics.gravity;

            float velocityChangeFromVelocity = (traction / Rb2d.mass) * Time.fixedDeltaTime * 10; // How much the rb2d velocity changes for each unit of relative velocity

            if (velocityChangeFromVelocity > 1)
                multiplier /= velocityChangeFromVelocity;

            // Applies wheel force to rb2d
            Rb2d.AddForceAtPosition(relativeVelocity * traction * multiplier, transform.position);

            if (collision.rigidbody != null)
                collision.rigidbody.AddForceAtPosition(-relativeVelocity * traction * multiplier, collision.contacts[0].point);

            float delta = relativeVelocityMagnitude * traction * multiplier;

            Vector2 relativeTestVel1 = collision.contacts[0].relativeVelocity - (wheelSurfaceVelocityNormal * ((angularVelocity + delta) * Mathf.Deg2Rad * wheelSize));
            Vector2 relativeTestVel2 = collision.contacts[0].relativeVelocity - (wheelSurfaceVelocityNormal * ((angularVelocity - delta) * Mathf.Deg2Rad * wheelSize));

            float v1 = relativeTestVel1.sqrMagnitude;
            float v2 = relativeTestVel2.sqrMagnitude;

            // As the wheel pushes against the ground, its velocity decreases
            if (v1 > v2)
                angularVelocity -= relativeVelocityMagnitude * traction * multiplier;
            else
                angularVelocity += relativeVelocityMagnitude * traction * multiplier;
        }

        // Inputs
        void Update()
        {
            float turnAxis = on.Value? -TurnAxis : 0;

            if (turnAxis == 0)
                turnAxis = Mathf.Clamp(-angularVelocity / Time.deltaTime / power, on.Value? -0.25f : -0.05f, on.Value? 0.25f : 0.05f); // Stop delta

            if (turnAxis != 0)
                angularVelocity = Mathf.Clamp(angularVelocity + turnAxis * power * Time.deltaTime, -maxAngularVelocity, maxAngularVelocity); // Angular velocity change

            if (float.IsNaN(angularVelocity))
                angularVelocity = 0;

            if (angularVelocity != 0)
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angularVelocity * Time.deltaTime); // Rotates the wheel
        }
    }
}