using System.Collections.Generic;

namespace SFS.Parsers.Ini
{
    public class IniDataEnv
    {
        public readonly Dictionary<string, IniDataSection> sections = new Dictionary<string, IniDataSection>();
        public readonly IniDataSection Global = new IniDataSection("Globals");

        public IniDataSection GetSection(string name)
        {
            if (sections.ContainsKey(name))
                return sections[name];
            else
            {
                var sec = new IniDataSection(name);
                sections[name] = sec;
                return sec;
            }
        }

        public Value this[string name]
        {
            get => Global[name];
            set => Global[name] = value;
        }
        public Value this[string name, string section]
        {
            get => GetSection(section)[name];
            set => GetSection(section)[name] = value;
        }

        public class Value
        {
            public string value;
            public string preLineComment;
            public string aftLineComment;
            public int whitespacesBefore;

            public Value(string value)
            {
                this.value = value;
            }

            public static implicit operator string(Value value) => value.value;
            public static implicit operator Value(string value) => new Value(value);
        }
    }
}
