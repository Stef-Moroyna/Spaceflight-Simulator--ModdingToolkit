using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace SFS
{
    [Serializable, InlineProperty]
    public class Orientation
    {
        [HorizontalGroup, HideLabel] public float x = 1;
        [HorizontalGroup, HideLabel] public float y = 1;
        [HorizontalGroup, HideLabel] public float z;

        public Orientation(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool InversedAxis() // Are the axis or rotation reversed
        {
            return Mathf.Abs(z % 180) > 0.001f;
        }
        
        public static Vector2 operator *(Vector2 a, Orientation orientation)
        {
            return Quaternion.Euler(0, 0, orientation.z) * new Vector2(a.x * orientation.x, a.y * orientation.y);
        }
        public static Orientation operator +(Orientation a, Orientation change)
        {
            return new Orientation(change.x < 0? -a.x : a.x, change.y < 0? -a.y : a.y, change.z + a.z);
        }

        public Orientation GetCopy()
        {
            return new Orientation(x, y, z);
        }
    }
}