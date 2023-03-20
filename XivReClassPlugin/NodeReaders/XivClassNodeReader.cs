using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;

namespace XivReClassPlugin.NodeReaders;

public class XivClassNodeReader : INodeInfoReader {
	public readonly XivReClassPluginExt Plugin;

	public XivClassNodeReader(XivReClassPluginExt plugin) {
		Plugin = plugin;
	}

	public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, nint nodeAddress, nint nodeValue) {
		if (nodeValue <= 0x10_000 || nodeAddress == 0)
			return null;

		if (Plugin.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue))
			return null;

		var ptr = reader.ReadRemoteIntPtr(nodeValue);
		if (ptr.MayBeValid() && Plugin.Symbols.TryGetClassName(ptr, out var className, Plugin.Settings.ShowNamespacesOnPointer))
			return $"-> {className}";

		if (Plugin.Settings.FallbackModuleOffset && !Plugin.Symbols.TryGetName(nodeValue, out _))
			return Plugin.Symbols.GetRelativeAddressName(nodeValue);
		return null;
	}
}
