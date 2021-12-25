using System.Collections.Generic;

namespace XivReClassPlugin.Data {
    public class ClassDef {
        private string? m_CachedName;
        private readonly XivClass? m_Data;

        public string Name { get; }
        public ulong Address { get; set; }
        public ClassDef? Parent { get; set; }

        public string FullName => m_CachedName ??= Parent == null ? $"{Name}" : $"{Name} : {Parent.FullName}";

        public readonly Dictionary<ulong, string> Instances = new();

        public ClassDef(string name, XivClass? data) {
            Name = name;
            m_Data = data;
            if (data?.Instances == null)
                return;
            var idx = 0;
            foreach (var instance in data.Instances)
                Instances.Add(instance.Address - DataManager.DataBaseAddress, $"{name}_{instance.Name ?? $"Instance{idx++}"}");
        }

        public Dictionary<int, string> GetVirtualFunctions() {
            var dict = new Dictionary<int, string>();
            var pfuncs = Parent?.GetVirtualFunctions() ?? new Dictionary<int, string>();
            foreach (var vf in pfuncs)
                dict[vf.Key] = vf.Value;
            if (m_Data != null && (m_Data.VirtualTables.Count <= 1 || m_Data.VirtualTables[0].Address - DataManager.DataBaseAddress == Address)) {
                foreach (var vf in m_Data.VirtualFunctions)
                    dict[vf.Key] = vf.Value;
            }
            return dict;
        }
    }
}