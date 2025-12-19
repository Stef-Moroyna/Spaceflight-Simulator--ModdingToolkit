using System;
using System.Collections.Generic;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class MathModule : MonoBehaviour
    {
        [Serializable]
        public struct NumericCalculator
        {
            public Composed_Double input;
            public Double_Reference output;

            public void Register()
            {
                NumericCalculator inst = this; // what the fuck?
                _ = input.Value; // Jordi: This probably initializes it?
                input.OnChange -= OnChange;
                input.OnChange += OnChange;

                void OnChange(double _, double value)
                {
                    inst.output.Value = value;
                }
            }
        }

        [Serializable]
        public struct BooleanComparisonCalculator
        {
            public enum Condition
            {
                GreaterThan,
                LessThan,
                GreaterThanOrEqual,
                LessThanOrEqual,
                Equal,
            }

            public Double_Reference a;
            public Double_Reference b;
            [Space] public Condition comparisonCondition;
            public Bool_Reference invert;

            [Tooltip(
                "Adds a tolerance of 0.00001 when comparing values. Less/GreaterThan actually become more strict as the difference has to be at least the epsilon.")]
            [LabelText("Lax Mode ⓘ")]
            public bool laxMode;

            [Space] public Bool_Reference output;

            const double epsilon = 1e-5;

            public void Register()
            {
                a.OnChange += OnChange;
                b.OnChange += OnChange;
                invert.OnChange += OnChange;
            }

            void OnChange()
            {
                bool result = false;
                if (comparisonCondition == Condition.Equal)
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    result = laxMode ? Math.Abs(a.Value - b.Value) <= epsilon : a.Value == b.Value;
                else if (comparisonCondition == Condition.GreaterThan)
                    result = laxMode ? a.Value - b.Value >= epsilon : a.Value > b.Value;
                else if (comparisonCondition == Condition.LessThan)
                    result = laxMode ? b.Value - a.Value >= epsilon : a.Value < b.Value;
                else if (comparisonCondition == Condition.GreaterThanOrEqual)
                    result = laxMode ? a.Value - b.Value >= -epsilon : a.Value >= b.Value;
                else if (comparisonCondition == Condition.LessThanOrEqual)
                    result = laxMode ? b.Value - a.Value >= -epsilon : a.Value <= b.Value;

                if (invert.Value)
                    result ^= true;

                output.Value = result;
            }
        }

        [Serializable]
        public struct BooleanOperatorCalculator
        {
            public enum Operator
            {
                AND,
                NAND,
                OR,
                NOR,
                XOR,
                XNOR
            }

            public Bool_Reference a;
            public Operator op;
            public Bool_Reference b;
            [Space] public Bool_Reference output;

            public void Register()
            {
                a.OnChange += OnChange;
                b.OnChange += OnChange;
            }

            void OnChange()
            {
                bool result = false;
                if (op == Operator.OR)
                    result = a.Value || b.Value;
                else if (op == Operator.XOR)
                    result = a.Value ^ b.Value;
                else if (op == Operator.XNOR)
                    result = !(a.Value ^ b.Value);
                else if (op == Operator.AND)
                    result = a.Value && b.Value;
                else if (op == Operator.NAND)
                    result = !(a.Value && b.Value);
                else if (op == Operator.NOR)
                    result = !(a.Value || b.Value);

                output.Value = result;
            }
        }


        [InfoBox(
            "Using variable names with numbers or special characters (except _) in the calculator expressions will bug out!")]
        public List<NumericCalculator> numericCalculators = new();

        public List<BooleanComparisonCalculator> booleanComparisonCalculators = new();
        public List<BooleanOperatorCalculator> booleanOperatorCalculators = new();

#if UNITY_EDITOR
        void OnValidate() => RegisterAll();
#else
        void Awake() => RegisterAll();
#endif

        void RegisterAll()
        {
            foreach (NumericCalculator c in numericCalculators)
                c.Register();

            foreach (BooleanComparisonCalculator c in booleanComparisonCalculators)
                c.Register();

            foreach (BooleanOperatorCalculator c in booleanOperatorCalculators)
                c.Register();
        }
    }
}