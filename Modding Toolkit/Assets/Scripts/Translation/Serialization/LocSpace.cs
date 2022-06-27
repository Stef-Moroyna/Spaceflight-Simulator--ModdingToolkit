using System;

namespace SFS.Translations
{
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
}
