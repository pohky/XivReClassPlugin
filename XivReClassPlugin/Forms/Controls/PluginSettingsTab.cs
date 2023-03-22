using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace XivReClassPlugin.Forms.Controls;

public partial class PluginSettingsTab : UserControl {
	public PluginSettingsTab() {
		InitializeComponent();
		
		TextBoxDataFile.Text = Ffxiv.Settings.ClientStructsDataPath;
		TextBoxDataFile.TextChanged += TextBoxDataFile_TextChanged;
		TextBoxDataFile.KeyDown += TextBoxDataFile_KeyDown;

		CheckBoxShowOffset.Checked = Ffxiv.Settings.FallbackModuleOffset;
		CheckBoxShowOffset.CheckedChanged += CheckBoxShowOffset_CheckedChanged;

		CheckBoxNamespace.Checked = Ffxiv.Settings.ShowNamespaces;
		CheckBoxNamespace.CheckedChanged += CheckBoxNamespace_CheckedChanged;

		CheckBoxNamespacePointer.Checked = Ffxiv.Settings.ShowNamespacesOnPointer;
		CheckBoxNamespacePointer.CheckedChanged += CheckBoxNamespacePointer_CheckedChanged;

		RadioButtonNamedAddress.Checked = Ffxiv.Settings.UseNamedAddresses;
		RadioButtonRtti.Checked = !Ffxiv.Settings.UseNamedAddresses;

		CheckBoxInheritance.Checked = Ffxiv.Settings.ShowInheritance;
		CheckBoxInheritance.CheckedChanged += CheckBoxInheritance_CheckedChanged;
	}

	private void OpenDataButton_Click(object sender, EventArgs e) {
		DataFileDialog.ShowDialog();
	}

	private void ButtonReloadData_Click(object sender, EventArgs e) {
		Ffxiv.Settings.ClientStructsDataPath = TextBoxDataFile.Text;
		Ffxiv.Symbols.Update();
	}

	private void DataFileDialog_FileOk(object sender, CancelEventArgs e) {
		if (sender is not OpenFileDialog dialog || !File.Exists(dialog.FileName))
			return;

		Ffxiv.Settings.ClientStructsDataPath = dialog.FileName;
		TextBoxDataFile.Text = dialog.FileName;
	}

	private void TextBoxDataFile_TextChanged(object sender, EventArgs e) {
		if (sender is not TextBox tb)
			return;
		Ffxiv.Settings.ClientStructsDataPath = tb.Text;
		if (File.Exists(tb.Text))
			Ffxiv.Symbols.Update();
	}

	private void TextBoxDataFile_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode != Keys.Return)
			return;
		if (sender is not TextBox tb)
			return;
		Ffxiv.Settings.ClientStructsDataPath = tb.Text;
		Ffxiv.Symbols.Update();
	}

	private void TextBoxDataFile_Leave(object sender, EventArgs e) {
		if (sender is not TextBox tb)
			return;
		Ffxiv.Settings.ClientStructsDataPath = tb.Text;
		Ffxiv.Symbols.Update();
	}


	private void CheckBoxShowOffset_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cb)
			Ffxiv.Settings.FallbackModuleOffset = cb.Checked;
	}

	private void CheckBoxNamespacePointer_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cb)
			Ffxiv.Settings.ShowNamespacesOnPointer = cb.Checked;
	}


	private void CheckBoxNamespace_CheckedChanged(object sender, EventArgs e) {
		if (sender is not CheckBox cb)
			return;
		Ffxiv.Settings.ShowNamespaces = cb.Checked;
		Ffxiv.Symbols.Update();
	}

	private void CheckBoxInheritance_CheckedChanged(object sender, EventArgs e) {
		if (sender is not CheckBox cb)
			return;
		Ffxiv.Settings.ShowInheritance = cb.Checked;
		Ffxiv.Symbols.Update();
	}


	private void RadioButtonNamedAddress_Click(object sender, EventArgs e) {
		Ffxiv.Settings.UseNamedAddresses = true;
		Ffxiv.Symbols.Update();
	}

	private void RadioButtonRtti_Click(object sender, EventArgs e) {
		Ffxiv.Settings.UseNamedAddresses = false;
		Ffxiv.Symbols.Update();
	}
}
