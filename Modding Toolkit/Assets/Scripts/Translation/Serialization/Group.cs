using System;
using System.Collections.Generic;

namespace SFS.Translations
{
    public class Group : Attribute
    {
        public string Name { get; }
        public bool hasSubs;
        public List<Func<string, string>> subExports = new List<Func<string, string>>();


        public Group(string name)
        {
            Name = name;
        }

        public Group(string name, params string[] exportNames) : this(name)
        {
            foreach (string exportName in exportNames)
                subExports.Add(SubExport.CreateExport(exportName).translationModifier);
        }
    }
}