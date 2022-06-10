using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller {
    class ServerProperties {

        public class Property {

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

            ResetValues();
        }

        public static Property ServerPort => serverPort;
        public static Property MaxPlayer => maxPlayer;
        public static Property SpawnProtection => spawnProtection;
        public static Property ViewDistance => viewDistance;
        public static Property PVP => pvp;
        public static Property Gamemode => gamemode;
        public static Property Difficulty => difficulty;
        public static Property EnableCommandBlock => enableCommandBlock;
        public static Property OnlineMode => onlineMode;
        public static Property Motd => motd;

        public void ResetValues() {

            serverPort.Value = "25565";
            maxPlayer.Value = "20";
            spawnProtection.Value = "16";
            viewDistance.Value = "10";
            pvp.Value = "true";
            gamemode.Value = "0";
            difficulty.Value = "1";
            enableCommandBlock.Value = "false";
            onlineMode.Value = "true";
            motd.Value = "A Minecraft Server";
        }

        public void CreateFile(string path) {

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
