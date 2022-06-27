using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Variables
{
    [Serializable, InlineProperty]
    public class Float_Reference : Double_Reference
    {
        public new float Value { get => (float)base.Value; set => base.Value = value; }
        protected override bool IsEqual(double a, double b) => a == b;
    }

    [Serializable, InlineProperty, PropertySpace(1, 0)]
    public class Double_Reference : ReferenceVariable<double>
    {
        public override VariableList<double>.Variable GetVariable(string variableName) => referenceToVariables.doubleVariables.GetVariable(variableName);
        public override VariableList<double> GetVariableList() => referenceToVariables.doubleVariables;
        protected override bool IsEqual(double a, double b) => a == b;
    }

    [Serializable, InlineProperty, PropertySpace(1, 0)]
    public class Bool_Reference : ReferenceVariable<bool>
    {
        public override VariableList<bool>.Variable GetVariable(string variableName) => referenceToVariables.boolVariables.GetVariable(variableName);
        public override VariableList<bool> GetVariableList() => referenceToVariables.boolVariables;
        protected override bool IsEqual(bool a, bool b) => a == b;
    }

    [Serializable, InlineProperty, PropertySpace(1, 0)]
    public class String_Reference : ReferenceVariable<string>
    {
        public override VariableList<string>.Variable GetVariable(string variableName) => referenceToVariables.stringVariables.GetVariable(variableName);
        public override VariableList<string> GetVariableList() => referenceToVariables.stringVariables;
        protected override bool IsEqual(string a, string b) => a == b;
    }

    
    [Serializable]
    public abstract class ReferenceVariable<T>
    {
        // Variables
        [ValueDropdown("Dropdown"), HorizontalGroup, HideLabel, SerializeField, DisableInPlayMode] string variableName = "";
        [HideInInspector] public VariablesModule referenceToVariables; 
        // 
        [HideInInspector] public T localValue;
        event Action onLocalValueChange;
        event Action<T> onLocalValueChangeNew;
        event Action<T, T> onLocalValueChangeOldNew;

        bool initialized;
        VariableList<T>.Variable variable;
        VariableList<T>.Variable Variable
        {
            get
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                    return GetVariable(variableName);
                #endif

                if (!initialized)
                {
                    variable = GetVariable(variableName);
                    initialized = true;
                }

                return variable;
            }
        }


        #if UNITY_EDITOR
        // Editor
        public List<string> Dropdown()
        {
            if (variableName == null)
                variableName = "";

            if (referenceToVariables == null)
                if (UnityEditor.Selection.transforms.Length == 1)
                    referenceToVariables = UnityEditor.Selection.transforms[0].GetComponentInParentTree<VariablesModule>();

            List<string> output = new List<string> { "" };
            if (referenceToVariables != null)
                output.AddRange(GetVariableList().GetVariableNameList());

            return output;
        }
        #endif


        // Get
        bool Local => variableName.Length == 0 || referenceToVariables == null;
        public abstract VariableList<T>.Variable GetVariable(string variableName);
        public abstract VariableList<T> GetVariableList();


        // Properties
        [ShowInInspector, HorizontalGroup, HideLabel]
        public virtual T Value
        {
            get => Local ? localValue : Variable.Value;

            set
            {
                if (Local)
                {
                    if (IsEqual(localValue, value))
                        return;

                    // Sets new value
                    T oldValue = localValue;
                    localValue = value;

                    // On change events
                    onLocalValueChange?.Invoke();
                    onLocalValueChangeNew?.Invoke(value);
                    onLocalValueChangeOldNew?.Invoke(oldValue, value);
                }
                else
                {
                    //#warning FUTURE / Make set work in editor
                    // Sets new value // Variable list takes care of on change events
                    Variable.Value = value;
                }
            }
        }

        protected abstract bool IsEqual(T a, T b);

        // Registration property
        public ReferenceVariable<T> OnChange
        {
            get => this;
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        // On change registration
        public static ReferenceVariable<T> operator +(ReferenceVariable<T> a, Action b)
        {
            b.Invoke();

            if (a.Local)
                a.onLocalValueChange += b;
            else
                a.Variable.onValueChange += b;

            return a;
        }
        public static ReferenceVariable<T> operator +(ReferenceVariable<T> a, Action<T> b)
        {
            b.Invoke(a.Value);

            if (a.Local)
                a.onLocalValueChangeNew += b;
            else
                a.Variable.onValueChangeNew += b;

            return a;
        }
        public static ReferenceVariable<T> operator +(ReferenceVariable<T> a, Action<T, T> b)
        {
            b.Invoke(a.Value, a.Value);

            if (a.Local)
                a.onLocalValueChangeOldNew += b;
            else
                a.Variable.onValueChangeOldNew += b;

            return a;
        }
        public static ReferenceVariable<T> operator -(ReferenceVariable<T> a, Action b)
        {
            if (a.Local)
                a.onLocalValueChange -= b;
            else
                a.Variable.onValueChange -= b;

            return a;
        }
        public static ReferenceVariable<T> operator -(ReferenceVariable<T> a, Action<T> b)
        {
            if (a.Local)
                a.onLocalValueChangeNew -= b;
            else
                a.Variable.onValueChangeNew -= b;

            return a;
        }
        public static ReferenceVariable<T> operator -(ReferenceVariable<T> a, Action<T, T> b)
        {
            if (a.Local)
                a.onLocalValueChangeOldNew -= b;
            else
                a.Variable.onValueChangeOldNew -= b;

            return a;
        }
    }
}