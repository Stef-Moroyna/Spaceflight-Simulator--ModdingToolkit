using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Translations
{
    [Serializable, InlineProperty]
    public class TranslationVariable
    {
        [SerializeField, HorizontalGroup, HideLabel] bool plainText;
        [ShowIf(nameof(plainText)), SerializeField, HorizontalGroup, HideLabel] string TranslatableName;

        // Editor
        #if UNITY_EDITOR
        [CustomValueDrawer(nameof(Dropdown)), Required, ShowInInspector, HideIf(nameof(plainText)), HorizontalGroup]
        #endif
        public Field Field
        {
            get
            {
                if (plainText)
                {
                    if (field.subs[0] == "NULL")
                        field = Field.Text(TranslatableName);
                }
                else
                    Loc.fields.TryGetValue(TranslatableName, out field);

                return field;
            }
            set => field = value;
        }

        Field field = Field.Text("NULL");

        public TranslationVariable() {}
        public TranslationVariable(Field field) => this.field = field;

        // Editor
        #if UNITY_EDITOR
        public Field Dropdown(Field input, GUIContent content)
        {
            List<FieldReference> keys = Loc.dropdownData;
            int selected = keys.FindIndex(key => key.name == TranslatableName);

            if (selected == -1)
                selected = 0;

            string[] options = keys.Select(key => key.MenuName).ToArray();
            selected = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown("", selected, options);

            field = Loc.fields[keys[selected].name];
            TranslatableName = keys[selected].name;
            return field;
        }
        #endif
    }
}