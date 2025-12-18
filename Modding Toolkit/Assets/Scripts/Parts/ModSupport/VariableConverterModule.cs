using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class VariableConverterModule : MonoBehaviour
    {
        [Serializable]
        public struct BoolToDouble
        {
            public Bool_Reference input;
            public Double_Reference output;

            public void Convert()
            {
                output.Value = input.Value ? 1 : 0;
            }
        }

        [Serializable]
        public struct DoubleToBool
        {
            public Double_Reference input;
            public Bool_Reference output;
            
            public Bool_Reference useEpsilon;
            
            public void Convert()
            {
                output.Value = Math.Abs(input.Value) > (useEpsilon.Value ? 1.0e-6 : 0);
            }
        }

        [Serializable]
        public struct DoubleToString
        {
            public Double_Reference input;
            public String_Reference output;

            public void Convert()
            {
                output.Value = input.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        [Serializable]
        public struct StringToDouble
        {
            public String_Reference input;
            public Double_Reference output;

            public void Convert()
            {
                if (double.TryParse(input.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    output.Value = result;
                }
            }
        }

        [Serializable]
        public struct BoolToString
        {
            public Bool_Reference input;
            public String_Reference output;

            public bool overrideOutputs;
            public String_Reference trueString;
            public String_Reference falseString;
            
            public void Convert()
            {
                if (overrideOutputs)
                    output.Value = input.Value ? trueString.Value : falseString.Value;
                else
                    output.Value = input.Value ? "true" : "false";
            }
        }

        [Serializable]
        public struct StringToBool
        {
            public String_Reference input;
            public Bool_Reference output;

            public bool overrideParser;

            [ShowIf("overrideParser")] public String_Reference[] trueStrings;
            
            public void Convert()
            {
                var t = this;
                if (overrideParser)
                {
                    output.Value = trueStrings.Any(s => s.Value == t.input.Value);
                }
                else if (bool.TryParse(input.Value, out bool result))
                {
                    output.Value = result;
                }
            }
        }

        public List<BoolToDouble> boolToDouble = new();
        public List<DoubleToBool> doubleToBool = new();
        [Space] 
        public List<DoubleToString> doubleToString = new();
        public List<StringToDouble> stringToDouble = new();
        [Space] 
        public List<BoolToString> boolToString = new();
        public List<StringToBool> stringToBool = new();

#if UNITY_EDITOR
        private void OnValidate() => SubscribeToInputChanges();
#else
        private void Awake() => SubscribeToInputChanges();
#endif

        private void SubscribeToInputChanges()
        {
            foreach (BoolToDouble item in boolToDouble)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }

            foreach (DoubleToBool item in doubleToBool)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }

            foreach (DoubleToString item in doubleToString)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }

            foreach (StringToDouble item in stringToDouble)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }

            foreach (StringToBool item in stringToBool)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }

            foreach (BoolToString item in boolToString)
            {
                item.input.OnChange -= item.Convert;
                item.input.OnChange += item.Convert;
            }
        }
    }
}