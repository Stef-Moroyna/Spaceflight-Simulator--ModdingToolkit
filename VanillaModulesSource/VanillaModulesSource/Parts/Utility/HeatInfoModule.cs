using SFS.Translations;
using SFS.World.Drag;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class HeatInfoModule : MonoBehaviour, I_PartMenu
    {
        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            float temperature = GetComponent<HeatModuleBase>().HeatTolerance;
            drawer.DrawStat(90, Loc.main.Max_Heat_Tolerance.Inject(temperature.ToTemperatureString(), "temperature"));
        }
    }
}