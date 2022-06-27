using SFS.Variables;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class OrientationModule : MonoBehaviour, I_InitializePartModule
    {
        [SerializeField] public Orientation_Local orientation = new Orientation_Local() { Value = new Orientation(1, 1, 0) };

        int I_InitializePartModule.Priority => 11;
        void I_InitializePartModule.Initialize() => orientation.OnChange += ApplyOrientation;


        public void ApplyOrientation()
        {
            transform.localScale = new Vector3(orientation.Value.x, orientation.Value.y, 1);
            transform.localEulerAngles = new Vector3(0, 0, orientation.Value.z);
        }

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
}