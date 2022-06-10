using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInstaller.Programs.Files {
    class Eula {

        public static void CreateFile(string path) {

            using (StreamWriter writer = new StreamWriter(path + "\\eula.txt")) {
                writer.WriteLine("eula=true");
            }
        }
    }
}
