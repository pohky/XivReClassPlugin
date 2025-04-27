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

public partial class AddonListForm : IconForm {
    private readonly List<(Addon Addon, ListViewItem Item)> m_AddonList = new(256);
    private readonly List<(Addon Addon, ListViewItem Item)> m_DisplayList = new(256);

    public AddonListForm() {
        InitializeComponent();
        try {
            var dbProp = ListViewAddons.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dbProp?.SetValue(ListViewAddons, true, null);
        } catch { /* ignore */
        }
    }

    private bool AddonFilter(Addon addon, string text) {
        if (CheckBoxHideInvisible.Checked && !addon.Visible)
            return false;

        if (string.IsNullOrWhiteSpace(text))
            return true;
        if (addon.Name.ToUpperInvariant().Contains(text))
            return true;
        if (addon.ClassName.ToUpperInvariant().Contains(text))
            return true;
        if (text.Length <= 3 && int.TryParse(text, out var id) && id != 0)
            if (addon.Id == id || addon.ParentId == id)
                return true;

        return false;
    }

    private void UpdateList() {
        AtkUnitManager.Update();

        m_AddonList.Clear();
        foreach (var addon in AtkUnitManager.Addons) {
            var item = new ListViewItem(addon.Name);
            item.SubItems.Add(addon.Id.ToString());
            item.SubItems.Add(addon.ParentId == 0 ? string.Empty : addon.ParentId.ToString());
            item.SubItems.Add(addon.Address.ToString("X"));
            item.SubItems.Add(addon.Size.ToString("X"));
            item.SubItems.Add(addon.VTableOffset.ToString("X"));
            item.SubItems.Add(addon.ClassName);
            m_AddonList.Add((addon, item));
        }

        RefreshList();
    }

    private void RefreshList() {
        m_DisplayList.Clear();

        var searchText = TextBoxSearch.Text.ToUpperInvariant();
        m_DisplayList.AddRange(m_AddonList.Where(e => AddonFilter(e.Addon, searchText)));

        ListViewAddons.VirtualListSize = m_DisplayList.Count;
        ListViewAddons.Refresh();
    }

    private void ListUpdateTimer_Tick(object sender, EventArgs e) {
        if (AtkUnitManager.Addons.Count == 0) {
            m_AddonList.Clear();
            m_DisplayList.Clear();
            ListViewAddons.VirtualListSize = 0;
        } else {
            UpdateList();
        }
    }

    private void ButtonUpdateList_Click(object sender, EventArgs e) {
        ButtonUpdateList.Enabled = false;
        ListUpdateTimer.Stop();
        Ffxiv.Reload();
        UpdateList();
        ListUpdateTimer.Start();
        ButtonUpdateList.Enabled = true;
    }

    private void ListViewAddons_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
        var index = m_DisplayList[e.ItemIndex];
        e.Item = index.Item;
    }

    private void CreateClassMenuItem_Click(object sender, EventArgs e) {
        ClassNode? node = null;
        foreach (int idx in ListViewAddons.SelectedIndices) {
            var entry = m_DisplayList[idx];
            node = entry.Addon.CreateClassNode();
        }

        if (node != null)
            Program.MainForm.CurrentClassNode = node;
    }

    private void CopyOffsetMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAddons.SelectedIndices.Count == 0) return;
        var idx = ListViewAddons.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        var entry = m_DisplayList[idx];
        Clipboard.SetText($"{entry.Addon.VTableOffset:X}");
    }

    private void CopyAddressMenuItem_Click(object sender, EventArgs e) {
        if (ListViewAddons.SelectedIndices.Count == 0) return;
        var idx = ListViewAddons.SelectedIndices[0];
        if (idx >= m_DisplayList.Count || idx < 0) return;
        var entry = m_DisplayList[idx];
        Clipboard.SetText($"0x{entry.Addon.VTableOffset + DataManager.DataBaseAddress:X}");
    }
}
