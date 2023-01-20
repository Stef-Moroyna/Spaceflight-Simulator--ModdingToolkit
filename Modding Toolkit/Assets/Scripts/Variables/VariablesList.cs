using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SFS.Variables
{
    [Serializable, ShowInInspector, InlineProperty, HideLabel]
    public class VariableList<T>
    {
        // Non generic class used for saving
        [SerializeField, HideInInspector] public List<VariableSave> saves = new List<VariableSave>();

        // Generic class used for listening
        [ShowInInspector, TableList, ValidateInput(nameof(SyncVariables))]
        List<Variable> variables
        {
            get
            {
                if (_variables == null)
                    _variables = saves.Select(save => GetVariable(save.name)).ToList();

                return _variables;
            }
            set { /* Can remain empty since changing indecies in editor wont re-set the list, but not having set leaves the list as readonly (kinda wierd) */}
        }
        List<Variable> _variables;

        // Get/Set
        public T GetValue(string variableName)
        {
            return Has(variableName) ? GetVariable(variableName).Value : default;
        }
        public Variable GetVariable(string variableName)
        {
            VariableSave save = null;
            
            foreach (VariableSave a in saves)
                if (a.name == variableName)
                    save = a;

            if (save == null)
                return null;

            if (save.runtimeVariable == null)
                save.runtimeVariable = new Variable(save);

            return (Variable)save.runtimeVariable;
        }
        public void SetValue(string variableName, T newValue, (bool, bool) addMissingVariables = default)
        {
            if (!Has(variableName))
                if (addMissingVariables.Item1)
                    saves.Add(new VariableSave(variableName) { save = addMissingVariables.Item2 });
                else
                    return;

            GetVariable(variableName).Value = newValue;
        }
        public bool Has(string variableName)
        {
            foreach (VariableSave a in saves)
                if (a.name == variableName)
                    return true;

            return false;
        }
        public void RegisterOnVariableChange(Action onChange, string variableName)
        {
            GetVariable(variableName).onValueChange += onChange;
        }
        public List<string> GetVariableNameList()
        {
            return saves.Select(a => a.name).ToList();
        }

        // Save/load
        public Dictionary<string, T> GetSaveDictionary()
        {
            Dictionary<string, T> output = new Dictionary<string, T>();

            foreach (VariableSave save in saves)
                if (save.save)
                    output[save.name] = GetVariable(save.name).Value;
            
            return output;
        }
        public void LoadDictionary(Dictionary<string, T> inputs, (bool, bool) addMissingVariables)
        {
            if (inputs == null)
                return;

            foreach (KeyValuePair<string, T> input in inputs)
                SetValue(input.Key, input.Value, addMissingVariables);
        }

        // Editor functionality
        void AddVariable()
        {
            saves.Add(new VariableSave(""));
        }
        void RemoveVariable(Variable toRemove)
        {
            saves.Remove((toRemove as IVariable).GetSave());
        }
        void RemoveVariableByIndex(int index)
        {
            saves.RemoveAt(index);
        }
        bool SyncVariables(List<Variable> variables)
        {
            foreach (Variable variable in variables)
                ((IVariable)variable).SaveAsBytes();

            saves = variables.Select(var => (var as IVariable).GetSave()).ToList();
            return true;
        }


        [HideReferenceObjectPicker]
        public class Variable : IVariable
        {
            #if UNITY_EDITOR
            [ShowInInspector]
            string Name
            {
                get => save.name;
                set => save.name = value;
            }
            [ShowInInspector]
            bool Save
            {
                get => save.save;
                set => save.save = value;
            }
            #endif

            VariableSave save;
            T value;

            public event Action onValueChange;
            public event Action<T> onValueChangeNew;
            public event Action<T, T> onValueChangeOldNew;

            public Variable()
            {
                save = new VariableSave("");
            }
            public Variable(VariableSave save)
            {
                this.save = save;

                if (save.data != null && save.data.Length > 0)
                    using (MemoryStream ms = new MemoryStream(save.data))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        value = (T)new BinaryFormatter().Deserialize(ms);
                    }
            }


            [ShowInInspector]
            public T Value
            {
                get => value;
                set
                {
                    if (Equals(value, this.value))
                        return;

                    T oldValue = this.value;
                    this.value = value;

                    onValueChange?.Invoke();
                    onValueChangeNew?.Invoke(value);
                    onValueChangeOldNew?.Invoke(oldValue, value);
                }
            }


            VariableSave IVariable.GetSave() => save;
            void IVariable.SaveAsBytes()
            {
                if (value != null)
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new BinaryFormatter().Serialize(ms, value);
                        save.data = ms.ToArray();
                    }
            }
        }
        interface IVariable
        {
            void SaveAsBytes();
            VariableSave GetSave();
        }
    }

    [Serializable]
    public class VariableSave
    {
        public string name;
        public bool save;
        
        // ??
        public byte[] data;

        [ShowInInspector]
        public object runtimeVariable;

        public VariableSave(string name)
        {
            this.name = name;
        }
    }
}
