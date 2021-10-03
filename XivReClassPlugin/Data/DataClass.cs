using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace XivReClassPlugin.Data {
    public class DataClass {
        public string version { get; set; } = string.Empty;
        public Dictionary<long, string> globals { get; set; } = new();
        public Dictionary<long, string?> functions { get; set; } = new();

        public Dictionary<string, XivClass?> classes { get; set; } = new();
    }
}