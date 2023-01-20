using SFS.Builds;
using Sirenix.OdinInspector;
using System;

namespace SFS.Parts.Modules
{
    [Serializable]
    public class Variants
    {
        [BoxGroup] public Variant[] variants = new Variant[0];
        [BoxGroup] public PickTag[] tags = new PickTag[0];


        // Pack
        [Serializable] public class Variant
        {
            // Stat changes
            public Variable[] changes = new Variable[0];
            // Pick
            public PickTag[] tags = new PickTag[0];

            // Tech tree
            public double cost;

            
            #if UNITY_EDITOR
            [Button] void ApplyVariant()
            {
                Part part = UnityEditor.Selection.transforms[0].GetComponent<Part>();
                ApplyVariant(part);

                foreach (BaseMesh a in part.GetModules<BaseMesh>())
                    a.GenerateMesh();

                foreach (PositionModule a in part.GetModules<PositionModule>())
                    a.Position();
            }
            #endif
            
            public void ApplyVariant(Part part)
            {
                foreach (Variable variable in changes)
                    switch (variable.type)
                    {
                        case Variable.ValueType.Number: part.variablesModule.doubleVariables.SetValue(variable.name, variable.numberValue); break;
                        case Variable.ValueType.Toggle: part.variablesModule.boolVariables.SetValue(variable.name, variable.toggleValue); break;
                        case Variable.ValueType.Text: part.variablesModule.stringVariables.SetValue(variable.name, variable.textValue); break;
                    }
            }
        }
        
        // Modules
        [Serializable] public class Variable
        {
            [HorizontalGroup, HideLabel] public string name;
            [HorizontalGroup, HideLabel] public ValueType type;
            [HorizontalGroup, HideLabel, ShowIf("type", ValueType.Number)] public double numberValue;
            [HorizontalGroup, HideLabel, ShowIf("type", ValueType.Toggle)] public bool toggleValue;
            [HorizontalGroup, HideLabel, ShowIf("type", ValueType.Text)] public string textValue;

            public string GetLabel()
            {
                switch (type)
                {
                    case ValueType.Number: return name + numberValue;
                    case ValueType.Toggle: return name + toggleValue;
                    case ValueType.Text: return name + textValue;
                    default: throw new Exception();
                }
            }
            
            public enum ValueType
            {
                Number,
                Toggle,
                Text,
            }
        }
        [Serializable] public class PickTag
        {
            [Required] public PickCategory tag;
            public int priority;
        }
    }
}