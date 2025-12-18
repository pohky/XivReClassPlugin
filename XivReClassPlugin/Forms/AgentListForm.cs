using System;
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
        } catch { /* ignore */
        }
    }

    private bool AgentFilter(AgentInterface agent, string text) {
        if (!string.IsNullOrWhiteSpace(text)) {
            if (!string.IsNullOrEmpty(agent.ClassName) && agent.ClassName.ToUpperInvariant().Contains(text))
                return true;
            if (!string.IsNullOrEmpty(agent.AddonName) && agent.AddonName.ToUpperInvariant().Contains(text))
                return true;
            if (text.Length <= 3 && int.TryParse(text, out var id) && id != 0)
                return agent.AgentId == id;
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

    private void UpdateList() {
        AtkUnitManager.Update();
        AgentModule.Update();

        if (m_AgentList.Count == 0)
            foreach (var agent in AgentModule.AgentList) {
                var item = new ListViewItem(agent.AgentId.ToString());
                item.SubItems.Add(agent.AddonName);
                item.SubItems.Add(agent.Address.ToString("X"));
                item.SubItems.Add(agent.Size.ToString("X"));
                item.SubItems.Add(agent.VTableOffset.ToString("X"));
                item.SubItems.Add(agent.ClassName);
                m_AgentList.Add((agent, item));
            }
        else
            foreach (var (agent, item) in m_AgentList) {
                //item.SubItems[0].Text = agent.AgentId.ToString();
                item.SubItems[1].Text = agent.AddonName;
                //item.SubItems[2].Text = agent.Address.ToString("X");
                item.SubItems[3].Text = agent.Size.ToString("X");
                item.SubItems[4].Text = agent.VTableOffset.ToString("X");
                item.SubItems[5].Text = agent.ClassName;
            }

        RefreshList();

        //m_AgentList.Clear();
        //m_DisplayList.Clear();
        //foreach (var agent in AgentModule.AgentList) {
        //	var item = new ListViewItem(agent.AgentId.ToString());
        //	item.SubItems.Add(agent.AddonName);
        //	item.SubItems.Add(agent.Address.ToString("X"));
        //	item.SubItems.Add(agent.Size.ToString("X"));
        //	item.SubItems.Add(agent.VTableOffset.ToString("X"));
        //	item.SubItems.Add(agent.ClassName);
        //	m_AgentList.Add((agent, item));
        //}
        //RefreshList();
        //ListViewAgents.AutoResizeColumns(ListViewAgents.VirtualListSize == 0 ? ColumnHeaderAutoResizeStyle.HeaderSize : ColumnHeaderAutoResizeStyle.ColumnContent);
    }

    private void RefreshList() {
        var searchText = TextBoxSearch.Text.ToUpperInvariant();
        m_DisplayList.Clear();
        m_DisplayList.AddRange(m_AgentList.Where(e => AgentFilter(e.Agent, searchText)));
        //m_DisplayList.ForEach(e => e.Item.SubItems[1].Text = e.Agent.AddonName);
        ListViewAgents.VirtualListSize = m_DisplayList.Count;
        ListViewAgents.Refresh();
    }

    private void ListUpdateTimer_Tick(object sender, EventArgs e) {
        if (AgentModule.AgentList.Count == 0) {
            m_AgentList.Clear();
            m_DisplayList.Clear();
            ListViewAgents.VirtualListSize = 0;
        } else {
            UpdateList();
        }
        //if (AgentModule.AgentList.Count == 0) {
        //	m_AgentList.Clear();
        //	m_DisplayList.Clear();
        //	ListViewAgents.VirtualListSize = 0;
        //} else {
        //	AtkUnitManager.Update();
        //	if (m_AgentList.Count == 0)
        //		UpdateList();
        //	else RefreshList();
        //}
    }

    private void ButtonUpdateList_Click(object sender, EventArgs e) {
        ButtonUpdateList.Enabled = false;
        ListUpdateTimer.Stop();
        m_AgentList.Clear();
        m_DisplayList.Clear();
        ListViewAgents.VirtualListSize = 0;
        Ffxiv.Reload();
        UpdateList();
        ListUpdateTimer.Start();
        ButtonUpdateList.Enabled = true;
    }

    private void TextBoxSearch_TextChanged(object sender, EventArgs e) {
        //UpdateList();
    }

    private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e) {
        //UpdateList();
    }

    private void ListViewAgents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
        var index = m_DisplayList[e.ItemIndex];
        e.Item = index.Item;
    }

    private void CreateClassMenuItem_Click(object sender, EventArgs e) {
        ClassNode? node = null;
        foreach (int idx in ListViewAgents.SelectedIndices) {
            var entry = m_DisplayList[idx];
            node = entry.Agent.CreateClassNode();
        }

        if (node != null)
            Program.MainForm.CurrentClassNode = node;
    }

    private void CopyOffsetMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAgents.SelectedIndices.Count == 0) return;
        var idx = ListViewAgents.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        var entry = m_DisplayList[idx];
        Clipboard.SetText($"{entry.Agent.VTableOffset:X}");
    }

    private void CopyAddressMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAgents.SelectedIndices.Count == 0) return;
        var idx = ListViewAgents.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        var entry = m_DisplayList[idx];
        Clipboard.SetText($"0x{entry.Agent.VTableOffset + DataManager.DataBaseAddress:X}");
    }

    private void ShowAgentMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAgents.SelectedIndices.Count == 0) return;
        var idx = ListViewAgents.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        m_DisplayList[idx].Agent.Show();
    }

    private void Show2AgentMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAgents.SelectedIndices.Count == 0) return;
        var idx = ListViewAgents.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        m_DisplayList[idx].Agent.Show2();
    }

    private void HideAgentMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAgents.SelectedIndices.Count == 0) return;
        var idx = ListViewAgents.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        m_DisplayList[idx].Agent.Hide();
    }
}
