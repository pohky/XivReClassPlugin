using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Game;

namespace XivReClassPlugin.NodeReaders;

public class XivClassNodeReader : INodeInfoReader {
    public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, nint nodeAddress, nint nodeValue) {
        if (nodeAddress == 0 || nodeValue <= 0x10_000)
            return null;

        if (Ffxiv.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue)) {
            if (!Ffxiv.Settings.GuessClassSizes)
                return null;
            var classSize = TryGetSizeForClass(nodeValue);
            return classSize > 0 ? $"(Size: 0x{classSize:X})" : null;
        }

        var ptr = reader.ReadRemoteIntPtr(nodeValue);
        if (ptr.MayBeValid() && Ffxiv.Symbols.TryGetClassName(ptr, out var className, Ffxiv.Settings.ShowNamespacesOnPointer, Ffxiv.Settings.ShowInheritanceOnPointer)) {
            if (!Ffxiv.Settings.ShowExcelSheetNames)
                return $"-> {className}";

            if (Ffxiv.Address.ExcelSheetVtable == 0 || Ffxiv.Address.ExcelSheetListVtable == 0)
                return $"-> {className}";

            var sheet = new ExcelSheet(nodeValue);
            if (!sheet.IsValid)
                return $"-> {className}";

            var sheetName = sheet.Name;
            if (string.IsNullOrEmpty(sheetName))
                return $"-> {className}";
            if (sheet.IsLinkedList)
                return $"-> LinkList<ExcelSheet>({sheetName})";
            return $"-> ExcelSheet({sheetName})";
        }

        if (Ffxiv.Settings.FallbackModuleOffset && !Ffxiv.Symbols.NamedAddresses.ContainsKey(nodeValue)) {
            var offset = Ffxiv.Memory.GetMainModuleOffset(nodeValue);
            if (offset == 0)
                return null;

            if (!Ffxiv.Settings.GuessClassSizes)
                return $"+{offset.ToString("X")}";

            var classSize = 0;
            if (Ffxiv.Memory.MightBeClass(nodeAddress)) 
                classSize = TryGetSizeForClass(nodeValue);

            return $"+{offset.ToString("X")}" + (classSize > 0 ? $" (Size: 0x{classSize:X})" : string.Empty);
        }

        return null;
    }

    private static int TryGetSizeForClass(nint nodeValue) {
        var vf0 = Ffxiv.Memory.Read<nint>(nodeValue);
        var classSize = Ffxiv.Memory.TryGetSizeFromFunction(vf0);
        if (classSize > 0)
            return classSize;

        if (classSize == 0) {
            var vf1 = Ffxiv.Memory.Read<nint>(nodeValue + 1 * 8);
            classSize = Ffxiv.Memory.TryGetSizeFromFunction(vf1);
            if (classSize > 0)
                return classSize;
        }

        if (classSize == -1) // might be a subclass that just calls the parent dtor
            return 0;

        if (!Ffxiv.Settings.TryGetSizeForEventInterfaces)
            return 0;

        var vf2 = Ffxiv.Memory.Read<nint>(nodeValue + 2 * 8);
        return Ffxiv.Memory.TryGetSizeFromFunction(vf2);
    }
}
