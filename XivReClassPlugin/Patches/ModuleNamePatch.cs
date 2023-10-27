using System;
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
	private static readonly Regex AddonIdRegex = new("Addon\\((?<AddonId>\\d+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AddonNameRegex = new("Addon\\((?<AddonName>\\w+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static bool Prefix(ref Module __result, string name) {
		if (string.IsNullOrEmpty(name))
			return true;

		if (Ffxiv.Symbols.TryGetInstance(name, out var address)) {
			__result = new Module { Start = address, Name = name };
			return false;
		}

		if (name.Equals("InstanceContentDirector", StringComparison.OrdinalIgnoreCase)) {
			if (EventFramework.DirectorModule.ActiveContentDirector != 0) {
				__result = new Module { Start = EventFramework.DirectorModule.ActiveContentDirector, Name = name };
				return false;
			}
		}

        if (Ffxiv.Address.Framework != 0 && name.Equals("Framework", StringComparison.OrdinalIgnoreCase)) {
            __result = new Module { Start = Ffxiv.Address.Framework, Name = name };
            return false;
        }
		
        if (Ffxiv.Address.UiModule != 0 && name.Equals("UIModule", StringComparison.OrdinalIgnoreCase)) {
            __result = new Module { Start = Ffxiv.Address.UiModule, Name = name };
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

		var matchAddonId = AddonIdRegex.Match(name);
		var matchAddonName = AddonNameRegex.Match(name);
		if ((matchAddonId.Success || matchAddonName.Success) && Ffxiv.Address.AtkUnitManager != 0) {
			AtkUnitManager.Update();
			Addon? addon = null;

			if (matchAddonId.Success) {
				if (!uint.TryParse(matchAddonId.Groups["AddonId"].Value, out var addonId))
					return true;

				if (!AtkUnitManager.TryGetAddonById(addonId, out addon))
					return true;
			}
			else if (matchAddonName.Success) {
				if (matchAddonName.Groups["AddonName"].Value is not { } addonName)
					return true;

				if (!AtkUnitManager.TryGetAddonByName(addonName, out addon))
					return true;
			}

			if (addon != null) {
				__result = new Module { Start = (nint)addon.Address, Name = addon.Name };
				return false;
			}
		}

		return true;
	}
}
