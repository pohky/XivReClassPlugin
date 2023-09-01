using System.Collections.Generic;
using System.Linq;
using XivReClassPlugin.Data;

namespace XivReClassPlugin.Game.Memory;

public class Symbols {
	public Dictionary<nint, string> NamedAddresses { get; } = new();
	public Dictionary<string, nint> NamedInstances { get; } = new();

	public bool TryGetClassName(nint vtableAddress, out string className, bool namespaces = false, bool inheritance = false) {
		className = string.Empty;
		var offset = Ffxiv.Memory.GetMainModuleOffset(vtableAddress);
		if (offset <= 0 || offset == vtableAddress) return false;
		if (!DataManager.ClassMap.TryGetValue((ulong)offset, out var info))
			return false;
		if (inheritance)
			className = info.GetInheritanceName(namespaces);
		else className = namespaces ? info.FullName : info.Name;
		return true;
	}

	public bool TryGetName(nint address, out string name) {
		return NamedAddresses.TryGetValue(address, out name);
	}

	public bool TryGetInstance(string name, out nint address) {
		return NamedInstances.TryGetValue(name, out address);
	}

	public void Update() {
		NamedAddresses.Clear();
		NamedInstances.Clear();

        nint pureCall = 0;
        foreach (var func in DataManager.Data.Functions) {
            var addr = Ffxiv.Memory.MainModule.Start + (nint)func.Key;
			NamedAddresses[addr] = func.Value;
            if (func.Value.Equals("_purecall"))
                pureCall = addr;
        }

        foreach (var info in DataManager.Classes) {
			var className = Ffxiv.Settings.ShowInheritance ? info.GetInheritanceName(Ffxiv.Settings.ShowNamespaces) : Ffxiv.Settings.ShowNamespaces ? info.FullName : info.Name;
			var classAddress = Ffxiv.Memory.MainModule.Start + (nint)info.Offset;

			NamedAddresses[classAddress] = className;

			foreach (var function in info.Functions)
				NamedAddresses[(nint)function.Key + Ffxiv.Memory.MainModule.Start] = function.Value;

			foreach (var instance in info.Instances)
				NamedInstances[instance.Value] = Ffxiv.Memory.MainModule.Start + (nint)instance.Key;

			var list = new List<ClassInfo>();
			var classInfo = info;
			while (classInfo != null) {
				list.Add(classInfo);
				classInfo = classInfo.ParentClass;
			}

			list.Reverse();
			list.ForEach(ci => {
				if (ci.Offset == 0) return;
				var vftable = Ffxiv.Memory.MainModule.Start + (nint)ci.Offset;
				foreach (var vf in ci.VirtualFunctions) {
					var addr = Ffxiv.Memory.Read<nint>(vftable + vf.Key * 8);
					if (addr != 0 && addr != pureCall && !NamedAddresses.ContainsKey(addr))
						NamedAddresses[addr] = $"{ci.Name}.{vf.Value}";
				}
			});
		}

		Ffxiv.Memory.Process.NamedAddresses.Clear();
		if (!Ffxiv.Settings.UseNamedAddresses)
			return;
		foreach (var kv in NamedAddresses)
			Ffxiv.Memory.Process.NamedAddresses[kv.Key] = kv.Value;
	}
}
