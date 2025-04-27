﻿using System;
using System.Collections.Generic;
using System.Linq;
using ReClassNET;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Game;

public static class AgentModule {
    public static nint Address => Ffxiv.Address.AgentModule;
    public static List<AgentInterface> AgentList { get; } = new();

    public static void Update() {
        AgentList.Clear();
        if (Address == 0) return;
        var uiModule = Ffxiv.Address.UiModule;
        if (uiModule == 0) return;
        for (var index = 0; index < 999; index++) {
            var ptr = Ffxiv.Memory.Read<nint>(Address + 0x20 + index * 8);
            if (ptr == 0 || ptr == uiModule) break;
            var agent = new AgentInterface(index, ptr);
            AgentList.Add(agent);
        }
    }
}

public class AgentInterface : IEquatable<AgentInterface>, IComparable<AgentInterface>, IComparable {
    public int AgentId { get; }
    public ulong Address { get; }
    public ulong VTableOffset { get; }
    public string ClassName { get; }
    public string Name { get; }
    public int Size { get; }

    public uint AddonId => Ffxiv.Memory.Read<uint>((nint)(Address + 0x20));
    public string AddonName => AtkUnitManager.TryGetAddonById(AddonId, out var addon) ? addon.Name : string.Empty;

    private readonly nint m_ShowAddress;
    private readonly nint m_HideAddress;

    public AgentInterface(int id, nint address) {
        AgentId = id;
        Address = (ulong)address;
        var vtable = Ffxiv.Memory.Read<nint>(address);
        VTableOffset = (ulong)Ffxiv.Memory.GetMainModuleOffset(vtable);
        ClassName = Ffxiv.Symbols.TryGetClassName(vtable, out var className, true) ? className : string.Empty;
        Name = Ffxiv.Symbols.TryGetClassName(vtable, out var name) ? name : string.Empty;
        Size = Ffxiv.Memory.TryGetSizeFromFunction(Ffxiv.Memory.Read<nint>(vtable + 2 * 8));
        m_ShowAddress = Ffxiv.Memory.Read<nint>(vtable + 3 * 8);
        m_HideAddress = Ffxiv.Memory.Read<nint>(vtable + 5 * 8);
    }

    public ClassNode? CreateClassNode() {
        if (Program.MainForm.CurrentProject.Classes.Any(c => c.Name.Equals(Name)))
            return null;

        var node = ClassNode.Create();
        node.AddressFormula = $"<Agent({AgentId})>";
        node.Name = $"Client::UI::Agent::Agent{AgentId}";

        if (!string.IsNullOrWhiteSpace(Name) && AgentModule.AgentList.Count(a => a.Name.Equals(Name)) == 1) {
            node.AddressFormula = $"<Agent({Name})>";
            node.Name = $"Client::UI::Agent::{Name}";
        }

        if (!string.IsNullOrEmpty(ClassName))
            node.Name = ClassName;

        var agentInterfaceNode = Program.MainForm.CurrentProject.Classes.FirstOrDefault(n => n.Name.Equals("Client::UI::Agent::AgentInterface"));
        if (agentInterfaceNode != null) {
            var instanceNode = new ClassInstanceNode();
            instanceNode.ChangeInnerNode(agentInterfaceNode);
            instanceNode.Name = "AgentInterface";
            node.AddNode(instanceNode);
            if (Size - 0x28 > 0)
                node.AddBytes(Size - 0x28);
        } else {
            node.AddBytes(Math.Max(0x28, Size));
        }

        return node;
    }

    public void Show() {
        if (m_ShowAddress == 0) return;
        Ffxiv.CreateRemoteThread(m_ShowAddress, (nint)Address);
    }

    public void Hide() {
        if (m_HideAddress == 0) return;
        Ffxiv.CreateRemoteThread(m_HideAddress, (nint)Address);
    }

    public bool Equals(AgentInterface? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return AgentId == other.AgentId;
    }

    public override bool Equals(object? obj) {
        return ReferenceEquals(this, obj) || (obj is AgentInterface other && Equals(other));
    }

    public override int GetHashCode() {
        return AgentId;
    }

    public static bool operator ==(AgentInterface? left, AgentInterface? right) {
        return Equals(left, right);
    }

    public static bool operator !=(AgentInterface? left, AgentInterface? right) {
        return !Equals(left, right);
    }

    public int CompareTo(AgentInterface? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return AgentId.CompareTo(other.AgentId);
    }

    public int CompareTo(object? obj) {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is AgentInterface other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(AgentInterface)}");
    }
}
