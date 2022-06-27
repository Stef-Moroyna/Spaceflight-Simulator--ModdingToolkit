using SFS.Parsers.Regex;
using System.Collections.Generic;
using System.Linq;

namespace SFS.Translations
{
    public class Field
    {
        public Dictionary<int, string> subs = new Dictionary<int, string>();

        public string GetSub(int index)
        {
            return subs.ContainsKey(index)? subs[index] : subs[0];
        }
        public (int, string)[] GetSubs()
        {
            return subs.Select(pair => (pair.Key, pair.Value)).ToArray();
        }
        public bool HasSub(int index)
        {
            return subs.ContainsKey(index);
        }
        public void SetSub(int index, string sub)
        {
            subs[index] = sub;
        }

        
        public Builder InjectField(Field subs, string variableName)
        {
            return GetBuilder(0).InjectField(subs, variableName);
        }
        public Builder Inject(string value, string variableName)
        {
            return GetBuilder(0).Inject(value, variableName);
        }
        Builder GetBuilder(int subIndex)
        {
            return new Builder(GetSub(subIndex));
        }
               

        public static implicit operator string(Field field)
        {
            return field.subs[0];
        }

        public static Field Text(string sub)
        {
            return new Field { subs = { [0] = sub } };
        }
        public static Field Subs(params string[] subs)
        {
            Field field = new Field();
            
            for (int x = 0; x < subs.Length; x++)
                field.subs[x] = subs[x];
            
            return field;
        }
        public static Field MultilineText(params string[] lines)
        {
            return Text(string.Join("\n", lines));
        }

        
        public class Builder
        {
            string translation;

            public Builder(string translation)
            {
                this.translation = translation;
            }
            
            public Builder InjectField(Field subs, string variableName)
            {
                SimpleRegex regex = new SimpleRegex("%" + variableName + @"{(?<number>\d*?)}%");

                while (regex.Input(translation))
                {
                    if (!int.TryParse(regex.GetGroup("number").Value, out int index))
                        continue;

                    string sub = subs.GetSub(index);
                    translation = translation.Remove(regex.Match.Index, regex.Match.Length);
                    translation = translation.Insert(regex.Match.Index, sub);
                }

                return Inject(subs, variableName);
            }
            public Builder Inject(string value, string variableName)
            {
                translation = translation.Replace("%" + variableName + "%", value);
                return this;
            }

            public string GetText()
            {
                return translation;
            }

            public static implicit operator string(Builder a)
            {
                return a.translation;
            }
        }
    }
}
