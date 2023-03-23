namespace XivReClassPlugin.Forms {
	partial class AgentModuleForm {
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
			this.ListViewAgents = new BrightIdeasSoftware.FastObjectListView();
			this.ColumnAgentId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ColumnAddonId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ColumnAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ColumnAgentSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ColumnVtable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ColumnClassName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.FlowPanelToolBar.SuspendLayout();
			this.ContextMenuMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ListViewAgents)).BeginInit();
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
			this.FlowPanelToolBar.Name = "FlowPanelToolBar";
			this.FlowPanelToolBar.Size = new System.Drawing.Size(800, 29);
			this.FlowPanelToolBar.TabIndex = 3;
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
			// CheckBoxHideInactive
			// 
			this.CheckBoxHideInactive.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.CheckBoxHideInactive.AutoSize = true;
			this.CheckBoxHideInactive.Checked = true;
			this.CheckBoxHideInactive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxHideInactive.Location = new System.Drawing.Point(84, 6);
			this.CheckBoxHideInactive.Name = "CheckBoxHideInactive";
			this.CheckBoxHideInactive.Size = new System.Drawing.Size(119, 17);
			this.CheckBoxHideInactive.TabIndex = 1;
			this.CheckBoxHideInactive.Text = "Hide without Addon";
			this.CheckBoxHideInactive.UseVisualStyleBackColor = true;
			// 
			// TextBoxSearch
			// 
			this.TextBoxSearch.Location = new System.Drawing.Point(209, 3);
			this.TextBoxSearch.Name = "TextBoxSearch";
			this.TextBoxSearch.Size = new System.Drawing.Size(220, 20);
			this.TextBoxSearch.TabIndex = 2;
			this.TextBoxSearch.TextChanged += new System.EventHandler(this.TextBoxSearch_TextChanged);
			this.TextBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSearch_KeyDown);
			// 
			// ListUpdateTimer
			// 
			this.ListUpdateTimer.Enabled = true;
			this.ListUpdateTimer.Tick += new System.EventHandler(this.ListUpdateTimer_Tick);
			// 
			// ContextMenuMain
			// 
			this.ContextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateClassMenuItem,
            this.CopyOffsetMenuItem});
			this.ContextMenuMain.Name = "ContextMenuMain";
			this.ContextMenuMain.Size = new System.Drawing.Size(175, 48);
			// 
			// CreateClassMenuItem
			// 
			this.CreateClassMenuItem.Name = "CreateClassMenuItem";
			this.CreateClassMenuItem.Size = new System.Drawing.Size(174, 22);
			this.CreateClassMenuItem.Text = "Create Class";
			this.CreateClassMenuItem.Click += new System.EventHandler(this.CreateClassMenuItem_Click);
			// 
			// CopyOffsetMenuItem
			// 
			this.CopyOffsetMenuItem.Name = "CopyOffsetMenuItem";
			this.CopyOffsetMenuItem.Size = new System.Drawing.Size(174, 22);
			this.CopyOffsetMenuItem.Text = "Copy VTable Offset";
			this.CopyOffsetMenuItem.Click += new System.EventHandler(this.CopyOffsetMenuItem_Click);
			// 
			// ListViewAgents
			// 
			this.ListViewAgents.AllColumns.Add(this.ColumnAgentId);
			this.ListViewAgents.AllColumns.Add(this.ColumnAddonId);
			this.ListViewAgents.AllColumns.Add(this.ColumnAddress);
			this.ListViewAgents.AllColumns.Add(this.ColumnAgentSize);
			this.ListViewAgents.AllColumns.Add(this.ColumnVtable);
			this.ListViewAgents.AllColumns.Add(this.ColumnClassName);
			this.ListViewAgents.CellEditUseWholeCell = false;
			this.ListViewAgents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnAgentId,
            this.ColumnAddonId,
            this.ColumnAddress,
            this.ColumnAgentSize,
            this.ColumnVtable,
            this.ColumnClassName});
			this.ListViewAgents.Cursor = System.Windows.Forms.Cursors.Default;
			this.ListViewAgents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListViewAgents.FullRowSelect = true;
			this.ListViewAgents.GridLines = true;
			this.ListViewAgents.HideSelection = false;
			this.ListViewAgents.Location = new System.Drawing.Point(0, 29);
			this.ListViewAgents.Name = "ListViewAgents";
			this.ListViewAgents.ShowFilterMenuOnRightClick = false;
			this.ListViewAgents.ShowGroups = false;
			this.ListViewAgents.Size = new System.Drawing.Size(800, 421);
			this.ListViewAgents.TabIndex = 5;
			this.ListViewAgents.UseCompatibleStateImageBehavior = false;
			this.ListViewAgents.View = System.Windows.Forms.View.Details;
			this.ListViewAgents.VirtualMode = true;
			// 
			// ColumnAgentId
			// 
			this.ColumnAgentId.AspectName = "AgentId";
			this.ColumnAgentId.MaximumWidth = 40;
			this.ColumnAgentId.MinimumWidth = 40;
			this.ColumnAgentId.Text = "ID";
			this.ColumnAgentId.Width = 40;
			// 
			// ColumnAddonId
			// 
			this.ColumnAddonId.AspectName = "AddonId";
			this.ColumnAddonId.MinimumWidth = 60;
			this.ColumnAddonId.Text = "Addon";
			// 
			// ColumnAddress
			// 
			this.ColumnAddress.AspectName = "Address";
			this.ColumnAddress.AspectToStringFormat = "{0:X}";
			this.ColumnAddress.MinimumWidth = 60;
			this.ColumnAddress.Text = "Address";
			// 
			// ColumnAgentSize
			// 
			this.ColumnAgentSize.AspectName = "Size";
			this.ColumnAgentSize.AspectToStringFormat = "{0:X}";
			this.ColumnAgentSize.MinimumWidth = 50;
			this.ColumnAgentSize.Text = "Size";
			// 
			// ColumnVtable
			// 
			this.ColumnVtable.AspectName = "VTableOffset";
			this.ColumnVtable.AspectToStringFormat = "+{0:X}";
			this.ColumnVtable.MinimumWidth = 60;
			this.ColumnVtable.Text = "VTable";
			// 
			// ColumnClassName
			// 
			this.ColumnClassName.AspectName = "ClassName";
			this.ColumnClassName.MinimumWidth = 60;
			this.ColumnClassName.Text = "Class";
			// 
			// AgentModuleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.ListViewAgents);
			this.Controls.Add(this.FlowPanelToolBar);
			this.Name = "AgentModuleForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Agent List";
			this.FlowPanelToolBar.ResumeLayout(false);
			this.FlowPanelToolBar.PerformLayout();
			this.ContextMenuMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ListViewAgents)).EndInit();
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
		private BrightIdeasSoftware.FastObjectListView ListViewAgents;
		private BrightIdeasSoftware.OLVColumn ColumnAgentId;
		private BrightIdeasSoftware.OLVColumn ColumnAddonId;
		private BrightIdeasSoftware.OLVColumn ColumnAddress;
		private BrightIdeasSoftware.OLVColumn ColumnAgentSize;
		private BrightIdeasSoftware.OLVColumn ColumnVtable;
		private BrightIdeasSoftware.OLVColumn ColumnClassName;
	}
}