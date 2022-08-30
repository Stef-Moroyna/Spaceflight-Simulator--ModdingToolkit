using System;
using System.IO;
using System.Linq;
using SPath = System.IO.Path;

namespace SFS.IO
{
    public class FilePath : BasePath
    {
        // Setting this field does not rename on disk
        public string FileName
        {
            get => SPath.GetFileName(Path);
            set => Path = GetParentPath() + "/" + value;
        }

        // Setting this field does not rename on disk
        public string CleanFileName => SPath.GetFileNameWithoutExtension(Path);

        // Setting this field does not rename on disk
        public string Extension
        {
            get
            {
                string fileName = FileName;

                if (!fileName.Contains("."))
                    return null;

                return fileName.Split('.').Last();
            }
        }

        public FilePath(string initialLocation) : base(initialLocation)
        { }

        public void WriteText(string text)
        {
            Write(() => File.WriteAllText(this, text));
        }
        public void WriteBytes(byte[] data)
        {
            Write(() => File.WriteAllBytes(this, data));
        }
        public void AppendText(string text)
        {
            if (!FileExists())
                WriteText("");
                
            File.AppendAllText(this, text);
        }

        public byte[] ReadBytes()
        {
            return File.ReadAllBytes(this);
        }
        public string ReadText()
        {
            if (!FileExists())
                throw new Exception($"File {Path} does not exist!");

            return File.ReadAllText(this);
        }
        public StreamWriter StreamWriter
        {
            get
            {
                if (!this.FileExists())
                    this.WriteText("");
                return new StreamWriter(this.Path);
            }
        }

            public void DeleteFile()
        {
            if (FileExists())
                File.Delete(this);
        }

        public FolderPath GetParent()
        {
            return new FolderPath(GetParentPath());
        }

        public bool FileExists()
        {
            return File.Exists(this);
        }
        public void Move(FilePath path)
        {
            File.Copy(this, path, true);
            DeleteFile();
        }
        public void Copy(FilePath path)
        {
            File.Copy(this, path, true);
        }

        // Utility
        public static string CleanupName(string fileName)
        {
            foreach (char c in SPath.GetInvalidPathChars())
                fileName = fileName.Replace(c, '_');

            foreach (char c in SPath.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');

            return fileName;
        }

        void Write(Action writeAction)
        {
            if (Path == null)
                return;

            writeAction.Invoke();
        }
        
        public string GetRelativePath(string root)
        {
            return System.IO.Path.GetRelativePath(root, Path);
            return Path.Replace(System.IO.Path.GetFullPath(root).Replace("\\", "/"), "");
        }
    }
}
