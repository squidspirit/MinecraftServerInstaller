using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinecraftServerInstaller.Events;

namespace MinecraftServerInstaller.Programs.Installers {
    interface IInstaller : IDisposable {

        void Install();

        event InstallProgressChangedEventHandler InstallProgressChanged;
        event InstallCompleteEventHandler InstallComplete;
    }
}
