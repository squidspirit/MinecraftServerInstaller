using System;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace MinecraftServerInstaller
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void CheckNew(object startup)
        {
            string path = Application.StartupPath + "\\Updater.exe";
            string lastVersion = null;
            bool error = false;
            using (WebClient client = new WebClient())
            {
                try
                {
                    Stream stream = client.OpenRead("https://www.dropbox.com/s/7sxx4e69ls7obyj/CheckNew.txt?dl=1");
                    StreamReader reader = new StreamReader(stream);
                    lastVersion = reader.ReadToEnd();
                    stream.Close();
                }
                catch (Exception)
                {
                    error = true;
                }
            }
            if (error)
            {
                if (!Convert.ToBoolean(startup))
                    MessageBox.Show(Language.GetUpdateError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Application.ProductVersion != lastVersion)
            {
                if (MessageBox.Show(Language.VersionInfoMessage + lastVersion, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile("https://www.dropbox.com/s/on166i5m0gvnlc3/Updater.exe?dl=1", path);
                        }
                        catch (Exception)
                        {
                            error = true;
                        }
                    }
                    if (error)
                        MessageBox.Show(Language.GetUpdateError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = path,
                            Arguments = Application.ExecutablePath,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };
                        Process.Start(startInfo);
                        Environment.Exit(1);
                    }
                }
            }
            else if (Application.ProductVersion == lastVersion)
                if (!Convert.ToBoolean(startup))
                    MessageBox.Show(Language.LatestVersionMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool CreatePath(string path)
        {
            var dialogResult = MessageBox.Show(Language.CreateFolderMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public static string GetIP()
        {
            string ipResult = "localhost, ";
            string hostName = Dns.GetHostName();  //取得本機名稱
            IPAddress[] addressList = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in addressList)
            {
                //過濾IPV4的位址
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ipResult += Convert.ToString(ip) + ", ";
            }
            char[] remove = { ',', ' ' };
            ipResult = ipResult.TrimEnd(remove);
            return ipResult;
        }
    }
}
