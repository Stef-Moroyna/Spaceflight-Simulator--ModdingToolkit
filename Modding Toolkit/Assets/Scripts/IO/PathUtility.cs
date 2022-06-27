using System.Collections.Generic;
using System.Linq;

namespace SFS.IO
{
    public static class PathUtility
    {
        public static string MakeUsable(string name, string defaultName)
        {
            name = IsValidFileName(name) ? name : defaultName;
            return FilePath.CleanupName(name);
        }
        public static string AutoNameExisting(string name, FolderPath path)
        {
            name = FilePath.CleanupName(name);
            
            List<string> taken = path.GetFilesInFolder(false).Select(file => file.CleanFileName).ToList();
            taken.AddRange(path.GetFoldersInFolder(false).Select(folder => folder.FolderName));
            
            return UseUntakenName(name, taken);
        }
        public static string AutoNameExisting(string name, OrderedPathList pathList)
        {
            name = FilePath.CleanupName(name);
            return UseUntakenName(name, pathList.GetOrder());
        }
        public static string UseUntakenName(string name, List<string> taken)
        {
            string original = name;

            int i = 1;

        RETRY:
            foreach (string t in taken)
                if (t.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    name = original + " " + i;
                    i++;
                    goto RETRY;
                }

            return name;
        }

        static bool IsValidFileName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.ToCharArray().ToList().Any(c => char.IsLetterOrDigit(c) && c != '.' && !System.IO.Path.GetInvalidFileNameChars().Contains(c));
        }
    }
}