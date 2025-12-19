using SFS.Variables;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using SFS.Career;
using UnityEngine;
using SFS.Parts;
using SFS.UI;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.Stats;
using SFS.Translations;

namespace SFS.World
{
    public class CrewModule : MonoBehaviour, Rocket.INJ_Rocket, I_PartMenu, I_InitializePartModule
    {
        public float baseMass;
        public Bool_Reference hasControl;
        public Bool_Reference needsCrewForControl;
        public Seat[] seats;

        public GameObject interior, hatch;
        
        public bool HasCrew => seats.Any(s => !string.IsNullOrEmpty(s.astronaut.Value));
        
        Part part;
        public Rocket Rocket { get; set; }
        
        
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize()
        {
            part = GetComponentInParent<Part>();
            
            needsCrewForControl.OnChange += OnSeatChange;
            foreach (Seat seat in seats)
            {
                seat.OnStart();
                seat.astronaut.OnChange += OnSeatChange;
            }
            
            if (DevSettings.DisableAstronauts && !Application.isEditor)
            {
                needsCrewForControl.Variable.Save = false;
                
                foreach (Seat seat in seats)
                    seat.astronaut.Variable.Save = false;
            }
        }
        void OnSeatChange()
        {
            bool hasCrew = DevSettings.DisableAstronauts || seats.Any(s => s.HasAstronaut) || !needsCrewForControl.Value;
            hasControl.Value = hasCrew;
            
            if (hatch != null)
                hatch.SetActive(hasCrew);
            if (interior != null)
                interior.SetActive(!hasCrew);
            
            part.mass.Value = baseMass + seats.Sum(s => s.HasAstronaut ? 0.2f : 0);
        }
        void OnDestroy()
        {
            foreach (Seat seat in seats)
                seat.OnDestroy();
        }
        
        // On part used
        public void OpenPartMenu_Seats()
        {
            if (!DevSettings.DisableAstronauts)
                OpenPartMenu(false);
        }
        // From astronaut
        public void OpenPartMenu(bool canBoardWorld)
        {
            PartDrawSettings settings = BuildManager.main != null? PartDrawSettings.BuildSettings : PartDrawSettings.WorldSettings;
            AttachableStatsMenu menu = BuildManager.main != null? BuildManager.main.buildMenus.partMenu : FindObjectOfType<AttachableStatsMenu>(true);

            if (canBoardWorld)
                settings.canBoardWorld = true;
            
            menu.Open(() => true, drawer => part.DrawPartStats(null, drawer, settings), AttachWithArrow.FollowPart(part), false, false, () => part.onPartDestroyed += Close, () => part.onPartDestroyed -= Close);
            void Close(Part _) => menu.Close();
        }

        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            if (DevSettings.DisableAstronauts)
                return;
            
            bool drawSeats = settings.build || settings.game;
            
            drawer.DrawStat(90, () =>
            {
                string seatCount = drawSeats? (seats.Count(seat => seat.HasAstronaut) + " / " + seats.Length) : seats.Length.ToString();
                return Loc.main.Crew_Count.Inject(seatCount, "count");
            },
            null,
            update =>
            {
                foreach (Seat seat in seats)
                    seat.astronaut.OnChange += update;
            },
            update =>
            {
                foreach (Seat seat in seats)
                    seat.astronaut.OnChange -= update;
            });
            
            if (drawSeats)
                DrawSeats(drawer, settings);
        }
        void DrawSeats(StatsMenu drawer, PartDrawSettings settings)
        {
            foreach (Seat a in seats)
            {
                Seat seat = a; // Delegate needs local

                Action update = null;
                drawer.DrawButton(-1,
                    () => seat.HasAstronaut ? seat.astronaut.Value : "",
                    () => BuildManager.main != null ? (seat.HasAstronaut ? Loc.main.Crew_Remove : Loc.main.Crew_Assign) : (seat.HasAstronaut ? Loc.main.EVA_Exit : Loc.main.EVA_Board),
                    OnClick,
                    () => BuildManager.main != null || (seat.HasAstronaut || settings.canBoardWorld),
                    u => update = u, null);
                
                void OnClick()
                {
                    if (BuildManager.main != null)
                    {
                        if (!seat.HasAstronaut)
                        {
                            AstronautMenu.main.OpenMenu(seat, update);
                        }
                        else
                        {
                            seat.Exit();
                            update?.Invoke();
                        }
                    }
                    else
                    {
                        if (!seat.HasAstronaut)
                            EVA_Board(a);
                        else
                            EVA_Exit(a);
                        
                        drawer.Close();
                    }
                }
            }
        }
        
        // World
        void EVA_Board(Seat seat)
        {
            // Valid check
            if (seat.HasAstronaut || !(PlayerController.main.player.Value is Astronaut_EVA astronaut) || !astronaut.hasControl.Value)
                return;
            
            // Distance check
            if ((transform.TransformPoint(seat.hatchPosition) - (Vector3)astronaut.rb2d.worldCenterOfMass).sqrMagnitude > (20 * 20))
            {
                MsgDrawer.main.Log(Loc.main.Cannot_Board_This_Far);
                return;   
            }

            // Boards
            seat.Board(astronaut.astronaut.astronautName, astronaut.resources.fuelPercent.Value, astronaut.resources.temperature.Value);
            StatsRecorder.OnMerge(Rocket.stats, astronaut.stats);
            AstronautManager.DestroyEVA(astronaut, false);
            PlayerController.main.SmoothChangePlayer(Rocket, 1);
        }
        void EVA_Exit(Seat seat)
        {
            // Spawn position
            Location location = new Location(Rocket.location.planet, WorldView.ToGlobalPosition(transform.TransformPoint(seat.hatchPosition)), Rocket.location.velocity);
            
            // Ground spawn
            double terrainHeight = location.GetTerrainHeight(true);
            if (terrainHeight < 100 && terrainHeight > 100)
            {
                int direction = location.position.AngleRadians < Rocket.location.position.Value.AngleRadians? 1 : -1;
                double angle = location.position.AngleRadians - 3 * direction / location.position.magnitude;
                Location a = new Location(location.planet, Double2.CosSin(angle, location.Radius));
                Astronaut_EVA.GetGroundRadius(location, out _, out double radius);
                a.position = Double2.CosSin(angle, radius);

                if ((a.position - location.position).Mag_LessThan(10) && location.planet.data.basics.gravity > 5)
                    location = a;
            }
            
            // Spawns EVA
            string astronautName = seat.astronaut.Value;
            double fuelPercent = seat.externalSeat? seat.resources.fuelPercent.Value : 1;
            float temperature = seat.externalSeat? seat.resources.temperature.Value : float.NegativeInfinity;
            seat.Exit();
            //
            Astronaut_EVA astronaut = AstronautManager.main.SpawnEVA(astronautName, location, Astronaut_EVA.GetTargetAngle(location), 0, false, fuelPercent, temperature);
            StatsRecorder.OnSplit(Rocket.stats, astronaut.stats);
            astronaut.stats.OnLeaveCapsule(astronautName);
            PlayerController.main.SmoothChangePlayer(astronaut, 1);
        }


        [Serializable]
        public class Seat
        {
            public String_Reference astronaut;
            public Vector2 hatchPosition;
            
            public bool externalSeat;
            [ShowIf("externalSeat"), Required] public GameObject astronautModel;
            [ShowIf("externalSeat"), Required] public EVA_Resources resources;
            
            public bool HasAstronaut => !string.IsNullOrEmpty(astronaut.Value);
            
            public void OnStart()
            {
                if (!HasAstronaut)
                    return;

                if (AstronautState.main.GetAstronautState(astronaut.Value) == AstronautState.State.Available)
                {
                    AstronautState.main.AddCrew(astronaut.Value);
                    AddSeatedAstronaut();
                }
                else
                {
                    Debug.Log("Astronaut " + astronaut.Value + " is not available");
                    astronaut.Value = "";
                    
                    if (externalSeat)
                        resources.fuelPercent.Value = -1;
                }
            }
            public void OnDestroy()
            {
                if (!HasAstronaut)
                    return;
                
                AstronautState.main.RemoveCrew(astronaut.Value);
                
                // Capsule crashed
                if (GameManager.main != null)
                    AstronautState.main.GetAstronautByName(astronaut.Value).alive = false;
            }
            
            public void Board(string astronautName, double fuelPercent, float temperature)
            {
                AstronautState.main.AddCrew(astronautName);
                astronaut.Value = astronautName;

                if (externalSeat)
                {
                    AddSeatedAstronaut();
                    resources.fuelPercent.Value = fuelPercent;
                    resources.temperature.Value = temperature;
                }
            }
            public void Exit()
            {
                if (DevSettings.DisableAstronauts)
                    return;
                
                AstronautState.main.RemoveCrew(astronaut.Value);
                astronaut.Value = "";

                if (externalSeat)
                    RemoveSeatedAstronaut();
            }

            void AddSeatedAstronaut()
            {
                if (externalSeat)
                    astronautModel.SetActive(true);
            }
            void RemoveSeatedAstronaut()
            {
                if (externalSeat)
                    astronautModel.SetActive(false);
            }
        }
    }
}