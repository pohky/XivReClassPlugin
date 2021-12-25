
namespace XivReClassPlugin.Forms
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
            this.DataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CheckBoxShowOffset = new System.Windows.Forms.CheckBox();
            this.CheckBoxNamespace = new System.Windows.Forms.CheckBox();
            this.CheckBoxNamespacePointer = new System.Windows.Forms.CheckBox();
            this.CheckBoxInheritance = new System.Windows.Forms.CheckBox();
            this.CheckBoxUseNamed = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OpenDataButton = new System.Windows.Forms.Button();
            this.TextBoxDataFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonReloadData = new System.Windows.Forms.Button();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxShowOffset);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxNamespace);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxNamespacePointer);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxInheritance);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxUseNamed);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 37);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(420, 242);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // CheckBoxShowOffset
            // 
            this.CheckBoxShowOffset.AutoSize = true;
            this.CheckBoxShowOffset.Location = new System.Drawing.Point(3, 3);
            this.CheckBoxShowOffset.Name = "CheckBoxShowOffset";
            this.CheckBoxShowOffset.Size = new System.Drawing.Size(186, 17);
            this.CheckBoxShowOffset.TabIndex = 2;
            this.CheckBoxShowOffset.Text = "Show Offset if Name is not known";
            this.CheckBoxShowOffset.UseVisualStyleBackColor = true;
            // 
            // CheckBoxNamespace
            // 
            this.CheckBoxNamespace.AutoSize = true;
            this.CheckBoxNamespace.Location = new System.Drawing.Point(195, 3);
            this.CheckBoxNamespace.Name = "CheckBoxNamespace";
            this.CheckBoxNamespace.Size = new System.Drawing.Size(167, 17);
            this.CheckBoxNamespace.TabIndex = 3;
            this.CheckBoxNamespace.Text = "Show Namespace on Classes";
            this.CheckBoxNamespace.UseVisualStyleBackColor = true;
            // 
            // CheckBoxNamespacePointer
            // 
            this.CheckBoxNamespacePointer.AutoSize = true;
            this.CheckBoxNamespacePointer.Location = new System.Drawing.Point(3, 26);
            this.CheckBoxNamespacePointer.Name = "CheckBoxNamespacePointer";
            this.CheckBoxNamespacePointer.Size = new System.Drawing.Size(177, 17);
            this.CheckBoxNamespacePointer.TabIndex = 4;
            this.CheckBoxNamespacePointer.Text = "Include Namespace on Pointers";
            this.CheckBoxNamespacePointer.UseVisualStyleBackColor = true;
            // 
            // CheckBoxInheritance
            // 
            this.CheckBoxInheritance.AutoSize = true;
            this.CheckBoxInheritance.Location = new System.Drawing.Point(186, 26);
            this.CheckBoxInheritance.Name = "CheckBoxInheritance";
            this.CheckBoxInheritance.Size = new System.Drawing.Size(179, 17);
            this.CheckBoxInheritance.TabIndex = 6;
            this.CheckBoxInheritance.Text = "Show full Inheritance on Classes";
            this.CheckBoxInheritance.UseVisualStyleBackColor = true;
            // 
            // CheckBoxUseNamed
            // 
            this.CheckBoxUseNamed.AutoSize = true;
            this.CheckBoxUseNamed.Location = new System.Drawing.Point(3, 49);
            this.CheckBoxUseNamed.Name = "CheckBoxUseNamed";
            this.CheckBoxUseNamed.Size = new System.Drawing.Size(176, 17);
            this.CheckBoxUseNamed.TabIndex = 5;
            this.CheckBoxUseNamed.Text = "Use ReClass Named Addresses";
            this.CheckBoxUseNamed.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(420, 28);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // OpenDataButton
            // 
            this.OpenDataButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenDataButton.Location = new System.Drawing.Point(360, 3);
            this.OpenDataButton.MinimumSize = new System.Drawing.Size(30, 20);
            this.OpenDataButton.Name = "OpenDataButton";
            this.OpenDataButton.Size = new System.Drawing.Size(30, 22);
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
            this.TextBoxDataFile.Size = new System.Drawing.Size(300, 20);
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
            this.ButtonReloadData.Location = new System.Drawing.Point(392, 3);
            this.ButtonReloadData.Name = "ButtonReloadData";
            this.ButtonReloadData.Size = new System.Drawing.Size(25, 22);
            this.ButtonReloadData.TabIndex = 13;
            this.ButtonReloadData.Text = "↻";
            this.ButtonReloadData.UseVisualStyleBackColor = true;
            this.ButtonReloadData.Click += new System.EventHandler(this.ButtonReloadData_Click);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 2;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(426, 282);
            this.mainTableLayoutPanel.TabIndex = 3;
            // 
            // PluginSettingsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "PluginSettingsTab";
            this.Size = new System.Drawing.Size(426, 282);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog DataFileDialog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox CheckBoxShowOffset;
        private System.Windows.Forms.CheckBox CheckBoxNamespace;
        private System.Windows.Forms.CheckBox CheckBoxNamespacePointer;
        private System.Windows.Forms.CheckBox CheckBoxUseNamed;
        private System.Windows.Forms.CheckBox CheckBoxInheritance;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button OpenDataButton;
        private System.Windows.Forms.TextBox TextBoxDataFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonReloadData;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
    }
}
