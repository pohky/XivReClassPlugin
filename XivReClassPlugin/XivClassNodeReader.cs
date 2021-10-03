using System;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;

namespace XivReClassPlugin {
    public class XivClassNodeReader : INodeInfoReader {
        public string ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, IntPtr nodeAddress, IntPtr nodeValue) {
            var info = GetNameForAddress(nodeValue, XivReClassPluginExt.Settings?.ShowFullInheritance == true);
            if (string.IsNullOrEmpty(info)) {
                var ptr = reader.ReadRemoteIntPtr(nodeValue);
                if (ptr.MayBeValid()) {
                    info = GetNameForAddress(ptr, false);
                    if (!string.IsNullOrEmpty(info))
                        info = $"-> {info}";
                }
            }
            if (XivReClassPluginExt.Settings?.FallbackModuleOffset == true && string.IsNullOrEmpty(info))
                info = GetModuleOffsetName(nodeValue);
            return info!;
        }

        private static string? GetModuleOffsetName(nint address) {
            var mod = Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return null;
            if (address <= mod.Start || address >= mod.End) return null;
            return $"{mod.Name}+{(address - mod.Start).ToString("X")}";
        }

        private static string? GetNameForAddress(nint address, bool includeInheritance) {
            var mod = Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return null;
            var offset = address - mod.Start;
            if (XivDataManager.TryGetClassName(offset, out var className)) {
                var includeNamespace = XivReClassPluginExt.Settings?.ShowNamespaces == true;
                if (includeInheritance) {
                    var inherit = XivDataManager.GetClassInheritance(className, includeNamespace);
                    if (!string.IsNullOrEmpty(inherit))
                        return inherit;
                }
                return includeNamespace ? className : RemoveNamespace(className);
            }
            if (XivDataManager.TryGetFunctionName(offset, out var funcName))
                return funcName;
            return null;
        }

        private static string RemoveNamespace(string str) {
            var idx = str.LastIndexOf(':');
            return idx > 0 && idx < str.Length ? str.Substring(idx + 1) : str;
        }
    }
}