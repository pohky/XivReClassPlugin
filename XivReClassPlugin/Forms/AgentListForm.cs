using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ReClassNET;
using ReClassNET.Forms;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.Forms; 

public partial class AgentListForm : IconForm {
	private readonly List<(AgentInterface Agent, ListViewItem Item)> m_AgentList = new(500);
	private readonly List<(AgentInterface Agent, ListViewItem Item)> m_DisplayList = new(500);

	public AgentListForm() {
		InitializeComponent();
		try {
			var dbProp = ListViewAgents.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			dbProp?.SetValue(ListViewAgents, true, null);
		} catch{/* ignore */}

		InitList();
	}
	
	private bool AgentFilter(AgentInterface agent) {
		if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text)) {
			var text = TextBoxSearch.Text.ToUpperInvariant();
			if (agent.ClassName.ToUpperInvariant().Contains(text))
				return true;
			if (agent.Address.ToString("X").Contains(text))
				return true;
			if (agent.VTableOffset.ToString("X").Contains(text))
				return true;
			return false;
		}
		if (CheckBoxHideInactive.Checked)
			return agent.AddonId != 0;
		return true;
	}

	private void InitList() {
		AtkUnitManager.Update();
		AgentModule.Update();

		m_AgentList.Clear();
		m_DisplayList.Clear();
		foreach (var agent in AgentModule.AgentList) {
			var item = new ListViewItem($"{agent.AgentId}");
			item.SubItems.Add($"{agent.AddonName}");
			item.SubItems.Add($"{agent.Address:X}");
			item.SubItems.Add($"{agent.Size:X}");
			item.SubItems.Add($"{agent.VTableOffset:X}");
			item.SubItems.Add($"{agent.ClassName}");
			m_AgentList.Add((agent, item));
		}
		UpdateList();
		//ListViewAgents.AutoResizeColumns(ListViewAgents.VirtualListSize == 0 ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
	}

	private void UpdateList() {
		m_DisplayList.Clear();
		m_DisplayList.AddRange(m_AgentList.Where(e => AgentFilter(e.Agent)));
		
		var dirty = false;

		foreach (var (agent, item) in m_DisplayList) {
			var subAddon = item.SubItems[1];
			if (!subAddon.Text.Equals(agent.AddonName)) {
				subAddon.Text = agent.AddonName;
				dirty = true;
			}
		}
		//m_DisplayList.ForEach(e => e.Item.SubItems[1].Text = e.Agent.AddonName);
		ListViewAgents.VirtualListSize = m_DisplayList.Count;
		if (dirty)
			ListViewAgents.Refresh();
	}

	private void ListUpdateTimer_Tick(object sender, System.EventArgs e) {
		if (AgentModule.AgentList.Count == 0) {
			m_AgentList.Clear();
			m_DisplayList.Clear();
			ListViewAgents.VirtualListSize = 0;
		} else {
			AtkUnitManager.Update();
			if (m_AgentList.Count == 0)
				InitList();
			else UpdateList();
		}
	}

	private void ButtonUpdateList_Click(object sender, System.EventArgs e) {
		ButtonUpdateList.Enabled = false;
		Ffxiv.Reload();
		InitList();
		ButtonUpdateList.Enabled = true;
	}

	private void TextBoxSearch_TextChanged(object sender, System.EventArgs e) {
		UpdateList();
	}

	private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e) {
		UpdateList();
	}

	private void ListViewAgents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
		var index = m_DisplayList[e.ItemIndex];
		e.Item = index.Item;
	}

	private void CreateClassMenuItem_Click(object sender, System.EventArgs e) {
		ClassNode? node = null;
		foreach (int idx in ListViewAgents.SelectedIndices) {
			var entry = m_DisplayList[idx];
			node = entry.Agent.CreateClassNode();
		}
		if (node != null)
			Program.MainForm.CurrentClassNode = node;
	}

	private void CopyOffsetMenuItem_Click(object sender, System.EventArgs e) {
		if (ListViewAgents.SelectedIndices.Count == 0) return;
		var idx = ListViewAgents.SelectedIndices[0];
		if (idx >= m_DisplayList.Count || idx < 0) return;
		var entry = m_DisplayList[idx];
		Clipboard.SetText($"{entry.Agent.VTableOffset:X}");
	}

	private void CopyAddressMenuItem_Click(object sender, System.EventArgs e) {
		if (ListViewAgents.SelectedIndices.Count == 0) return;
		var idx = ListViewAgents.SelectedIndices[0];
		if (idx >= m_DisplayList.Count || idx < 0) return;
		var entry = m_DisplayList[idx];
		Clipboard.SetText($"0x{entry.Agent.VTableOffset + DataManager.DataBaseAddress:X}");
	}

	private void ShowAgentMenuItem_Click(object sender, System.EventArgs e) {
		if (ListViewAgents.SelectedIndices.Count == 0) return;
		var idx = ListViewAgents.SelectedIndices[0];
		if (idx >= m_DisplayList.Count || idx < 0) return;
		m_DisplayList[idx].Agent.Show();
	}

	private void HideAgentMenuItem_Click(object sender, System.EventArgs e) {
		if (ListViewAgents.SelectedIndices.Count == 0) return;
		var idx = ListViewAgents.SelectedIndices[0];
		if (idx >= m_DisplayList.Count || idx < 0) return;
		m_DisplayList[idx].Agent.Hide();
	}
}
