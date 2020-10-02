namespace MinecraftServerInstaller
{
    class ForgeVersion
    {
        static private int index = -1;
        static private string[] versions;
        static private string[] urls;

        static public int Index
        {
            get { return index; }
            set { index = value; }
        }

        static public string[] Versions
        {
            get { return versions; }
            set { versions = value; }
        }

        static public string[] Urls
        {
            get { return urls; }
            set { urls = value; }
        }
    }
}
