namespace XivReClassPlugin.Forms
{
    partial class AddonListForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ColumnHeaderClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ListViewAddons = new System.Windows.Forms.ListView();
            this.ColumnHeaderParentId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderVTable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContextMenuAddon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CreateClassMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyOffsetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyAddressMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ButtonUpdateList = new System.Windows.Forms.Button();
            this.CheckBoxHideInvisible = new System.Windows.Forms.CheckBox();
            this.TextBoxSearch = new System.Windows.Forms.TextBox();
            this.FlowPanelToolBar = new System.Windows.Forms.FlowLayoutPanel();
            this.ContextMenuAddon.SuspendLayout();
            this.FlowPanelToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ColumnHeaderClass
            // 
            this.ColumnHeaderClass.Text = "Class";
            this.ColumnHeaderClass.Width = 260;
            // 
            // ColumnHeaderSize
            // 
            this.ColumnHeaderSize.Text = "Size";
            this.ColumnHeaderSize.Width = 80;
            // 
            // ColumnHeaderAddress
            // 
            this.ColumnHeaderAddress.Text = "Address";
            this.ColumnHeaderAddress.Width = 130;
            // 
            // ColumnHeaderId
            // 
            this.ColumnHeaderId.Text = "Id";
            this.ColumnHeaderId.Width = 40;
            // 
            // ColumnHeaderName
            // 
            this.ColumnHeaderName.Text = "Name";
            this.ColumnHeaderName.Width = 140;
            // 
            // ListViewAddons
            // 
            this.ListViewAddons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderName,
            this.ColumnHeaderId,
            this.ColumnHeaderParentId,
            this.ColumnHeaderAddress,
            this.ColumnHeaderSize,
            this.ColumnHeaderVTable,
            this.ColumnHeaderClass});
            this.ListViewAddons.ContextMenuStrip = this.ContextMenuAddon;
            this.ListViewAddons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListViewAddons.FullRowSelect = true;
            this.ListViewAddons.GridLines = true;
            this.ListViewAddons.HideSelection = false;
            this.ListViewAddons.Location = new System.Drawing.Point(0, 29);
            this.ListViewAddons.Margin = new System.Windows.Forms.Padding(2);
            this.ListViewAddons.Name = "ListViewAddons";
            this.ListViewAddons.Size = new System.Drawing.Size(800, 421);
            this.ListViewAddons.TabIndex = 6;
            this.ListViewAddons.UseCompatibleStateImageBehavior = false;
            this.ListViewAddons.View = System.Windows.Forms.View.Details;
            this.ListViewAddons.VirtualMode = true;
            this.ListViewAddons.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewAddons_RetrieveVirtualItem);
            // 
            // ColumnHeaderParentId
            // 
            this.ColumnHeaderParentId.Text = "Parent";
            this.ColumnHeaderParentId.Width = 50;
            // 
            // ColumnHeaderVTable
            // 
            this.ColumnHeaderVTable.Text = "VTable";
            this.ColumnHeaderVTable.Width = 130;
            // 
            // ContextMenuAddon
            // 
            this.ContextMenuAddon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuAddon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateClassMenuItem,
            this.CopyOffsetMenuItem,
            this.CopyAddressMenuItem});
            this.ContextMenuAddon.Name = "ContextMenuMain";
            this.ContextMenuAddon.Size = new System.Drawing.Size(185, 70);
            // 
            // CreateClassMenuItem
            // 
            this.CreateClassMenuItem.Name = "CreateClassMenuItem";
            this.CreateClassMenuItem.Size = new System.Drawing.Size(184, 22);
            this.CreateClassMenuItem.Text = "Create Class";
            this.CreateClassMenuItem.Click += new System.EventHandler(this.CreateClassMenuItem_Click);
            // 
            // CopyOffsetMenuItem
            // 
            this.CopyOffsetMenuItem.Name = "CopyOffsetMenuItem";
            this.CopyOffsetMenuItem.Size = new System.Drawing.Size(184, 22);
            this.CopyOffsetMenuItem.Text = "Copy VTable Offset";
            this.CopyOffsetMenuItem.Click += new System.EventHandler(this.CopyOffsetMenuItem_Click);
            // 
            // CopyAddressMenuItem
            // 
            this.CopyAddressMenuItem.Name = "CopyAddressMenuItem";
            this.CopyAddressMenuItem.Size = new System.Drawing.Size(184, 22);
            this.CopyAddressMenuItem.Text = "Copy VTable Address";
            this.CopyAddressMenuItem.Click += new System.EventHandler(this.CopyAddressMenuItem_Click);
            // 
            // ListUpdateTimer
            // 
            this.ListUpdateTimer.Enabled = true;
            this.ListUpdateTimer.Interval = 250;
            this.ListUpdateTimer.Tick += new System.EventHandler(this.ListUpdateTimer_Tick);
            // 
            // ButtonUpdateList
            // 
            this.ButtonUpdateList.Location = new System.Drawing.Point(3, 3);
            this.ButtonUpdateList.Name = "ButtonUpdateList";
            this.ButtonUpdateList.Size = new System.Drawing.Size(75, 23);
            this.ButtonUpdateList.TabIndex = 0;
            this.ButtonUpdateList.Text = "Update";
            this.ButtonUpdateList.UseVisualStyleBackColor = true;
            this.ButtonUpdateList.Click += new System.EventHandler(this.ButtonUpdateList_Click);
            // 
            // CheckBoxHideInvisible
            // 
            this.CheckBoxHideInvisible.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CheckBoxHideInvisible.AutoSize = true;
            this.CheckBoxHideInvisible.Location = new System.Drawing.Point(84, 6);
            this.CheckBoxHideInvisible.Name = "CheckBoxHideInvisible";
            this.CheckBoxHideInvisible.Size = new System.Drawing.Size(98, 17);
            this.CheckBoxHideInvisible.TabIndex = 1;
            this.CheckBoxHideInvisible.Text = "Hide not visible";
            this.CheckBoxHideInvisible.UseVisualStyleBackColor = true;
            // 
            // TextBoxSearch
            // 
            this.TextBoxSearch.Location = new System.Drawing.Point(188, 3);
            this.TextBoxSearch.Name = "TextBoxSearch";
            this.TextBoxSearch.Size = new System.Drawing.Size(220, 20);
            this.TextBoxSearch.TabIndex = 2;
            // 
            // FlowPanelToolBar
            // 
            this.FlowPanelToolBar.AutoSize = true;
            this.FlowPanelToolBar.Controls.Add(this.ButtonUpdateList);
            this.FlowPanelToolBar.Controls.Add(this.CheckBoxHideInvisible);
            this.FlowPanelToolBar.Controls.Add(this.TextBoxSearch);
            this.FlowPanelToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.FlowPanelToolBar.Location = new System.Drawing.Point(0, 0);
            this.FlowPanelToolBar.Name = "FlowPanelToolBar";
            this.FlowPanelToolBar.Size = new System.Drawing.Size(800, 29);
            this.FlowPanelToolBar.TabIndex = 5;
            // 
            // AddonListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ListViewAddons);
            this.Controls.Add(this.FlowPanelToolBar);
            this.Name = "AddonListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Addon List";
            this.ContextMenuAddon.ResumeLayout(false);
            this.FlowPanelToolBar.ResumeLayout(false);
            this.FlowPanelToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader ColumnHeaderClass;
        private System.Windows.Forms.ColumnHeader ColumnHeaderSize;
        private System.Windows.Forms.ColumnHeader ColumnHeaderAddress;
        private System.Windows.Forms.ColumnHeader ColumnHeaderId;
        private System.Windows.Forms.ColumnHeader ColumnHeaderName;
        private System.Windows.Forms.ListView ListViewAddons;
        private System.Windows.Forms.ColumnHeader ColumnHeaderVTable;
        private System.Windows.Forms.ContextMenuStrip ContextMenuAddon;
        private System.Windows.Forms.ToolStripMenuItem CreateClassMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyOffsetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyAddressMenuItem;
        private System.Windows.Forms.Timer ListUpdateTimer;
        private System.Windows.Forms.Button ButtonUpdateList;
        private System.Windows.Forms.CheckBox CheckBoxHideInvisible;
        private System.Windows.Forms.TextBox TextBoxSearch;
        private System.Windows.Forms.FlowLayoutPanel FlowPanelToolBar;
        private System.Windows.Forms.ColumnHeader ColumnHeaderParentId;
    }
}