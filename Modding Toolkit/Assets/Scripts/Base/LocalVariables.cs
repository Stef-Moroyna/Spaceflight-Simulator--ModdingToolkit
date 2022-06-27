using Sirenix.OdinInspector;
using System;

namespace SFS.Variables
{
    // Reference to destroyable
    [Serializable, InlineProperty]
    public abstract class Obs_Destroyable<T> : Obs<T> where T : I_ObservableMonoBehaviour
    {
        public new T Value
        {
            get => base.Value;

            set
            {
                T oldValue = base.Value;
                base.Value = value;

                if (oldValue != null)
                    oldValue.OnDestroy -= OnDestroy; // Unregisters from old value

                if (value != null)
                    value.OnDestroy += OnDestroy; // Registers to new value
            }
        }

        void OnDestroy()
        {
            Value = default;
        }
    }

    // Destroyable interface
    public interface I_ObservableMonoBehaviour
    {
        Action OnDestroy { get; set; }
    }
    
    //
    [Serializable]
    public class Double2_Local : Obs<Double2>
    {
        protected override bool IsEqual(Double2 a, Double2 b) => a == b;
    }
    [Serializable]
    public class Double3_Local : Obs<Double3>
    {
        protected override bool IsEqual(Double3 a, Double3 b) => a == b;
    }
}