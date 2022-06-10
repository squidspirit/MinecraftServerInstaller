using System;

namespace MinecraftServerInstaller.Events {
    public class InstallCompleteEventArgs : EventArgs {

        public InstallCompleteEventArgs(Exception error) {

            Error = error;
        }

        public Exception Error { get; }
    }
}
