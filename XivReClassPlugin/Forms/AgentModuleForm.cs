using System.Windows.Forms;
using BrightIdeasSoftware;
using ReClassNET;
using ReClassNET.Nodes;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.Forms; 

public partial class AgentModuleForm : Form {
	public AgentModuleForm() {
		InitializeComponent();

		ListViewAgents.ModelFilter = new ModelFilter(AgentFilter);
		ListViewAgents.UseFiltering = true;
		ListViewAgents.ContextMenuStrip = ContextMenuMain;

		InitList();
	}
	
	private bool AgentFilter(object obj) {
		if (obj is not AgentInterface agent)
			return true;
		if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text)) {
			var text = TextBoxSearch.Text.ToUpperInvariant();
			if (agent.ClassName.ToUpperInvariant().Contains(text))
				return true;
			if (agent.Address.ToString("X").Contains(text))
				return true;
			return false;
		}
		if (CheckBoxHideInactive.Checked)
			return agent.AddonId != 0;
		return true;
	}

	private void InitList() {
		if (AgentModule.AgentList.Count == 0)
			return;
		ListViewAgents.BeginUpdate();
		ListViewAgents.ClearObjects();
		ListViewAgents.AddObjects(AgentModule.AgentList);
		ListViewAgents.AutoResizeColumns(AgentModule.AgentList.Count == 0 ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
		ListViewAgents.EndUpdate();
	}

	private void UpdateList() {
		ListViewAgents.BeginUpdate();
		ListViewAgents.UpdateObjects(AgentModule.AgentList);
		ListViewAgents.AutoResizeColumns(ListViewAgents.GetItemCount() == 0 ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
		ListViewAgents.EndUpdate();
	}

	private void ListUpdateTimer_Tick(object sender, System.EventArgs e) {
		if (ListViewAgents.GetItemCount() == 0 || AgentModule.AgentList.Count == 0)
			InitList();
		UpdateList();
	}

	private void ButtonUpdateList_Click(object sender, System.EventArgs e) {
		Ffxiv.Update();
		InitList();
	}

	private void TextBoxSearch_TextChanged(object sender, System.EventArgs e) {
		UpdateList();
	}

	private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e) {
		UpdateList();
	}

	private void CreateClassMenuItem_Click(object sender, System.EventArgs e) {
		ClassNode? firstNode = null;
		foreach (var obj in ListViewAgents.SelectedObjects) {
			if (obj is not AgentInterface agent) continue;
			var newNode = agent.CreateClassNode();
			firstNode ??= newNode;
		}

		if (firstNode != null)
			Program.MainForm.CurrentClassNode = firstNode;
	}

	private void CopyOffsetMenuItem_Click(object sender, System.EventArgs e) {
		if (ListViewAgents.SelectedObjects.Count >= 1) {
			if (ListViewAgents.SelectedObjects[0] is AgentInterface agent)
				Clipboard.SetText($"{agent.VTableOffset:X}");
		}
	}
}
