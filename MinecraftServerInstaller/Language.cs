using System.Windows.Forms;

namespace MinecraftServerInstaller
{
    class Language
    {
        static private int languageCode = 0;

        //Titles & Tabs
        private static readonly string[] title = { "Minecraft 伺服器安裝器", "Minecraft 服务器安装器" };
        private static readonly string[] basicSettingTab = { "基本設定", "基本设定" };
        private static readonly string[] advancedOptionTab = { "進階選項", "进阶选项" };
        private static readonly string[] aboutTab = { "關於", "关于" };

        //Labels
        private static readonly string[] gameVersion = { "版本", "版本" };
        private static readonly string[] installPath = { "安裝位置", "安装位置" };
        private static readonly string[] forgeVersion = { "模組版本", "模块版本" };
        private static readonly string[] maxRamLimitation = { "最大記憶體限制", "最大内存限制" };
        private static readonly string[] minRamLimitation = { "最小記憶體限制", "最小内存限制" };
        private static readonly string[] gui = { "GUI介面", "GUI界面" };

        //CheckBoxes
        private static readonly string[] guiCheck = { "啟用伺服器GUI介面", "启用服务器GUI接口" };
        private static readonly string[] eulaCheck = { "我同意EULA條款", "我同意EULA条款" };

        //Buttons
        private static readonly string[] selectVersion = { "選擇", "选择" };
        private static readonly string[] browse = { "瀏覽", "浏览" };
        private static readonly string[] changeRam = { "重置記憶體", "重置内存" };
        private static readonly string[] startInstall = { "開始安裝", "开始安装" };
        private static readonly string[] optionReset = { "重置為預設值", "重置为默认值" };
        private static readonly string[] checkNew = { "檢查更新", "检查更新" };

        //Messages
        private static readonly string[] changeRamMessage = { "修改記憶體參數可能造成伺服器不穩定，或無法啟動伺服器，若發生上述問題請使用其預設值。", "修改内存参数可能造成服务器不稳定，或无法启动服务器，若发生上述问题请使用其默认值。" };
        private static readonly string[] installPathMessage = { "請選擇Minecraft伺服器要安裝的位置，建議此資料夾為空的。", "请选择Minecraft服务器要安装的位置，建议此文件夹为空的。" };
        private static readonly string[] worldPathMessage = { "請選擇欲遊玩之地圖資料夾位置，若留空則創建新的世界。", "请选择欲游玩之地图文件夹位置，若留空则创建新的世界。" };
        private static readonly string[] createFolderMessage = { "找不到指定安裝位置，是否建立資料夾？", "找不到指定安装位置，是否建立文件夹？" };
        private static readonly string[] optionResetMessage = { "是否重置所有進階選項設定值？", "是否重置所有进阶选项设定值？" };
        private static readonly string[] installSuccessMessage = { "安裝成功！", "安装成功！" };
        private static readonly string[] latestVersionMessage = { "你的版本已是最新版本！", "你的版本已是最新版本！" };
        private static readonly string[] versionInfoMessage = { "偵測到新版本，是否自動下載新版本？\n目前版本：" + Application.ProductVersion + "\n最新版本：", "侦测到新版本，是否自动下载新版本？\n目前版本：" + Application.ProductVersion + "\n最新版本：" };

        //Errors
        private static readonly string[] invalidPathError = { "無效的安裝位置或地圖檔位置", "无效的安装位置或地图文件位置" };
        private static readonly string[] ramError = { "最大記憶體限制必須大於或等於最小值", "最大内存限制必须大于或等于最小值" };
        private static readonly string[] eulaError = { "你必須同意EULA條款", "你必须同意EULA条款" };
        private static readonly string[] downloadError = { "無法下載檔案，請檢查網路連線是否正常，或伺服器已正在執行", "无法下载文件，请检查网络联机是否正常，或服务器已正在执行" };
        private static readonly string[] versionSelectError = { "請選擇版本", "请选择版本" };
        private static readonly string[] getUpdateError = { "無法取得更新，請檢察網路連線是否正常", "无法取得更新，请检察网络联机是否正常" };
        private static readonly string[] getVersionError = { "無法取得版本列表，請檢查網路連線是否正常", "无法取得版本列表，请检查网络联机是否正常" };



        //Titles & Tabs
        static public int LanguageCode
        {
            get { return languageCode; }
            set { languageCode = value; }
        }

        static public string Title
        {
            get { return title[languageCode]; }
        }

        static public string BasicSettingTab
        {
            get { return basicSettingTab[languageCode]; }
        }

        static public string AdvancedOptionTab
        {
            get { return advancedOptionTab[languageCode]; }
        }

        static public string AboutTab
        {
            get { return aboutTab[languageCode]; }
        }

        //Labels
        static public string GameVersion
        {
            get { return gameVersion[languageCode]; }
        }

        static public string InstallPath
        {
            get { return installPath[languageCode]; }
        }

        static public string ForgeVersion
        {
            get { return forgeVersion[languageCode]; }
        }

        static public string MaxRamLimitation
        {
            get { return maxRamLimitation[languageCode]; }
        }

        static public string MinRamLimitation
        {
            get { return minRamLimitation[languageCode]; }
        }

        //CheckBoxes
        static public string Gui
        {
            get { return gui[languageCode]; }
        }

        static public string GuiCheck
        {
            get { return guiCheck[languageCode]; }
        }

        static public string EulaCheck
        {
            get { return eulaCheck[languageCode]; }
        }

        //Buttons
        static public string SelectVersion
        {
            get { return selectVersion[languageCode]; }
        }

        static public string Browse
        {
            get { return browse[languageCode]; }
        }

        static public string ChangeRam
        {
            get { return changeRam[languageCode]; }
        }

        static public string StartInstall
        {
            get { return startInstall[languageCode]; }
        }

        static public string OptionReset
        {
            get { return optionReset[languageCode]; }
        }

        static public string CheckNew
        {
            get { return checkNew[languageCode]; }
        }

        //Messages
        static public string ChangeRamMessage
        {
            get { return changeRamMessage[languageCode]; }
        }

        static public string InstallPathMessage
        {
            get { return installPathMessage[languageCode]; }
        }

        static public string WorldPathMessage
        {
            get { return worldPathMessage[languageCode]; }
        }

        static public string CreateFolderMessage
        {
            get { return createFolderMessage[languageCode]; }
        }

        static public string OptionResetMessage
        {
            get { return optionResetMessage[languageCode]; }
        }

        static public string InstallSuccessMessage
        {
            get { return installSuccessMessage[languageCode]; }
        }

        static public string LatestVersionMessage
        {
            get { return latestVersionMessage[languageCode]; }
        }

        static public string VersionInfoMessage
        {
            get { return versionInfoMessage[languageCode]; }
        }
        
        //Errors
        static public string InvalidPathError
        {
            get { return invalidPathError[languageCode]; }
        }

        static public string RamError
        {
            get { return ramError[languageCode]; }
        }

        static public string EulaError
        {
            get { return eulaError[languageCode]; }
        }

        static public string DownloadError
        {
            get { return downloadError[languageCode]; }
        }

        static public string VersionSelectError
        {
            get { return versionSelectError[languageCode]; }
        }

        static public string GetUpdateError
        {
            get { return getUpdateError[languageCode]; }
        }

        static public string GetVersionError
        {
            get { return getVersionError[languageCode]; }
        }
    }
}
