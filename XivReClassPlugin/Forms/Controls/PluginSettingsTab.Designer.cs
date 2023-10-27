
namespace XivReClassPlugin.Forms.Controls
{
    partial class PluginSettingsTab
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OpenDataButton = new System.Windows.Forms.Button();
            this.TextBoxDataFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonReloadData = new System.Windows.Forms.Button();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CheckBoxShowOffset = new System.Windows.Forms.CheckBox();
            this.CheckBoxNamespacePointer = new System.Windows.Forms.CheckBox();
            this.ConfigToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PanelOtherSettings = new System.Windows.Forms.Panel();
            this.GroupBoxClassNames = new System.Windows.Forms.GroupBox();
            this.CheckBoxNamespace = new System.Windows.Forms.CheckBox();
            this.CheckBoxInheritance = new System.Windows.Forms.CheckBox();
            this.RadioButtonRtti = new System.Windows.Forms.RadioButton();
            this.RadioButtonNamedAddress = new System.Windows.Forms.RadioButton();
            this.GroupBoxClassSize = new System.Windows.Forms.GroupBox();
            this.CheckBoxGuessClassSize = new System.Windows.Forms.CheckBox();
            this.CheckBoxGuessEventInterfaces = new System.Windows.Forms.CheckBox();
            this.CheckBoxShowExcelSheet = new System.Windows.Forms.CheckBox();
            this.TextBoxSizeNote = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.PanelOtherSettings.SuspendLayout();
            this.GroupBoxClassNames.SuspendLayout();
            this.GroupBoxClassSize.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataFileDialog
            // 
            this.DataFileDialog.DefaultExt = "yml";
            this.DataFileDialog.FileName = "data.yml";
            this.DataFileDialog.Filter = "YAML File|*.yml";
            this.DataFileDialog.InitialDirectory = "A:\\GitHub\\FFXIVClientStructs\\ida\\";
            this.DataFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.DataFileDialog_FileOk);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.64462F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.264462F));
            this.tableLayoutPanel1.Controls.Add(this.OpenDataButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TextBoxDataFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonReloadData, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(553, 28);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // OpenDataButton
            // 
            this.OpenDataButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenDataButton.Location = new System.Drawing.Point(470, 3);
            this.OpenDataButton.MinimumSize = new System.Drawing.Size(30, 20);
            this.OpenDataButton.Name = "OpenDataButton";
            this.OpenDataButton.Size = new System.Drawing.Size(38, 22);
            this.OpenDataButton.TabIndex = 12;
            this.OpenDataButton.Text = "...";
            this.OpenDataButton.UseVisualStyleBackColor = true;
            this.OpenDataButton.Click += new System.EventHandler(this.OpenDataButton_Click);
            // 
            // TextBoxDataFile
            // 
            this.TextBoxDataFile.AllowDrop = true;
            this.TextBoxDataFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxDataFile.Location = new System.Drawing.Point(61, 3);
            this.TextBoxDataFile.MinimumSize = new System.Drawing.Size(300, 20);
            this.TextBoxDataFile.Name = "TextBoxDataFile";
            this.TextBoxDataFile.Size = new System.Drawing.Size(403, 20);
            this.TextBoxDataFile.TabIndex = 11;
            this.TextBoxDataFile.Leave += new System.EventHandler(this.TextBoxDataFile_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(52, 28);
            this.label1.TabIndex = 10;
            this.label1.Text = "Data File:";
            // 
            // ButtonReloadData
            // 
            this.ButtonReloadData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonReloadData.Location = new System.Drawing.Point(514, 3);
            this.ButtonReloadData.Name = "ButtonReloadData";
            this.ButtonReloadData.Size = new System.Drawing.Size(36, 22);
            this.ButtonReloadData.TabIndex = 13;
            this.ButtonReloadData.Text = "↻";
            this.ButtonReloadData.UseVisualStyleBackColor = true;
            this.ButtonReloadData.Click += new System.EventHandler(this.ButtonReloadData_Click);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.CheckBoxShowOffset, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.CheckBoxNamespacePointer, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.PanelOtherSettings, 0, 3);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(559, 350);
            this.mainTableLayoutPanel.TabIndex = 3;
            // 
            // CheckBoxShowOffset
            // 
            this.CheckBoxShowOffset.AutoSize = true;
            this.CheckBoxShowOffset.Location = new System.Drawing.Point(3, 37);
            this.CheckBoxShowOffset.Name = "CheckBoxShowOffset";
            this.CheckBoxShowOffset.Size = new System.Drawing.Size(186, 17);
            this.CheckBoxShowOffset.TabIndex = 8;
            this.CheckBoxShowOffset.Text = "Show Offset if Name is not known";
            this.CheckBoxShowOffset.UseVisualStyleBackColor = true;
            // 
            // CheckBoxNamespacePointer
            // 
            this.CheckBoxNamespacePointer.AutoSize = true;
            this.CheckBoxNamespacePointer.Location = new System.Drawing.Point(3, 60);
            this.CheckBoxNamespacePointer.Name = "CheckBoxNamespacePointer";
            this.CheckBoxNamespacePointer.Size = new System.Drawing.Size(177, 17);
            this.CheckBoxNamespacePointer.TabIndex = 10;
            this.CheckBoxNamespacePointer.Text = "Include Namespace on Pointers";
            this.CheckBoxNamespacePointer.UseVisualStyleBackColor = true;
            // 
            // PanelOtherSettings
            // 
            this.PanelOtherSettings.Controls.Add(this.GroupBoxClassSize);
            this.PanelOtherSettings.Controls.Add(this.GroupBoxClassNames);
            this.PanelOtherSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelOtherSettings.Location = new System.Drawing.Point(3, 84);
            this.PanelOtherSettings.Name = "PanelOtherSettings";
            this.PanelOtherSettings.Size = new System.Drawing.Size(553, 263);
            this.PanelOtherSettings.TabIndex = 11;
            // 
            // GroupBoxClassNames
            // 
            this.GroupBoxClassNames.Controls.Add(this.CheckBoxShowExcelSheet);
            this.GroupBoxClassNames.Controls.Add(this.CheckBoxNamespace);
            this.GroupBoxClassNames.Controls.Add(this.CheckBoxInheritance);
            this.GroupBoxClassNames.Controls.Add(this.RadioButtonRtti);
            this.GroupBoxClassNames.Controls.Add(this.RadioButtonNamedAddress);
            this.GroupBoxClassNames.Location = new System.Drawing.Point(0, 3);
            this.GroupBoxClassNames.Name = "GroupBoxClassNames";
            this.GroupBoxClassNames.Size = new System.Drawing.Size(215, 154);
            this.GroupBoxClassNames.TabIndex = 14;
            this.GroupBoxClassNames.TabStop = false;
            this.GroupBoxClassNames.Text = "Class Name Display";
            // 
            // CheckBoxNamespace
            // 
            this.CheckBoxNamespace.AutoSize = true;
            this.CheckBoxNamespace.Location = new System.Drawing.Point(10, 65);
            this.CheckBoxNamespace.Name = "CheckBoxNamespace";
            this.CheckBoxNamespace.Size = new System.Drawing.Size(121, 17);
            this.CheckBoxNamespace.TabIndex = 13;
            this.CheckBoxNamespace.Text = "Include Namespace";
            this.CheckBoxNamespace.UseVisualStyleBackColor = true;
            // 
            // CheckBoxInheritance
            // 
            this.CheckBoxInheritance.AutoSize = true;
            this.CheckBoxInheritance.Location = new System.Drawing.Point(10, 88);
            this.CheckBoxInheritance.Name = "CheckBoxInheritance";
            this.CheckBoxInheritance.Size = new System.Drawing.Size(125, 17);
            this.CheckBoxInheritance.TabIndex = 14;
            this.CheckBoxInheritance.Text = "Show full Inheritance";
            this.CheckBoxInheritance.UseVisualStyleBackColor = true;
            // 
            // RadioButtonRtti
            // 
            this.RadioButtonRtti.AutoSize = true;
            this.RadioButtonRtti.Location = new System.Drawing.Point(10, 42);
            this.RadioButtonRtti.Name = "RadioButtonRtti";
            this.RadioButtonRtti.Size = new System.Drawing.Size(89, 17);
            this.RadioButtonRtti.TabIndex = 1;
            this.RadioButtonRtti.TabStop = true;
            this.RadioButtonRtti.Text = "Pseudo RTTI";
            this.RadioButtonRtti.UseVisualStyleBackColor = true;
            this.RadioButtonRtti.Click += new System.EventHandler(this.RadioButtonRtti_Click);
            // 
            // RadioButtonNamedAddress
            // 
            this.RadioButtonNamedAddress.AutoSize = true;
            this.RadioButtonNamedAddress.Location = new System.Drawing.Point(10, 19);
            this.RadioButtonNamedAddress.Name = "RadioButtonNamedAddress";
            this.RadioButtonNamedAddress.Size = new System.Drawing.Size(100, 17);
            this.RadioButtonNamedAddress.TabIndex = 0;
            this.RadioButtonNamedAddress.TabStop = true;
            this.RadioButtonNamedAddress.Text = "Named Address";
            this.RadioButtonNamedAddress.UseVisualStyleBackColor = true;
            this.RadioButtonNamedAddress.Click += new System.EventHandler(this.RadioButtonNamedAddress_Click);
            // 
            // GroupBoxClassSize
            // 
            this.GroupBoxClassSize.Controls.Add(this.TextBoxSizeNote);
            this.GroupBoxClassSize.Controls.Add(this.CheckBoxGuessEventInterfaces);
            this.GroupBoxClassSize.Controls.Add(this.CheckBoxGuessClassSize);
            this.GroupBoxClassSize.Location = new System.Drawing.Point(223, 3);
            this.GroupBoxClassSize.Name = "GroupBoxClassSize";
            this.GroupBoxClassSize.Size = new System.Drawing.Size(241, 154);
            this.GroupBoxClassSize.TabIndex = 15;
            this.GroupBoxClassSize.TabStop = false;
            this.GroupBoxClassSize.Text = "Class Sizes";
            // 
            // CheckBoxGuessClassSize
            // 
            this.CheckBoxGuessClassSize.AutoSize = true;
            this.CheckBoxGuessClassSize.Location = new System.Drawing.Point(6, 20);
            this.CheckBoxGuessClassSize.Name = "CheckBoxGuessClassSize";
            this.CheckBoxGuessClassSize.Size = new System.Drawing.Size(140, 17);
            this.CheckBoxGuessClassSize.TabIndex = 0;
            this.CheckBoxGuessClassSize.Text = "Try to guess Class Sizes";
            this.CheckBoxGuessClassSize.UseVisualStyleBackColor = true;
            // 
            // CheckBoxGuessEventInterfaces
            // 
            this.CheckBoxGuessEventInterfaces.AutoSize = true;
            this.CheckBoxGuessEventInterfaces.Location = new System.Drawing.Point(6, 42);
            this.CheckBoxGuessEventInterfaces.Name = "CheckBoxGuessEventInterfaces";
            this.CheckBoxGuessEventInterfaces.Size = new System.Drawing.Size(154, 17);
            this.CheckBoxGuessEventInterfaces.TabIndex = 1;
            this.CheckBoxGuessEventInterfaces.Text = "Try for AtkEventInterfaces*";
            this.CheckBoxGuessEventInterfaces.UseVisualStyleBackColor = true;
            // 
            // CheckBoxShowExcelSheet
            // 
            this.CheckBoxShowExcelSheet.AutoSize = true;
            this.CheckBoxShowExcelSheet.Location = new System.Drawing.Point(10, 111);
            this.CheckBoxShowExcelSheet.Name = "CheckBoxShowExcelSheet";
            this.CheckBoxShowExcelSheet.Size = new System.Drawing.Size(176, 17);
            this.CheckBoxShowExcelSheet.TabIndex = 15;
            this.CheckBoxShowExcelSheet.Text = "Try showing ExcelSheet Names";
            this.CheckBoxShowExcelSheet.UseVisualStyleBackColor = true;
            // 
            // TextBoxSizeNote
            // 
            this.TextBoxSizeNote.BackColor = System.Drawing.SystemColors.Control;
            this.TextBoxSizeNote.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxSizeNote.Enabled = false;
            this.TextBoxSizeNote.Location = new System.Drawing.Point(6, 75);
            this.TextBoxSizeNote.Multiline = true;
            this.TextBoxSizeNote.Name = "TextBoxSizeNote";
            this.TextBoxSizeNote.ReadOnly = true;
            this.TextBoxSizeNote.Size = new System.Drawing.Size(229, 63);
            this.TextBoxSizeNote.TabIndex = 2;
            this.TextBoxSizeNote.Text = "Guesses are based on an attempt to read the size from the virtual destructor.\r\nIt" +
    "\'s not guaranteed to be successful or always correct.";
            // 
            // PluginSettingsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "PluginSettingsTab";
            this.Size = new System.Drawing.Size(559, 350);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.PanelOtherSettings.ResumeLayout(false);
            this.GroupBoxClassNames.ResumeLayout(false);
            this.GroupBoxClassNames.PerformLayout();
            this.GroupBoxClassSize.ResumeLayout(false);
            this.GroupBoxClassSize.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog DataFileDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button OpenDataButton;
        private System.Windows.Forms.TextBox TextBoxDataFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonReloadData;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.CheckBox CheckBoxShowOffset;
        private System.Windows.Forms.CheckBox CheckBoxNamespacePointer;
		private System.Windows.Forms.ToolTip ConfigToolTip;
        private System.Windows.Forms.Panel PanelOtherSettings;
        private System.Windows.Forms.GroupBox GroupBoxClassSize;
        private System.Windows.Forms.GroupBox GroupBoxClassNames;
        private System.Windows.Forms.CheckBox CheckBoxNamespace;
        private System.Windows.Forms.CheckBox CheckBoxInheritance;
        private System.Windows.Forms.RadioButton RadioButtonRtti;
        private System.Windows.Forms.RadioButton RadioButtonNamedAddress;
        private System.Windows.Forms.CheckBox CheckBoxGuessEventInterfaces;
        private System.Windows.Forms.CheckBox CheckBoxGuessClassSize;
        private System.Windows.Forms.CheckBox CheckBoxShowExcelSheet;
        private System.Windows.Forms.TextBox TextBoxSizeNote;
    }
}
