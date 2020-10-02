using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace MinecraftServerInstaller
{
    public partial class Form3 : Form
    {
        private readonly Form1 form1;

        public Form3(Form1 _form1)
        {
            form1 = _form1;
            InitializeComponent();
        }

        private void RemoveVersionInfo()
        {
            string path = Application.StartupPath + "\\ForgeVersions.info";
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine("del " + path);
            process.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string gameVersion = form1.GetGameVersionSetting();
            this.Text = "選擇 Forge 版本 - " + gameVersion;
            closeButton.Enabled = false;
            progressBar1.Visible = true;
            string file = "ForgeVersions.info";
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
                            string[] buffer = new string[3];
                            string[] coreVersions = new string[line];
                            string[] subVersions = new string[line];
                            string[] complexVersions = new string[line];
                            string[] urls = new string[line];
                            using (StreamReader reader = new StreamReader(file))
                            {
                                for (int i = 0, count = 0; i < line; i++)
                                {
                                    str = reader.ReadLine();
                                    buffer = str.Split(' ');
                                    if (buffer[0] != gameVersion)
                                        continue;
                                    coreVersions[count] = buffer[0];
                                    subVersions[count] = buffer[1];
                                    complexVersions[count] = buffer[2];
                                    urls[count] = "http://files.minecraftforge.net/maven/net/minecraftforge/forge/" + buffer[2] + "/forge-" + buffer[2] + "-installer.jar";
                                    listBox1.Items.Add(subVersions[count]);
                                    count++;
                                }
                                reader.Close();
                            }
                            if (listBox1.Items.Count == 0)
                            {
                                MessageBox.Show("沒有 " + gameVersion + " 的 Forge 版本", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                ForgeVersion.Index = -1;
                                Visible = false;
                                return;
                            }
                            ForgeVersion.Versions = complexVersions;
                            ForgeVersion.Urls = urls;
                            if (ForgeVersion.Index == -1)
                                listBox1.SelectedIndex = 0;
                            else
                                listBox1.SelectedIndex = ForgeVersion.Index;
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
                    client.DownloadFileAsync(new Uri("https://www.dropbox.com/s/0ioc3d0m6lfitlr/ForgeVersions.txt?dl=1"), file);
                }
                catch (Exception)
                {
                    RemoveVersionInfo();
                    MessageBox.Show(Language.GetVersionError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Visible = false;
                }
            }
        }

        private void closeButton_Click_1(object sender, EventArgs e)
        {
            ForgeVersion.Index = listBox1.SelectedIndex;
            form1.SetForgeVersionSetting(ForgeVersion.Versions[ForgeVersion.Index]);
            Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            ForgeVersion.Index = -1;
            form1.SetForgeVersionSetting("");
            Visible = false;
        }
    }
}
