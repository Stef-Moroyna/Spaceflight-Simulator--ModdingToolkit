using System.Collections.Generic;
using SFS.Builds;
using SFS.Translations;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    public class ToggleModule : MonoBehaviour
    {
        public TranslationVariable label;
        [Required] public MoveModule state;

        void Start()
        {
            if (BuildManager.main != null)
                state.animationTime = 0.5f;
        }
        
        public void Draw(List<ToggleModule> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            if (settings.build || settings.game)
                drawer.DrawToggle(-1, () => label.Field, OnToggle, () => state.targetTime.Value > 0, update => state.targetTime.OnChange += update, update => state.targetTime.OnChange -= update);

            void OnToggle()
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    state.Toggle();
                    float targetTime = state.targetTime.Value;
                    
                    foreach (ToggleModule module in modules)
                        module.state.targetTime.Value = targetTime;
                });
            }
        }
    }
}