using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace XivReClassPlugin.Data {
    public class XivClass {
        public string? inherits_from { get; set; } = string.Empty;
        public long vtbl { get; set; } = 0;
        public Dictionary<long, string> funcs { get; set; } = new();
        public Dictionary<int, string> vfuncs { get; set; } = new();

        public static readonly XivClass Empty = new();
    }
}