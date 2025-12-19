using SFS.Builds;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Career;
using UnityEngine;

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


    [Serializable, InlineProperty]
    public class VariantRef : I_TechTreeData
    {
        // Part dropdown
        [Required, ValueDropdown(nameof(PartOptions)), HideLabel] public Part part;
        Part[] PartOptions() => PartsLoader.LoadParts().Values.ToArray();
        
        
        // Variant dropdown
        [HideInInspector] public int variantIndex_A = -1;
        [HideInInspector] public int variantIndex_B = -1;
        //
        [ValueDropdown(nameof(Variants)), ShowInInspector, HideLabel] string Variant
        {
            get => GetLabel(variantIndex_A, variantIndex_B);
            set
            {
                variantIndex_A = int.Parse(value.Split(',')[0]);
                variantIndex_B = int.Parse(value.Split(',')[1]);
            }
        }
        string[] Variants()
        {
            if (part == null)
                return new []{ -1 + ", " + -1 };

            List<string> output = new List<string>();
            
            for (int a = 0; a < part.variants.Length; a++)
                for (int b = 0; b < part.variants[a].variants.Length; b++)
                    output.Add(GetLabel(a, b));

            return output.ToArray();
        }


        string GetLabel(int a, int b)
        {
            string label = a + ", " + b;
                
            bool valid = part != null && part.variants.IsValidIndex(a) && part.variants[a].variants.IsValidIndex(b);
            
            if (valid)
                foreach (Variants.Variable change in part.variants[a].variants[b].changes)
                    label += ", " + change.GetLabel();

            return label;
        }
        

        // Functionality
        public Variants.Variant GetVariant()
        {
            return part.variants[variantIndex_A].variants[variantIndex_B];
        }
        public string GetNameID()
        {
            return part.name + "_" + variantIndex_A + "_" + variantIndex_B;
        }
        public VariantRef(Part part, int variantIndex_A, int variantIndex_B)
        {
            this.part = part;
            this.variantIndex_A = variantIndex_A;
            this.variantIndex_B = variantIndex_B;
        }
        public List<Variants.PickTag> GetPickTags()
        {
            if (variantIndex_A == -1)
                return new List<Variants.PickTag>();

            List<Variants.PickTag> output = new List<Variants.PickTag>();

            if (part.variants[variantIndex_A].tags != null)
                foreach (Variants.PickTag tag in part.variants[variantIndex_A].tags)
                    if (!output.Contains(tag))
                        output.Add(tag);

            if (GetVariant().tags != null)
                foreach (Variants.PickTag tag in GetVariant().tags)
                    if (!output.Contains(tag))
                        output.Add(tag);

            return output;
        }
        public int GetPriority(PickCategory tag)
        {
            foreach (Variants.PickTag pickTag in part.variants[variantIndex_A].tags)
                if (pickTag.tag == tag)
                    return pickTag.priority;

            foreach (Variants.PickTag pickTag in GetVariant().tags)
                if (pickTag.tag == tag)
                    return pickTag.priority;

            throw new Exception("Tag not found");
        }


        // Implementation
        bool I_TechTreeData.IsComplete => CareerState.main.HasPart(this);
        bool I_TechTreeData.GrayOut => !((I_TechTreeData)this).IsComplete;
        string I_TechTreeData.Name_ID => GetNameID();
        public int Value => 1;
    }
}