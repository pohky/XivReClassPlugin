using HarmonyLib;
using ReClassNET.Memory;

namespace XivReClassPlugin.Patches; 

[HarmonyPatch(typeof(RemoteProcess), nameof(RemoteProcess.ReadRemoteRuntimeTypeInformation))]
public class RttiInfoPatch {
	private static void Postfix(ref string __result, nint address) {
		if (address <= 0x10_000 || Ffxiv.Settings.UseNamedAddresses)
			return;
		if (Ffxiv.Symbols.TryGetName(address, out var name))
			__result = name;
	}
}
