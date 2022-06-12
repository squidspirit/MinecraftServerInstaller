using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;


namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallForge : IInstaller {

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

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {

            InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {

            InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs((int)(e.ProgressPercentage * 0.8)));
        }

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;
    }
}
