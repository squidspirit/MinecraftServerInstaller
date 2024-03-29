﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using MinecraftServerInstaller.Forms;


namespace MinecraftServerInstaller.Programs {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static class Information {
            public static readonly string NAME = Application.ProductName;
            public static readonly string VERSION = Application.ProductVersion;
            public static readonly string COPYRIGHT = "Copyright ©  2022 魚之魷魂 SquidSpirit";
            public static readonly string EMAIL = "service@squidspirit.com";
            public static readonly string TUTORIAL = "https://youtu.be/yNis5vcueQY";
            public static readonly string WEBSITE = "https://squidspirit.com/tutorial/minecraft/minecraft-server-installer/";
        }

        public static class Status {
            public static readonly string READY = "已就緒";
            public static readonly string ERROR = "狀態異常";
            public static readonly string DOWNLOADING = "下載中";
            public static readonly string GETTING_LIST = "取得列表中";
            public static readonly string GETTING_JAVA = "安裝 Java 中";
            public static readonly string PROCESSING = "處理中";
        }

        public static class Url {
            public static readonly string LAST_VERSION = "https://www.dropbox.com/s/7sxx4e69ls7obyj/CheckNew.txt?dl=1";
            public static readonly string UPDATER = "https://www.dropbox.com/s/on166i5m0gvnlc3/Updater.exe?dl=1";
            public static readonly string GAME_VERSION = "https://www.dropbox.com/s/mtz3moc9dpjtz7s/GameVersions.txt?dl=1";
            public static readonly string FORGE_VERSION = "https://www.dropbox.com/s/0ioc3d0m6lfitlr/ForgeVersions.txt?dl=1";
            public static readonly string FABRIC_VERSION = "https://www.dropbox.com/s/p5le9abdiwx10wi/FabricInstallerVersion.txt?dl=1";
            public static readonly string JAVA_VERSION = "https://www.dropbox.com/s/ssw3fujk2spr218/JavaVersions.txt?dl=1";
            public static readonly string EULA = "https://www.minecraft.net/en-us/eula";

            public static string ForgeVersionToUrl(string version) {
                return String.Format("https://maven.minecraftforge.net/net/minecraftforge/forge/{0}/forge-{0}-installer.jar", version);
            }

            public static string FabricVersionToUrl(string version) {
                return String.Format("https://maven.fabricmc.net/net/fabricmc/fabric-installer/{0}/fabric-installer-{0}.jar", version);
            }
        }

        public static class Path {
            public static readonly string APPDATA = Environment.GetEnvironmentVariable("APPDATA") + "\\MinecraftServerInstaller";
            public static readonly string UPDATER = APPDATA + "\\Updater.exe";
            public static readonly string GAME_VERSION = APPDATA + "\\GameVersions";
            public static readonly string FORGE_VERSION = APPDATA + "\\ForgeVersion";
            public static readonly string FABRIC_VERSION = APPDATA + "\\FabricVersion";
        }

        public static class DialogTitle {
            public static readonly string INFO = "提示";
            public static readonly string WARNING = "警告";
            public static readonly string ERROR = "錯誤";
        }

        public static class DialogContent {
            public static readonly string UPDATE_INFO = "您的版本已是最新版本。";
            public static readonly string RESET_INFO = "確定要重置本頁面的所有選項嗎？";
            public static readonly string INTERNET_ERROR = "請檢常網路設備是否連接正常。";
            public static readonly string INSTALL_PATH_DESCRIPT = "請選擇安裝伺服器的資料夾，建議此資料夾為空。";
            public static readonly string INSTALL_PATH_WARNING = "您選擇的資料夾內已有其他檔案，確定要繼續嗎？";
            public static readonly string GAME_VERSION_DESCRIPT = "請選擇主遊戲版本。";
            public static readonly string FORGE_VERSION_DESCRIPT = "請選擇 Forge 版本。";
            public static readonly string FORGE_VERSION_INFO = "找不到此版本的 Forge，請嘗試其他遊戲版本。";
            public static readonly string FABRIC_VERSION_INFO = "Fabric 僅支援 1.14 或以上的版本。";
            public static readonly string JAVA_WARNING = "下載 Java 程式會需要額外約 100 MB 的空間，僅支援 64 位元作業系統。（安裝模組伺服器時仍須仰賴本機已安裝的 Java）";
            public static readonly string RAM_WARNING = "更改伺服器記憶體限制可能會影響伺服器的穩定性，或甚至無法正常啟動，請小心調整。";
            public static readonly string SERVER_PORT_INFO = "請輸入 1025 ~ 65535 內的值。";
            public static readonly string SERVER_PORT_WARNING = "更改伺服器連接埠可能與其他應用程式產生衝突，造成無法連線或系統不穩定，請確認設置的連接埠是否為空連接埠，並為其設置防火牆通道允許通過。";
            public static readonly string MAX_PLAYER_INFO = "請輸入正整數。";
            public static readonly string SPAWN_PROTECTION_INFO = "請輸入正整數。";
            public static readonly string VIEW_DISTANCE_INFO = "請輸入正整數。";
            public static readonly string INSTALL_SUCCESS = "安裝完成！請至安裝資料夾中以 StartServer 檔案啟動伺服器。";

            public static string GetUpdateInfo(string currentVer, string newVer) {
                return $"檢查到更新的版本，請問要進行自動更新嗎？\n目前版本：{currentVer}\n最新版本：{newVer}";
            }
        }
    }
}
