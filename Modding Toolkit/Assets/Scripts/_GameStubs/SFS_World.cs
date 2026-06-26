// Auto-generated compile-only dummy stubs for the public modding toolkit. Game-only SFS.World types. No implementation, no anti-hack.

using System;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.WorldBase;
using SFS.Input;
using SFS.UI;
using SFS.Stats;

namespace SFS.World
{
    // ---- Player base ----
    public abstract class Player : ObservableMonoBehaviour
    {
        public WorldLocation location;
        public MapPlayer mapPlayer;
        public Bool_Local isPlayer;
        public Bool_Local hasControl;

        public abstract float GetSizeRadius();
        public abstract void ClampTrackingOffset(ref Vector2 trackingOffset, float cameraDistance);
        public abstract bool OnInputEnd_AsPlayer(OnInputEndData data);
        public abstract float TryWorldSelect(TouchPosition data);
        public abstract bool CanTimewarp(I_MsgLogger logger, bool showSpeed, out bool isInWater);
    }

    [Serializable]
    public class Player_Local : Obs_Destroyable<Player>
    {
        protected override bool IsEqual(Player a, Player b) => a == b;
    }

    // ---- Rocket ----
    public partial class Rocket : Player, I_Physics
    {
        public Mass_Calculator mass;
        public Rigidbody2D rb2d;
        public PartHolder partHolder;
        public Arrowkeys arrowkeys;
        public Throttle throttle;
        public Resources resources;
        public Aero_Rocket aero;
        public StatsRecorder stats;
        public Physics physics;

        public string rocketName;
        public JointGroup jointsGroup;
        public float collisionImmunity;
        public bool floating;

        public Float_Local output_TurnAxisTorque;
        public Float_Local output_TurnAxisWheels;
        public Vector2_Local output_DirectionalAxis;

        public void EnableCollisionImmunity(float duration) { }

        public override float GetSizeRadius() { return default; }
        public override void ClampTrackingOffset(ref Vector2 trackingOffset, float cameraDistance) { }
        public override bool OnInputEnd_AsPlayer(OnInputEndData data) { return default; }
        public override float TryWorldSelect(TouchPosition data) { return default; }
        public override bool CanTimewarp(I_MsgLogger logger, bool showSpeed, out bool isInWater) { isInWater = default; return default; }

        bool I_Physics.PhysicsMode { get => default; set { } }
        Vector2 I_Physics.LocalPosition { get => default; set { } }
        Vector2 I_Physics.LocalVelocity { get => default; set { } }
        void I_Physics.OnFixedUpdate(Vector2 gravity) { }
        void I_Physics.OnCrashIntoPlanet() { }

        public void SetJointGroup(JointGroup jointsGroup) { }
        public void SetParts(Part[] newParts) { }

        public static void SetPlayerToBestControllable(params Rocket[] rockets) { }
        public static UsePartData[] UseParts(bool fromStaging, params (Part, PolygonData)[] regions) { return default; }

        // Injection interfaces
        public interface INJ_Rocket { Rocket Rocket { set; } }
        public interface INJ_IsPlayer { bool IsPlayer { set; } }
        public interface INJ_HasControl { bool HasControl { set; } }
        public interface INJ_ThrottleOn { bool ThrottleOn { set; } }
        public interface INJ_Throttle { float Throttle { set; } }
        public interface INJ_TurnAxisTorque { float TurnAxis { set; } }
        public interface INJ_TurnAxisWheels { float TurnAxis { set; } }
        public interface INJ_DirectionalAxis { Vector2 DirectionalAxis { set; } }
        public interface INJ_Physics { Rigidbody2D Rb2d { set; } }
        public interface INJ_Location { Location Location { set; } }
    }

    // ---- Location containers ----
    public class WorldLocation : MonoBehaviour
    {
        public Planet_Local planet = new Planet_Local();
        public Double2_Local position = new Double2_Local();
        public Double2_Local velocity = new Double2_Local();

        public double Height => default;

        public Location Value { get => default; set { } }
    }

    [Serializable]
    public class Location
    {
        public double time;
        public Planet planet;
        public Double2 position;
        public Double2 velocity;

        public double Radius => default;
        public double Height => default;
        public double VerticalVelocity => default;

        public double GetTerrainHeight(bool clampToWater) { return default; }
        public Double2 GetSolarSystemPosition(double time) { return default; }

        public Location(double time, Planet planet, Double2 position, Double2 velocity) { }
        public Location(Planet planet, Double2 position, Double2 velocity = default) { }

        public static Location operator +(Location a, Location b) { return default; }
    }

    // ---- Physics ----
    public class Physics : MonoBehaviour
    {
        public WorldLocation location;
        public WorldLoader loader;
        public Trajectory trajectory;

        public I_Physics PhysicsObject { get => default; }
        public bool PhysicsMode { get => default; set { } }

        public void SetLocationAndState(Location newLocation, bool physicsMode) { }
        public Trajectory GetTrajectory() { return default; }
    }

    public interface I_Physics
    {
        bool PhysicsMode { get; set; }
        void OnFixedUpdate(Vector2 gravity);
        Vector2 LocalPosition { get; set; }
        Vector2 LocalVelocity { get; set; }
        void OnCrashIntoPlanet();
    }

    // ---- Joints ----
    [Serializable]
    public class JointGroup
    {
        public Dictionary<Part, List<PartJoint>> dictionary;
        public List<PartJoint> joints;
        public List<Part> parts;

        public JointGroup(List<PartJoint> joints, List<Part> parts) { }

        public static void OnPartDestroyed(Part part, Rocket rocket, DestructionReason reason) { }
        public static void DestroyJoint(PartJoint joint, Rocket rocket, out bool split, out Rocket newRocket) { split = default; newRocket = default; }
        public static List<JointGroup> RecreateRockets(Rocket rocket, out List<Rocket> childRockets) { childRockets = default; return default; }

        public void RecreateGroups(out List<JointGroup> newGroups) { newGroups = default; }
        public void AddJoint(PartJoint joint) { }
        public void RemovePartAndItsJoints(Part part) { }
        public void RepositionParts() { }
        public List<List<ResourceModule>> GetResourceGroups() { return default; }
        public List<SplitModule> GetConnectedFairings(Part startPart, SplitModule fairing) { return default; }
        public List<PartJoint> GetConnectedJoints(Part part) { return default; }
    }

    [Serializable]
    public class PartJoint
    {
        public Part a, b;
        public Vector2 anchor;

        public PartJoint(Part a, Part b, Vector2 anchor) { }

        public Part GetOtherPart(Part part) { return default; }
        public Vector2 GetRelativeAnchor(Part part) { return default; }
    }

    public enum DestructionReason
    {
        TerrainCollision,
        WaterCollision,
        RocketCollision,
        Overheat,
        Intentional
    }

    // ---- Player sub-components ----
    public class Throttle : MonoBehaviour
    {
        public Bool_Local throttleOn;
        public Float_Local throttlePercent;
        public Float_Local output_Throttle;
    }

    public class Arrowkeys : MonoBehaviour
    {
        public Bool_Local rcs;
        public Float_Local turnAxis;
        public Vector2_Local horizontalAxis;
        public Vector2_Local verticalAxis;
    }

    public class Resources : ObservableMonoBehaviour
    {
        public ResourceModule[] globalGroups = new ResourceModule[0];
        public void ToggleTransfer(Part part, ResourceModule group) { }
    }

    // ---- Trajectory / loader (referenced as Physics fields only) ----
    public class Trajectory { }
    public class WorldLoader : MonoBehaviour { }

    // ---- Managers ----
    public class RocketManager : MonoBehaviour
    {
        public static void MergeRockets(Rocket rocket_A, Part part_A, Rocket rocket_B, Part part_B, Vector2 anchor) { }
        public static void DestroyRocket(Rocket rocket, DestructionReason reason) { }
        public static Rocket CreateRocket_Child(JointGroup group, Rocket parent, Vector2 offset) { return default; }
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager main;
        public List<Rocket> rockets = new List<Rocket>();
        public WorldEnvironment environment;
    }

    public class WorldEnvironment : MonoBehaviour
    {
        public void SpawnGroundParticles(Vector2 position, Vector2 velocity, Vector2 surfaceNormal, int count) { }
    }

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController main;
        public Player_Local player;
        public Vector2_Local cameraOffset;

        public void CreateShakeEffect(float intensity, float duration, float range, Vector2 position) { }
        public void SetOffset(Vector2 offset, float smoothTime) { }
        public void SmoothChangePlayer(Player newPlayer, float smoothTime = 0.25f) { }
    }

    public class WorldView : MonoBehaviour
    {
        public static WorldView main;
        public Location ViewLocation { get; private set; }
        public Action<Vector2> onVelocityOffset;

        public static Double2 ToGlobalPosition(Vector2 localPosition) { return default; }
        public static Double2 ToGlobalVelocity(Vector2 localVelocity) { return default; }
        public static Vector2 ToLocalPosition(Double2 globalPosition) { return default; }
        public static Vector2 ToLocalVelocity(Double2 globalVelocity) { return default; }
    }

    public class WorldTime : MonoBehaviour
    {
        public static WorldTime main;
        public double worldTime;
        public double timewarpSpeed;
        public Bool_Local realtimePhysics;

        public static float FixedDeltaTime => default;
        public static float DeltaTime => default;

        public void StopTimewarp(bool showMsg) { }
    }

    public class MapPlayer : MonoBehaviour { }

    // ---- Astronauts ----
    public class Astronaut_EVA : Player, I_Physics
    {
        public EVA_Resources resources;
        public Physics physics;
        public Arrowkeys arrowkeys;
        public StatsRecorder stats;
        public Rigidbody2D rb2d;
        public WorldSave.Astronauts.Data astronaut;

        public override float GetSizeRadius() { return default; }
        public override void ClampTrackingOffset(ref Vector2 trackingOffset, float cameraDistance) { }
        public override bool OnInputEnd_AsPlayer(OnInputEndData data) { return default; }
        public override float TryWorldSelect(TouchPosition data) { return default; }
        public override bool CanTimewarp(I_MsgLogger logger, bool showSpeed, out bool isInWater) { isInWater = default; return default; }

        bool I_Physics.PhysicsMode { get => default; set { } }
        Vector2 I_Physics.LocalPosition { get => default; set { } }
        Vector2 I_Physics.LocalVelocity { get => default; set { } }
        void I_Physics.OnFixedUpdate(Vector2 gravity) { }
        void I_Physics.OnCrashIntoPlanet() { }

        public static void GetGroundRadius(Location location, out double groundAngleRadians, out double groundRadius) { groundAngleRadians = default; groundRadius = default; }
        public static float GetTargetAngle(Location loc) { return default; }
    }

    public class AstronautManager : MonoBehaviour
    {
        public static AstronautManager main;
        public List<Astronaut_EVA> eva = new List<Astronaut_EVA>();

        public Astronaut_EVA SpawnEVA(string astronautName, Location location, float rotation, float angularVelocity, bool ragdoll, double fuelPercent, float temperature) { return default; }
        public static void DestroyEVA(Astronaut_EVA astronaut, bool death) { }
    }

    public class EVA_Resources : SFS.World.Drag.HeatModuleBase
    {
        public Astronaut_EVA astronaut;
        public const double Fuel_DeltaV = 200;
        public Double_Reference fuelPercent;
        public Float_Reference temperature;

        public void ConsumeFuel(double amount) { }

        public override string Name => default;
        public override bool IsHeatShield => default;
        public override float Temperature { get => default; set { } }
        public override int LastAppliedIndex { get; set; }
        public override float ExposedSurface { get; set; }
        public override float HeatTolerance => default;
        public override void OnOverheat(bool _) { }
    }

    public class ResourceDrawer : MonoBehaviour
    {
        public interface I_Resource
        {
            ResourceType ResourceType { get; }
            float WetMass { get; }
            Double_Reference ResourcePercent { get; }
        }
    }

    // ---- World save (only the nested astronaut data referenced) ----
    public class WorldSave
    {
        public class Astronauts
        {
            [Serializable]
            public class Data
            {
                public string astronautName;
                public bool alive;

                public Data() { }
                public Data(string astronautName, bool alive) { }
            }
        }
    }

    // ---- Sandbox settings ----
    public class SandboxSettings : MonoBehaviour
    {
        public static SandboxSettings main;
        public Data settings;

        public class Data
        {
            public bool infiniteFuel;
            public bool unbreakableParts;
            public bool noGravity;
        }
    }

    public class Aero_Rocket
    {
        public HeatManager heatManager;
    }

    public class HeatManager
    {
        public void HeatPart(SFS.World.Drag.HeatModuleBase a) { }
    }
}
