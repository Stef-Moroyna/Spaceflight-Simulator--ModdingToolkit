using SFS.Variables;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SFS.Translations
{
    public static class Loc
    {
        public static string languageName;
        public static SFS_Translation main;
        public static Dictionary<string, Field> fields;
        
        public static Event_Local OnChange = new Event_Local();

        public static void SetLanguage(string newLanguageName, SFS_Translation language)
        {
            if (newLanguageName == languageName)
                return;
            languageName = newLanguageName;
            
            
            main = language;
            
            fields = new Dictionary<string, Field>();
            foreach ((PropertyInfo propertyInfo, Group _) in TranslationSerialization.GetFieldReferences<SFS_Translation>())
                if (propertyInfo.GetValue(main) is Field field)
                    fields[propertyInfo.Name] = field;

            OnChange?.Invoke();
        }
        
        
        #if UNITY_EDITOR
        public static List<FieldReference> dropdownData;
        #endif
        
        static Loc()
        {
            SetLanguage("Default", TranslationSerialization.CreateTranslation<SFS_Translation>());
            
            #if UNITY_EDITOR
            dropdownData = TranslationSerialization.GetFieldReferences<SFS_Translation>().Select(a => new FieldReference(a.Item1.Name, a.Item2.Name)).ToList();
            #endif
        }
    }
}