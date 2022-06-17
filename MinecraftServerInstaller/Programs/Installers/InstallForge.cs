using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;
using MinecraftServerInstaller.Forms;


namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallForge : IInstaller {

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;

        private readonly CommandOutputForm outputForm = new CommandOutputForm();

        public InstallForge(string url, string path) {

            Url = url;
            Path = path;
        }

        public string Url { get; set; }
        public string Path { get; set; }

        public void Install() {

            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(Url), Path + "\\forge-installer.jar");
            }
        }

        private async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs(0));
                InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
                return;
            }

            using (StreamWriter writer = new StreamWriter(Path + "\\install.bat")) {
                if (Directory.Exists(Path + "\\libraries"))
                    writer.WriteLine("rd libraries /Q /S");
                writer.WriteLine("java -jar forge-installer.jar --installServer");
                writer.WriteLine("del forge-installer.jar /Q");
                writer.WriteLine("del user_jvm_args.txt /Q");
                writer.WriteLine("del run.* /Q");
                writer.WriteLine("move forge-*.jar server.jar");
                writer.WriteLine("del install.bat /Q");
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
