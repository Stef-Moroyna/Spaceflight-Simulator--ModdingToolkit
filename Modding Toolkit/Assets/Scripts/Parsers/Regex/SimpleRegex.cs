using System.Text.RegularExpressions;

namespace SFS.Parsers.Regex
{
    using Regex = System.Text.RegularExpressions.Regex;

    public class SimpleRegex
    {
        public Regex Regex { get; private set; }
        public Match Match { get; private set; }


        public SimpleRegex(string regex)
        {
            Regex = new Regex(regex, RegexOptions.CultureInvariant | RegexOptions.Multiline);
        }

        public bool Input(string input)
        {
            Match = Regex.Match(input);

            return Match != null && Match.Success;
        }

        public bool Input(string input, int start)
        {
            Match = Regex.Match(input, start);

            return Match != null && Match.Success;
        }

        public Group GetGroup(string name)
        {
            foreach (Group group in Match.Groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        public bool Next()
        {
            if (!Match.Success)
                return false;


            Match = Match.NextMatch();
            return Match != null && Match.Success;
        }

        public static implicit operator SimpleRegex(Regex regex) => new SimpleRegex("") { Regex = regex };
    }
}
