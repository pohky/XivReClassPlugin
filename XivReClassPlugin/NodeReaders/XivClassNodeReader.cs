using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.NodeReaders;

public class XivClassNodeReader : INodeInfoReader {
    public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, nint nodeAddress, nint nodeValue) {
        if (nodeValue <= 0x10_000 || nodeAddress == 0)
            return null;

        if (Ffxiv.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue))
            return null;

        var ptr = reader.ReadRemoteIntPtr(nodeValue);
        if (ptr.MayBeValid() && Ffxiv.Symbols.TryGetClassName(ptr, out var className, Ffxiv.Settings.ShowNamespacesOnPointer)) {
            if (className.EndsWith("ExcelSheet")) {
                var sheet = new ExcelSheet(nodeValue);
                var sheetName = sheet.Name;
                if (!string.IsNullOrWhiteSpace(sheetName)) {
                    if (sheet.IsLinkedList)
                        return $"-> LinkedList<ExcelSheet>({sheetName})";
                    return $"-> ExcelSheet({sheetName})";
                }
            }
            return $"-> {className}";
        }

        if (Ffxiv.Settings.FallbackModuleOffset && !Ffxiv.Symbols.NamedAddresses.ContainsKey(nodeValue)) {
            var classSize = 0;
            if (Ffxiv.Memory.MightBeClass(nodeAddress))
                classSize = Ffxiv.Memory.TryGetSizeFromFunction(Ffxiv.Memory.Read<nint>(nodeValue));

            var offset = Ffxiv.Memory.GetMainModuleOffset(nodeValue);
            if (offset != 0)
                return $"{Ffxiv.Memory.MainModule.Name}+{offset.ToString("X")}" + (classSize == 0 ? string.Empty : $" (Size: 0x{classSize:X})");
        }

        return null;
    }
}
