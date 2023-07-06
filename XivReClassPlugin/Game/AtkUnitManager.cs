using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

	public Addon(nint address, AtkUnitBase unitBase) {
		Address = (ulong)address;
		UnitBase = unitBase;
		Name = Encoding.UTF8.GetString(unitBase.Name, 0x20);
		var idx = Name.IndexOf('\0');
		if (idx != -1)
			Name = Name.Remove(idx);
	}
}

[StructLayout(LayoutKind.Explicit, Size = 0x810)]
public unsafe struct AtkUnitList {
	public const int AllUnitsListOffset = 0x6900;

	[FieldOffset(0x00)] public nint VFTable;
	[FieldOffset(0x08)] public fixed ulong UnitArray[100];
	[FieldOffset(0x808)] public int Length;
}

[StructLayout(LayoutKind.Explicit, Size = 0x220)]
public unsafe struct AtkUnitBase {
	[FieldOffset(0x00)] public nint VFTable;
	[FieldOffset(0x08)] public fixed byte Name[0x20];
	
	[FieldOffset(0xC8)] public nint RootNode;

	[FieldOffset(0x160)] public nint AtkValues;
	
	[FieldOffset(0x182)] public byte Flags;

	[FieldOffset(0x1CC)] public ushort Id;
	[FieldOffset(0x1CE)] public ushort ParentId;

	public bool IsVisible => (Flags & 0x20) == 0x20;
}
