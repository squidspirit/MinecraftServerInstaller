using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller.Programs.Files {
    class StartServerBat {

        public StartServerBat() {

            ResetSettings();
        }

        public static int MaxRam { get; set; }
        public static int MinRam { get; set; }

        public static void ResetSettings() {

            MaxRam = 2048;
            MinRam = 2048;
        }

        public static void CreateFile(string path) {

            using (StreamWriter writer = new StreamWriter(path + "\\StartServer.bat")) {
                writer.WriteLine($"java -Xmx{MaxRam}M -Xms{MinRam}M -jar server.jar");
            }
        }
    }
}
