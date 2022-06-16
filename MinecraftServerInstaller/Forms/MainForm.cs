using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;

using MinecraftServerInstaller.Programs;
using MinecraftServerInstaller.Programs.Files;
using MinecraftServerInstaller.Programs.Installers;
using MinecraftServerInstaller.Events;


namespace MinecraftServerInstaller.Forms {
    public partial class MainForm : Form {

        private bool serverPortTextBoxLock = false;
        private bool maxPlayerTextBoxLock = false;
        private bool spawnProtectionTextBoxLock = false;
        private bool viewDistanceTextBoxLock = false;
        readonly private Dictionary<string, string> gameVersionsDictionary =
            new Dictionary<string, string>();
        readonly private Dictionary<string, string> forgeVersionsDictionary =
            new Dictionary<string, string>();

        public MainForm() {

            Directory.CreateDirectory(Program.Path.APPDATA);

            InitializeComponent();
            ResetBasicOptions(toConfirm : false);
            ResetAdvancedOptions(toConfirm : false);

            programNameTextBox.Text = Program.Information.NAME;
            versionTextBox.Text = Program.Information.VERSION;
            copyrightTextBox.Text = Program.Information.COPYRIGHT;
            emailTextBox.Text = Program.Information.EMAIL;
            tutorialTextBox.Text = Program.Information.TUTORIAL;
            websiteTextBox.Text = Program.Information.WEBSITE;

            javaInstallCheckBox.Hide();
            javaInstallLabel.Hide();

            Programs.Update.CheckNew();
        }

        //
        // 功能函數
        //
        private bool IsValueInTrackBar(int value, TrackBar trackBar) {

            return (value >= trackBar.Minimum && value <= trackBar.Maximum);
        }

        private void CheckInstallable() {

            bool enableFlag = true;

            if (string.IsNullOrEmpty(gameVersionTextBox.Text))
                enableFlag = false;
            if (modVersionComboBox.SelectedIndex == 1) {
                if (string.IsNullOrEmpty(forgeVersionTextBox.Text))
                    enableFlag = false;
            }
            if (string.IsNullOrEmpty(installPathTextBox.Text))
                enableFlag = false;
            if (!eulaCheckBox.Checked)
                enableFlag = false;

            installButton.Enabled = enableFlag;
        }
        //
        // 進度條更新
        //
        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            
            statusProgressBar.Value = e.ProgressPercentage;
        }

        private void Install_InstallProgressChanged(object serder, InstallProgressChangedEventArgs e) {

            statusProgressBar.Value = e.ProgressPercentage;
        }
        //
        // 遊戲版本選擇
        //
        private void GameVersionButton_Click(object sender, EventArgs e) {

            tabControl.Enabled = false;
            statusProgressBar.Value = 0;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                client.DownloadFileCompleted += GameVersion_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(Program.Url.GAME_VERSION), Program.Path.GAME_VERSION);
            }
        }

        private void GameVersion_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                MessageBox.Show(
                    Program.DialogContent.INTERNET_ERROR,
                    Program.DialogTitle.ERROR,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                tabControl.Enabled = true;
                return;
            }

            gameVersionsDictionary.Clear();
            using (StreamReader reader = new StreamReader(Program.Path.GAME_VERSION)) {
                while (true) {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] lineSplited = line.Split(' ');
                    gameVersionsDictionary.Add(lineSplited[0], lineSplited[1]);
                }
            }
            VersionSelectForm versionSelect = new VersionSelectForm(
                Program.DialogContent.GAME_VERSION_DESCRIPT,
                gameVersionsDictionary.Keys.ToArray<string>(),
                gameVersionTextBox.Text
            );
            versionSelect.Disposed += new EventHandler((_sender, _e) => {
                if (versionSelect.Result != null && versionSelect.Result.Length > 0) {
                    if (gameVersionTextBox.Text != versionSelect.Result) {
                        gameVersionTextBox.Text = versionSelect.Result;
                        modVersionComboBox.SelectedIndex = 0;
                    }
                    modVersionComboBox.Enabled = true;
                }
                tabControl.Enabled = true;
                CheckInstallable();
            });
            versionSelect.Show();
        }
        //
        // 模組種類更換
        //
        private void ModVersionComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            forgeVersionTextBox.Text = null;
            if (modVersionComboBox.SelectedIndex == 1) {
                forgeVersionTextBox.Enabled = true;
                forgeVersionButton.Enabled = true;
            }
            else {
                forgeVersionTextBox.Enabled = false;
                forgeVersionButton.Enabled = false;
            }

            if (modVersionComboBox.SelectedIndex == 2 &&
                Convert.ToInt32(gameVersionTextBox.Text.Split('.')[1]) < 14) {

                MessageBox.Show(
                    Program.DialogContent.FABRIC_VERSION_INFO,
                    Program.DialogTitle.WARNING,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                modVersionComboBox.SelectedIndex = 0;
            }
            CheckInstallable();
        }
        //
        // 模組版本選擇
        //
        private void ForgeVersionButton_Click(object sender, EventArgs e) {

            tabControl.Enabled = false;
            statusProgressBar.Value = 0;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                client.DownloadFileCompleted += ForgeVersion_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(Program.Url.FORGE_VERSION), Program.Path.FORGE_VERSION);
            }
        }

        private void ForgeVersion_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {

            if (e.Error != null) {
                MessageBox.Show(
                    Program.DialogContent.INTERNET_ERROR,
                    Program.DialogTitle.ERROR,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                tabControl.Enabled = true;
                return;
            }

            forgeVersionsDictionary.Clear();
            using (StreamReader reader = new StreamReader(Program.Path.FORGE_VERSION)) {
                while (true) {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] lineSplited = line.Split(' ');
                    if (lineSplited[0] == gameVersionTextBox.Text) {
                        forgeVersionsDictionary.Add(
                            lineSplited[1],
                            Program.Url.ForgeVersionToUrl(lineSplited[2])
                        );
                    }
                }
            }
            if (forgeVersionsDictionary.Count == 0) {
                MessageBox.Show(
                    Program.DialogContent.FORGE_VERSION_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                tabControl.Enabled = true;
            }
            else {
                VersionSelectForm versionSelect = new VersionSelectForm(
                    Program.DialogContent.FORGE_VERSION_DESCRIPT,
                    forgeVersionsDictionary.Keys.ToArray<string>(),
                    forgeVersionTextBox.Text
                );
                versionSelect.Disposed += new EventHandler((_sender, _e) => {
                    if (versionSelect.Result != null && versionSelect.Result.Length > 0)
                        forgeVersionTextBox.Text = versionSelect.Result;
                    tabControl.Enabled = true;
                    CheckInstallable();
                });
                versionSelect.Show();
            }
        }
        //
        // 安裝路徑瀏覽
        //
        private void InstallPathButton_Click(object sender, EventArgs e) {

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog {
                Description = Program.DialogContent.INSTALL_PATH_DESCRIPT,
                SelectedPath = installPathTextBox.Text
            };

            while (folderBrowser.ShowDialog() == DialogResult.OK) {
                bool isSelected = false;
                if (Directory.EnumerateFileSystemEntries(folderBrowser.SelectedPath).Any()) {
                    if (MessageBox.Show(
                        Program.DialogContent.INSTALL_PATH_WARNING,
                        Program.DialogTitle.WARNING,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning) == DialogResult.OK) {

                        isSelected = true;
                    }
                }
                else isSelected = true;

                if (isSelected) {
                    installPathTextBox.Text = folderBrowser.SelectedPath;
                    break;
                }
            }
            CheckInstallable();
        }
        //
        // EULA 連結點擊
        //
        private void EulaLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            System.Diagnostics.Process.Start(Program.Url.EULA);
        }
        //
        // EULA 勾選框
        //
        private void EulaCheckBox_CheckedChanged(object sender, EventArgs e) {

            CheckInstallable();
        }
        //
        // GUI 勾選框
        //
        private void GuiCheckBox_CheckedChanged(object sender, EventArgs e) {

            StartServerBat.IsNoGui = !guiCheckBox.Checked;
        }
        //
        // Java 勾選框
        //
        private void JavaInstallCheckBox_CheckedChanged(object sender, EventArgs e) {
            
            if (javaInstallCheckBox.Checked == true) {
                MessageBox.Show(
                    Program.DialogContent.JAVA_WARNING,
                    Program.DialogTitle.WARNING,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            StartServerBat.IsLocalJava = javaInstallCheckBox.Checked;
        }
        //
        // 更改記憶體勾選框
        //
        private void RamSettingCheckBox_CheckedChanged(object sender, EventArgs e) {

            if (ramSettingCheckBox.Checked == true) {
                MessageBox.Show(
                    Program.DialogContent.RAM_WARNING,
                    Program.DialogTitle.WARNING,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                ramSettingCheckBox.Enabled = false;
                maxRamTrackBar.Enabled = true;
                minRamTrackBar.Enabled = true;
                maxRamTextBox.Enabled = true;
                minRamTextBox.Enabled = true;
            }
        }
        //
        // 最大記憶體數值條滑動
        //
        private void MaxRamTrackBar_Scroll(object sender, EventArgs e) {

            maxRamTextBox.Text = maxRamTrackBar.Value.ToString();
        }

        private void MaxRamTrackBarUpdate() {

            int newValue = maxRamTrackBar.Value;
            if (newValue < StartServerBat.MaxRam) {
                maxRamTextBox.Text = StartServerBat.MinRam.ToString();
                maxRamTrackBar.Value = StartServerBat.MinRam;
                StartServerBat.MaxRam = StartServerBat.MinRam;
            }
            else {
                maxRamTextBox.Text = newValue.ToString();
                StartServerBat.MaxRam = newValue;
            }
        }

        private void MaxRamTrackBar_MouseUp(object sender, MouseEventArgs e) {

            MaxRamTrackBarUpdate();
        }

        private void MaxRamTrackBar_KeyUp(object sender, KeyEventArgs e) {

            MaxRamTrackBarUpdate();
        }
        //
        // 最小記憶體數值條滑動
        //
        private void MinRamTrackBar_Scroll(object sender, EventArgs e) {

            minRamTextBox.Text = minRamTrackBar.Value.ToString();
        }

        private void MinRamTrackBarUpdate() {

            int newValue = minRamTrackBar.Value;
            if (newValue > StartServerBat.MaxRam) {
                minRamTextBox.Text = StartServerBat.MaxRam.ToString();
                minRamTrackBar.Value = StartServerBat.MaxRam;
                StartServerBat.MinRam = StartServerBat.MaxRam;
            }
            else {
                minRamTextBox.Text = newValue.ToString();
                StartServerBat.MinRam = newValue;
            }
        }

        private void MinRamTrackBar_MouseUp(object sender, MouseEventArgs e) {

            MinRamTrackBarUpdate();
        }

        private void MinRamTrackBar_KeyUp(object sender, KeyEventArgs e) {

            MinRamTrackBarUpdate();
        }
        //
        // 最大記憶體文字框輸入
        //
        public void MaxRamTextBoxUpdate() {

            int newValue;
            try {
                newValue = Convert.ToInt32(maxRamTextBox.Text);
            } catch (Exception) {
                maxRamTextBox.Text = StartServerBat.MaxRam.ToString();
                return;
            }

            if (newValue < StartServerBat.MinRam && IsValueInTrackBar(newValue, maxRamTrackBar))
                maxRamTextBox.Text = StartServerBat.MaxRam.ToString();
            else {
                maxRamTrackBar.Value = newValue;
                StartServerBat.MaxRam = newValue;
            }
        }

        private void MaxRamTextBox_LostFocus(object sender, EventArgs e) {

            MaxRamTextBoxUpdate();
        }

        private void MaxRamTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                MaxRamTextBoxUpdate();
        }
        //
        // 最小記憶體文字框輸入
        //
        private void MinRamTextBoxUpdate() {

            int newValue;
            try {
                newValue = Convert.ToInt32(minRamTextBox.Text);
            }
            catch (Exception) {
                minRamTextBox.Text = StartServerBat.MinRam.ToString();
                return;
            }

            if (newValue > StartServerBat.MaxRam && IsValueInTrackBar(newValue, minRamTrackBar))
                minRamTextBox.Text = StartServerBat.MinRam.ToString();
            else {
                minRamTrackBar.Value = newValue;
                StartServerBat.MinRam = newValue;
            }
        }

        private void MinRamTextBox_LostFocus(object sender, EventArgs e) {

            MinRamTextBoxUpdate();
        }

        private void MinRamTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                MinRamTextBoxUpdate();
        }
        //
        // 伺服器連接埠文字框輸入
        //
        private void ServerPortTextBoxUpdate() {

            if (serverPortTextBoxLock) return;
            serverPortTextBoxLock = true;

            bool errorFlag = false;
            int newValue = 25565;
            try {
                newValue = Convert.ToInt32(serverPortTextBox.Text);
            }
            catch (Exception) { errorFlag = true; }

            if (errorFlag || newValue < 1025 || 65535 < newValue) {
                MessageBox.Show(
                    Program.DialogContent.SERVER_PORT_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                serverPortTextBox.Text = ServerProperties.ServerPort;
            }
            else {
                if (serverPortTextBox.Text != "25565")
                    MessageBox.Show(
                        Program.DialogContent.SERVER_PORT_WARNING,
                        Program.DialogTitle.WARNING,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                ServerProperties.ServerPort = serverPortTextBox.Text;
            }
            serverPortTextBoxLock = false;
        }

        private void ServerPortTextBox_LostFocus(object sender, EventArgs e) {

            ServerPortTextBoxUpdate();
        }

        private void ServerPortTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                ServerPortTextBoxUpdate();
        }
        //
        // 玩家數上限文字框輸入
        //
        private void MaxPlayerTextBoxUpdate() {

            if (maxPlayerTextBoxLock) return;
            maxPlayerTextBoxLock = true;

            bool errorFlag = false;
            int newValue = 20;
            try {
                newValue = Convert.ToInt32(maxPlayerTextBox.Text);
            }
            catch (Exception) { errorFlag = true; }

            if (errorFlag || newValue < 1) {
                MessageBox.Show(
                    Program.DialogContent.MAX_PLAYER_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                maxPlayerTextBox.Text = ServerProperties.MaxPlayer;
            }
            else ServerProperties.MaxPlayer = maxPlayerTextBox.Text;
            maxPlayerTextBoxLock = false;
        }

        private void MaxPlayerTextBox_LostFocus(object sender, EventArgs e) {

            MaxPlayerTextBoxUpdate();
        }

        private void MaxPlayerTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                MaxPlayerTextBoxUpdate();
        }
        //
        // 重生點保護文字框輸入
        //
        private void SpawnProtectionTextBoxUpdate() {

            if (spawnProtectionTextBoxLock) return;
            spawnProtectionTextBoxLock = true;

            bool errorFlag = false;
            int newValue = 16;
            try {
                newValue = Convert.ToInt32(spawnProtectionTextBox.Text);
            }
            catch (Exception) { errorFlag = true; }

            if (errorFlag || newValue < 1) {
                MessageBox.Show(
                    Program.DialogContent.MAX_PLAYER_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                spawnProtectionTextBox.Text = ServerProperties.SpawnProtection;
            }
            else ServerProperties.SpawnProtection = spawnProtectionTextBox.Text;
        }

        private void SpawnProtectionTextBox_LostFocus(object sender, EventArgs e) {

            SpawnProtectionTextBoxUpdate();
        }

        private void SpawnProtectionTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                SpawnProtectionTextBoxUpdate();
        }
        //
        // 最大視野距離文字框輸入
        //
        private void ViewDistanceTextBoxUpdate() {

            if (viewDistanceTextBoxLock) return;
            viewDistanceTextBoxLock = true;

            bool errorFlag = false;
            int newValue = 10;
            try {
                newValue = Convert.ToInt32(viewDistanceTextBox.Text);
            }
            catch (Exception) { errorFlag = true; }

            if (errorFlag || newValue < 1) {
                MessageBox.Show(
                    Program.DialogContent.MAX_PLAYER_INFO,
                    Program.DialogTitle.INFO,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                viewDistanceTextBox.Text = ServerProperties.ViewDistance;
            }
            else ServerProperties.ViewDistance = viewDistanceTextBox.Text;
            viewDistanceTextBoxLock = false;
        }

        private void ViewDistanceTextBox_LostFocus(object sender, EventArgs e) {

            ViewDistanceTextBoxUpdate();
        }

        private void ViewDistanceTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                ViewDistanceTextBoxUpdate();
        }
        //
        // PVP 選單更改
        //
        private void PvpComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.Pvp = pvpComboBox.Text;
        }
        //
        // 遊戲模式選單更改
        //
        private void GamemodeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.Gamemode = gamemodeComboBox.SelectedIndex.ToString();
        }
        //
        // 遊戲難度選單更改
        //
        private void DifficultyComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.Difficulty = difficultyComboBox.SelectedIndex.ToString();
        }
        //
        // 指令方塊選單更改
        //
        private void EnableCommandBlockComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.EnableCommandBlock = enableCommandBlockComboBox.Text;
        }
        //
        // 正版驗證選單更改
        //
        private void OnlineModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.OnlineMode = onlineModeComboBox.Text;
        }
        //
        // 說明文字
        //
        private void MotdTextBox_TextChanged(object sender, EventArgs e) {

            ServerProperties.Motd = motdTextBox.Text;
        }
        //
        // 重置基本設定
        //
        private void ResetBasicOptions() {

            ResetBasicOptions(true);
        }

        private void ResetBasicOptions(bool toConfirm) {

            if (toConfirm && MessageBox.Show(
                Program.DialogContent.RESET_INFO,
                Program.DialogTitle.INFO,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No) {

                return;
            }

            StartServerBat.ResetSettings();

            gameVersionTextBox.Text = null;
            forgeVersionTextBox.Text = null;
            installPathTextBox.Text = null;
            guiCheckBox.Checked = false;
            eulaCheckBox.Checked = false;
            ramSettingCheckBox.Checked = false;
            modVersionComboBox.SelectedIndex = 0;
            maxRamTrackBar.Value = StartServerBat.MaxRam;
            minRamTrackBar.Value = StartServerBat.MinRam;
            StartServerBat.MaxRam = maxRamTrackBar.Value;
            StartServerBat.MinRam = minRamTrackBar.Value;
            maxRamTextBox.Text = StartServerBat.MaxRam.ToString();
            minRamTextBox.Text = StartServerBat.MinRam.ToString();

            modVersionComboBox.Enabled = false;
            forgeVersionTextBox.Enabled = false;
            forgeVersionButton.Enabled = false;
            ramSettingCheckBox.Enabled = true;
            maxRamTrackBar.Enabled = false;
            minRamTrackBar.Enabled = false;
            maxRamTextBox.Enabled = false;
            minRamTextBox.Enabled = false;
            installButton.Enabled = false;
        }

        private void ResetBasicOptionsButton_Click(object sender, EventArgs e) {

            ResetBasicOptions();
        }
        //
        // 重置進階選項
        //
        private void ResetAdvancedOptions() {

            ResetAdvancedOptions(true);
        }

        private void ResetAdvancedOptions(bool toConfirm) {

            if (toConfirm && MessageBox.Show(
                Program.DialogContent.RESET_INFO,
                Program.DialogTitle.INFO,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No) {

                return;
            }

            ServerProperties.ResetProperties();

            serverPortTextBox.Text = ServerProperties.ServerPort;
            maxPlayerTextBox.Text = ServerProperties.MaxPlayer;
            spawnProtectionTextBox.Text = ServerProperties.SpawnProtection;
            viewDistanceTextBox.Text = ServerProperties.ViewDistance;
            pvpComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.Pvp) ? 1 : 0;
            gamemodeComboBox.SelectedIndex = Convert.ToInt32(ServerProperties.Gamemode);
            difficultyComboBox.SelectedIndex = Convert.ToInt32(ServerProperties.Difficulty);
            enableCommandBlockComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.EnableCommandBlock) ? 1 : 0;
            onlineModeComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.OnlineMode) ? 1 : 0;
            motdTextBox.Text = ServerProperties.Motd;
        }

        private void ResetAdvancedOptionsButton_Click(object sender, EventArgs e) {

            ResetAdvancedOptions();
        }
        //
        // 檢查更新
        //
        private void CheckNewButton_Click(object sender, EventArgs e) {

            Programs.Update.CheckNew();
        }
        //
        // 開始安裝
        //
        private void InstallButton_Click(object sender, EventArgs e) {

            tabControl.Enabled = false;
            statusProgressBar.Value = 0;

            IInstaller installer;
            switch (modVersionComboBox.SelectedIndex) {
                case 0: // Vanilla
                    installer = new InstallVanilla( // url, filename
                        gameVersionsDictionary[gameVersionTextBox.Text],
                        installPathTextBox.Text
                    );
                    break;
                case 1: // Forge
                    installer = new InstallForge( // url, filename
                        forgeVersionsDictionary[forgeVersionTextBox.Text],
                        installPathTextBox.Text
                    );
                    StartServerBat.IsNewForge =
                        Convert.ToInt32(gameVersionTextBox.Text.Split('.')[1]) >= 17;
                    break;
                case 2: // Fabric
                    installer = new InstallFabric( // gameVersion, filename
                        gameVersionTextBox.Text,
                        installPathTextBox.Text
                    );
                    StartServerBat.IsFabric = true;
                    break;
                default: return;
            }
            installer.InstallComplete += Install_InstallComplete;
            installer.InstallProgressChanged += Install_InstallProgressChanged;
            installer.Install();
            installer.Dispose();
        }

        private void Install_InstallComplete(object serder, InstallCompleteEventArgs e) {

            if (e.Error != null) {
                MessageBox.Show(
                    Program.DialogContent.INTERNET_ERROR,
                    Program.DialogTitle.ERROR,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                tabControl.Enabled = true;
                return;
            }

            ServerProperties.CreateFile(installPathTextBox.Text);
            StartServerBat.CreateFile(installPathTextBox.Text);
            Eula.CreateFile(installPathTextBox.Text);
            MessageBox.Show(
                Program.DialogContent.INSTALL_SUCCESS,
                Program.DialogTitle.INFO,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            tabControl.Enabled = true;
        }
    }
}