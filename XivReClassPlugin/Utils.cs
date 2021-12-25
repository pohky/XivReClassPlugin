using System.Text.RegularExpressions;
using ReClassNET;

namespace XivReClassPlugin {
    public static class Utils {
        private static readonly Regex m_NamespaceRegex = new("\\w+::", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string RemoveNamespace(string str) {
            return m_NamespaceRegex.Replace(str, "");
        }

        public static string? GetModuleRelativeName(nint address) {
            var mod = XivReClassPluginExt.MainModule ?? Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return null;
            if (address <= mod.Start || address >= mod.End) return null;
            return $"{mod.Name}+{(address - mod.Start).ToString("X")}";
        }

        public static nint GetModuleOffset(nint address) {
            var mod = XivReClassPluginExt.MainModule ?? Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return 0;
            return address - mod.Start;
        }
    }
}