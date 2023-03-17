using HarmonyLib;
using ReClassNET.Memory;
using ReClassNET;
using System.Text.RegularExpressions;
using System;
using ReClassNET.Extensions;

namespace XivReClassPlugin; 

[HarmonyPatch(typeof(RemoteProcess), nameof(RemoteProcess.GetModuleByName))]
public class ModuleNamePatch {
	private static readonly Regex AgentIdRegex = new("Agent\\((?<AgentId>\\d+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static bool Prefix(ref Module __result, string name) {
		if (XivReClassPluginExt.InternalInstanceNames.TryGetValue(name, out var address)) {
			__result = new Module {Start = address, Name = name};
			return false;
		}

		var match = AgentIdRegex.Match(name);
		if (!match.Success || !XivReClassPluginExt.InternalInstanceNames.TryGetValue("AgentModule_Instance", out var agentModule))
			return true;
		if (!int.TryParse(match.Groups["AgentId"].Value, out var agentId))
			return true;
		var agentPtr = agentModule + 0x20 + agentId * IntPtr.Size;
		var agent = Program.RemoteProcess.ReadRemoteIntPtr(agentPtr);
		__result = new Module {Start = agent, Name = name};
		return false;
	}
}

[HarmonyPatch(typeof(RemoteProcess), nameof(RemoteProcess.ReadRemoteRuntimeTypeInformation))]
public class RttiInfoPatch {
	public static bool Prefix(ref string __result, nint address) {
		if (address <= 0x10_000)
			return false;
		if (XivReClassPluginExt.Settings.UseNamedAddresses)
			return true;
		if (!XivReClassPluginExt.InternalNamedAddresses.TryGetValue(address, out var name))
			return true;
		__result = name;
		return false;
	}
}