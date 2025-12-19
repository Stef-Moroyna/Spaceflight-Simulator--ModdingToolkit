using SFS.Parts.Modules;
using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace InterplanetaryModule
{
    public class FlapModule : MonoBehaviour, Rocket.INJ_TurnAxisTorque, Rocket.INJ_Location
    {
        public MoveModule moveModule;
        public Float_Reference duration;
        public bool unscaledTime;
        public Bool_Reference activated;
        Location location;
        float turnAxis;
        public TorqueModule torqueModule;
        public float coefficient;


        Location Rocket.INJ_Location.Location
        {
            set => location = value;
        }

        float Rocket.INJ_TurnAxisTorque.TurnAxis
        {
            set => turnAxis = value;
        }

        // Update is called once per frame
        void Update()
        {
            float pressure = (float)location.planet.GetAtmosphericDensity(location.Height);
            float speed = (float)location.velocity.magnitude;
            torqueModule.torque.Value = (activated.Value) ? coefficient * pressure * speed * speed : 0;

            float deltaTime = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float delta = (duration.Value != 0) ? deltaTime / duration.Value : 10000f;
            float destination = activated.Value ? turnAxis : 0;
            moveModule.time.Value = Mathf.MoveTowards(moveModule.time.Value, destination, delta);
        }
    }
}