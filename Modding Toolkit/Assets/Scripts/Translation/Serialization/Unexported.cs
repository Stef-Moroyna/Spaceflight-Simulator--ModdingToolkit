using System;

namespace SFS.Translations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class Unexported : Attribute
    { }
}
