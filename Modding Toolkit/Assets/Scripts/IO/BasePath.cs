namespace SFS.IO
{
    public abstract class BasePath
    {
        string path;

        protected string Path
        {
            get
            {
                path = System.IO.Path.GetFullPath(path).Replace("\\", "/");

                if (path.EndsWith("/"))
                    return path.Substring(0, path.Length - 1);

                return path;
            }
            set => path = System.IO.Path.GetFullPath(value).Replace("\\", "/");
        }

        protected BasePath(string initialLocation)
        {
            Path = initialLocation;
        }

        protected string GetParentPath()
        {
            string path = this.path;

            while (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            return System.IO.Path.GetDirectoryName(path);
        }

        public override string ToString() => Path;

        public static implicit operator string(BasePath wrapper) => wrapper.path;
    }
}