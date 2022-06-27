namespace SFS.IO
{
#pragma warning disable CS0660
#pragma warning disable CS0661
    public abstract class BasePath
    {
        public static bool setBasePath = true;
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

        public static implicit operator string(BasePath wrapper) => wrapper.path;

        public static bool operator ==(BasePath a, BasePath b)
        {
            if (a is null)
                return b is null;

            if (b is null)
                return false;

            return a.Path == b.Path;
        }
        public static bool operator !=(BasePath a, BasePath b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(BasePath a, string b)
        {
            if (a is null)
                return b is null;

            if (b is null)
                return false;

            return a.Path == System.IO.Path.GetFullPath(b).Replace("\\", "/");
        }
        public static bool operator !=(BasePath a, string b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(string a, BasePath b)
        {
            return b == a;
        }
        public static bool operator !=(string a, BasePath b)
        {
            return !(b == a);
        }

    }
#pragma warning restore CS0661
#pragma warning restore CS0660
}