using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFS.Parsers.Json
{
    public class MainContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            List<JsonProperty> includedProperties = new List<JsonProperty>();

            foreach (JsonProperty prop in properties.ToArray())
            {
                if (!prop.Writable)
                    continue;

                if (!ExclusionList.Exclude(prop))
                    includedProperties.Add(prop);
            }

            return includedProperties;
        }
    }
}