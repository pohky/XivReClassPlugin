﻿using System;
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
            if (XivReClassPluginExt.InternalNamedAddresses.TryGetValue(address, out var name))
                return includeNamespace ? name : Utils.RemoveNamespace(name);
            var offset = Utils.GetModuleOffset(address);
            if (offset == 0) return null;

            if (XivDataManager.TryGetClassName(offset, out var className))
                return includeNamespace ? className : Utils.RemoveNamespace(className);

            if (XivDataManager.TryGetFunctionName(offset, out var funcName))
                return funcName;

            return null;
        }
    }
}