using System;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;

namespace XivReClassPlugin.NodeReaders {
    public class XivClassNodeReader : INodeInfoReader {
        public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, IntPtr nodeAddress, IntPtr nodeValue) {
            if (nodeValue == IntPtr.Zero || nodeValue <= (nint)0x10_000 || nodeAddress == IntPtr.Zero)
                return null;
            
            if (XivReClassPluginExt.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue))
                return null;

            string? info = null;
            var ptr = reader.ReadRemoteIntPtr(nodeValue);
            if (ptr.MayBeValid()) {
                info = GetNameForPointer(ptr);
                if (!string.IsNullOrEmpty(info))
                    info = $"-> {info}";
            }

            if (XivReClassPluginExt.Settings.FallbackModuleOffset && !XivReClassPluginExt.InternalNamedAddresses.ContainsKey(nodeValue) && string.IsNullOrEmpty(info))
                info = Utils.GetModuleRelativeName(nodeValue);

            return info;
        }

        private static string? GetNameForPointer(nint address) {
            var offset = Utils.GetModuleOffset(address);
            if (offset == 0) return null;

            if (DataManager.TryGetClassByOffset((ulong)offset, out var info))
                return XivReClassPluginExt.Settings.ShowNamespacesOnPointer ? info.FullName : info.Name;
            
            return null;
        }
    }
}