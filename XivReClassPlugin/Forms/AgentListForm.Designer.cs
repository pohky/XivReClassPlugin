namespace XivReClassPlugin.Forms {
	partial class AgentListForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.FlowPanelToolBar = new System.Windows.Forms.FlowLayoutPanel();
			this.ButtonUpdateList = new System.Windows.Forms.Button();
			this.CheckBoxHideInactive = new System.Windows.Forms.CheckBox();
			this.TextBoxSearch = new System.Windows.Forms.TextBox();
			this.ListUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.ContextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CreateClassMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CopyOffsetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ShowAgentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.HideAgentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListViewAgents = new System.Windows.Forms.ListView();
			this.ColumnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAddon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderVTable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FlowPanelToolBar.SuspendLayout();
			this.ContextMenuMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// FlowPanelToolBar
			// 
			this.FlowPanelToolBar.AutoSize = true;
			this.FlowPanelToolBar.Controls.Add(this.ButtonUpdateList);
			this.FlowPanelToolBar.Controls.Add(this.CheckBoxHideInactive);
			this.FlowPanelToolBar.Controls.Add(this.TextBoxSearch);
			this.FlowPanelToolBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.FlowPanelToolBar.Location = new System.Drawing.Point(0, 0);
			this.FlowPanelToolBar.Margin = new System.Windows.Forms.Padding(4);
			this.FlowPanelToolBar.Name = "FlowPanelToolBar";
			this.FlowPanelToolBar.Size = new System.Drawing.Size(1067, 36);
			this.FlowPanelToolBar.TabIndex = 3;
			// 
			// ButtonUpdateList
			// 
			this.ButtonUpdateList.Location = new System.Drawing.Point(4, 4);
			this.ButtonUpdateList.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonUpdateList.Name = "ButtonUpdateList";
			this.ButtonUpdateList.Size = new System.Drawing.Size(100, 28);
			this.ButtonUpdateList.TabIndex = 0;
			this.ButtonUpdateList.Text = "Update";
			this.ButtonUpdateList.UseVisualStyleBackColor = true;
			this.ButtonUpdateList.Click += new System.EventHandler(this.ButtonUpdateList_Click);
			// 
			// CheckBoxHideInactive
			// 
			this.CheckBoxHideInactive.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.CheckBoxHideInactive.AutoSize = true;
			this.CheckBoxHideInactive.Checked = true;
			this.CheckBoxHideInactive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxHideInactive.Location = new System.Drawing.Point(112, 8);
			this.CheckBoxHideInactive.Margin = new System.Windows.Forms.Padding(4);
			this.CheckBoxHideInactive.Name = "CheckBoxHideInactive";
			this.CheckBoxHideInactive.Size = new System.Drawing.Size(144, 20);
			this.CheckBoxHideInactive.TabIndex = 1;
			this.CheckBoxHideInactive.Text = "Hide without Addon";
			this.CheckBoxHideInactive.UseVisualStyleBackColor = true;
			// 
			// TextBoxSearch
			// 
			this.TextBoxSearch.Location = new System.Drawing.Point(264, 4);
			this.TextBoxSearch.Margin = new System.Windows.Forms.Padding(4);
			this.TextBoxSearch.Name = "TextBoxSearch";
			this.TextBoxSearch.Size = new System.Drawing.Size(292, 22);
			this.TextBoxSearch.TabIndex = 2;
			this.TextBoxSearch.TextChanged += new System.EventHandler(this.TextBoxSearch_TextChanged);
			this.TextBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSearch_KeyDown);
			// 
			// ListUpdateTimer
			// 
			this.ListUpdateTimer.Enabled = true;
			this.ListUpdateTimer.Interval = 250;
			this.ListUpdateTimer.Tick += new System.EventHandler(this.ListUpdateTimer_Tick);
			// 
			// ContextMenuMain
			// 
			this.ContextMenuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.ContextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateClassMenuItem,
            this.CopyOffsetMenuItem,
            this.toolStripSeparator1,
            this.ShowAgentMenuItem,
            this.HideAgentMenuItem});
			this.ContextMenuMain.Name = "ContextMenuMain";
			this.ContextMenuMain.Size = new System.Drawing.Size(205, 106);
			// 
			// CreateClassMenuItem
			// 
			this.CreateClassMenuItem.Name = "CreateClassMenuItem";
			this.CreateClassMenuItem.Size = new System.Drawing.Size(204, 24);
			this.CreateClassMenuItem.Text = "Create Class";
			this.CreateClassMenuItem.Click += new System.EventHandler(this.CreateClassMenuItem_Click);
			// 
			// CopyOffsetMenuItem
			// 
			this.CopyOffsetMenuItem.Name = "CopyOffsetMenuItem";
			this.CopyOffsetMenuItem.Size = new System.Drawing.Size(204, 24);
			this.CopyOffsetMenuItem.Text = "Copy VTable Offset";
			this.CopyOffsetMenuItem.Click += new System.EventHandler(this.CopyOffsetMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
			// 
			// ShowAgentMenuItem
			// 
			this.ShowAgentMenuItem.Name = "ShowAgentMenuItem";
			this.ShowAgentMenuItem.Size = new System.Drawing.Size(204, 24);
			this.ShowAgentMenuItem.Text = "Show Agent";
			this.ShowAgentMenuItem.Click += new System.EventHandler(this.ShowAgentMenuItem_Click);
			// 
			// HideAgentMenuItem
			// 
			this.HideAgentMenuItem.Name = "HideAgentMenuItem";
			this.HideAgentMenuItem.Size = new System.Drawing.Size(204, 24);
			this.HideAgentMenuItem.Text = "Hide Agent";
			this.HideAgentMenuItem.Click += new System.EventHandler(this.HideAgentMenuItem_Click);
			// 
			// ListViewAgents
			// 
			this.ListViewAgents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderId,
            this.ColumnHeaderAddon,
            this.ColumnHeaderAddress,
            this.ColumnHeaderSize,
            this.ColumnHeaderVTable,
            this.ColumnHeaderClass});
			this.ListViewAgents.ContextMenuStrip = this.ContextMenuMain;
			this.ListViewAgents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListViewAgents.FullRowSelect = true;
			this.ListViewAgents.GridLines = true;
			this.ListViewAgents.HideSelection = false;
			this.ListViewAgents.Location = new System.Drawing.Point(0, 36);
			this.ListViewAgents.Name = "ListViewAgents";
			this.ListViewAgents.Size = new System.Drawing.Size(1067, 518);
			this.ListViewAgents.TabIndex = 4;
			this.ListViewAgents.UseCompatibleStateImageBehavior = false;
			this.ListViewAgents.View = System.Windows.Forms.View.Details;
			this.ListViewAgents.VirtualMode = true;
			this.ListViewAgents.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewAgents_RetrieveVirtualItem);
			// 
			// ColumnHeaderId
			// 
			this.ColumnHeaderId.Text = "ID";
			// 
			// ColumnHeaderAddon
			// 
			this.ColumnHeaderAddon.Text = "Addon";
			this.ColumnHeaderAddon.Width = 200;
			// 
			// ColumnHeaderAddress
			// 
			this.ColumnHeaderAddress.Text = "Address";
			this.ColumnHeaderAddress.Width = 120;
			// 
			// ColumnHeaderSize
			// 
			this.ColumnHeaderSize.Text = "Size";
			// 
			// ColumnHeaderVTable
			// 
			this.ColumnHeaderVTable.Text = "VTable";
			this.ColumnHeaderVTable.Width = 120;
			// 
			// ColumnHeaderClass
			// 
			this.ColumnHeaderClass.Text = "Class";
			this.ColumnHeaderClass.Width = 300;
			// 
			// AgentListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1067, 554);
			this.Controls.Add(this.ListViewAgents);
			this.Controls.Add(this.FlowPanelToolBar);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "AgentListForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Agent List";
			this.FlowPanelToolBar.ResumeLayout(false);
			this.FlowPanelToolBar.PerformLayout();
			this.ContextMenuMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.FlowLayoutPanel FlowPanelToolBar;
		private System.Windows.Forms.Button ButtonUpdateList;
		private System.Windows.Forms.CheckBox CheckBoxHideInactive;
		private System.Windows.Forms.TextBox TextBoxSearch;
		private System.Windows.Forms.Timer ListUpdateTimer;
		private System.Windows.Forms.ContextMenuStrip ContextMenuMain;
		private System.Windows.Forms.ToolStripMenuItem CreateClassMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CopyOffsetMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ShowAgentMenuItem;
		private System.Windows.Forms.ToolStripMenuItem HideAgentMenuItem;
		private System.Windows.Forms.ListView ListViewAgents;
		private System.Windows.Forms.ColumnHeader ColumnHeaderId;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAddon;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAddress;
		private System.Windows.Forms.ColumnHeader ColumnHeaderSize;
		private System.Windows.Forms.ColumnHeader ColumnHeaderVTable;
		private System.Windows.Forms.ColumnHeader ColumnHeaderClass;
	}
}