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
        public static bool IsNoGui { get; set; }
        public static bool IsNewForge { get; set; }
        public static bool IsFabric { get; set; }
        public static bool IsLocalJava { get; set; }


        public static void ResetSettings() {

            MaxRam = 2048;
            MinRam = 2048;
            IsNoGui = true;
            IsNewForge = false;
            IsFabric = false;
            IsLocalJava = false;
        }

        public static void CreateFile(string path) {

            using (StreamWriter writer = new StreamWriter(path + "\\StartServer.bat")) {
                writer.Write(IsLocalJava ? "jdk\\bin\\" : "");
                writer.Write($"java -Xmx{MaxRam}M -Xms{MinRam}M");
                if (IsNewForge) {
                    if (Directory.Exists(path + "\\libraries")) {
                        string version = Directory.EnumerateFileSystemEntries(
                            path + "\\libraries\\net\\minecraftforge\\forge").First().Split('\\').Last();
                        writer.Write($" @libraries/net/minecraftforge/forge/{version}/win_args.txt");
                    }
                    else throw new Exception($"{path}\\libraries not found.");
                }
                else if (IsFabric) writer.Write(" -jar fabric-server-launch.jar");
                else writer.Write(" -jar server.jar");
                writer.Write(IsNoGui ? " nogui" : "");
                writer.WriteLine(IsNewForge ? " %*" : "");
                writer.WriteLine("pause");
            }
        }
    }
}
