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