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
        private string serverPortValue;
        private string maxPlayerValue;
        private string spawnProtectionValue;
        private string viewDistanceValue;
        private bool serverPortTextBoxLock = false;
        private bool maxPlayerTextBoxLock = false;
        private bool spawnProtectionTextBoxLock = false;
        private bool viewDistanceTextBoxLock = false;
        private Dictionary<string, string> versionsDictionary =
            new Dictionary<string, string>();
        


        public MainForm() {

            Directory.CreateDirectory(Program.Path.APPDATA);

            InitializeComponent();
            resetBasicOptions();
            resetAdvancedOptions();

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

            if (gameVersionTextBox.Text == null || gameVersionTextBox.Text.Length == 0)
                enableFlag = false;
            else if (modVersionComboBox.SelectedIndex > 0)
                if (forgeVersionTextBox.Text == null || gameVersionTextBox.Text.Length == 0)
                    enableFlag = false;
                else if (installPathTextBox.Text == null || installPathTextBox.Text.Length == 0)
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

            statusProgressBar.Value = 0;
            tabControl.Enabled = false;
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

            forgeVersionTextBox.Clear();
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

            statusProgressBar.Value = 0;
            tabControl.Enabled = false;
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
            checkInstallable();
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
                serverPortTextBox.Text = serverPortValue;
            }
            else {
                if (serverPortTextBox.Text != "25565")
                    MessageBox.Show(
                        Program.DialogContent.SERVER_PORT_WARNING,
                        Program.DialogTitle.WARNING,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                serverPortValue = serverPortTextBox.Text;
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
                maxPlayerTextBox.Text = maxPlayerValue;
            }
            else maxPlayerValue = maxPlayerTextBox.Text;
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
                spawnProtectionTextBox.Text = spawnProtectionValue;
            }
            else spawnProtectionValue = spawnProtectionTextBox.Text;
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
                viewDistanceTextBox.Text = viewDistanceValue;
            }
            else viewDistanceValue = viewDistanceTextBox.Text;
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
        // 重置基本設定
        //
        private void resetBasicOptions() {

            gameVersionTextBox.Clear();
            forgeVersionTextBox.Clear();
            installPathTextBox.Clear();
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

            resetBasicOptions();
        }
        //
        // 重置進階選項
        //
        private void resetAdvancedOptions() {

            serverPortTextBox.Text = "25565";
            maxPlayerTextBox.Text = "20";
            spawnProtectionTextBox.Text = "16";
            viewDistanceTextBox.Text = "10";
            pvpComboBox.SelectedIndex = 1; // true
            gamemodeComboBox.SelectedIndex = 0; // survival
            difficultyComboBox.SelectedIndex = 1; // easy
            enableCommandBlockComboBox.SelectedIndex = 0; // false
            onlineModeComboBox.SelectedIndex = 1; // true
            motdTextBox.Text = "A Minecraft Server";

            serverPortValue = serverPortTextBox.Text;
            maxPlayerValue = maxPlayerTextBox.Text;
            spawnProtectionValue = spawnProtectionTextBox.Text;
            viewDistanceValue = viewDistanceTextBox.Text;
        }

        private void resetAdvancedOptionsButton_Click(object sender, EventArgs e) {

            resetAdvancedOptions();
        }

        private void checkNewButton_Click(object sender, EventArgs e) {

        }

        private void installButton_Click(object sender, EventArgs e) {

        }
    }
}