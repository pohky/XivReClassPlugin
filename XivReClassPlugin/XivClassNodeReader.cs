using System;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;

namespace XivReClassPlugin {
    public class XivClassNodeReader : INodeInfoReader {
        public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, IntPtr nodeAddress, IntPtr nodeValue) {
            if (nodeValue == IntPtr.Zero || nodeAddress == IntPtr.Zero)
                return null;
            
            if (XivReClassPluginExt.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue))
                return null;

            var info = GetNameForAddress(nodeValue, XivReClassPluginExt.Settings.ShowNamespaces);
            if (string.IsNullOrEmpty(info)) {
                var ptr = reader.ReadRemoteIntPtr(nodeValue);
                if (ptr.MayBeValid()) {
                    info = GetNameForAddress(ptr, XivReClassPluginExt.Settings.ShowNamespacesOnPointer);
                    if (!string.IsNullOrEmpty(info))
                        info = $"-> {info}";
                }
            }

            if (XivReClassPluginExt.Settings.FallbackModuleOffset && string.IsNullOrEmpty(info))
                info = Utils.GetModuleRelativeName(nodeValue);

            return info;
        }

        private static string? GetNameForAddress(nint address, bool includeNamespace) {
            var offset = Utils.GetModuleOffset(address);
            if (offset == 0) return null;

            if (DataManager.TryGetClass((ulong)offset, out var classDef) && classDef != null) {
                var name = XivReClassPluginExt.Settings.ShowInheritance ? classDef.ToString() : classDef.Name;
                return includeNamespace ? name : Utils.RemoveNamespace(name);
            }

            if (DataManager.TryGetFunction((ulong)offset, out var func) && func != null)
                return includeNamespace ? func : Utils.RemoveNamespace(func);

            return null;
        }
    }
}