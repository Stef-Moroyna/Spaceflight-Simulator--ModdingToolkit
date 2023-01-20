using SFS.Variables;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class OrientationModule : MonoBehaviour
    {
        [SerializeField] public Orientation_Local orientation = new Orientation_Local() { Value = new Orientation(1, 1, 0) };

        public static Vector2 operator *(Vector2 a, OrientationModule orientation)
        {
            return Quaternion.Euler(0, 0, orientation.orientation.Value.z) * new Vector2(a.x * orientation.orientation.Value.x, a.y * orientation.orientation.Value.y);
        }
        public static Vector3 operator *(Vector3 a, OrientationModule orientation)
        {
            return Quaternion.Euler(0, 0, orientation.orientation.Value.z) * new Vector3(a.x * orientation.orientation.Value.x, a.y * orientation.orientation.Value.y, 0);
        }
    }

    [Serializable]
    public class Orientation_Local : Obs<Orientation>
    {
        protected override bool IsEqual(Orientation a, Orientation b)
        {
            return a == b || a != null && b != null && a.x == b.x && a.y == b.y && a.z == b.z;
        }
    }
    
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