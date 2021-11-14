using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace XivReClassPlugin.Forms {
    public partial class PluginSettingsTab : UserControl {
        public PluginSettingsTab() {
            InitializeComponent();
            TextBoxDataFile.Text = XivReClassPluginExt.Settings.ClientStructsDataPath;
            CheckBoxShowOffset.Checked = XivReClassPluginExt.Settings.FallbackModuleOffset;
            CheckBoxNamespace.Checked = XivReClassPluginExt.Settings.ShowNamespaces;
            CheckBoxNamespacePointer.Checked = XivReClassPluginExt.Settings.ShowNamespacesOnPointer;
            CheckBoxUseNamed.Checked = XivReClassPluginExt.Settings.UseNamedAddresses;
            CheckBoxInheritance.Checked = XivReClassPluginExt.Settings.ShowInheritance;
        }

        private void OpenDataButton_Click(object sender, EventArgs e) {
            DataFileDialog.ShowDialog();
        }

        private void DataFileDialog_FileOk(object sender, CancelEventArgs e) {
            if (sender is not OpenFileDialog dialog || !File.Exists(dialog.FileName))
                return;

            XivReClassPluginExt.Settings.ClientStructsDataPath = dialog.FileName;
            TextBoxDataFile.Text = dialog.FileName;
            XivReClassPluginExt.Update();
        }

        private void TextBoxDataFile_TextChanged(object sender, EventArgs e) {
            if (sender is not TextBox tb)
                return;
            XivReClassPluginExt.Settings.ClientStructsDataPath = tb.Text;
            XivReClassPluginExt.Update();
        }

        private void CheckBoxShowOffset_CheckedChanged(object sender, EventArgs e) {
            if (sender is CheckBox cb)
                XivReClassPluginExt.Settings.FallbackModuleOffset = cb.Checked;
        }

        private void CheckBoxNamespace_CheckedChanged(object sender, EventArgs e) {
            if (sender is not CheckBox cb)
                return;
            XivReClassPluginExt.Settings.ShowNamespaces = cb.Checked;
            XivReClassPluginExt.Update();
        }

        private void CheckBoxNamespacePointer_CheckedChanged(object sender, EventArgs e) {
            if (sender is CheckBox cb)
                XivReClassPluginExt.Settings.ShowNamespacesOnPointer = cb.Checked;
        }

        private void CheckBoxUseNamed_CheckedChanged(object sender, EventArgs e) {
            if (sender is not CheckBox cb)
                return;
            XivReClassPluginExt.Settings.UseNamedAddresses = cb.Checked;
            XivReClassPluginExt.Update();
        }

        private void CheckBoxInheritance_CheckedChanged(object sender, EventArgs e) {
            if (sender is not CheckBox cb)
                return;
            XivReClassPluginExt.Settings.ShowInheritance = cb.Checked;
            XivReClassPluginExt.Update();
        }
    }
}