using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServerInstaller.Programs {
    class Update {

        private static bool firstRun = true;
        private static bool updateLock = false;

        public static void CheckNew() {

            if (updateLock) return;
            updateLock = true;
            using (WebClient client = new WebClient()) {
                client.OpenReadCompleted += Client_OpenReadCompleted;
                client.OpenReadAsync(new Uri(Program.Url.LAST_VERSION));
            }
        }

        private static void Client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e) {

            string lastVersion = null;
            using (StreamReader reader = new StreamReader(e.Result)) {
                lastVersion = reader.ReadLine();
            }
            if (Application.ProductVersion != lastVersion) {
                if (MessageBox.Show(
                    Program.DialogContent.GetUpdateInfo(Application.ProductVersion, lastVersion),
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes) {

                    using (WebClient client = new WebClient()) {

                        client.DownloadFileCompleted += Client_DownloadFileCompleted;
                        client.DownloadFileAsync(new Uri(Program.Url.UPDATER), Program.Path.UPDATER);
                    }
                }
                else if (!firstRun) {
                    MessageBox.Show(
                        Program.DialogContent.INTERNET_ERROR,
                        Program.DialogTitle.ERROR,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else if (!firstRun) {
                MessageBox.Show(
                    Program.DialogContent.UPDATE_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            firstRun = false;
            updateLock = false;
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            
            using (Process process = new Process()) {
                process.StartInfo.FileName = Program.Path.UPDATER;
                process.StartInfo.Arguments = Application.ExecutablePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
            }
            Environment.Exit(0);
        }
    }
}
