using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SFS.Parsers.Json.Exclusions
{
    public class Exclusion<T> : BaseExclusion
    {
        public List<string> names;

        public Exclusion(params string[] names)
        {
            this.names = names.ToList();
        }

        public override bool Exclude(JsonProperty prop)
        {
            if (prop.DeclaringType != typeof(T))
                return false;

            return (from name in names where prop.PropertyName == name select name).ToList().Count > 0;
        }
    }
}
