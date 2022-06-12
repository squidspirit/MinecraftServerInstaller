using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;


namespace MinecraftServerInstaller.Programs.Installers {
    partial class InstallFabric : IInstaller {

        public InstallFabric(string url, string path) {

            Url = url;
            Path = path;
        }

        public string Url { get; set; }
        public string Path { get; set; }

        public void Install() {

        }

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;
    }
}
