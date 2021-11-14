using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace XivReClassPlugin.Data {
    public static class DataManager {
        public const ulong DataBaseAddress = 0x1_4000_0000;
        public static ClientStructsData Data { get; private set; } = new();

        public static readonly Dictionary<ulong, ClassDef> Classes = new();
        public static readonly Dictionary<ulong, string> Functions = new();

        private static readonly HashSet<ulong> m_ClassBlacklist = new();
        private static readonly HashSet<ulong> m_FunctionBlacklist = new();

        public static void Update() {
            var dataPath = XivReClassPluginExt.Settings.ClientStructsDataPath;
            if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
                Data = new ClientStructsData();
            else {
                Data = new DeserializerBuilder().Build()
                    .Deserialize<ClientStructsData>(File.ReadAllText(dataPath, Encoding.UTF8));
            }
            UpdateClasses();
            UpdateFunctions();
        }

        public static void Clear() {
            Classes.Clear();
            Functions.Clear();
            m_ClassBlacklist.Clear();
            m_FunctionBlacklist.Clear();
            Data = new ClientStructsData();
        }

        private static void UpdateFunctions() {
            Functions.Clear();
            foreach (var kv in Data.Functions)
                Functions[kv.Key - DataBaseAddress] = kv.Value;

            foreach (var kv in Data.Classes.Where(kv => kv.Value != null)) {
                if (kv.Value == null || kv.Value.Functions.Count <= 0)
                    continue;
                foreach (var func in kv.Value.Functions)
                    Functions[func.Key - DataBaseAddress] = $"{kv.Key}.{func.Value}";
            }
        }

        private static void UpdateClasses() {
            var classDefs = new List<ClassDef>();
            foreach (var dataClass in Data.Classes) {
                var name = dataClass.Key;
                if (dataClass.Value?.VirtualTables.Count > 0) {
                    foreach (var vt in dataClass.Value.VirtualTables) {
                        var def = new ClassDef(name, dataClass.Value) {
                            Address = vt.Address - DataBaseAddress
                        };
                        if (!string.IsNullOrEmpty(vt.Base) && Data.Classes.TryGetValue(vt.Base, out var parent))
                            def.Parent = new ClassDef(vt.Base, parent);
                        classDefs.Add(def);
                    }
                } else classDefs.Add(new ClassDef(name, dataClass.Value));
            }

            Classes.Clear();
            foreach (var def in classDefs.Where(d => d.Address != 0))
                Classes.Add(def.Address, def);
            classDefs.Clear();
        }

        public static bool TryGetClass(ulong address, out ClassDef? classDef) {
            classDef = null;
            if (m_ClassBlacklist.Contains(address))
                return false;
            if (Classes.TryGetValue(address, out classDef))
                return true;
            m_ClassBlacklist.Add(address);
            return false;
        }

        public static bool TryGetFunction(ulong address, out string? func) {
            func = null;
            if (m_FunctionBlacklist.Contains(address))
                return false;
            if (Functions.TryGetValue(address, out func))
                return true;
            m_FunctionBlacklist.Add(address);
            return false;
        }
    }
}