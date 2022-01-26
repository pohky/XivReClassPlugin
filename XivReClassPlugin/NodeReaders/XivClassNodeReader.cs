using System;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using XivReClassPlugin.Data;

namespace XivReClassPlugin.NodeReaders {
    public class XivClassNodeReader : INodeInfoReader {
        public string? ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, IntPtr nodeAddress, IntPtr nodeValue) {
            if (nodeValue == IntPtr.Zero || nodeAddress == IntPtr.Zero)
                return null;
            
            if (XivReClassPluginExt.Settings.UseNamedAddresses && Program.RemoteProcess.NamedAddresses.ContainsKey(nodeValue))
                return null;

            //var info = GetNameForAddress(nodeValue);
            //if (string.IsNullOrEmpty(info)) {
            //    var ptr = reader.ReadRemoteIntPtr(nodeValue);
            //    if (ptr.MayBeValid()) {
            //        info = GetNameForPointer(ptr);
            //        if (!string.IsNullOrEmpty(info))
            //            info = $"-> {info}";
            //    }
            //}
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

            if (DataManager.TryGetClass((ulong)offset, out var classDef) && classDef != null)
                return XivReClassPluginExt.Settings.ShowNamespacesOnPointer ? classDef.Name : Utils.RemoveNamespace(classDef.Name);
            
            return null;
        }

        private static string? GetNameForAddress(nint address) {
            var offset = Utils.GetModuleOffset(address);
            if (offset == 0) return null;

            if (!DataManager.TryGetClass((ulong)offset, out var classDef) || classDef == null)
                return null;

            var name = XivReClassPluginExt.Settings.ShowInheritance ? classDef.FullName : classDef.Name;
            return XivReClassPluginExt.Settings.ShowNamespaces ? name : Utils.RemoveNamespace(name);
        }
    }
}