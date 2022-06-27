using System;

namespace SFS.Translations
{
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