using SFS.Variables;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        
        
        public static List<FieldReference> dropdownData;
        
        static Loc()
        {
            SetLanguage("Default", TranslationSerialization.CreateTranslation<SFS_Translation>());
            
            dropdownData = TranslationSerialization.GetFieldReferences<SFS_Translation>().Select(a => new FieldReference(a.Item1.Name, a.Item2.Name)).ToList();
        }
    }
}