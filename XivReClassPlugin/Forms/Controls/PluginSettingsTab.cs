using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace XivReClassPlugin.Forms.Controls;

public partial class PluginSettingsTab : UserControl {
    public PluginSettingsTab() {
        InitializeComponent();

        TextBoxDataFile.Text = Ffxiv.Settings.ClientStructsDataPath;
        TextBoxDataFile.TextChanged += TextBoxDataFile_TextChanged;
        TextBoxDataFile.KeyDown += TextBoxDataFile_KeyDown;
        if (!File.Exists(Ffxiv.Settings.ClientStructsDataPath))
            TextBoxDataFile.BackColor = Color.IndianRed;

        CheckBoxShowOffset.Checked = Ffxiv.Settings.FallbackModuleOffset;
        CheckBoxShowOffset.CheckedChanged += CheckBoxShowOffset_CheckedChanged;

        CheckBoxNamespace.Checked = Ffxiv.Settings.ShowNamespaces;
        CheckBoxNamespace.CheckedChanged += CheckBoxNamespace_CheckedChanged;

        CheckBoxNamespacePointer.Checked = Ffxiv.Settings.ShowNamespacesOnPointer;
        CheckBoxNamespacePointer.CheckedChanged += CheckBoxNamespacePointer_CheckedChanged;

        CheckBoxInheritancePointer.Checked = Ffxiv.Settings.ShowInheritanceOnPointer;
        CheckBoxInheritancePointer.CheckedChanged += CheckBoxInheritancePointerOnCheckedChanged;

        RadioButtonNamedAddress.Checked = Ffxiv.Settings.UseNamedAddresses;
        RadioButtonRtti.Checked = !Ffxiv.Settings.UseNamedAddresses;

        CheckBoxInheritance.Checked = Ffxiv.Settings.ShowInheritance;
        CheckBoxInheritance.CheckedChanged += CheckBoxInheritance_CheckedChanged;

        CheckBoxGuessClassSize.Checked = Ffxiv.Settings.GuessClassSizes;
        CheckBoxGuessClassSize.CheckedChanged += CheckBoxGuessClassSizeOnCheckedChanged;

        CheckBoxGuessEventInterfaces.Checked = Ffxiv.Settings.TryGetSizeForEventInterfaces;
        CheckBoxGuessEventInterfaces.CheckedChanged += CheckBoxGuessEventInterfacesOnCheckedChanged;

        CheckBoxShowExcelSheet.Checked = Ffxiv.Settings.ShowExcelSheetNames;
        CheckBoxShowExcelSheet.CheckedChanged += CheckBoxShowExcelSheetOnCheckedChanged;

        CheckBoxDecodeStrings.Checked = Ffxiv.Settings.DecodeUtf8Strings;
        CheckBoxDecodeStrings.CheckedChanged += CheckBoxDecodeStringsOnCheckedChanged;

        const string sizeWarningText = @"NOTE: increased chance for wrong guesses in some cases.";
        ConfigToolTip.SetToolTip(CheckBoxGuessEventInterfaces, sizeWarningText);
    }

    private void CheckBoxDecodeStringsOnCheckedChanged(object sender, EventArgs e) {
        if (sender is CheckBox cb)
            Ffxiv.Settings.DecodeUtf8Strings = cb.Checked;
    }

    private void CheckBoxShowExcelSheetOnCheckedChanged(object sender, EventArgs e) {
        if (sender is CheckBox cb)
            Ffxiv.Settings.ShowExcelSheetNames = cb.Checked;
    }

    private void CheckBoxGuessEventInterfacesOnCheckedChanged(object sender, EventArgs e) {
        if (sender is CheckBox cb)
            Ffxiv.Settings.TryGetSizeForEventInterfaces = cb.Checked;
    }

    private void CheckBoxGuessClassSizeOnCheckedChanged(object sender, EventArgs e) {
        if (sender is CheckBox cb)
            Ffxiv.Settings.GuessClassSizes = cb.Checked;
    }

    private void OpenDataButton_Click(object sender, EventArgs e) {
        DataFileDialog.ShowDialog();
    }

    private void ButtonReloadData_Click(object sender, EventArgs e) {
        Ffxiv.Settings.ClientStructsDataPath = TextBoxDataFile.Text;
        Ffxiv.Reload();
    }

    private void DataFileDialog_FileOk(object sender, CancelEventArgs e) {
        if (sender is not OpenFileDialog dialog || !File.Exists(dialog.FileName))
            return;
        TextBoxDataFile.TextChanged -= TextBoxDataFile_TextChanged;
        TextBoxDataFile.Text = dialog.FileName;
        TextBoxDataFile.ResetBackColor();
        TextBoxDataFile.TextChanged += TextBoxDataFile_TextChanged;
        Ffxiv.Settings.ClientStructsDataPath = dialog.FileName;
        Ffxiv.Reload();
    }

    private void TextBoxDataFile_TextChanged(object sender, EventArgs e) {
        if (sender is not TextBox tb)
            return;
        var changed = !Ffxiv.Settings.ClientStructsDataPath.Equals(tb.Text.Trim());
        if (changed && File.Exists(tb.Text)) {
            tb.ResetBackColor();
            Ffxiv.Settings.ClientStructsDataPath = tb.Text.Trim();
            Ffxiv.Reload();
        }
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

    private void CheckBoxInheritancePointerOnCheckedChanged(object sender, EventArgs e) {
        if (sender is CheckBox cb)
            Ffxiv.Settings.ShowInheritanceOnPointer = cb.Checked;
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
