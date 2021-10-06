using System;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;

namespace XivReClassPlugin {
    public class XivClassNodeReader : INodeInfoReader {
        public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, IntPtr nodeAddress, IntPtr nodeValue) {
            if (!XivDataManager.HasData || nodeValue == IntPtr.Zero)
                return null;

            var info = GetNameForAddress(nodeValue, XivReClassPluginExt.Settings.ShowFullInheritance);
            if (string.IsNullOrEmpty(info)) {
                var ptr = reader.ReadRemoteIntPtr(nodeValue);
                if (ptr.MayBeValid()) {
                    info = GetNameForAddress(ptr, false);
                    if (!string.IsNullOrEmpty(info))
                        info = $"-> {info}";
                }
            }

            if (XivReClassPluginExt.Settings.FallbackModuleOffset && string.IsNullOrEmpty(info))
                info = Utils.GetModuleRelativeName(nodeValue);

            return info;
        }

        private static string? GetNameForAddress(nint address, bool includeInheritance) {
            var offset = Utils.GetModuleOffset(address);
            if (offset == 0) return null;

            if (XivDataManager.TryGetClassName(offset, out var className)) {
                var includeNamespace = XivReClassPluginExt.Settings.ShowNamespaces;

                if (!includeInheritance)
                    return includeNamespace ? className : Utils.RemoveNamespace(className);
                
                var inherit = XivDataManager.GetClassInheritance(className, includeNamespace);
                if (!string.IsNullOrEmpty(inherit))
                    return inherit;
            }

            if (XivDataManager.TryGetFunctionName(offset, out var funcName))
                return funcName;

            return null;
        }
    }
}