using System;
using System.Collections.Generic;
using System.IO;
using SFS.Parsers.Json;

namespace SFS.IO
{
    public class OrderedPathList
    {
        const string OrderFileName = "display_order.ordr";
        FolderPath root;
        List<string> fileNames;

        public OrderedPathList(FolderPath root, BasePath[] paths)
        {
            this.root = root;

            // Loads
            fileNames = new List<string>();
            if (JsonWrapper.TryLoadJson(root.ExtendToFile(OrderFileName), out List<string> loadedFileNames))
                foreach (string name in loadedFileNames.ToArray())
                    if (root.ExtendToFile(name).FileExists() || root.CloneAndExtend(name).FolderExists()) // Removes non existent
                        fileNames.Add(name);

            // Sorts paths by date
            Array.Sort(paths, (a, b) => GetCreationTime(a).CompareTo(GetCreationTime(b)));
            DateTime GetCreationTime(BasePath a)
            {
                if (a is FilePath file1)
                    return new FileInfo(file1).CreationTime;
                if (a is FolderPath dir1)
                    return new DirectoryInfo(dir1).CreationTime;
                
                return new DateTime();
            }
            
            // Adds missing file names
            foreach (BasePath path in paths)
            {
                string name;

                if (path is FilePath file)
                    name = file.FileName;
                else if (path is FolderPath folder)
                    name = folder.FolderName;
                else
                    continue;

                if (!fileNames.Contains(name))
                    fileNames.Add(name);
            }
        }

        public List<string> GetOrder()
        {
            List<string> paths = new List<string>();

            foreach (string name in fileNames)
            {
                FilePath file = root.ExtendToFile(name);
                FolderPath folder = root.CloneAndExtend(name);

                if (file.FileExists())
                    paths.Add(file.CleanFileName);
                else if (folder.FolderExists())
                    paths.Add(folder.FolderName);
            }

            return paths;
        }

        public void Rename(string oldName, string newName)
        {
            if (!fileNames.Contains(oldName))
                return;

            int index = fileNames.IndexOf(oldName);
            fileNames[index] = newName;
            SaveOrder();
        }
        public void Move(string name, int newIndex)
        {
            if (!fileNames.Contains(name))
                return;

            fileNames.Remove(name);
            fileNames.Insert(newIndex, name);
            SaveOrder();
        }
        public void Remove(string name)
        {
            fileNames.RemoveAll(fileName => fileName == name);
            SaveOrder();
        }
        
        void SaveOrder()
        {
            JsonWrapper.SaveAsJson(root.ExtendToFile(OrderFileName), fileNames, false);
        }
    }
}