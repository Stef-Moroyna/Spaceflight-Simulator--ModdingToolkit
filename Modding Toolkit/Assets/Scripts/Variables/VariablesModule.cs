using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace SFS.Variables
{
    [HideMonoScript]
    public class VariablesModule : MonoBehaviour
    {
        public DoubleVariableList doubleVariables = new DoubleVariableList();
        public BoolVariableList boolVariables = new BoolVariableList();
        public StringVariableList stringVariables = new StringVariableList();
    }

    
    [Serializable] public class DoubleVariableList : VariableList<double> { }
    [Serializable] public class BoolVariableList : VariableList<bool> { }
    [Serializable] public class StringVariableList : VariableList<string> { }
}