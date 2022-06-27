using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.Parsers.Ini.IniDataEnv;

namespace SFS.Parsers.Ini
{
    public class IniConverter
    {
        public IniDataEnv data = new IniDataEnv();
        public IniConverter()
        {
            data = new IniDataEnv();
        }
        public IniConverter(string iniText)
        {
            LoadIni(iniText);
        }
        
        public IniDataSection GetSection(string section)
        {
            if (!data.sections.ContainsKey(section))
                data.sections[section] = new IniDataSection(section);

            return data.sections[section];
        }

        /* [Group Name/Section Name]
         * # PreComment
         * Key=Value # AftComment
         */
        public void LoadIni(string iniText)
        {
            string[] iniLines = iniText.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            IniDataSection section = data.Global;
            int whitespacesBefore = 0;
            StringBuilder preCommentBuilder = new StringBuilder();

            bool ReadComment(StringReader reader, out string comment)
            {
                string[] commentPrefixes = { ";", "//", "#" };
                StringBuilder commentBuilder = new StringBuilder();

                foreach (string commentPrefix in commentPrefixes)
                    if (reader.Peek(commentPrefix.Length) == commentPrefix)
                    {
                        reader.Read(commentPrefix.Length);
                        while (!reader.IsAtEnd())
                            commentBuilder.Append(reader.Read());

                        comment = commentBuilder.ToString();
                        return true;
                    }

                comment = null;
                return false;
            }
            bool ReadKey(StringReader reader, out string keyName)
            {
                StringReader splittedReader = reader.Split();
                StringBuilder keyNameBuilder = new StringBuilder();

                while (!splittedReader.IsAtEnd())
                {
                    if (splittedReader.Peek() == '=')
                    {
                        splittedReader.Read();
                        reader.Merge(splittedReader);
                        keyName = keyNameBuilder.ToString();
                        return true;
                    }

                    keyNameBuilder.Append(splittedReader.Read());
                }

                keyName = null;
                return false;
            }
            bool ReadValue(StringReader reader, out string value, out string aftComment)
            {
                aftComment = null;
                StringBuilder valueBuilder = new StringBuilder();

                while (!reader.IsAtEnd())
                {
                    if (ReadComment(reader, out aftComment))
                        break;

                    valueBuilder.Append(reader.Read());
                }

                value = valueBuilder.ToString();
                return value.Length > 0;
            }
            bool ReadSection(StringReader reader, out string sectionName)
            {
                StringBuilder sectionNameBuilder = new StringBuilder();

                if (reader.Peek() == '[')
                {
                    reader.Read();
                    while (reader.Peek() != ']')
                        sectionNameBuilder.Append(reader.Read());

                    sectionName = sectionNameBuilder.ToString();
                    return true;
                }

                sectionName = null;
                return false;
            }

            foreach (string iniLine in iniLines)
            {
                string trimmedLine = iniLine.Trim();
                if (string.IsNullOrWhiteSpace(iniLine))
                {
                    whitespacesBefore++;
                    continue;
                }

                StringReader reader = new StringReader(trimmedLine);

                if (ReadComment(reader, out string preComment))
                    preCommentBuilder.AppendLine(preComment);
                else if (ReadSection(reader, out string sectionName))
                {
                    section = data.GetSection(sectionName);

                    section.whitespacesBefore = whitespacesBefore;
                    whitespacesBefore = 0;

                    if (preCommentBuilder.Length > 0)
                        section.comment = preCommentBuilder.ToString();
                    preCommentBuilder.Clear();
                }
                else if (ReadKey(reader, out string keyName))
                {
                    ReadValue(reader, out string value, out string aftComment);
                    if (section.data.ContainsKey(keyName))
                    {
                        section[keyName].value += "\n" + value;
                        continue;
                    }

                    Value dataValue = new Value(value);
                    dataValue.aftLineComment = aftComment;

                    dataValue.whitespacesBefore = whitespacesBefore;
                    whitespacesBefore = 0;

                    if (preCommentBuilder.Length > 0)
                        dataValue.preLineComment = preCommentBuilder.ToString();
                    preCommentBuilder.Clear();

                    section[keyName] = dataValue;
                }
            }
        }
        public string Serialize()
        {
            StringBuilder iniTextBuilder = new StringBuilder();
            int whitelines = 0;

            void AppendWhiteLine()
            {
                whitelines++;
                iniTextBuilder.AppendLine();
            }
            void EnsureWhitelines(int amount)
            {
                int toAdd = Math.Max(0, amount - whitelines);
                for (int x = 0; x < toAdd; x++)
                    AppendWhiteLine();
            }

            void Append(string txt, bool clearWhitelines)
            {
                whitelines = clearWhitelines ? 0 : whitelines;
                iniTextBuilder.Append(txt);
            }
            void AppendLine(string txt, bool clearWhitelines)
            {
                Append(txt + "\n", clearWhitelines);
            }

            void AppendComment(string comment, bool canUseWhiteline)
            {
                if (comment == null)
                    return;

                if (whitelines == 0 && canUseWhiteline)
                    EnsureWhitelines(1);

                foreach (string commentLine in comment.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None))
                    AppendLine("# " + commentLine, false);
            }
            void AppendAftComment(string aftComment)
            {
                if (aftComment != null)
                    AppendComment(aftComment, false);
                else
                    AppendLine("", true);
            }

            void AppendDataLine(string keyName, Value value, bool canUseInitialWhiteline)
            {
                void AppendDataValue(string val) => Append("    " + keyName + "=" + val, true);

                if (value.whitespacesBefore != 0 && canUseInitialWhiteline)
                    EnsureWhitelines(value.whitespacesBefore);

                AppendComment(value.preLineComment, canUseInitialWhiteline);

                if (value.value.Contains("\n"))
                {
                    EnsureWhitelines(1);

                    string[] lines = value.value.Split('\n');
                    for (int x = 0; x < lines.Length; x++)
                        AppendDataValue(lines[x] + (x < lines.Length - 1 ? "\n" : ""));

                    AppendAftComment(value.aftLineComment);
                    AppendWhiteLine();
                }
                else
                {
                    AppendDataValue(value.value);
                    AppendAftComment(value.aftLineComment);
                }
            }
            void AppendSection(IniDataSection section)
            {
                AppendComment(section.comment, true);

                if (iniTextBuilder.Length > 0) // Comment creates a whiteline
                    EnsureWhitelines(1);

                AppendLine($"[{section.name}]", true);

                KeyValuePair<string, Value>[] dataList = section.data.ToArray();
                for (int x = 0; x < section.data.Count; x++)
                {
                    KeyValuePair<string, Value> pair = dataList[x];
                    AppendDataLine(pair.Key, pair.Value, x > 0);
                }
            }

            data.sections.ForEach(section => AppendSection(section.Value));
            return iniTextBuilder.ToString();
        }

        public string[] GetSectionNames() => new List<IniDataSection>(data.sections.Values).ConvertAll(data => data.name).ToArray();

        class StringReader
        {
            public readonly string input;
            public int pos;

            public StringReader(string input)
            {
                this.input = input;
            }

            public bool IsAtEnd()
            {
                return pos >= input.Length;
            }

            public char Read()
            {
                return input[pos++];
            }
            public string Read(int length)
            {
                length = Math.Min(input.Length - pos, length);
                string read = input.Substring(pos, length);
                pos += length;
                return read;
            }
            public char Peek()
            {
                return input[pos];
            }
            public string Peek(int length)
            {
                return input.Substring(pos, Math.Min(input.Length - pos, length));
            }

            public StringReader Split()
            {
                return new StringReader(input)
                {
                    pos = pos
                };
            }
            public void Merge(StringReader reader)
            {
                pos = reader.pos;
            }
        }
    }
}
