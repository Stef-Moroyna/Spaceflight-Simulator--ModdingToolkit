using System.Collections.Generic;
using System.IO;

namespace SFS.IO
{
    public class FolderPath : BasePath
    {
        public string FolderName
        {
            get => new DirectoryInfo(Path).Name;
            set => Path = GetParentPath() + "/" + value;
        }

        public FolderPath(string initialLocation) : base(initialLocation)
        { }

        public FolderPath CloneAndExtend(string path)
        {
            return new FolderPath(Path + "/" + path.Replace("\\", "/"));
        }
        public FolderPath Extend(string path)
        {
            Path += "/" + path.Replace("\\", "/");
            return this;
        }
        public FolderPath CreateFolder()
        {
            if (!FolderExists())
                Directory.CreateDirectory(Path);

            return this;
        }
        public void RenameFolder(string name)
        {
            Move(new FolderPath(GetParentPath()).Extend(name));
        }
        public void DeleteFolder()
        {
            if (FolderExists())
                Directory.Delete(this, true);
        }


        public FilePath ExtendToFile(string nameWithExtension)
        {
            return new FilePath(Path + "/" + nameWithExtension.Replace("\\", "/"));
        }

        public FolderPath Parent => new FolderPath(GetParentPath());

        public string GetRelativePath(string root)
        {
            return Path.Replace(System.IO.Path.GetFullPath(root).Replace("\\", "/"), "");
        }

        public IEnumerable<FilePath> GetFilesInFolder(bool recursively)
        {
            foreach (BasePath wrapper in recursively ? EnumerateContentsRecursively() : EnumerateContents())
                if (wrapper is FilePath filePath)
                    yield return filePath;
        }
        public IEnumerable<FolderPath> GetFoldersInFolder(bool recursively)
        {
            foreach (BasePath wrapper in recursively ? EnumerateContentsRecursively() : EnumerateContents())
                if (wrapper is FolderPath folderPath)
                    yield return folderPath;
        }
        public IEnumerable<BasePath> EnumerateContents()
        {
            foreach (string directory in Directory.EnumerateDirectories(Path))
                yield return new FolderPath(directory);

            foreach (string file in Directory.EnumerateFiles(Path))
                yield return new FilePath(file);
        }
        public IEnumerable<BasePath> EnumerateContentsRecursively()
        {
            Stack<FolderPath> directories = new Stack<FolderPath>();

            foreach (FolderPath directory in GetFoldersInFolder(false))
                directories.Push(directory);

            foreach (FilePath file in GetFilesInFolder(false))
                yield return file;

            while (directories.Count > 0)
            {
                FolderPath directory = directories.Pop();
                yield return directory;

                foreach (FolderPath subDirectory in directory.GetFoldersInFolder(false))
                    directories.Push(subDirectory);

                foreach (FilePath file in directory.GetFilesInFolder(false))
                    yield return file;
            }
        }

        public FolderPath Clone()
        {
            return new FolderPath(Path);
        }

        public bool FolderExists()
        {
            return Directory.Exists(this);
        }
        public void CopyFolder(FolderPath path)
        {
            path.DeleteFolder(); // Clears new path

            foreach (FilePath file in GetFilesInFolder(true))
            {
                FolderPath tempTarget = path.Clone();

                foreach (string pathPart in file.GetParent().GetRelativePath(this).Split('/'))
                {
                    if (string.IsNullOrWhiteSpace(pathPart))
                        continue;

                    tempTarget.Extend(pathPart);
                }

                tempTarget.CreateFolder();
                FilePath targetFile = new FilePath(tempTarget.ExtendToFile(file.FileName));
                file.Copy(targetFile);
            }
        }
        public void Move(FolderPath path)
        {
            if (Path == path.Path)
                return;

            FolderPath tempPath = Clone();
            tempPath.FolderName += "_temporary_premove";
            Directory.Move(this, tempPath);
            path.DeleteFolder(); // Clears new path
            Directory.Move(tempPath, path);
        }
    }
}
