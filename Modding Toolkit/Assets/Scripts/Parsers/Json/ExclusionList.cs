using Newtonsoft.Json.Serialization;
using SFS.Parsers.Json.Exclusions;
using System.Linq;
using UnityEngine;

namespace SFS.Parsers.Json
{
    public class ExclusionList
    {
        public static readonly BaseExclusion[] exclusions = new BaseExclusion[]
        {
            new Inclusion<Vector2>("x","y"),
            new Inclusion<Color>("r","g","b","a"),
            new Inclusion<Double3>("x", "y", "z")
        };
        

        // 
        /// If propery has to be exluded function returns null.
        /// <para>Used for optimizing with PLINQ</para>
        /// 
        public static bool Exclude(JsonProperty prop)
        {
            var res = from ex in exclusions
                      where ex.Exclude(prop)
                      select ex;

            return res.Count() > 0;
        }
    }
}
