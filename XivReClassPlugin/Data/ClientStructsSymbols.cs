using System;
using System.Collections.Generic;
using System.Linq;
using ReClassNET;
using ReClassNET.Extensions;

namespace XivReClassPlugin.Data; 

public class ClientStructsSymbols {
	public const string AgentModuleInstance = "Client::UI::Agent::AgentModule_Instance";
	public readonly XivReClassPluginExt Plugin;
	public readonly Dictionary<nint, string> NamedAddresses = new();
	public readonly Dictionary<string, nint> InstanceNames = new();

	public ClientStructsSymbols(XivReClassPluginExt plugin) {
		Plugin = plugin;
	}

	public bool TryGetName(nint address, out string name) {
		return NamedAddresses.TryGetValue(address, out name);
	}

	public bool TryGetAddress(string name, out nint address) {
		return InstanceNames.TryGetValue(name, out address);
	}

	public string? GetRelativeAddressName(nint staticAddress) {
		if (staticAddress < Plugin.MainModule.Start || staticAddress > Plugin.MainModule.End)
			return null;
		return $"{Plugin.MainModule.Name}+{(staticAddress - Plugin.MainModule.Start).ToString("X")}";
	}

	public bool TryGetClassName(nint vtableAddress, out string className, bool includeNamespace = false) {
		className = string.Empty;
		var offset = vtableAddress - Plugin.MainModule.Start;
		if (offset <= 0 || offset == vtableAddress) return false;
		if (!Plugin.Data.TryGetClassByOffset((ulong)offset, out var info))
			return false;
		className = includeNamespace ? info.FullName : info.Name;
		return true;
	}

	public void Clear() {
		NamedAddresses.Clear();
		InstanceNames.Clear();
	}

	public void Reload() {
		Clear();
		if (!Program.RemoteProcess.IsValid) return;

		var pureCall = (nint)Plugin.Data.Data.Functions.FirstOrDefault(kv => kv.Value.Equals("_purecall")).Key;
		if (pureCall != 0) {
			pureCall = Plugin.MainModule.Start + pureCall;
			NamedAddresses[pureCall] = "_purecall";
		}

		foreach (var info in Plugin.Data.Classes) {
			var className = Plugin.Settings.ShowInheritance ? info.GetInheritanceName(Plugin.Settings.ShowNamespaces) : Plugin.Settings.ShowNamespaces ? info.FullName : info.Name;
			var classAddress = Plugin.MainModule.Start + (nint)info.Offset;

			NamedAddresses[classAddress] = className;

			foreach (var function in info.Functions)
				NamedAddresses[(nint)function.Key + Plugin.MainModule.Start] = function.Value;

			foreach (var instance in info.Instances)
				InstanceNames[instance.Value] = Plugin.MainModule.Start + (nint)instance.Key;

			var list = new List<ClassInfo>();
			var classInfo = info;
			while (classInfo != null) {
				list.Add(classInfo);
				classInfo = classInfo.ParentClass;
			}

			list.Reverse();
			list.ForEach(ci => {
				if (ci.Offset == 0) return;
				var vftable = Plugin.MainModule.Start + (nint)ci.Offset;
				foreach (var vf in ci.VirtualFunctions) {
					var addr = (nint)Program.RemoteProcess.ReadRemoteIntPtr(vftable + vf.Key * IntPtr.Size);
					if (addr != 0 && addr != pureCall && !NamedAddresses.ContainsKey(addr))
						NamedAddresses[addr] = $"{ci.Name}.{vf.Value}";
				}
			});
		}

		foreach (var kv in Plugin.Data.Data.Globals) {
			var address = Plugin.MainModule.Start + (nint)kv.Key;
			var name = kv.Value;
			InstanceNames[name] = address;
		}

		foreach (var kv in Plugin.Data.Data.Functions) {
			var address = Plugin.MainModule.Start + (nint)kv.Key;
			var name = kv.Value;
			NamedAddresses[address] = name;
		}

		var agentModule = GetAgentModule();
		if (agentModule != 0)
			InstanceNames[AgentModuleInstance] = agentModule;
	}

	public nint GetAgentModule() {
		if (!InstanceNames.TryGetValue("Client::System::Framework::Framework_InstancePointer2", out var fwPointer))
			return 0;
		var fwAddress = Program.RemoteProcess.ReadRemoteIntPtr(fwPointer);
		if (fwAddress == IntPtr.Zero)
			return 0;

		var uiVf = NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("Framework.GetUIModule", StringComparison.OrdinalIgnoreCase));
		var agentVf = NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("UiModule.GetAgentModule", StringComparison.OrdinalIgnoreCase));

		var data = new byte[8];
		if (!Program.RemoteProcess.ReadRemoteMemoryIntoBuffer(uiVf.Key + 8, ref data))
			return 0;
		var uiOffset = BitConverter.ToInt32(data, 3);

		if (!Program.RemoteProcess.ReadRemoteMemoryIntoBuffer(agentVf.Key, ref data))
			return 0;
		var agentOffset = BitConverter.ToInt32(data, 3);

		if (uiOffset <= 0 || agentOffset <= 0)
			return 0;

		var uiModule = Program.RemoteProcess.ReadRemoteIntPtr(fwAddress + uiOffset);
		var agentModule = uiModule + agentOffset;

		return agentModule;
	}
}
