using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReClassNET;
using YamlDotNet.Serialization;

namespace XivReClassPlugin.Data {
    public static class DataManager {
        public const ulong DataBaseAddress = 0x1_4000_0000;

        public static ClientStructsData Data { get; private set; } = new();

        public static readonly Dictionary<ulong, ClassDef> Classes = new();
        private static readonly HashSet<ulong> m_ClassBlacklist = new();

        public static event Action? DataUpdated;
        
        public static void ReloadData() {
            var path = XivReClassPluginExt.Settings.ClientStructsDataPath;

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                Data = new ClientStructsData();
                UpdateData();
                return;
            }

            try {
                Data = new DeserializerBuilder().Build().Deserialize<ClientStructsData>(File.ReadAllText(path, Encoding.UTF8));
            } catch (Exception ex) {
                Data = new ClientStructsData();
                Program.ShowException(ex);
            }

            UpdateData();
        }

        public static void UpdateData() {
            UpdateClasses();
            DataUpdated?.Invoke();
        }

        public static void Clear() {
            Classes.Clear();
            m_ClassBlacklist.Clear();
            Data = new ClientStructsData();
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
    }
}