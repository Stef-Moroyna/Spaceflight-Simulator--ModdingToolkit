using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace SFS.Variables
{
    // Base values
    [Serializable] public class Vector2_Local : Obs<Vector2>
    {
        protected override bool IsEqual(Vector2 a, Vector2 b) => a == b;
    }
    [Serializable] public class Float_Local : Obs<float>
    {
        protected override bool IsEqual(float a, float b) => a == b;
    }
    [Serializable] public class Double_Local : Obs<double>
    {
        protected override bool IsEqual(double a, double b) => a == b;
    }
    [Serializable] public class Int_Local : Obs<int>
    {
        protected override bool IsEqual(int a, int b) => a == b;
    }
    [Serializable] public class Bool_Local : Obs<bool>
    {
        protected override bool IsEqual(bool a, bool b) => a == b;
    }
    [Serializable] public class String_Local : Obs<string>
    {
        protected override bool IsEqual(string a, string b) => a == b;
    }


    #pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    #pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    [Serializable, InlineProperty]
    public abstract class Obs<T>
    {
        // Variables
        [SerializeField, HideInInspector] T value;
        //
        bool hasFilter;
        Func<T, T, T> filter;
        //
        event Action onChange;
        event Action<T> onChangeNew;
        event Action<T,T> onChangeOldNew;


        // Properties
        [ShowInInspector, HideLabel, HideReferenceObjectPicker]
        public T Value
        {
            get => value;
            set
            {
                // Applies filter
                if (hasFilter)
                    value = filter.Invoke(this.value, value);

                // Check for changes
                if (IsEqual(this.value, value))
                    return;

                // Applies new value
                T oldValue = this.value;
                this.value = value;

                // Invokes events
                onChange?.Invoke();
                onChangeNew?.Invoke(value);
                onChangeOldNew?.Invoke(oldValue, value);
            }
        }
        public Func<T, T, T> Filter
        {
            set
            {
                hasFilter = true;
                filter = value;
            }
        }
        protected abstract bool IsEqual(T a, T b);

        // Registration
        public Obs<T> OnChange
        {
            get => this;
            
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        // Registration
        public static Obs<T> operator +(Obs<T> a, Action b)
        {
            b.Invoke();
            a.onChange += b;
            return a;
        }
        public static Obs<T> operator -(Obs<T> a, Action b)
        {
            a.onChange -= b;
            return a;
        }
        //
        public static Obs<T> operator +(Obs<T> a, Action<T> b)
        {
            b.Invoke(a.value);
            a.onChangeNew += b;
            return a;
        }
        public static Obs<T> operator -(Obs<T> a, Action<T> b)
        {
            a.onChangeNew -= b;
            return a;
        }
        //
        public static Obs<T> operator +(Obs<T> a, Action<T,T> b)
        {
            b.Invoke(a.value, a.value);
            a.onChangeOldNew += b;
            return a;
        }
        public static Obs<T> operator -(Obs<T> a, Action<T,T> b)
        {
            a.onChangeOldNew -= b;
            return a;
        }


        // Value cast
        public static implicit operator T(Obs<T> a) => a.Value;


        // Prevents use of wrapper
        [Obsolete("Use Value instead", true)] public static bool operator ==(Obs<T> a, Obs<T> b) => throw new Exception("Use variable.Value");
        [Obsolete("Use Value instead", true)] public static bool operator !=(Obs<T> a, Obs<T> b) => throw new Exception("Use variable.Value");
    }
    #pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    #pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)

    [Serializable]
    public class Event_Local
    {
        event Action onChange;

        // Registration
        public static Event_Local operator +(Event_Local a, Action b)
        {
            b.Invoke();
            a.onChange += b;
            return a;
        }
        public static Event_Local operator -(Event_Local a, Action b)
        {
            a.onChange -= b;
            return a;
        }

        // Invoke
        public void Invoke()
        {
            onChange?.Invoke();
        }
    }
}