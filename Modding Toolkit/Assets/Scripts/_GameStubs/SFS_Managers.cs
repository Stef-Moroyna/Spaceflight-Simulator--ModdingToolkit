// Auto-generated compile-only dummy stubs for the public modding toolkit. Game-only manager types + shallow cascade. No implementation, no anti-hack.

using System;
using System.Collections.Generic;
using UnityEngine;
using SFS.Parts;
using SFS.Parts.Modules;

// ---------------------------------------------------------------------------
// SFS.Builds
// ---------------------------------------------------------------------------
namespace SFS.Builds
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager main;

        public BuildGrid buildGrid;
        public BuildMenus buildMenus;
    }

    public class BuildGrid : MonoBehaviour
    {
        public PartGrid activeGrid;
    }

    // activeGrid type, exposing the parts holder (PartHolder is already defined in the toolkit).
    public class PartGrid : MonoBehaviour
    {
        public PartHolder partsHolder;
    }

    public class BuildMenus : MonoBehaviour
    {
        public SkinMenu colorSkins, shapeSkins;
        public SFS.UI.AttachableStatsMenu partMenu;
    }

    // colorSkins / shapeSkins type.
    public class SkinMenu : MonoBehaviour
    {
        public void DrawSkinButtons() { }
    }

    public class Undo : MonoBehaviour
    {
        public static Undo main;

        // Editor-style record (matches the explicit toolkit surface).
        public static void RecordObject(UnityEngine.Object objectToUndo, string name) { }

        public void RecordOtherStep(OtherAction.Type type) { }

        public void RecordStatChangeStep<T>(List<T> modules, Action action, bool createStep = true) where T : MonoBehaviour { }
        public void RecordStatChangeStep(Part[] parts, Action action, bool createStep = true) { }

        [Serializable]
        public class OtherAction
        {
            public enum Type
            {
                EnableInteriorView,
                DisableInteriorView,
            }
        }
    }
}

// ---------------------------------------------------------------------------
// SFS.Career
// ---------------------------------------------------------------------------
namespace SFS.Career
{
    public class AstronautState : MonoBehaviour
    {
        public static AstronautState main;

        public void AddCrew(string astronautName) { }
        public void RemoveCrew(string astronautName) { }
        public State GetAstronautState(string astronautName) { return default; }
        public SFS.World.WorldSave.Astronauts.Data GetAstronautByName(string astronautName) { return default; }

        public enum State
        {
            Available,
            CrewBuild,
            EVA,
            CrewWorld,
            Deceased,
        }
    }

    public class HubManager : MonoBehaviour
    {
        public static HubManager main;
    }
}

// ---------------------------------------------------------------------------
// SFS.World
// ---------------------------------------------------------------------------
namespace SFS.World
{
    public class EffectManager : MonoBehaviour
    {
        public static void CreateExplosion(Vector3 position, float size) { }
        public static void CreatePartOverheatEffect(Vector3 position, float size) { }
        public static Transform CreateEffect(Transform prefab, Vector3 position, float duration) { return default; }
    }
}

// ---------------------------------------------------------------------------
// SFS.Input
// ---------------------------------------------------------------------------
namespace SFS.Input
{
    public class KeybindingsPC : MonoBehaviour
    {
        public static Data keys = new Data();

        public class Data
        {
            public Key[] Turn_Rocket;
            public Key[] Move_Rocket_Using_RCS;
        }

        public class Key
        {
            public KeyCode key;
        }
    }
}

// ---------------------------------------------------------------------------
// SFS.UI
// ---------------------------------------------------------------------------
namespace SFS.UI
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager main;
    }

    public class AttachWithArrow : MonoBehaviour
    {
        public static Func<Vector2> FollowPart(Part part) { return default; }
    }
}

// ---------------------------------------------------------------------------
// GLOBAL namespace (game files have no namespace block)
// ---------------------------------------------------------------------------
public class DevSettings : MonoBehaviour
{
    public static bool DisableAstronauts => true;
    public static bool FullVersion => true;
}

public class ResourcesLoader : MonoBehaviour
{
    public static T[] GetFiles_Array<T>(string path) where T : UnityEngine.Object { return default; }
}
