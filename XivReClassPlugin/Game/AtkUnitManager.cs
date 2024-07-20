using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ReClassNET;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Game; 

public static unsafe class AtkUnitManager {
	public static List<Addon> Addons { get; } = new(100);

	public static bool TryGetAddonByName(string name, out Addon addon) {
		var value = Addons.FirstOrDefault(a => a.Name.Equals(name));
		addon = value ?? null!;
		return value != null;
	}

	public static bool TryGetAddonById(uint id, out Addon addon) {
		var value = Addons.FirstOrDefault(a => a.Id == id);
		addon = value ?? null!;
		return value != null;
	}

	public static void Update() {
		Addons.Clear();
		if (Ffxiv.Address.AtkUnitManager == 0) return;
		var unitList = Ffxiv.Memory.Read<AtkUnitList>(Ffxiv.Address.AtkUnitManager + AtkUnitList.AllUnitsListOffset);
		for (var i = 0; i < unitList.Length; i++) {
			var ptr = (nint)unitList.UnitArray[i];
			if (ptr == 0) continue;
			var unit = Ffxiv.Memory.Read<AtkUnitBase>(ptr);
			var addon = new Addon(ptr, unit);
			Addons.Add(addon);
		}
	}
}

public unsafe class Addon {
	public readonly AtkUnitBase UnitBase;
	public uint Id => UnitBase.Id;
	public uint ParentId => UnitBase.ParentId;
	public bool Visible => UnitBase.IsVisible;
	public ulong Address { get; }
	public string Name { get; }
	public int Size { get; }
	public ulong VTableOffset { get; }
	public string ClassName { get; }

	public Addon(nint address, AtkUnitBase unitBase) {
		Address = (ulong)address;
		UnitBase = unitBase;

		Name = Encoding.UTF8.GetString(unitBase.Name, 0x20);
		var idx = Name.IndexOf('\0');
		if (idx != -1)
			Name = Name.Remove(idx);

		var vtable = Ffxiv.Memory.Read<nint>(address);
		VTableOffset = (ulong)Ffxiv.Memory.GetMainModuleOffset(vtable);
		ClassName = Ffxiv.Symbols.TryGetClassName(vtable, out var className, true) ? className : string.Empty;

		Size = Ffxiv.Memory.TryGetSizeFromFunction(Ffxiv.Memory.Read<nint>(vtable + 0 * 8));
	}

	public ClassNode? CreateClassNode() {
		if (Program.MainForm.CurrentProject.Classes.Any(c => c.Name.Equals(Name)))
			return null;

		var node = ClassNode.Create();
		node.AddressFormula = $"<Addon({Name})>";
		node.Name = $"Client::UI::Addon{Name}";

		var atkUnitBaseNode = Program.MainForm.CurrentProject.Classes.FirstOrDefault(node => node.Name.Equals("Component::GUI::AtkUnitBase"));
		if (atkUnitBaseNode != null) {
			var instanceNode = new ClassInstanceNode();
			instanceNode.ChangeInnerNode(atkUnitBaseNode);
			instanceNode.Name = "AtkUnitBase";
			node.AddNode(instanceNode);
			if (Size - 0x230 > 0)
				node.AddBytes(Size - 0x230);
		} else {
			node.AddBytes(Math.Max(0x230, Size));
		}

		return node;
	}
}

[StructLayout(LayoutKind.Explicit, Size = 0x810)]
public unsafe struct AtkUnitList {
	public const int AllUnitsListOffset = 0x6900;

	[FieldOffset(0x00)] public nint VFTable;
	[FieldOffset(0x08)] public fixed ulong UnitArray[256];
	[FieldOffset(0x808)] public ushort Length;
}

[StructLayout(LayoutKind.Explicit, Size = 0x230)]
public unsafe struct AtkUnitBase {
	[FieldOffset(0x00)] public nint VFTable;
	[FieldOffset(0x08)] public fixed byte Name[0x20];
	
	[FieldOffset(0xC8)] public nint RootNode;

	[FieldOffset(0x170)] public nint AtkValues;
	
	[FieldOffset(0x190)] public uint Flags;

	[FieldOffset(0x1DC)] public ushort Id;
	[FieldOffset(0x1DE)] public ushort ParentId;

	public bool IsVisible => (Flags & 0x200000) != 0;
}
