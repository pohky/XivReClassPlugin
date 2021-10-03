using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using XivReClassPlugin.Data;

namespace XivReClassPlugin.Forms {
    public partial class PluginSettingsTab : UserControl {
        public PluginSettingsTab() {
            InitializeComponent();
            if (XivReClassPluginExt.Settings != null) {
                TextBoxDataFile.Text = XivReClassPluginExt.Settings.ClientStructsDataPath;
                CheckBoxShowOffset.Checked = XivReClassPluginExt.Settings.FallbackModuleOffset;
                CheckBoxNamespace.Checked = XivReClassPluginExt.Settings.ShowNamespaces;
                CheckBoxInheritance.Checked = XivReClassPluginExt.Settings.ShowFullInheritance;
            }
        }

        private void OpenDataButton_Click(object sender, EventArgs e) {
            DataFileDialog.ShowDialog();
        }

        private void DataFileDialog_FileOk(object sender, CancelEventArgs e) {
            if (XivReClassPluginExt.Settings == null) return;
            if (sender is OpenFileDialog d) {
                if (File.Exists(d.FileName)) {
                    XivReClassPluginExt.Settings.ClientStructsDataPath = d.FileName;
                    TextBoxDataFile.Text = d.FileName;
                    XivDataManager.Update();
                }
            }
        }

        private void TextBoxDataFile_TextChanged(object sender, EventArgs e) {
            if (XivReClassPluginExt.Settings == null) return;
            if (sender is TextBox tb) {
                XivReClassPluginExt.Settings.ClientStructsDataPath = tb.Text;
                XivDataManager.Update();
            }
        }

        private void CheckBoxShowOffset_CheckedChanged(object sender, EventArgs e) {
            if (XivReClassPluginExt.Settings == null) return;
            if (sender is CheckBox cb)
                XivReClassPluginExt.Settings.FallbackModuleOffset = cb.Checked;
        }

        private void CheckBoxNamespace_CheckedChanged(object sender, EventArgs e) {
            if (XivReClassPluginExt.Settings == null) return;
            if (sender is CheckBox cb)
                XivReClassPluginExt.Settings.ShowNamespaces = cb.Checked;
        }

        private void CheckBoxInheritance_CheckedChanged(object sender, EventArgs e)
        {
            if (XivReClassPluginExt.Settings == null) return;
            if (sender is CheckBox cb)
                XivReClassPluginExt.Settings.ShowFullInheritance = cb.Checked;
        }
    }
}