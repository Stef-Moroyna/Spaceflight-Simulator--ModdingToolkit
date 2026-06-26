using SFS.Parsers.Ini;
using SFS.Parsers.Regex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SFS.Translations
{
    public static class TranslationSerialization
    {
        // Serialization
        public static string Serialize<T>(T translation)
        {
            List<(PropertyInfo, Group)> fieldReferences = GetFieldReferences<T>();
            foreach ((PropertyInfo propertyInfo, Group group) in fieldReferences)
            {
                if (group.subExports.Count == 0)
                    continue;

                Field field = propertyInfo.GetValue(translation) as Field;
                foreach (Func<string, string> exportFunction in group.subExports)
                    field?.SetSub(field.subs.Count, exportFunction(field));
            }
            
            IniConverter converter = new IniConverter();

            foreach ((PropertyInfo propertyInfo, Group group) in fieldReferences)
            {
                Unexported skipAttribute = propertyInfo.GetCustomAttribute<Unexported>();
                if (skipAttribute != null)
                    continue;

                FieldReference info = new FieldReference(propertyInfo.Name, group.Name);
                Field subs = propertyInfo.GetValue(translation) as Field;
                // ReSharper disable once PossibleNullReferenceException
                bool hasSubGroup = subs.subs.Count > 1 || group.hasSubs || propertyInfo.GetCustomAttributes<MarkAsSub>().Any();
                bool isFirstSub = true;

                foreach (KeyValuePair<int, string> sub in subs.subs)
                {
                    IniDataSection section = converter.GetSection(info.group);

                    IniDataEnv.Value value = new IniDataEnv.Value(sub.Value);
                    section[hasSubGroup ? info.name + "{" + sub.Key + "}" : info.name] = value;

                    LocSpace locSpaceAttribute = propertyInfo.GetCustomAttribute<LocSpace>();
                    Documentation documentationAttribute = propertyInfo.GetCustomAttribute<Documentation>();

                    if (isFirstSub)
                    {
                        isFirstSub = false;
                        if (documentationAttribute != null)
                            if (documentationAttribute.attachToGroup)
                                section.comment = documentationAttribute.comment;
                            else
                            {
                                if (documentationAttribute.afterLine)
                                    value.aftLineComment = documentationAttribute.comment;
                                else
                                    value.preLineComment = documentationAttribute.comment;
                            }

                        if (locSpaceAttribute != null)
                            if (locSpaceAttribute.attachToGroup)
                                section.whitespacesBefore = locSpaceAttribute.amount;
                            else
                                value.whitespacesBefore = locSpaceAttribute.amount;
                    }
                }
            }

            string data = converter.Serialize();
            return data;
        }

        // Deserialization
        public static SFS_Translation Deserialize(string iniText, out List<FieldReference> unused, out List<FieldReference> missing, out List<FieldReference> changed)
        {
            Dictionary<FieldReference, Field> loadedFields = ParseTranslations(iniText);
            SFS_Translation output = new SFS_Translation();

            unused = new List<FieldReference>(loadedFields.Keys);
            missing = new List<FieldReference>();
            changed = new List<FieldReference>();

            // Loops trough fields, and tries to find a field for it
            foreach ((PropertyInfo propertyInfo, Group group) in GetFieldReferences<SFS_Translation>())
            {
                FieldReference field = new FieldReference(propertyInfo.Name, group.Name);

                // Checks if field is missing
                Unexported skipAttribute = propertyInfo.GetCustomAttribute<Unexported>();
                if (!loadedFields.ContainsKey(field))
                {
                    if (skipAttribute == null)
                        missing.Add(field);
                    continue;
                }

                unused.Remove(field);

                Field toInject = loadedFields[field];
                Field output_Field = propertyInfo.GetMethod.Invoke(output, new object[0]) as Field;

                // Finds changed fields
                foreach (KeyValuePair<int, string> sub in output_Field.subs)
                    if (!toInject.HasSub(sub.Key) || sub.Value != output_Field.GetSub(sub.Key))
                        changed.Add(field);

                // Injects translation
                output_Field.subs = toInject.subs;
            }

            return output;
        }
        static Dictionary<FieldReference, Field> ParseTranslations(string iniText)
        {
            IniConverter iniConverter = new IniConverter(iniText);
            Dictionary<FieldReference, Field> translations = new Dictionary<FieldReference, Field>();

            string[] sectionNames = iniConverter.GetSectionNames();
            // Collect all translations
            for (int i = 0; i < sectionNames.Length; i++)
            {
                string sectionName = sectionNames[i];
                IniDataSection dataSection = iniConverter.GetSection(sectionName);

                string groupName = sectionName;

                foreach (KeyValuePair<string, IniDataEnv.Value> pair in dataSection.data)
                {
                    string fieldName = pair.Key;
                    int subGroup = 0;

                    // Fast string parsing instead of regex
                    int openBrace = fieldName.LastIndexOf('{');
                    if (openBrace >= 0)
                    {
                        if (fieldName[fieldName.Length - 1] != '}')
                        {
                            Debug.LogError($"Wrong Translation key syntax:{fieldName}");
                            continue;
                        }

                        string numberPart = fieldName.Substring(openBrace + 1, fieldName.Length - openBrace - 2);
                        
                        if (int.TryParse(numberPart, out subGroup))
                        {
                            fieldName = fieldName.Substring(0, openBrace);
                        }
                        // If parse fails, treat as regular field name (no modification)
                    }

                    FieldReference info = new FieldReference(fieldName, groupName);

                    Field field;
                    if (translations.ContainsKey(info))
                        field = translations[info];
                    else
                    {
                        field = new Field();
                        translations[info] = field;
                    }

                    field.SetSub(subGroup, pair.Value.value);
                }
            }

            return translations;
        }

        // Create new
        public static T CreateTranslation<T>() where T : new()
        {
            return new T();
        }
        public static SFS_Translation CreateTranslation()
        {
            SFS_Translation translation = new SFS_Translation();

            foreach ((PropertyInfo propertyInfo, Group _) in GetFieldReferences<SFS_Translation>())
                if (propertyInfo.GetValue(translation) is Field field)
                    translation.fields[propertyInfo.Name] = field;

            return translation;
        }

        // Utility
        public static List<(PropertyInfo, Group)> GetFieldReferences<T>()
        {
            List<(PropertyInfo, Group)> properties = new List<(PropertyInfo, Group)>();
            Group currentGroup = new Group("None");

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                try
                {
                    if (property.PropertyType == typeof(Field))
                    {
                        if (property.GetCustomAttribute<Group>() != null)
                            currentGroup = property.GetCustomAttribute<Group>();

                        properties.Add((property, currentGroup));
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Lang prop error: " + property.Name + "\n" + e);
                }
            }

            return properties;
        }
    }
    
    // Attributes
    public class Unexported : Attribute { }
    public class MarkAsSub : Attribute { }
    
    public class LocSpace : Attribute
    {
        public readonly int amount;
        public readonly bool attachToGroup;

        public LocSpace(int amount = 1, bool attachToGroup = false)
        {
            this.amount = amount;
            this.attachToGroup = attachToGroup;
        }
    }
    
    public class Group : Attribute
    {
        public string Name { get; }
        public bool hasSubs;
        public List<Func<string, string>> subExports = new List<Func<string, string>>();
        
        public Group(string name)
        {
            Name = name;
        }
    }
    
    public class Documentation : Attribute
    {
        public string comment;
        public bool attachToGroup;
        public bool afterLine;

        public Documentation(string comment, bool attachToGroup = false, bool afterLine = false)
        {
            this.comment = comment;
            this.attachToGroup = attachToGroup;
            this.afterLine = afterLine;
        }
    }
}

public class LLMComment : Attribute
{
    public string comment;

    public LLMComment(string comment)
    {
        this.comment = comment;
    }
}

/// <summary>
/// Give a shortened version of this word.
/// </summary>
public class LLMCommentShorten : LLMComment
{
    public LLMCommentShorten(string comment)
        : base($"If this translates with too many characters, shorten it to {comment}")
    {
    }
}