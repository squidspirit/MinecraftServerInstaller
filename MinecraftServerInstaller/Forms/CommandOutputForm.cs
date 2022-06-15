using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServerInstaller.Forms {
    public partial class CommandOutputForm : Form {

        public CommandOutputForm() {

            InitializeComponent();
        }

        public void TextBoxAppend(string line) {
            if (textBox.InvokeRequired) {
                TextBoxAppendCallback callback = new TextBoxAppendCallback(TextBoxAppend);
                this.Invoke(callback, new object[] { line });
            }
            else {
                textBox.AppendText(line + Environment.NewLine);
            }
        }

        public void Clear() {
            textBox.Clear();
        }

        delegate void TextBoxAppendCallback(string str);
    }
}
    