using System;
using UnityEngine;

namespace SFS.World
{
    [Serializable]
    public class Joint<T> : IEquatable<Joint<T>> where T : class, I_JointNode
    {
        public T a;
        public T b;
        public Vector2 anchor;

        // Constructs a joint from the provided values
        public Joint(T a, T b, Vector2 anchor)
        {
            this.a = a;
            this.b = b;
            this.anchor = anchor;
        }
        
        public bool IsSoft => a.IsSoft || b.IsSoft;
        public bool IsHard => !IsSoft;
        public bool ShouldDetach => a.ShouldDetach(b) || b.ShouldDetach(a);

        // Returns true if joints directly connect
        public static bool Connected(Joint<T> jointA, Joint<T> jointB) => jointA.a.Equals(jointB.a) || jointA.a.Equals(jointB.b) || jointB.a.Equals(jointA.b) || jointB.b.Equals(jointA.b);

        // Returns true if joints connect
        public bool IsConnectedTo(T obj) => obj.Equals(a) || obj.Equals(b);

        // Returns true if this joint is the same as the other joint
        bool IEquatable<Joint<T>>.Equals(Joint<T> other) => other.a.Equals(a) && other.b.Equals(b);

        // Returns the hash code of this joint
        public override int GetHashCode() => a.GetHashCode() ^ b.GetHashCode();

        // Returns true if obj is this joint
        public override bool Equals(object obj)
        {
            if (!(obj is Joint<T>))
                return false;

            Joint<T> other = (Joint<T>)obj;
            return other.a == a && other.b == b;
        }

        // Returns the object connected to this joint that is not the provided object
        public T GetOtherObject(T obj) => obj.Equals(a) ? b : obj.Equals(b) ? a : throw new Exception("Provided object is not linked to joint!");

        // Returns anchor relative to the object linked by this joint
        public Vector2 GetRelativeAnchor(T obj) => obj.Equals(a) ? anchor : obj.Equals(b) ? -anchor : throw new Exception("Provided object is not linked to joint!");
    }
}