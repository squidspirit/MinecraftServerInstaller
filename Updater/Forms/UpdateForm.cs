using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Updater.Froms {
    public partial class UpdateForm : Form {

        private string path = null;
        private string envPath = Environment.GetEnvironmentVariable("APPDATA") + "\\MinecraftServerInstaller";

        public UpdateForm(string path) {

            InitializeComponent();

            this.path = path;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(
                    new Uri("https://www.dropbox.com/s/onq4lc9c8uqars6/MinecraftServerInstaller.exe?dl=1"),
                    envPath + "\\MinecraftServerInstaller.exe"
                );
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                MessageBox.Show(
                    "無法取得更新，請檢察網路連線是否正常", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                Environment.Exit(-1);
            }

            using (StreamWriter writer = new StreamWriter(envPath + "\\Update.bat", false, System.Text.Encoding.GetEncoding("big5"))) {
                writer.WriteLine($"copy MinecraftServerInstaller.exe \"{path}\"");
                writer.WriteLine($"\"{path}\"");
            }
            MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            using (Process process = new Process()) {
                process.StartInfo.FileName = envPath + "\\Update.bat";
                process.StartInfo.WorkingDirectory = envPath;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {

            progressBar.Value = e.ProgressPercentage;
        }
    }
}
