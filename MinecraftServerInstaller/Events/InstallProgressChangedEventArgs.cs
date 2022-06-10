using System;

namespace MinecraftServerInstaller.Events {
    public class InstallProgressChangedEventArgs : EventArgs {

        public InstallProgressChangedEventArgs(int progressPercentage) {

            ProgressPercentage = progressPercentage;
        }

        public int ProgressPercentage { get; }
    }
}
