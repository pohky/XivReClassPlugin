using ReClassNET;

namespace XivReClassPlugin {
    public static class Utils {
        public static string RemoveNamespace(string str) {
            var idx = str.LastIndexOf(':');
            return idx > 0 && idx < str.Length ? str.Substring(idx + 1) : str;
        }

        public static string? GetModuleRelativeName(nint address) {
            var mod = Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return null;
            if (address <= mod.Start || address >= mod.End) return null;
            return $"{mod.Name}+{(address - mod.Start).ToString("X")}";
        }

        public static nint GetModuleOffset(nint address) {
            var mod = Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess.Name);
            if (mod == null) return 0;
            return address - mod.Start;
        }
    }
}