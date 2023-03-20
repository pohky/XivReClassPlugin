using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using XivReClassPlugin.Data;

namespace XivReClassPlugin;

public static class HarmonyPatches {
	private const string HarmonyId = "reclass.plugin.ffxiv";
	private static XivReClassPluginExt Plugin = null!;
	private static Harmony? Harmony;
	private static readonly Regex AgentIdRegex = new("Agent\\((?<AgentId>\\d+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static void Setup(XivReClassPluginExt plugin) {
		Plugin = plugin;
		Harmony = new Harmony(HarmonyId);
		
		var moduleByName = new HarmonyMethod(AccessTools.Method(typeof(HarmonyPatches), nameof(GetModuleByName)));
		var rttiInfo = new HarmonyMethod(AccessTools.Method(typeof(HarmonyPatches), nameof(ReadRemoteRuntimeTypeInformation)));

		Harmony.Patch(AccessTools.Method(typeof(RemoteProcess), nameof(RemoteProcess.GetModuleByName)), moduleByName);
		Harmony.Patch(AccessTools.Method(typeof(RemoteProcess), nameof(RemoteProcess.ReadRemoteRuntimeTypeInformation)), null, rttiInfo);
	}

	public static void Remove() {
		Harmony?.UnpatchAll(HarmonyId);
	}

	private static void ReadRemoteRuntimeTypeInformation(ref string __result, nint address) {
		if (address <= 0x10_000 || Plugin.Settings.UseNamedAddresses)
			return;
		if (!Plugin.Symbols.TryGetName(address, out var name))
			return;
		__result = name;
	}

	private static bool GetModuleByName(ref Module __result, string name) {
		if (string.IsNullOrEmpty(name))
			return true;
		if (Plugin.Symbols.TryGetAddress(name, out var address)) {
			__result = new Module { Start = address, Name = name };
			return false;
		}

		var match = AgentIdRegex.Match(name);
		if (!match.Success || !Plugin.Symbols.TryGetAddress(ClientStructsSymbols.AgentModuleInstance, out var agentModule))
			return true;
		if (!int.TryParse(match.Groups["AgentId"].Value, out var agentId))
			return true;
		var agentPtr = agentModule + 0x20 + agentId * IntPtr.Size;
		var agent = Program.RemoteProcess.ReadRemoteIntPtr(agentPtr);
		__result = new Module { Start = agent, Name = name };
		return false;
	}
}
