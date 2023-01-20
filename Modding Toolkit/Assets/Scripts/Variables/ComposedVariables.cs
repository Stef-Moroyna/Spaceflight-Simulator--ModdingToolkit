using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using SFS.Parts.Modules;
using SFS.Parsers.Constructed;
using System.Globalization;

namespace SFS.Variables
{
    [Serializable, InlineProperty, PropertySpace(0, 2)]
    public class MinMaxRange
    {
        [HorizontalGroup, LabelWidth(30)] public Composed_Float min, max;
    }
    [Serializable, InlineProperty, PropertySpace(0, 2)]
    public class SizeRange
    {
        [HorizontalGroup, LabelWidth(30)] public Composed_Float start, size;
    }

    // Composed composites
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)] public class Composed_Pipe : Composed<Pipe>
    {
        public List<Composed_PipePoint> points;

        protected override Pipe GetResult(bool initialize)
        {
            if (initialize)
            {
                foreach (Composed_PipePoint point in points)
                    point.OnChange += Recalculate;
            }
            
            Pipe a = new Pipe();

            foreach (Composed_PipePoint point in points)
                a.AddPoint(point.Value.position, point.Value.width);

            return a;
        }

        protected override bool Equals(Pipe a, Pipe b) => false;
    }
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)] public class Composed_PipePoint : Composed<PipePoint>
    {
        public Composed_Vector2 position, width;

        protected override PipePoint GetResult(bool initialize)
        {
            if (initialize)
            {
                position.OnChange += Recalculate;
                width.OnChange += Recalculate;
            }
            
            return new PipePoint(position.Value, width.Value, float.NaN, float.NaN, float.NaN);
        }

        protected override bool Equals(PipePoint a, PipePoint b) => false;

        public Composed_PipePoint(Vector2 position, Vector2 width)
        {
            this.position = new Composed_Vector2(position);
            this.width = new Composed_Vector2(width);
        }
    }
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)]
    public class Composed_Rect : Composed<Rect>
    {
        [LabelWidth(100)] public Composed_Vector2 position, size;

        protected override Rect GetResult(bool initialize)
        {
            if (initialize)
            {
                position.OnChange += Recalculate;
                size.OnChange += Recalculate;
            }
            
            return new Rect(position.Value, size.Value);
        }

        protected override bool Equals(Rect a, Rect b) => false;
    }
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)] public class Composed_Vector2 : Composed<Vector2>
    {
        [LabelWidth(15), HorizontalGroup] public Composed_Float x, y;

        protected override Vector2 GetResult(bool initialize)
        {
            if (initialize)
            {
                x.OnChange += Recalculate;
                y.OnChange += Recalculate;
            }
            
            return new Vector2(x.Value, y.Value);
        }

        protected override bool Equals(Vector2 a, Vector2 b) => a == b;

        public Composed_Vector2(Vector2 a)
        {
            x = new Composed_Float(a.x.ToString(CultureInfo.InvariantCulture));
            y = new Composed_Float(a.y.ToString(CultureInfo.InvariantCulture));
        }

        public void Offset(Vector2 offset)
        {
            x.Offset(offset.x);
            y.Offset(offset.y);
        }
    }

    // Base composites
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)] public class Composed_Float : Composed<float>
    {
        [HideLabel, HorizontalGroup] public string input;
        Compute.I_Node compiled;

        protected override float GetResult(bool initialize)
        {
            if (initialize)
            {
                compiled = Compute.Compile(input, variables, out List<string> usedVariables);

                foreach (string usedVariable in usedVariables)
                    variables.doubleVariables.RegisterOnVariableChange(Recalculate, usedVariable);
            }

            if (compiled == null)
                return Compute.Compile(input, variables, out _).Value;
                
            return compiled.Value;
        }

        protected override bool Equals(float a, float b) => a == b;

        public Composed_Float(string a)
        {
            input = a;

            if (input.Contains("E"))
                input = "0";
        }
        public void Offset(float offset)
        {
            if (Compute.GetVariablesUsed(input).Count > 0)
                return;
            
            float newValue = Compute.Compile(input, variables, out _).Value + offset;
            input = newValue.ToString().Contains("E")? "0" : newValue.ToString(CultureInfo.InvariantCulture);
        }
    }
    [Serializable, InlineProperty, HideReferenceObjectPicker, PropertySpace(0, 2)] public class Composed_Double : Composed<double>
    {
        [HideLabel, HorizontalGroup] public string input;
        Compute.I_Node compiled;
        
        protected override double GetResult(bool initialize)
        {
            if (initialize)
            {
                compiled = Compute.Compile(input, variables, out List<string> usedVariables);

                foreach (string usedVariable in usedVariables)
                    variables.doubleVariables.RegisterOnVariableChange(Recalculate, usedVariable);
            }

            if (compiled == null)
                return Compute.Compile(input, variables, out _).Value;
            
            return compiled.Value;
        }

        protected override bool Equals(double a, double b) => a == b;
    }

    // Abstract/Generic base
    [Serializable]
    public abstract class Composed<T>
    {
        [CustomValueDrawer("GetVariablesReference"), ShowInInspector, SerializeField] protected VariablesModule variables;

        #if UNITY_EDITOR
        public VariablesModule GetVariablesReference(VariablesModule a, GUIContent content) => (a == null && UnityEditor.Selection.transforms.Length == 1) ? UnityEditor.Selection.transforms[0].GetComponentInParentTree<VariablesModule>() : a;
        #endif

        bool initialized;

        T value;
        event Action onChange;
        event Action<T,T> onChangeOldNew;


        public T Value
        {
            get
            {
                CheckInitialize();
                return value;
            }
            set
            {
                if (Equals(this.value, value))
                    return;
                
                T valueOld = this.value;
                this.value = value;

                onChange?.Invoke();
                onChangeOldNew?.Invoke(valueOld, value);
            }
        }


        // Registration
        public Composed<T> OnChange
        {
            get => this;
            set {}
        }
        //
        public static Composed<T> operator +(Composed<T> a, Action b)
        {
            a.CheckInitialize();
            
            b.Invoke();
            a.onChange += b;
            
            return a;
        }
        public static Composed<T> operator -(Composed<T> a, Action b)
        {
            a.onChange -= b;
            return a;
        }
        //
        public static Composed<T> operator +(Composed<T> a, Action<T,T> b)
        {
            a.CheckInitialize();
            
            b.Invoke(a.Value, a.Value);
            a.onChangeOldNew += b;
            
            return a;
        }
        public static Composed<T> operator -(Composed<T> a, Action<T,T> b)
        {
            a.onChangeOldNew -= b;
            return a;
        }


        void CheckInitialize()
        {
            if (initialized)
                return;

            initialized = Application.isPlaying;
            Value = GetResult(initialized);
        }

        protected void Recalculate()
        {
            Value = GetResult(false);
        }
        
        protected abstract T GetResult(bool initialize);
        protected abstract bool Equals(T a, T b);
    }
}