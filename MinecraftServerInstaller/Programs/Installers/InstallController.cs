using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;


namespace MinecraftServerInstaller.Programs.Installers {

    enum InstallType {

        Vanilla,
        Forge,
        Fabric
    }

    class InstallController {

        public event InstallProgressChangedEventHandler InstallProgressChanged;
        public event InstallCompleteEventHandler InstallComplete;

        public InstallController(InstallType type, string urlOrVersion, string path) {

            Type = type;
            Url = urlOrVersion;
            Version = urlOrVersion;
            Path = path;
            InstallJava = false;
        }

        public InstallType Type { get; }
        public bool InstallJava { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }


        public void Install() {

            switch(Type) {
                case InstallType.Vanilla:
                    using (InstallVanilla installer = new InstallVanilla(Url, Path)) {
                        installer.InstallProgressChanged += Installer_InstallProgressChanged;
                        installer.InstallComplete += Installer_InstallComplete;
                        installer.Install();
                    }
                    break;
                case InstallType.Forge:
                    using (InstallForge installer = new InstallForge(Url, Path)) {
                        installer.InstallProgressChanged += Installer_InstallProgressChanged;
                        installer.InstallComplete += Installer_InstallComplete;
                        installer.Install();
                    }
                    break;
                case InstallType.Fabric:
                    using (InstallFabric installer = new InstallFabric(Version, Path)) {
                        installer.InstallProgressChanged += Installer_InstallProgressChanged;
                        installer.InstallComplete += Installer_InstallComplete;
                        installer.Install();
                    }
                    break;
                default: return;
            }
        }

        private void Installer_InstallComplete(object serder, Events.InstallCompleteEventArgs e) {

            if (InstallJava) {
                using (InstallJava javaInstaller = new InstallJava(Version, Path)) {
                    javaInstaller.InstallProgressChanged += Installer_InstallProgressChanged;
                    javaInstaller.InstallComplete += JavaInstaller_InstallComplete;
                    javaInstaller.Install();
                }
            }
            else InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
        }

        private void JavaInstaller_InstallComplete(object serder, InstallCompleteEventArgs e) {

            InstallComplete?.Invoke(this, new InstallCompleteEventArgs(e.Error));
        }

        private void Installer_InstallProgressChanged(object serder, InstallProgressChangedEventArgs e) {

            InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs(e.ProgressPercentage));
        }
    }
}
