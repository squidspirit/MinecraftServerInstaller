using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;
using MinecraftServerInstaller.Forms;

namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallFabric : IInstaller {

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;

        private readonly CommandOutputForm outputForm = new CommandOutputForm();

        public InstallFabric(string version, string path) {

            Version = version;
            Path = path;
        }

        public string Version { get; set; }
        public string Path { get; set; }

        public void Install() {

            string fabricVersion = new WebClient().DownloadString(Program.Url.FABRIC_VERSION);
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(
                    new Uri(Program.Url.FabricVersionToUrl(fabricVersion)),
                    Path + "\\fabric-installer.jar"
                );
            }
        }

        private async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs(0));
                InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
                return;
            }

            using (StreamWriter writer = new StreamWriter(Path + "\\install.bat")) {
                writer.WriteLine($"java -jar fabric-installer.jar server -downloadMinecraft -mcversion {Version}");
                writer.WriteLine("del fabric-installer.jar");
                writer.WriteLine("del install.bat");
            }
            outputForm.Clear();
            outputForm.Show();
            await Task.Run(() => RunInstallBat());
            outputForm.Hide();

            InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs(100));
            InstallComplete?.Invoke(this, new InstallCompleteEventArgs(null));
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {

            InstallProgressChanged?.Invoke(this,
                new InstallProgressChangedEventArgs((int)(e.ProgressPercentage * 0.99)));
        }

        private void RunInstallBat() {

            using (Process process = new Process()) {
                process.StartInfo.FileName = Path + "\\install.bat";
                process.StartInfo.WorkingDirectory = Path;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.OutputDataReceived += (sender, e) => outputForm.TextBoxAppend(e.Data);
                process.ErrorDataReceived += (sender, e) => outputForm.TextBoxAppend(e.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
    }
}
