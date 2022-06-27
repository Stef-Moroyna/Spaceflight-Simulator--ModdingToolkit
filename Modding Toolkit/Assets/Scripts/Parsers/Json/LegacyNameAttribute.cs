using System;

namespace SFS.Parsers.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LegacyNameAttribute : Attribute
    {
        public string legacyName;

        public LegacyNameAttribute(string legacyName)
        {
            this.legacyName = legacyName;
        }
    }
}
