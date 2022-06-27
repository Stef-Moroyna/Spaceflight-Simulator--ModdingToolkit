using System;

namespace SFS.Translations
{
    public class SubExport
    {
        public const string Placeholder = "placeholder";
        public const string Lowercase = "lowercase";
        public const string Multiplication_S = "multi_s";

        public Func<string, string> translationModifier;

        SubExport()
        { }

        public static SubExport CreateExport(string name)
        {
            switch (name)
            {
                case Lowercase:
                    return new SubExport()
                    {
                        translationModifier = sub => sub.ToLower()
                    };

                case Placeholder:
                    return new SubExport()
                    {
                        translationModifier = sub => "Empty sub"
                    };
                case Multiplication_S:
                    return new SubExport()
                    {
                        translationModifier = sub => sub + "s"
                    };
                default:
                    return new SubExport()
                    {
                        translationModifier = sub => "unknown export"
                    };
            }
        }
    }
}