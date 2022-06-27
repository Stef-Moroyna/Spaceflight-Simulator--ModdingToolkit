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
                    field?.SetSub(field.GetSubs().Length, exportFunction(field));
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
                bool hasSubGroup = subs.GetSubs().Length > 1 || group.hasSubs || propertyInfo.GetCustomAttributes<MarkAsSub>().Any();
                bool isFirstSub = true;

                foreach ((int, string) sub in subs.GetSubs())
                {
                    IniDataSection section = converter.GetSection(info.group);

                    IniDataEnv.Value value = new IniDataEnv.Value(sub.Item2);
                    section[hasSubGroup ? info.name + "{" + sub.Item1 + "}" : info.name] = value;

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
                foreach ((int, string) sub in output_Field.GetSubs())
                    if (!toInject.HasSub(sub.Item1) || sub.Item2 != output_Field.GetSub(sub.Item1))
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

            SimpleRegex subGroupRegex = new SimpleRegex(@"(?<name>.*){(?<number>\d*)}");

            // Collect all translations
            foreach (string sectionName in iniConverter.GetSectionNames())
            {
                IniDataSection dataSection = iniConverter.GetSection(sectionName);

                string groupName = sectionName;

                foreach (KeyValuePair<string, IniDataEnv.Value> pair in dataSection.data)
                {
                    FieldReference info = new FieldReference(pair.Key, groupName);

                    int subGroup = 0;

                    if (subGroupRegex.Input(info.name))
                    {
                        info.name = subGroupRegex.GetGroup("name").Value;

                        if (!int.TryParse(subGroupRegex.GetGroup("number").Value, out subGroup))
                            continue;
                    }

                    Field field = new Field();

                    if (translations.ContainsKey(info))
                        field = translations[info];
                    else
                        translations[info] = field;

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
}