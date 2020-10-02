using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MinecraftServerInstaller
{
    public partial class Form2 : Form
    {
        private readonly Form1 form1;

        public Form2(Form1 _form1)
        {
            form1 = _form1;
            InitializeComponent();
        }

        private void RemoveVersionInfo()
        {
            string path = Application.StartupPath + "\\GameVersions.info";
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine("del " + path);
            process.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            closeButton.Enabled = false;
            progressBar1.Visible = true;
            string file = "GameVersions.info";
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFileCompleted += (senderDownload, eDownload) =>
                    {
                        if (eDownload.Error == null)
                        {
                            int line = 0;
                            using (StreamReader reader = new StreamReader(file))
                            {
                                while (reader.ReadLine() != null)
                                {
                                    line++;
                                }
                                reader.Close();
                            }
                            string str;
                            string[] buffer = new string[2];
                            string[] versions = new string[line];
                            string[] urls = new string[line];
                            using (StreamReader reader = new StreamReader(file))
                            {
                                for (int i = 0; i < line; i++)
                                {
                                    str = reader.ReadLine();
                                    buffer = str.Split(' ');
                                    versions[i] = buffer[0];
                                    urls[i] = buffer[1];
                                    listBox1.Items.Add(versions[i]);
                                }
                                reader.Close();
                            }
                            GameVersion.Versions = versions;
                            GameVersion.Urls = urls;
                            if (GameVersion.Index == -1)
                                listBox1.SelectedIndex = 0;
                            else
                                listBox1.SelectedIndex = GameVersion.Index;
                            RemoveVersionInfo();
                            progressBar1.Visible = false;
                            closeButton.Enabled = true;
                        }
                        else
                        {
                            RemoveVersionInfo();
                            MessageBox.Show(Language.GetVersionError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Visible = false;
                        }
                    };
                    client.DownloadProgressChanged += (senderDownload, eDownload) =>
                    {
                        progressBar1.Value = eDownload.ProgressPercentage;
                    };
                    client.DownloadFileAsync(new Uri("https://www.dropbox.com/s/mtz3moc9dpjtz7s/GameVersions.txt?dl=1"), file);
                }
                catch (Exception)
                {
                    RemoveVersionInfo();
                    MessageBox.Show(Language.GetVersionError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Visible = false;
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            GameVersion.Index = listBox1.SelectedIndex;
            form1.SetGameVersionSetting(GameVersion.Versions[GameVersion.Index]);
            Visible = false;
        }
    }
}
