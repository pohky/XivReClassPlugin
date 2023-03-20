using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace XivReClassPlugin.Forms;

public partial class PluginSettingsTab : UserControl {
	public readonly XivReClassPluginExt Plugin;

	public PluginSettingsTab(XivReClassPluginExt plugin) {
		InitializeComponent();
		Plugin = plugin;

		TextBoxDataFile.Text = Plugin.Settings.ClientStructsDataPath;
		TextBoxDataFile.TextChanged += TextBoxDataFile_TextChanged;
		TextBoxDataFile.KeyDown += TextBoxDataFile_KeyDown;

		CheckBoxShowOffset.Checked = Plugin.Settings.FallbackModuleOffset;
		CheckBoxShowOffset.CheckedChanged += CheckBoxShowOffset_CheckedChanged;

		CheckBoxNamespace.Checked = Plugin.Settings.ShowNamespaces;
		CheckBoxNamespace.CheckedChanged += CheckBoxNamespace_CheckedChanged;

		CheckBoxNamespacePointer.Checked = Plugin.Settings.ShowNamespacesOnPointer;
		CheckBoxNamespacePointer.CheckedChanged += CheckBoxNamespacePointer_CheckedChanged;

		RadioButtonNamedAddress.Checked = Plugin.Settings.UseNamedAddresses;
		RadioButtonRtti.Checked = !Plugin.Settings.UseNamedAddresses;

		CheckBoxInheritance.Checked = Plugin.Settings.ShowInheritance;
		CheckBoxInheritance.CheckedChanged += CheckBoxInheritance_CheckedChanged;
	}

	private void OpenDataButton_Click(object sender, EventArgs e) {
		DataFileDialog.ShowDialog();
	}

	private void ButtonReloadData_Click(object sender, EventArgs e) {
		Plugin.Settings.ClientStructsDataPath = TextBoxDataFile.Text;
		Plugin.Update();
	}

	private void DataFileDialog_FileOk(object sender, CancelEventArgs e) {
		if (sender is not OpenFileDialog dialog || !File.Exists(dialog.FileName))
			return;

		Plugin.Settings.ClientStructsDataPath = dialog.FileName;
		TextBoxDataFile.Text = dialog.FileName;
	}

	private void TextBoxDataFile_TextChanged(object sender, EventArgs e) {
		if (sender is not TextBox tb)
			return;
		Plugin.Settings.ClientStructsDataPath = tb.Text;
		if (File.Exists(tb.Text))
			Plugin.Update();
	}

	private void TextBoxDataFile_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode != Keys.Return)
			return;
		if (sender is not TextBox tb)
			return;
		Plugin.Settings.ClientStructsDataPath = tb.Text;
		Plugin.Update();
	}

	private void TextBoxDataFile_Leave(object sender, EventArgs e) {
		if (sender is not TextBox tb)
			return;
		Plugin.Settings.ClientStructsDataPath = tb.Text;
		Plugin.Update();
	}


	private void CheckBoxShowOffset_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cb)
			Plugin.Settings.FallbackModuleOffset = cb.Checked;
	}

	private void CheckBoxNamespacePointer_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cb)
			Plugin.Settings.ShowNamespacesOnPointer = cb.Checked;
	}


	private void CheckBoxNamespace_CheckedChanged(object sender, EventArgs e) {
		if (sender is not CheckBox cb)
			return;
		Plugin.Settings.ShowNamespaces = cb.Checked;
		Plugin.Update();
	}

	private void CheckBoxInheritance_CheckedChanged(object sender, EventArgs e) {
		if (sender is not CheckBox cb)
			return;
		Plugin.Settings.ShowInheritance = cb.Checked;
		Plugin.Update();
	}


	private void RadioButtonNamedAddress_Click(object sender, EventArgs e) {
		Plugin.Settings.UseNamedAddresses = true;
		Plugin.Update();
	}

	private void RadioButtonRtti_Click(object sender, EventArgs e) {
		Plugin.Settings.UseNamedAddresses = false;
		Plugin.Update();
	}
}
