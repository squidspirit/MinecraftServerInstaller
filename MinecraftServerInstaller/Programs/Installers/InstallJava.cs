using MinecraftServerInstaller.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallJava : IInstaller {

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;

        public InstallJava(string version, string path) {

            Version = version;
            Path = path;
        }

        public string Version { get; set; }
        public string Path { get; set; }

        public void Install() {

            using (WebClient client = new WebClient()) {


                // client.DownloadFileAsync();
            }
        }
    }
}
