
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
            this.TextBoxDataFile = new System.Windows.Forms.TextBox();
            this.OpenDataButton = new System.Windows.Forms.Button();
            this.CheckBoxShowOffset = new System.Windows.Forms.CheckBox();
            this.CheckBoxNamespace = new System.Windows.Forms.CheckBox();
            this.CheckBoxNamespacePointer = new System.Windows.Forms.CheckBox();
            this.CheckBoxUseNamed = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataFileDialog
            // 
            this.DataFileDialog.DefaultExt = "yml";
            this.DataFileDialog.FileName = "data.yml";
            this.DataFileDialog.Filter = "YAML File|*.yml";
            this.DataFileDialog.InitialDirectory = "C:\\Users\\Pohky\\Documents\\GitHub\\FFXIVClientStructs\\ida\\";
            this.DataFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.DataFileDialog_FileOk);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.TextBoxDataFile);
            this.flowLayoutPanel1.Controls.Add(this.OpenDataButton);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxShowOffset);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxNamespace);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxNamespacePointer);
            this.flowLayoutPanel1.Controls.Add(this.CheckBoxUseNamed);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(426, 282);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // TextBoxDataFile
            // 
            this.TextBoxDataFile.Location = new System.Drawing.Point(3, 3);
            this.TextBoxDataFile.MinimumSize = new System.Drawing.Size(350, 20);
            this.TextBoxDataFile.Name = "TextBoxDataFile";
            this.TextBoxDataFile.Size = new System.Drawing.Size(350, 20);
            this.TextBoxDataFile.TabIndex = 0;
            this.TextBoxDataFile.TextChanged += new System.EventHandler(this.TextBoxDataFile_TextChanged);
            // 
            // OpenDataButton
            // 
            this.OpenDataButton.Location = new System.Drawing.Point(359, 3);
            this.OpenDataButton.MinimumSize = new System.Drawing.Size(30, 20);
            this.OpenDataButton.Name = "OpenDataButton";
            this.OpenDataButton.Size = new System.Drawing.Size(30, 20);
            this.OpenDataButton.TabIndex = 1;
            this.OpenDataButton.Text = "...";
            this.OpenDataButton.UseVisualStyleBackColor = true;
            this.OpenDataButton.Click += new System.EventHandler(this.OpenDataButton_Click);
            // 
            // CheckBoxShowOffset
            // 
            this.CheckBoxShowOffset.AutoSize = true;
            this.CheckBoxShowOffset.Location = new System.Drawing.Point(3, 29);
            this.CheckBoxShowOffset.Name = "CheckBoxShowOffset";
            this.CheckBoxShowOffset.Size = new System.Drawing.Size(186, 17);
            this.CheckBoxShowOffset.TabIndex = 2;
            this.CheckBoxShowOffset.Text = "Show Offset if Name is not known";
            this.CheckBoxShowOffset.UseVisualStyleBackColor = true;
            this.CheckBoxShowOffset.CheckedChanged += new System.EventHandler(this.CheckBoxShowOffset_CheckedChanged);
            // 
            // CheckBoxNamespace
            // 
            this.CheckBoxNamespace.AutoSize = true;
            this.CheckBoxNamespace.Location = new System.Drawing.Point(195, 29);
            this.CheckBoxNamespace.Name = "CheckBoxNamespace";
            this.CheckBoxNamespace.Size = new System.Drawing.Size(167, 17);
            this.CheckBoxNamespace.TabIndex = 3;
            this.CheckBoxNamespace.Text = "Show Namespace on Classes";
            this.CheckBoxNamespace.UseVisualStyleBackColor = true;
            this.CheckBoxNamespace.CheckedChanged += new System.EventHandler(this.CheckBoxNamespace_CheckedChanged);
            // 
            // CheckBoxNamespacePointer
            // 
            this.CheckBoxNamespacePointer.AutoSize = true;
            this.CheckBoxNamespacePointer.Location = new System.Drawing.Point(3, 52);
            this.CheckBoxNamespacePointer.Name = "CheckBoxNamespacePointer";
            this.CheckBoxNamespacePointer.Size = new System.Drawing.Size(177, 17);
            this.CheckBoxNamespacePointer.TabIndex = 4;
            this.CheckBoxNamespacePointer.Text = "Include Namespace on Pointers";
            this.CheckBoxNamespacePointer.UseVisualStyleBackColor = true;
            this.CheckBoxNamespacePointer.CheckedChanged += new System.EventHandler(this.CheckBoxNamespacePointer_CheckedChanged);
            // 
            // CheckBoxUseNamed
            // 
            this.CheckBoxUseNamed.AutoSize = true;
            this.CheckBoxUseNamed.Location = new System.Drawing.Point(186, 52);
            this.CheckBoxUseNamed.Name = "CheckBoxUseNamed";
            this.CheckBoxUseNamed.Size = new System.Drawing.Size(176, 17);
            this.CheckBoxUseNamed.TabIndex = 5;
            this.CheckBoxUseNamed.Text = "Use ReClass Named Addresses";
            this.CheckBoxUseNamed.UseVisualStyleBackColor = true;
            this.CheckBoxUseNamed.CheckedChanged += new System.EventHandler(this.CheckBoxUseNamed_CheckedChanged);
            // 
            // PluginSettingsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PluginSettingsTab";
            this.Size = new System.Drawing.Size(426, 282);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog DataFileDialog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox TextBoxDataFile;
        private System.Windows.Forms.Button OpenDataButton;
        private System.Windows.Forms.CheckBox CheckBoxShowOffset;
        private System.Windows.Forms.CheckBox CheckBoxNamespace;
        private System.Windows.Forms.CheckBox CheckBoxNamespacePointer;
        private System.Windows.Forms.CheckBox CheckBoxUseNamed;
    }
}
