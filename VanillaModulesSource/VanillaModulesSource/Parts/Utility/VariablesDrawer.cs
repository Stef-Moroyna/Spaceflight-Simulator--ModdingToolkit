using System;
using System.Collections.Generic;
using System.Globalization;
using SFS.Builds;
using SFS.Translations;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class VariablesDrawer : MonoBehaviour
    {
        public DrawElement[] elements;
        
        public void Draw(List<VariablesDrawer> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            if (!(settings.build || settings.game))
                return;

            for (int i = 0; i < elements.Length; i++)
            {
                DrawElement element = elements[i];
                int ii = i;
                
                switch (element.variableType)
                {
                    case VariableType.Float:
                        
                        if (element.floatDrawType == FloatDrawType.Stat)
                            drawer.DrawStat_Separate(element.priority, () => element.label.Field, GetValue, null, action => element.floatReference.OnChange += action, action => element.floatReference.OnChange -= action);
                        else if (element.floatDrawType == FloatDrawType.Slider)
                            drawer.DrawSlider_Separate(element.priority, () => element.label.Field, GetValue, null, () => (element.floatReference.Value - element.minValue) / (element.maxValue - element.minValue), SetFillAmount, action => element.floatReference.OnChange += action, action => element.floatReference.OnChange -= action);

                        string GetValue() => element.floatReference.Value.ToString(CultureInfo.InvariantCulture) + (string.IsNullOrWhiteSpace(element.units) ? "" : " " + element.units);

                        void SetFillAmount(float t, bool touchStart)
                        {
                            Undo.main.RecordStatChangeStep(modules, () =>
                            {
                                float x = Mathf.Lerp(element.minValue, element.maxValue, t);
                                foreach (VariablesDrawer module in modules)
                                    module.elements[ii].floatReference.Value = x;
                            }, touchStart);
                        }
                        break;

                    case VariableType.Bool:
                        
                        drawer.DrawToggle(element.priority, () => element.label.Field, Toggle, () => element.boolReference.Value, action => element.boolReference.OnChange += action, action => element.boolReference.OnChange -= action);
                        
                        void Toggle()
                        {
                            Undo.main.RecordStatChangeStep(modules, () =>
                            {
                                bool x = !element.boolReference.Value;
                                foreach (VariablesDrawer module in modules)
                                    module.elements[ii].boolReference.Value = x;
                            });
                        }
                        break;

                    case VariableType.String:
                        
                        if (element.stringDrawType == StringDrawType.Text)
                            drawer.DrawText(element.priority, element.stringReference.Value);
                        else if (element.stringDrawType == StringDrawType.Stat)
                            drawer.DrawStat_Separate(element.priority, () => element.label.Field, () => element.stringReference.Value + (string.IsNullOrWhiteSpace(element.units) ? "" : " " + element.units), null, action => element.stringReference.OnChange += action, action => element.stringReference.OnChange -= action);
                        
                        break;
                }
            }
        }

        [Serializable]
        public class DrawElement
        {
            public int priority;
            public TranslationVariable label;

            public VariableType variableType;

            [ShowIf(nameof(variableType), VariableType.Float)]
            public Float_Reference floatReference;
            [ShowIf(nameof(variableType), VariableType.Bool)]
            public Bool_Reference boolReference;
            [ShowIf(nameof(variableType), VariableType.String)]
            public String_Reference stringReference;

            // Float
            [ShowIf(nameof(variableType), VariableType.Float)]
            public FloatDrawType floatDrawType;
            
            [ShowIf(nameof(variableType), VariableType.String)]
            public StringDrawType stringDrawType;

            bool DrawUnits => variableType == VariableType.Float || (variableType == VariableType.String && stringDrawType == StringDrawType.Stat);
            [ShowIf(nameof(DrawUnits)), LabelText("Units [Optional]")]
            public string units;

            bool DrawMinMaxValue => variableType == VariableType.Float && floatDrawType == FloatDrawType.Slider;
            [ShowIf(nameof(DrawMinMaxValue))]
            public float minValue, maxValue;
        }

        public enum VariableType
        {
            Float, String, Bool
        }

        public enum FloatDrawType
        {
            Stat,
            Slider
        }

        public enum StringDrawType
        {
            Text,
            Stat
        }
    }
}