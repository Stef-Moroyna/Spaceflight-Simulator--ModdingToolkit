using System;
using SFS.Parsers.Regex;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SFS.Translations
{
    public class Field
    {
        public Dictionary<int, string> subs = new Dictionary<int, string>();

        public string GetSub(int index)
        {
            return subs.ContainsKey(index)? subs[index] : subs[0];
        }
        public bool HasSub(int index)
        {
            return subs.ContainsKey(index);
        }
        public void SetSub(int index, string sub)
        {
            subs[index] = sub;
        }

        public Builder InjectField(TranslationVariable subs, string variableName, bool formatCapitalization = false)
        {
            return InjectField(subs.Field, variableName, formatCapitalization);
        }
        public Builder InjectField(Field subs, string variableName, bool formatCapitalization = false)
        {
            return GetBuilder(0).InjectField(subs, variableName, formatCapitalization);
        }
        public Builder InjectPlanet(Field subs)
        {
            return GetBuilder(0).InjectField(subs, "planet", false);
        }
        public Builder Inject(string value, string variableName, bool formatCapitalization = false)
        {
            return GetBuilder(0).Inject(value, variableName, formatCapitalization);
        }
        Builder GetBuilder(int subIndex)
        {
            return new Builder(GetSub(subIndex));
        }

        public static implicit operator string(Field field)
        {
            if (field == null)
                return string.Empty;

            return field.ToString();
        }

        public override string ToString()
        {
            if (subs.Count == 0)
                return string.Empty;

            return subs[0];
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
            static readonly StringBuilder stringBuilder = new StringBuilder();
            string translation;

            public Builder(string translation)
            {
                this.translation = translation;
            }
            
            public Builder InjectField(Field subs, string variableName, bool formatCapitalization = false)
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

                return Inject(subs, variableName, formatCapitalization);
            }
            public Builder Inject(string value, string variableName, bool formatCapitalization = false)
            {
                string variableKey = $"%{variableName}%";

                // Check if we need to (de)capitalize the value
                if (formatCapitalization && !string.IsNullOrEmpty(value) && value.Length > 2)
                {
                    if (ShouldCapitalizeWord(translation, variableKey))
                        value = char.ToUpper(value[0]) + value.Substring(1);
                    else
                        value = value.ToLower();
                }

                translation = translation.Replace(variableKey, value);
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

            public static bool ShouldCapitalizeWord(string text, string variableName)
            {
                if (string.IsNullOrEmpty(text) || !text.Contains(variableName))
                    return false;

                int wordIndex = text.IndexOf(variableName);

                // Case 1: %word% is at the very beginning of the text
                if (wordIndex == 0)
                    return true;

                // Case 2: %word% is at the beginning of a sentence
                // Check if the character before %word% is a sentence terminator followed by whitespace
                if (wordIndex >= 2)
                {
                    char charBefore = text[wordIndex - 1];
                    char charBeforeThat = text[wordIndex - 2];

                    // Check for ". ", "! ", "? ", or "\n" patterns
                    if ((charBefore == ' ' && IsSentenceTerminator(charBeforeThat)) ||
                        (charBefore == '\n' && IsSentenceTerminator(charBeforeThat)) ||
                        charBefore == '\n')
                    {
                        return true;
                    }
                }

                // Case 3: %word% is after a newline (start of a new line/paragraph)
                if (wordIndex > 0 && text[wordIndex - 1] == '\n')
                    return true;

                return false;
            }
        }
        private static bool IsSentenceTerminator(char c)
        {
            return c == '.' || c == '!' || c == '?';
        }
    }
}
