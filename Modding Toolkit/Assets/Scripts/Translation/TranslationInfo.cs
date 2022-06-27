using System;

namespace SFS.Translations
{
    [Serializable]
    public class FieldReference
    {       
        public string name; // Clean name, version removed
        public string group; // Clean name, sub group is stripped


        public string MenuName => group != null ? group + "/" + name : name;


        public FieldReference(string name, Group group)
        {
            this.name = name;
            this.group = group.Name;
        }
        public FieldReference(string name, string group)
        {
            this.name = name;
            this.group = group;
        }
        public override bool Equals(object obj)
        {
            if (obj is FieldReference inf)
                return name == inf.name && group == inf.group && MenuName == inf.MenuName;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (name ?? "").GetHashCode();
            hash = hash * 23 + (group ?? "").GetHashCode();
            return hash;
        }
        public static bool operator ==(FieldReference a, FieldReference b)
        {
            if (a is null)
                return b is null;
            if (b is null)
                return false;

            return a.Equals(b);
        }
        public static bool operator !=(FieldReference a, FieldReference b)
        {
            return !(a == b);
        }
    }
}
