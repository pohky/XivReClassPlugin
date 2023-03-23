﻿using System;
using System.Linq;
using ReClassNET.Memory;
using System.Text.RegularExpressions;
using HarmonyLib;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.Patches; 

[HarmonyPatch(typeof(RemoteProcess), nameof(RemoteProcess.GetModuleByName))]
public class ModuleNamePatch {
	private static readonly Regex AgentIdRegex = new("Agent\\((?<AgentId>\\d+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AgentNameRegex = new("Agent\\((?<AgentName>\\w+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static bool Prefix(ref Module __result, string name) {
		if (string.IsNullOrEmpty(name))
			return true;
		if (Ffxiv.Symbols.TryGetInstance(name, out var address)) {
			__result = new Module { Start = address, Name = name };
			return false;
		}

		var match = AgentIdRegex.Match(name);
		if (match.Success && Ffxiv.Address.AgentModule != 0) {
			if (!int.TryParse(match.Groups["AgentId"].Value, out var agentId))
				return true;
			var agentPtr = Ffxiv.Address.AgentModule + 0x20 + agentId * 8;
			var agent = Ffxiv.Memory.Read<nint>(agentPtr);
			__result = new Module { Start = agent, Name = name };
			return false;
		}

		match = AgentNameRegex.Match(name);
		if (match.Success && AgentModule.AgentList.Count != 0) {
			if (match.Groups["AgentName"].Value is not { } agentName)
				return true;
			var agent = AgentModule.AgentList.FirstOrDefault(ai => ai.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));
			if (agent == null)
				return true;
			__result = new Module { Start = (nint)agent.Address, Name = agent.Name };
			return false;
		}
		return true;
	}
}