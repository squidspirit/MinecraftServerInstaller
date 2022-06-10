using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller.Programs.Files {
    class ServerProperties {

        public struct Property {

            public Property(string key, string value) {
                Key = key;
                Value = value;
            }

            public string Key { get; }
            public string Value { get; set; }

            public override string ToString() => $"{Key}={Value}";
        }

        private static Property serverPort;
        private static Property maxPlayer;
        private static Property spawnProtection;
        private static Property viewDistance;
        private static Property pvp;
        private static Property gamemode;
        private static Property difficulty;
        private static Property enableCommandBlock;
        private static Property onlineMode;
        private static Property motd;


        public ServerProperties() {

            serverPort = new Property("server-port", null);
            maxPlayer = new Property("max-player", null);
            spawnProtection = new Property("spawn-protection", null);
            viewDistance = new Property("view-distance", null);
            pvp = new Property("pvp", null);
            gamemode = new Property("gamemode", null);
            difficulty = new Property("difficulty", null);
            enableCommandBlock = new Property("enable-command-block", null);
            onlineMode = new Property("online-mode", null);
            motd = new Property("motd", null);

            ResetProperties();
        }

        public static string ServerPort { get => serverPort.Value; set => serverPort.Value = value; }
        public static string MaxPlayer { get => maxPlayer.Value; set => maxPlayer.Value = value; }
        public static string SpawnProtection { get => spawnProtection.Value; set => spawnProtection.Value = value; }
        public static string ViewDistance { get => viewDistance.Value; set => viewDistance.Value = value; }
        public static string PVP { get => pvp.Value; set => pvp.Value = value; }
        public static string Gamemode { get => gamemode.Value; set => gamemode.Value = value; }
        public static string Difficulty { get => difficulty.Value; set => difficulty.Value = value; }
        public static string EnableCommandBlock { get => enableCommandBlock.Value; set => enableCommandBlock.Value = value; }
        public static string OnlineMode { get => onlineMode.Value; set => onlineMode.Value = value; }
        public static string Motd { get => motd.Value; set => motd.Value = value; }

        public static void ResetProperties() {

            ServerPort = "25565";
            MaxPlayer = "20";
            SpawnProtection = "16";
            ViewDistance = "10";
            PVP = "true";
            Gamemode = "0";
            Difficulty = "1";
            EnableCommandBlock = "false";
            OnlineMode = "true";
            Motd = "A Minecraft Server";
        }

        public static void CreateFile(string path) {

            using (StreamWriter writer = new StreamWriter(path + "\\server.properties")) {
                writer.WriteLine(serverPort.ToString());
                writer.WriteLine(maxPlayer.ToString());
                writer.WriteLine(spawnProtection.ToString());
                writer.WriteLine(viewDistance.ToString());
                writer.WriteLine(pvp.ToString());
                writer.WriteLine(gamemode.ToString());
                writer.WriteLine(difficulty.ToString());
                writer.WriteLine(enableCommandBlock.ToString());
                writer.WriteLine(onlineMode.ToString());
                writer.WriteLine(motd.ToString());
            }
        }
    }
}
