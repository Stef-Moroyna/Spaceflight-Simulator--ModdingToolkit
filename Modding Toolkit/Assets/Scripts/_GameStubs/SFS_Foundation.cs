// Auto-generated compile-only dummy stubs for the public modding toolkit. Foundational/base game-only types. No implementation, no anti-hack.

using System;
using System.Collections.Generic;
using UnityEngine;
using SFS.Variables;
using SFS.World.PlanetModules;

// ---- Global namespace foundational bases ----

public class MonoBehaviourCached : MonoBehaviour
{
    public new Transform transform => default;
    public new GameObject gameObject => default;
}

public class ObservableMonoBehaviour : MonoBehaviourCached, I_ObservableMonoBehaviour
{
    Action I_ObservableMonoBehaviour.OnDestroy { get => default;
        set { } }
}

namespace SFS.WorldBase
{
    public class Planet : MonoBehaviour
    {
        public string codeName;
        public PlanetData data;

        public double Radius => default;
        public double AtmosphereHeightPhysics => default;
        public double VacuumStartHeight => default;
        public bool HasAtmospherePhysics => default;

        public bool CanUseVacuumEngines(double height) { return default; }
        public double GetGravity(double radius) { return default; }
        public double GetAtmosphericDensity(double height) { return default; }
    }

    public class PlanetData
    {
        public BasicModule basics;
        public Atmosphere_Physics atmospherePhysics;
        public bool hasWater;
        public PostProcessingModule postProcessing;
    }

    public class WorldBaseManager : MonoBehaviour
    {
        public Bool_Local insideWorld = new Bool_Local();
        public WorldSettings settings;

        public bool AllowsCheats => default;
    }

    public class WorldSettings
    {
        public Difficulty difficulty;
    }

    public class Difficulty
    {
        public double IspMultiplier => default;
        public double DryMassMultiplier => default;
        public double EngineMassMultiplier => default;
    }
}

namespace SFS.World.PlanetModules
{
    public class BasicModule
    {
        public double radius;
        public double gravity;
        public double timewarpHeight;
    }

    public class Atmosphere_Physics
    {
        public double height;
        public double parachuteMultiplier;
    }
}

namespace SFS.Parts
{
    public class PartsLoader : MonoBehaviour
    {
        public Material partMaterial, engineGlowMaterial;
        public Dictionary<string, ColorTexture> colorTextures;
        public Dictionary<string, ShapeTexture> shapeTextures;
        public static Dictionary<string, Part> LoadParts() { return default; }
        public static Part[] DuplicateParts(string sortingLayer, params Part[] parts) { return default; }
    }
}

namespace SFS
{
    public class SceneLoader : MonoBehaviour
    {
        public bool isUnloading;
        public void LoadBuildScene(bool askBuildNew) { }
    }

    public static class Base
    {
        public static SceneLoader sceneLoader;
        public static WorldBase.WorldBaseManager worldBase;
        public static Parts.PartsLoader partsLoader;
    }
}