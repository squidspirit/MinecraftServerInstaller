using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace MinecraftServerInstaller {
    public partial class MainForm : Form {

        private int maxRamValue;
        private int minRamValue;
        private bool serverPortTextBoxLock = false;
        private bool maxPlayerTextBoxLock = false;
        private bool spawnProtectionTextBoxLock = false;
        private bool viewDistanceTextBoxLock = false;
        private Dictionary<string, string> versionsDictionary =
            new Dictionary<string, string>();
        private ServerProperties ServerProperties = new ServerProperties();

        public MainForm() {

            Directory.CreateDirectory(Program.Path.APPDATA);

            InitializeComponent();
            ResetBasicOptions();
            ResetAdvancedOptions();

            programNameTextBox.Text = Program.Information.NAME;
            versionTextBox.Text = Program.Information.VERSION;
            copyrightTextBox.Text = Program.Information.COPYRIGHT;
            emailTextBox.Text = Program.Information.EMAIL;
            tutorialTextBox.Text = Program.Information.TUTORIAL;
            websiteTextBox.Text = Program.Information.WEBSITE;
        }

        //
        // 功能函數
        //
        private bool valueInTrackBar(int value, TrackBar trackBar) {

            return (value >= trackBar.Minimum && value <= trackBar.Maximum);
        }

        private void checkInstallable() {

            bool enableFlag = true;

            if (string.IsNullOrEmpty(gameVersionTextBox.Text))
                enableFlag = false;
            else if (modVersionComboBox.SelectedIndex > 0) {
                if (string.IsNullOrEmpty(forgeVersionTextBox.Text))
                    enableFlag = false;
            }
            else if (string.IsNullOrEmpty(installPathTextBox.Text))
                enableFlag = false;
            else if (!eulaCheckBox.Checked)
                enableFlag = false;

            installButton.Enabled = enableFlag;
        }
        //
        // 進度條更新
        //
        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            statusProgressBar.Value = e.ProgressPercentage;
        }
        //
        // 遊戲版本選擇
        //
        private void gameVersionButton_Click(object sender, EventArgs e) {

            tabControl.Enabled = false;
            statusProgressBar.Value = 0;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                client.DownloadFileCompleted +=
                    new AsyncCompletedEventHandler(gameVersion_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Program.Url.GAME_VERSION), Program.Path.GAME_VERSION);
            }
        }

        private void gameVersion_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {

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

            versionsDictionary.Clear();
            using (StreamReader reader = new StreamReader(Program.Path.GAME_VERSION)) {
                while (true) {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] lineSplited = line.Split(' ');
                    versionsDictionary.Add(lineSplited[0], lineSplited[1]);
                }
            }
            VersionSelectForm versionSelect = new VersionSelectForm(
                Program.DialogContent.GAME_VERSION_DESCRIPT,
                versionsDictionary.Keys.ToArray<string>(),
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
                checkInstallable();
            });
            versionSelect.Show();
        }
        //
        // 模組種類更換
        //
        private void modVersionComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            forgeVersionTextBox.Text = null;
            if (modVersionComboBox.SelectedIndex == 1) {
                forgeVersionTextBox.Enabled = true;
                forgeVersionButton.Enabled = true;
            }
            else {
                forgeVersionTextBox.Enabled = false;
                forgeVersionButton.Enabled = false;
            }
            checkInstallable();
        }
        //
        // 模組版本選擇
        //
        private void forgeVersionButton_Click(object sender, EventArgs e) {

            tabControl.Enabled = false;
            statusProgressBar.Value = 0;
            using (WebClient client = new WebClient()) {
                client.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                client.DownloadFileCompleted +=
                    new AsyncCompletedEventHandler(forgeVersion_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Program.Url.FORGE_VERSION), Program.Path.FORGE_VERSION);
            }
        }

        private void forgeVersion_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {

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

            versionsDictionary.Clear();
            using (StreamReader reader = new StreamReader(Program.Path.FORGE_VERSION)) {
                while (true) {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] lineSplited = line.Split(' ');
                    if (lineSplited[0] == gameVersionTextBox.Text) {
                        versionsDictionary.Add(lineSplited[1],
                            Program.Url.forgeVersionToUrl(lineSplited[2]));
                    }
                }
            }
            if (versionsDictionary.Count == 0) {
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
                    versionsDictionary.Keys.ToArray<string>(),
                    forgeVersionTextBox.Text
                );
                versionSelect.Disposed += new EventHandler((_sender, _e) => {
                    if (versionSelect.Result != null && versionSelect.Result.Length > 0)
                        forgeVersionTextBox.Text = versionSelect.Result;
                    tabControl.Enabled = true;
                    checkInstallable();
                });
                versionSelect.Show();
            }
        }
        //
        // 安裝路徑瀏覽
        //
        private void installPathButton_Click(object sender, EventArgs e) {

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = Program.DialogContent.INSTALL_PATH_DESCRIPT;
            folderBrowser.SelectedPath = installPathTextBox.Text;
            while (folderBrowser.ShowDialog() == DialogResult.OK) {
                if (MessageBox.Show(
                    Program.DialogContent.INSTALL_PATH_WARNING,
                    Program.DialogTitle.WARNING,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning) == DialogResult.OK) {
                    installPathTextBox.Text = folderBrowser.SelectedPath;
                    break;
                }
            }
            checkInstallable();
        }
        //
        // EULA 連結點擊
        //
        private void eulaLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            System.Diagnostics.Process.Start(Program.Url.EULA);
        }
        //
        // EULA 勾選框
        //
        private void eulaCheckBox_CheckedChanged(object sender, EventArgs e) {

            checkInstallable();
        }
        //
        // 更改記憶體勾選框
        //
        private void ramSettingCheckBox_CheckedChanged(object sender, EventArgs e) {

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
        private void maxRamTrackBar_Scroll(object sender, EventArgs e) {

            maxRamTextBox.Text = maxRamTrackBar.Value.ToString();
        }

        private void maxRamTrackBarUpdate() {

            int newValue = maxRamTrackBar.Value;
            if (newValue < minRamValue) {
                maxRamTextBox.Text = minRamValue.ToString();
                maxRamTrackBar.Value = minRamValue;
                maxRamValue = minRamValue;
            }
            else {
                maxRamTextBox.Text = newValue.ToString();
                maxRamValue = newValue;
            }
        }

        private void maxRamTrackBar_MouseUp(object sender, MouseEventArgs e) {

            maxRamTrackBarUpdate();
        }

        private void maxRamTrackBar_KeyUp(object sender, KeyEventArgs e) {

            maxRamTrackBarUpdate();
        }
        //
        // 最小記憶體數值條滑動
        //
        private void minRamTrackBar_Scroll(object sender, EventArgs e) {

            minRamTextBox.Text = minRamTrackBar.Value.ToString();
        }

        private void minRamTrackBarUpdate() {

            int newValue = minRamTrackBar.Value;
            if (newValue > maxRamValue) {
                minRamTextBox.Text = maxRamValue.ToString();
                minRamTrackBar.Value = maxRamValue;
                minRamValue = maxRamValue;
            }
            else {
                minRamTextBox.Text = newValue.ToString();
                minRamValue = newValue;
            }
        }

        private void minRamTrackBar_MouseUp(object sender, MouseEventArgs e) {

            minRamTrackBarUpdate();
        }

        private void minRamTrackBar_KeyUp(object sender, KeyEventArgs e) {

            minRamTrackBarUpdate();
        }
        //
        // 最大記憶體文字框輸入
        //
        public void maxRamTextBoxUpdate() {

            int newValue;
            try {
                newValue = Convert.ToInt32(maxRamTextBox.Text);
            } catch (Exception) {
                maxRamTextBox.Text = maxRamValue.ToString();
                return;
            }

            if (newValue < minRamValue && valueInTrackBar(newValue, maxRamTrackBar))
                maxRamTextBox.Text = maxRamValue.ToString();
            else {
                maxRamTrackBar.Value = newValue;
                maxRamValue = newValue;
            }
        }

        private void maxRamTextBox_LostFocus(object sender, EventArgs e) {

            maxRamTextBoxUpdate();
        }

        private void maxRamTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                maxRamTextBoxUpdate();
        }
        //
        // 最小記憶體文字框輸入
        //
        private void minRamTextBoxUpdate() {

            int newValue;
            try {
                newValue = Convert.ToInt32(minRamTextBox.Text);
            }
            catch (Exception) {
                minRamTextBox.Text = minRamValue.ToString();
                return;
            }

            if (newValue > maxRamValue && valueInTrackBar(newValue, minRamTrackBar))
                minRamTextBox.Text = minRamValue.ToString();
            else {
                minRamTrackBar.Value = newValue;
                minRamValue = newValue;
            }
        }

        private void minRamTextBox_LostFocus(object sender, EventArgs e) {

            minRamTextBoxUpdate();
        }

        private void minRamTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                minRamTextBoxUpdate();
        }
        //
        // 伺服器連接埠文字框輸入
        //
        private void serverPortTextBoxUpdate() {

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
                serverPortTextBox.Text = ServerProperties.ServerPort.Value;
            }
            else {
                if (serverPortTextBox.Text != "25565")
                    MessageBox.Show(
                        Program.DialogContent.SERVER_PORT_WARNING,
                        Program.DialogTitle.WARNING,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                ServerProperties.ServerPort.Value = serverPortTextBox.Text;
            }
            serverPortTextBoxLock = false;
        }

        private void serverPortTextBox_LostFocus(object sender, EventArgs e) {

            serverPortTextBoxUpdate();
        }

        private void serverPortTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                serverPortTextBoxUpdate();
        }
        //
        // 玩家數上限文字框輸入
        //
        private void maxPlayerTextBoxUpdate() {

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
                maxPlayerTextBox.Text = ServerProperties.MaxPlayer.Value;
            }
            else ServerProperties.MaxPlayer.Value = maxPlayerTextBox.Text;
            maxPlayerTextBoxLock = false;
        }

        private void maxPlayerTextBox_LostFocus(object sender, EventArgs e) {

            maxPlayerTextBoxUpdate();
        }

        private void maxPlayerTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                maxPlayerTextBoxUpdate();
        }
        //
        // 重生點保護文字框輸入
        //
        private void spawnProtectionTextBoxUpdate() {

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
                spawnProtectionTextBox.Text = ServerProperties.SpawnProtection.Value;
            }
            else ServerProperties.SpawnProtection.Value = spawnProtectionTextBox.Text;
        }

        private void spawnProtectionTextBox_LostFocus(object sender, EventArgs e) {

            spawnProtectionTextBoxUpdate();
        }

        private void spawnProtectionTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                spawnProtectionTextBoxUpdate();
        }
        //
        // 最大視野距離文字框輸入
        //
        private void viewDistanceTextBoxUpdate() {

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
                viewDistanceTextBox.Text = ServerProperties.ViewDistance.Value;
            }
            else ServerProperties.ViewDistance.Value = viewDistanceTextBox.Text;
            viewDistanceTextBoxLock = false;
        }

        private void viewDistanceTextBox_LostFocus(object sender, EventArgs e) {

            viewDistanceTextBoxUpdate();
        }

        private void viewDistanceTextBox_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar == (char)Keys.Enter)
                viewDistanceTextBoxUpdate();
        }
        //
        // PVP 選單更改
        //
        private void pvpComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.PVP.Value = pvpComboBox.Text;
        }
        //
        // 遊戲模式選單更改
        //
        private void gamemodeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.Gamemode.Value = gamemodeComboBox.SelectedIndex.ToString();
        }
        //
        // 遊戲難度選單更改
        //
        private void difficultyComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.Difficulty.Value = difficultyComboBox.SelectedIndex.ToString();
        }
        //
        // 指令方塊選單更改
        //
        private void enableCommandBlockComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.EnableCommandBlock.Value = enableCommandBlockComboBox.Text;
        }
        //
        // 正版驗證選單更改
        //
        private void onlineModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            ServerProperties.OnlineMode.Value = onlineModeComboBox.Text;
        }
        //
        // 說明文字
        //
        private void motdTextBox_TextChanged(object sender, EventArgs e) {

            ServerProperties.Motd.Value = motdTextBox.Text;
        }
        //
        // 重置基本設定
        //
        private void ResetBasicOptions() {

            gameVersionTextBox.Text = null;
            forgeVersionTextBox.Text = null;
            installPathTextBox.Text = null;
            guiCheckBox.Checked = false;
            eulaCheckBox.Checked = false;
            ramSettingCheckBox.Checked = false;
            modVersionComboBox.SelectedIndex = 0;
            maxRamTrackBar.Value = 2048;
            minRamTrackBar.Value = 2048;
            maxRamValue = maxRamTrackBar.Value;
            minRamValue = minRamTrackBar.Value;
            maxRamTextBox.Text = maxRamValue.ToString();
            minRamTextBox.Text = minRamValue.ToString();

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

        private void resetBasicOptionsButton_Click(object sender, EventArgs e) {

            ResetBasicOptions();
        }
        //
        // 重置進階選項
        //
        private void ResetAdvancedOptions() {

            ServerProperties.ResetValues();

            serverPortTextBox.Text = ServerProperties.ServerPort.Value;
            maxPlayerTextBox.Text = ServerProperties.MaxPlayer.Value;
            spawnProtectionTextBox.Text = ServerProperties.SpawnProtection.Value;
            viewDistanceTextBox.Text = ServerProperties.ViewDistance.Value;
            pvpComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.PVP.Value) ? 1 : 0;
            gamemodeComboBox.SelectedIndex = Convert.ToInt32(ServerProperties.Gamemode.Value);
            difficultyComboBox.SelectedIndex = Convert.ToInt32(ServerProperties.Difficulty.Value);
            enableCommandBlockComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.EnableCommandBlock.Value) ? 1 : 0;
            onlineModeComboBox.SelectedIndex = Convert.ToBoolean(ServerProperties.OnlineMode.Value) ? 1 : 0;
            motdTextBox.Text = ServerProperties.Motd.Value;
        }

        private void resetAdvancedOptionsButton_Click(object sender, EventArgs e) {

            ResetAdvancedOptions();
        }
        //
        // 檢查更新
        //
        private void checkNewButton_Click(object sender, EventArgs e) {

        }
        //
        // 開始安裝
        //
        private void installButton_Click(object sender, EventArgs e) {

            ServerProperties.CreateFile(installPathTextBox.Text);

            switch (modVersionComboBox.SelectedIndex) {
                case 0: // Vanilla
                    Console.WriteLine(versionsDictionary[gameVersionTextBox.Text]);
                    break;
                case 1: // Forge
                    Console.WriteLine(versionsDictionary[forgeVersionTextBox.Text]);
                    break;
                case 2: // Fabric

                    break;
                default: break;
            }
        }
    }
}