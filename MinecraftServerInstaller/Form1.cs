using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace MinecraftServerInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool firstChangeRam = true;

        private void OptionReset()
        {
            languageSetting.SelectedIndex = 0;
            serverportSetting.Value = 25565;
            maxplayerSetting.Value = 20;
            spawnprotectionSetting.Value = 16;
            pvpSetting.SelectedIndex = 0;
            commandblockSetting.SelectedIndex = 1;
            onlinemodeSetting.SelectedIndex = 0;
            gamemodeSetting.SelectedIndex = 0;
            difficultySetting.SelectedIndex = 2;
            motdSetting.Text = "A Minecraft Server";
        }

        private void RemoveUpdater()
        {
            string path = Application.StartupPath + "\\Updater.exe";
            if (File.Exists(path))
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine("del " + path);
                process.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            installFolderBrowserDialog.Description = Language.InstallPathMessage;
            worldFolderBrowserDialog.Description = Language.WorldPathMessage;
            versionInfo.Text = Application.ProductVersion;
            nameInfo.Text = Application.ProductName;
            authorInfo.Text = Application.CompanyName;
            progressBar1.Hide();
            selectForgeVersion.Enabled = false;
            ipLabel.Text = Program.GetIP();
            OptionReset();
            RemoveUpdater();
            Thread threadCheckNew = new Thread(new ParameterizedThreadStart(Program.CheckNew));
            threadCheckNew.Start(true);
        }

        private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Environment.Exit(0);
            throw new System.NotImplementedException();
        }

        private void browseInstallButton_Click(object sender, EventArgs e)
        {
            installFolderBrowserDialog.ShowDialog();
            installPath.Text = installFolderBrowserDialog.SelectedPath;
        }

        private void browseWorldButton_Click(object sender, EventArgs e)
        {
            worldFolderBrowserDialog.ShowDialog();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (firstChangeRam)
            {
                MessageBox.Show(Language.ChangeRamMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstChangeRam = false;
            }
            ramMaxSetting.Value = ramMaxBar.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (firstChangeRam)
            {
                MessageBox.Show(Language.ChangeRamMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstChangeRam = false;
            }
            ramMinSetting.Value = ramMinBar.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (firstChangeRam)
            {
                MessageBox.Show(Language.ChangeRamMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstChangeRam = false;
            }
            ramMaxBar.Value = Convert.ToInt32(ramMaxSetting.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (firstChangeRam)
            {
                MessageBox.Show(Language.ChangeRamMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstChangeRam = false;
            }
            ramMinBar.Value = Convert.ToInt32(ramMinSetting.Value);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            installFolderBrowserDialog.SelectedPath = installPath.Text;
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Enabled = false;
            string errorMessage = null;
            bool error = false;
            if (GameVersion.Index == -1)
            {
                errorMessage = Language.VersionSelectError;
                error = true;
            }
            else if (!Directory.Exists(installPath.Text))
            {
                if (!Program.CreatePath(installPath.Text))
                {
                    errorMessage = Language.InvalidPathError;
                    error = true;
                }
            }
            else if (ramMaxSetting.Value < ramMinSetting.Value)
            {
                errorMessage = Language.RamError;
                error = true;
            }
            else if (!eulaCheckBox.Checked)
            {
                errorMessage = Language.EulaError;
                error = true;
            }
            if (error)
            {
                MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar1.Hide();
                Enabled = true;
            }
            else
            {
                progressBar1.Show();
                progressBar1.Show();
                string versionStr = null;
                for (int i = 2; i < gameVersionSetting.Text.Length; i++)
                {
                    versionStr += gameVersionSetting.Text[i];
                }
                float versionVal = Convert.ToSingle(versionStr);
                using (WebClient client = new WebClient())
                {
                    // 原版伺服器
                    if (ForgeVersion.Index == -1)
                    {
                        try
                        {
                            client.DownloadFileCompleted += (senderDownload, eDownload) =>
                            {
                                if (eDownload.Error == null)
                                {
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\StartServer.bat", false))
                                    {
                                        writer.Write("java -jar -Xmx" + ramMaxSetting.Value + "M -Xms" + ramMinSetting.Value + "M server.jar");
                                        if (!guiCheckBox.Checked)
                                            writer.Write(" nogui");
                                        writer.Write("\nPAUSE");
                                    }
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\eula.txt", false))
                                    {
                                        writer.Write("eula = true");
                                    }
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\server.properties", false))
                                    {
                                        string equalSign = "=";
                                        writer.WriteLine(serverportLabel.Text + equalSign + serverportSetting.Value);
                                        writer.WriteLine(maxplayerLabel.Text + equalSign + maxplayerSetting.Value);
                                        writer.WriteLine(spawnprotectionLabel.Text + equalSign + spawnprotectionSetting.Value);
                                        writer.WriteLine(onlinmodeLabel.Text + equalSign + onlinemodeSetting.Text);
                                        writer.WriteLine(commandblockLabel.Text + equalSign + commandblockSetting.Text);
                                        writer.WriteLine(pvpLabel.Text + equalSign + pvpSetting.Text);
                                        if (versionVal >= 14.0)
                                        {
                                            writer.WriteLine(gamemodeLabel.Text + equalSign + gamemodeSetting.Text);
                                            writer.WriteLine(difficultyLabel.Text + equalSign + difficultySetting.Text);
                                        }
                                        else
                                        {
                                            writer.WriteLine(gamemodeLabel.Text + equalSign + gamemodeSetting.SelectedIndex);
                                            writer.WriteLine(difficultyLabel.Text + equalSign + difficultySetting.SelectedIndex);
                                        }
                                        writer.WriteLine(motdLabel.Text + equalSign + motdSetting.Text);
                                    }
                                    MessageBox.Show(Language.InstallSuccessMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    progressBar1.Hide();
                                    Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show(Language.DownloadError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    progressBar1.Hide();
                                    Enabled = true;
                                }
                            };
                            client.DownloadProgressChanged += (senderDownload, eDownload) =>
                            {
                                progressBar1.Value = eDownload.ProgressPercentage;
                            };
                            client.DownloadFileAsync(new Uri(GameVersion.Urls[GameVersion.Index]), installPath.Text + "\\server.jar");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(Language.DownloadError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            progressBar1.Hide();
                            Enabled = true;
                        }
                    }
                    // 模組伺服器
                    else
                    {
                        try
                        {
                            client.DownloadFileCompleted += (senderDownload, eDownload) =>
                            {
                                if (eDownload.Error == null)
                                {
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\install.bat"))
                                    {
                                        writer.WriteLine("java -jar forge-installer.jar --installServer");
                                        writer.WriteLine("del forge-installer.jar");
                                        writer.WriteLine("move forge*.jar forge_server.jar");
                                        writer.WriteLine("exit");
                                    }
                                    Process installProcess = new Process();
                                    FileInfo file = new FileInfo(installPath.Text + "\\install.bat");
                                    installProcess.StartInfo.WorkingDirectory = file.Directory.FullName;
                                    installProcess.StartInfo.FileName = installPath.Text + "\\install.bat";
                                    installProcess.StartInfo.CreateNoWindow = false;
                                    installProcess.Start();
                                    installProcess.WaitForExit();
                                    Process removeProcess = new Process();
                                    removeProcess.StartInfo.FileName = "cmd";
                                    removeProcess.StartInfo.UseShellExecute = false;
                                    removeProcess.StartInfo.RedirectStandardInput = true;
                                    removeProcess.StartInfo.CreateNoWindow = true;
                                    removeProcess.Start();
                                    removeProcess.StandardInput.WriteLine("del " + installPath.Text + "\\install.bat");
                                    removeProcess.Close();
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\StartServer.bat", false))
                                    {
                                        writer.Write("java -jar -Xmx" + ramMaxSetting.Value + "M -Xms" + ramMinSetting.Value + "M forge_server.jar");
                                        if (!guiCheckBox.Checked)
                                            writer.Write(" nogui");
                                        writer.Write("\nPAUSE");
                                    }
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\eula.txt", false))
                                    {
                                        writer.Write("eula = true");
                                    }
                                    using (StreamWriter writer = new StreamWriter(installPath.Text + "\\server.properties", false))
                                    {
                                        string equalSign = "=";
                                        writer.WriteLine(serverportLabel.Text + equalSign + serverportSetting.Value);
                                        writer.WriteLine(maxplayerLabel.Text + equalSign + maxplayerSetting.Value);
                                        writer.WriteLine(spawnprotectionLabel.Text + equalSign + spawnprotectionSetting.Value);
                                        writer.WriteLine(onlinmodeLabel.Text + equalSign + onlinemodeSetting.Text);
                                        writer.WriteLine(commandblockLabel.Text + equalSign + commandblockSetting.Text);
                                        writer.WriteLine(pvpLabel.Text + equalSign + pvpSetting.Text);
                                        if (versionVal >= 14.0)
                                        {
                                            writer.WriteLine(gamemodeLabel.Text + equalSign + gamemodeSetting.Text);
                                            writer.WriteLine(difficultyLabel.Text + equalSign + difficultySetting.Text);
                                        }
                                        else
                                        {
                                            writer.WriteLine(gamemodeLabel.Text + equalSign + gamemodeSetting.SelectedIndex);
                                            writer.WriteLine(difficultyLabel.Text + equalSign + difficultySetting.SelectedIndex);
                                        }
                                        writer.WriteLine(motdLabel.Text + equalSign + motdSetting.Text);
                                    }
                                    MessageBox.Show(Language.InstallSuccessMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    progressBar1.Hide();
                                    Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show(Language.DownloadError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    progressBar1.Hide();
                                    Enabled = true;
                                }
                            };
                            client.DownloadProgressChanged += (senderDownload, eDownload) =>
                            {
                                progressBar1.Value = eDownload.ProgressPercentage;
                            };
                            client.DownloadFileAsync(new Uri(ForgeVersion.Urls[ForgeVersion.Index]), installPath.Text + "\\forge-installer.jar");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(Language.DownloadError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            progressBar1.Hide();
                            Enabled = true;
                        }
                    }
                }
            }
        }

        private void resetRamButton_Click(object sender, EventArgs e)
        {
            ramMaxBar.Value = 1024;
            ramMinBar.Value = 1024;
            ramMaxSetting.Value = 1024;
            ramMinSetting.Value = 1024;
            firstChangeRam = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://account.mojang.com/documents/minecraft_eula");
        }

        private void optionResetButton_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(Language.OptionResetMessage, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
                OptionReset();
        }

        private void checkNew_Click(object sender, EventArgs e)
        {
            checkNewButton.Enabled = false;
            checkNewButton.Enabled = false;
            Program.CheckNew(false);
            checkNewButton.Enabled = true;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText("servive@squidspirit.ddns.net");
        }

        private void tutorialLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://youtu.be/yNis5vcueQY");
        }

        private void qaLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://squidspirit.ddns.net/tutorial/minecraft-server-installer/");
        }

        private void selectGameVersion_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Visible = true;
        }

        private void forgeVersionSelect_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(this);
            form3.Visible = true;
        }

        private void gameVersionSetting_TextChanged(object sender, EventArgs e)
        {
            ForgeVersion.Index = -1;
            SetForgeVersionSetting("");
            if (gameVersionSetting.Text != "")
                selectForgeVersion.Enabled = true;
            else
                selectForgeVersion.Enabled = false;
        }

        public void SetGameVersionSetting(string str)
        {
            gameVersionSetting.Text = str;
        }

        public string GetGameVersionSetting()
        {
            return gameVersionSetting.Text;
        }

        public void SetForgeVersionSetting(string str)
        {
            forgeVersionSetting.Text = str;
        }

        private void languageSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language.LanguageCode = languageSetting.SelectedIndex;
            Text = Language.Title;
            tabControl1.TabPages[0].Text = Language.BasicSettingTab;
            tabControl1.TabPages[1].Text = Language.AdvancedOptionTab;
            tabControl1.TabPages[2].Text = Language.AboutTab;
            gameVersionLabel.Text = Language.GameVersion;
            installPathLabel.Text = Language.InstallPath;
            forgeVersionLabel.Text = Language.ForgeVersion;
            maxRamLimitationLabel.Text = Language.MaxRamLimitation;
            minRamLimitationLabel.Text = Language.MinRamLimitation;
            guiLabel.Text = Language.Gui;
            guiCheckBox.Text = Language.GuiCheck;
            eulaCheckBox.Text = Language.EulaCheck;
            selectGameVersion.Text = Language.SelectVersion;
            selectForgeVersion.Text = Language.SelectVersion;
            browseInstallButton.Text = Language.Browse;
            resetRamButton.Text = Language.ChangeRam;
            installButton.Text = Language.StartInstall;
            optionResetButton.Text = Language.OptionReset;
            checkNewButton.Text = Language.CheckNew;
        }
    }
}
