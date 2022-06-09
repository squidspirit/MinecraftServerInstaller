using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServerInstaller {
    public partial class VersionSelectForm : Form {

        private string result = null;

        public VersionSelectForm(string description, string[] versions, string selected) {

            InitializeComponent();
            descriptionLabel.Text = description;
            versionsComboBox.Items.AddRange(versions);
            versionsComboBox.MaxDropDownItems = 8;
            if (selected == null || selected.Length == 0)
                versionsComboBox.SelectedIndex = 0;
            else versionsComboBox.SelectedItem = selected;
        }

        private void okButton_Click(object sender, EventArgs e) {

            result = (string)versionsComboBox.SelectedItem;
            this.Dispose();
        }

        private void cancelButton_Click(object sender, EventArgs e) {

            this.Dispose();
        }



        public string Result => result;
    }
}
