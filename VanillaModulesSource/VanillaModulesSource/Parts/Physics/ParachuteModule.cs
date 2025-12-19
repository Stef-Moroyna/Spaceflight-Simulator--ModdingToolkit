using SFS.Translations;
using SFS.UI;
using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SFS.Parts.Modules
{
    public class ParachuteModule : MonoBehaviour, Rocket.INJ_Location, I_PartMenu
    {
        // Reference
        public double maxDeployHeight;
        public double maxDeployVelocity;
        [Space]
        public AnimationCurve drag;
        [Required] public Transform parachute;
        
        // State
        [Space]
        public Float_Reference state;
        public Float_Reference targetState;
        Double2 oldPosition;
        
        // Output
        [Space]
        public UnityEvent onDeploy;
        
        public AudioModule deploySound_Partial, deploySound_Fully;


        // Data injection
        public Location Location { private get; set; }

        // Description
        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            drawer.DrawStat(40, Loc.main.Info_Parachute_Max_Height.Inject(maxDeployHeight.ToDistanceString(), "height"), null);
        }
        

        public void DeployParachute(UsePartData data)
        {
            bool success = false;

            double M = Location.planet.HasAtmospherePhysics? Location.planet.data.atmospherePhysics.parachuteMultiplier : 1;
            
            if (targetState.Value == 0 && state.Value == 0)
            {
                if (!Location.planet.HasAtmospherePhysics || Location.Height > Location.planet.AtmosphereHeightPhysics * 0.9) // Checks if in vacuum
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Deploy_Parachute_In_Vacuum);

                else if (Location.GetTerrainHeight(true) > maxDeployHeight * M) // Height max
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Deploy_Parachute_Above.Inject((maxDeployHeight * M).ToDistanceString(false), "height"));

                else if (Location.velocity.magnitude > maxDeployVelocity * M) // Speed max
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Deploy_Parachute_While_Faster.Inject((maxDeployVelocity * M).ToVelocityString(false), "velocity"));

                else if (Location.velocity.magnitude < 3f) // Speed min
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Deploy_Parachute_While_Not_Moving);
                
                else if (Location.planet.data.hasWater && WorldView.ToGlobalPosition(parachute.position).magnitude < Location.planet.Radius) // Water
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Deploy_Parachute_Under_Water);
                
                else
                {
                    MsgDrawer.main.Log(Loc.main.Msg_Parachute_Half_Deployed);
                    targetState.Value = 1;
                    onDeploy.Invoke();
                    //deploySound_Partial?.Play();
                    success = true;
                }
            }
            else if (targetState.Value == 1 && state.Value == 1)
            {
                if (Location.GetTerrainHeight(true) > maxDeployHeight * 0.2 * M) // Height max
                    MsgDrawer.main.Log(Loc.main.Msg_Cannot_Fully_Deploy_Above.Inject((maxDeployHeight * 0.2 * M).ToDistanceString(false), "height"));
                
                else
                {
                    MsgDrawer.main.Log(Loc.main.Msg_Parachute_Fully_Deployed);
                    targetState.Value = 2;
                    //deploySound_Fully?.Play();
                    success = true;
                }
            }
            else if (targetState.Value == 2 && state.Value == 2)
            {
                // Cuts parachute
                MsgDrawer.main.Log(Loc.main.Msg_Parachute_Cut);
                targetState.Value = 3;
                state.Value = 3;
                success = true;
            }
            else if (targetState.Value == 3)
            {
                // Already cut
                success = true;   
            }

            if (!success)
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
            // Only enables to check stuff when half or fully deployed
            enabled = targetState.Value == 1 || targetState.Value == 2;
        }
        void LateUpdate()
        {
            if (GameManager.main == null || Location.planet == null)
                return;

            if (oldPosition.x == 0 && oldPosition.y == 0)
            {
                // Parachute just got enabled
                oldPosition = WorldView.ToGlobalPosition(transform.position) - Location.velocity;
                AngleToOldPosition();
                return;
            }

            AngleToOldPosition();

            // Fully deploys parachute if below height
            if (targetState.Value == 1 && Location.GetTerrainHeight(true) < maxDeployHeight * 0.2 * Location.planet.data.atmospherePhysics.parachuteMultiplier - 300)
            {
                targetState.Value = 2;
                //deploySound_Fully?.Play();
            }

            // Cuts parachute if fully deployed && (stationary or under water)
            if (targetState.Value == 2)
            {
                // Is stationary or under water
                if (Location.velocity.Mag_LessThan(0.01f) || (Location.planet.data.hasWater && WorldView.ToGlobalPosition(parachute.position).magnitude < Location.planet.Radius))
                {
                    targetState.Value = 3;
                    state.Value = 3;
                }
            }
        }
        
        void AngleToOldPosition()
        {
            Double2 currentPosition = WorldView.ToGlobalPosition(transform.position);
            Double2 difference = oldPosition - currentPosition;

            if (difference.Mag_MoreThan(10.0))
                oldPosition = currentPosition + difference.normalized * 10f;

            Vector2 localDifference = parachute.parent.InverseTransformVector(difference);
            float parachuteAngle = Mathf.Atan2(localDifference.y, localDifference.x) * Mathf.Rad2Deg - 90f;
            parachute.localEulerAngles = new Vector3(0, 0, parachuteAngle + (Mathf.Sin(Time.time) * 3f * parachute.parent.lossyScale.x * parachute.parent.lossyScale.y));
        }
    }
}