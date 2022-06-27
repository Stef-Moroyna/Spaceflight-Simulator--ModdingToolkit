using Newtonsoft.Json.Serialization;

namespace SFS.Parsers.Json.Exclusions
{
    public abstract class BaseExclusion
    {
        public abstract bool Exclude(JsonProperty prop);
    }
}
