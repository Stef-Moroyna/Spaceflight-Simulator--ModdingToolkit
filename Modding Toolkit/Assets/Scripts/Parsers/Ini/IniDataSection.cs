using System;
using System.Collections.Generic;

namespace SFS.Parsers.Ini
{
    public class IniDataSection
    {
        public readonly string name;
        public string comment;
        public int whitespacesBefore;

        public readonly Dictionary<string, IniDataEnv.Value> data = new Dictionary<string, IniDataEnv.Value>();

        public IniDataSection(string name)
        {
            if (name.Contains("\n") || name.Contains("\r\n"))
                throw new Exception("Section name is invalid!");

            this.name = name;
        }

        public IniDataEnv.Value this[string index]
        {
            get => data.ContainsKey(index) ? data[index] : null;
            set => data[index] = value;
        }

        public Dictionary<string, string> Simplify()
        {
            Dictionary<string, string> simple = new Dictionary<string, string>();

            foreach (var pair in data)
                simple[pair.Key] = pair.Value;

            return simple;
        }
    }
}
