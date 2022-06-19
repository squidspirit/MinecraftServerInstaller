using MinecraftServerInstaller.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallJava : IInstaller {

        readonly private Dictionary<int, string> javaVersionsDictionary =
            new Dictionary<int, string>();

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;

        public InstallJava(string version, string path) {

            Version = version;
            Path = path;
        }

        public string Version { get; set; }
        public string Path { get; set; }

        public void Install() {

            string javaVersions = new WebClient().DownloadString(Program.Url.JAVA_VERSION);
            foreach (string line in javaVersions.Split('\n')) {
                string[] lineArr = line.Split(' ');
                javaVersionsDictionary.Add(Convert.ToInt32(lineArr[0]), lineArr[1]);
            }
            int gameMinorVersion = Convert.ToInt32(Version.Split('.')[1]);
            string javaUrl = null;
            if (gameMinorVersion >= 16) javaUrl = javaVersionsDictionary[17];
            else javaUrl = javaVersionsDictionary[8];

            using (WebClient client = new WebClient()) {

                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(javaUrl), Path + "\\jre.zip");
            }
        }

        private async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs(0));
                InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
                return;
            }

            await Task.Run(() => ZipFile.ExtractToDirectory(Path + "\\jre.zip", Path));
            using (StreamWriter writer = new StreamWriter(Path + "\\install.bat")) {
                writer.WriteLine("rd jre /Q /S");
                writer.WriteLine("move zulu* jre");
                writer.WriteLine("rd zulu* /Q /S");
                writer.WriteLine("del jre.zip");
                writer.WriteLine("del install.bat");
            }
            using (Process process = new Process()) {
                process.StartInfo.FileName = Path + "\\install.bat";
                process.StartInfo.WorkingDirectory = Path;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
            InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {

            InstallProgressChanged?.Invoke(
                this, new InstallProgressChangedEventArgs(e.ProgressPercentage));
        }
    }
}
