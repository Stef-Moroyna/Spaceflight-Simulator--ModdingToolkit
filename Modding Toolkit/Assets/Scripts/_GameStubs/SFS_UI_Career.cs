// Auto-generated compile-only dummy stubs for the public modding toolkit. No implementation, no anti-hack.

using System;
using UnityEngine;
using UnityEngine.UI;

// I_MsgLogger / MsgNone live in the plain SFS namespace in the game (UI/Elements/MsgDrawer.cs),
// reachable from SFS.Parts.* code as a parent namespace.
namespace SFS
{
    public interface I_MsgLogger
    {
        void Log(string msg);
    }

    public class MsgNone : I_MsgLogger
    {
        void I_MsgLogger.Log(string msg) { }
    }
}

// StatsMenu lives in the GLOBAL namespace in the game (UI/Menus/StatsMenu.cs has no namespace block);
// Parts reference it unqualified without `using SFS.UI;`, so it must be global here too.
public class StatsMenu : MonoBehaviour
{
        public void Close() { }

        public void DrawTitle(string title) { }

        public void DrawText(int priority, string text) { }

        public void DrawStat(int priority, string labelAndValue, string valueSize_Optional = null) { }
        public void DrawStat(int priority, Func<string> labelAndValue, Func<string> valueSize_Optional, Action<Action> register, Action<Action> unregister) { }
        public void DrawStat_Separate(int priority, Func<string> label, Func<string> value, Func<string> valueSize_Optional, Action<Action> register, Action<Action> unregister) { }

        public void DrawToggle(int priority, Func<string> labelText, Action toggle, Func<bool> getValue, Action<Action> register, Action<Action> unregister) { }

        public void DrawSlider(int priority, Func<string> labelAndValue, Func<string> valueSize_Optional, Func<float> getFillPercent, Action<float, bool> setFillPercent, Action<Action> register, Action<Action> unregister, bool startUndoStep = false) { }
        public void DrawSlider_Separate(int priority, Func<string> labelText, Func<string> valueText, Func<string> valueSize_Optional, Func<float> getFillPercent, Action<float, bool> setFillPercent, Action<Action> register, Action<Action> unregister, bool startUndoStep = false) { }

        public void DrawButton(int priority, string labelText, string buttonText, Action buttonAction, bool enabled = true) { }
        public void DrawButton(int priority, Func<string> labelText, Func<string> buttonText, Action buttonAction, Func<bool> enabled, Action<Action> register, Action<Action> unregister) { }

        public void DrawSpace(int priority, float height = 5) { }
}

namespace SFS.UI
{
    public class AttachableStatsMenu : MonoBehaviour
    {
        public StatsMenu drawer;

        // Real game signature returns OpenTracker (global namespace); return value is discarded by all Parts callers, so void keeps the stub self-contained.
        public void Open(Func<bool> hasControl, Action<StatsMenu> draw, Func<Vector2> getScreenPosition, bool dontUpdateOnZoomChange, bool skipAnimation, Action onOpen = null, Action onClose = null) { }
        public void Close() { }
    }

    public class AstronautMenu : MonoBehaviour
    {
        public static AstronautMenu main;

        public void OpenMenu(SFS.World.CrewModule.Seat seat, Action redrawSeat) { }
    }

    public class MsgDrawer : MonoBehaviour, SFS.I_MsgLogger
    {
        public static MsgDrawer main;

        public void Log(string msg) { }
        public void Log(string msg, bool big) { }
    }

    public class ButtonPC : Button
    {
        public void SetSelected(bool b) { }
    }
}

namespace SFS.Career
{
    public interface I_TechTreeData
    {
        bool IsComplete { get; }
        bool GrayOut { get; }
        string Name_ID { get; }
        int Value { get; }
    }

    public class CareerState : MonoBehaviour
    {
        public static CareerState main;

        public bool HasPart(SFS.Parts.Modules.VariantRef a) { return default; }
        public bool HasPart(SFS.Parts.Part part) { return default; }
    }
}
