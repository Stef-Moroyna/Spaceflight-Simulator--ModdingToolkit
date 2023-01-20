using SFS.Variables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SFS.Parsers.Constructed
{
    public static class Compute
    {
        public static I_Node Compile(string code, VariablesModule variables, out List<string> usedVariables)
        {
            int i = 0;
            return Compile(ref i, ref code, variables, out usedVariables);
        }
        static I_Node Compile(ref int i, ref string input, VariablesModule variables, out List<string> usedVariables)
        {
            usedVariables = new List<string>();
            List<int> elements = new List<int>(); // above 0 = node, below 0 = operator
            List<I_Node> nodes = new List<I_Node>();

            if (string.IsNullOrWhiteSpace(input))
                return new Number();

            input = input.Replace(" ", "");

            while (i < input.Length)
            {
                // Brackets
                if (input[i] == '(')
                {
                    i++;
                    nodes.Add(Compile(ref i, ref input, variables, out List<string> _usedVariables));

                    foreach (string usedVariable in _usedVariables)
                        if (!usedVariables.Contains(usedVariable))
                            usedVariables.Add(usedVariable);

                    elements.Add(nodes.Count - 1);
                    continue;
                }
                if (input[i] == ')')
                {
                    i++;
                    break;
                }

                bool minus = input[i] == '-' && (elements.Count == 0 || elements.Last() < 0);
                if (minus)
                    i++;

                // Variable
                string letters = "";
                while (i < input.Length && (char.IsLetter(input[i]) || input[i] == '_'))
                {
                    letters += input[i];
                    i++;
                }
                if (letters.Length > 0)
                {
                    nodes.Add(new Variable()
                    {
                        variable = variables.doubleVariables.GetVariable(letters),
                        modifier = minus? -1 : 1
                    });
                    elements.Add(nodes.Count - 1);
                    usedVariables.Add(letters);
                    continue;
                }

                // Number
                string numbers = "";
                while (i < input.Length && (elements.Count == 0 || elements.Last() < 0) && (char.IsNumber(input[i]) || input[i] == '.'))
                {
                    numbers += input[i];
                    i++;
                }
                if (numbers.Length > 0)
                {
                    float value = float.Parse(numbers, CultureInfo.InvariantCulture);
                    nodes.Add(new Number() { Value = minus? -value : value });
                    elements.Add(nodes.Count - 1);
                    continue;
                }


                if (input[i] == '+')
                    elements.Add(-1);

                else if (input[i] == '-')
                    elements.Add(-2);

                else if (input[i] == '*')
                    elements.Add(-3);

                else if (input[i] == '/')
                    elements.Add(-4);

                i++;
            }

            // Finalize
            int protection = 0;
            while (elements.Contains(-3) || elements.Contains(-4))
            {
                if (protection < 1000)
                    protection++;
                else
                    throw new Exception();

                for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                    if (elements[elementIndex] == -3)
                    {
                        Operator(new Multiply(), elementIndex);
                        break;
                    }
                    else if (elements[elementIndex] == -4)
                    {
                        Operator(new Divide(), elementIndex);
                        break;
                    }
            }
            while (elements.Contains(-1) || elements.Contains(-2))
            {
                if (protection < 1000)
                    protection++;
                else
                    throw new Exception();

                for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                    if (elements[elementIndex] == -1)
                    {
                        Operator(new Add(), elementIndex);
                        break;
                    }
                    else if (elements[elementIndex] == -2)
                    {
                        Operator(new Subtract(), elementIndex);
                        break;
                    }
            }
            void Operator(Operator _operator, int elementIndex)
            {
                _operator.A = nodes[elements[elementIndex - 1]];
                _operator.B = nodes[elements[elementIndex + 1]];
                nodes.Add(_operator);
                elements.RemoveRange(elementIndex - 1, 3);
                elements.Insert(elementIndex - 1, nodes.Count - 1);
            }


            if (elements.Count != 1)
                throw new Exception("Failed to compile: " + input);


            return nodes[nodes.Count - 1];
        }


        // Operators
        class Add : Operator
        {
            public override float Value => A.Value + B.Value;
        }
        class Subtract : Operator
        {
            public override float Value => A.Value - B.Value;
        }
        class Multiply : Operator
        {
            public override float Value => A.Value * B.Value;
        }
        class Divide : Operator
        {
            public override float Value => A.Value / B.Value;
        }
        abstract class Operator : I_Node
        {
            public I_Node A, B;
            public abstract float Value { get; }
        }
        
        // Basic
        class Number : I_Node
        {
            public float Value { get; set; }
        }
        class Variable : I_Node
        {
            public double modifier;
            public VariableList<double>.Variable variable;
            public float Value => (float)variable.Value * (float)modifier;
        }
        
        // Base
        public interface I_Node
        {
            float Value { get; }
        }
        
        
        
        public static List<string> GetVariablesUsed(string valueString)
        {
            valueString = valueString.Replace(" ", "");

            List<string> output = new List<string>() { "" };

            foreach (char c in valueString)
            {
                if (char.IsLetter(c) || c == "_"[0])
                {
                    output[output.Count - 1] += c;
                }
                else
                {
                    if (output[output.Count - 1].Length > 0)
                        output.Add("");
                }
            }

            if (output[output.Count - 1] == "")
                output.RemoveAt(output.Count - 1);

            return output;
        }
    }
}
