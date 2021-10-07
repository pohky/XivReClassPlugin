using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace XivReClassPlugin.Data {
    public static class XivDataManager {
        public const long DataBaseAddress = 0x140000000;
        public static DataClass Data { get; private set; } = new();

        public static bool HasData => !string.IsNullOrEmpty(Data.version);

        private static readonly Dictionary<long, string> m_ClassCache = new(256);
        private static readonly HashSet<long> m_ClassBlacklist = new(256);
        
        static XivDataManager() => Update();

        public static void Update() {
            Clear();

            var dataPath = XivReClassPluginExt.Settings.ClientStructsDataPath;
            if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
                return;

            var des = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            Data = des.Deserialize<DataClass>(File.ReadAllText(dataPath));
        }

        public static void Clear() {
            m_ClassCache.Clear();
            m_ClassBlacklist.Clear();
            Data = new DataClass();
        }

        public static bool TryGetFunctionName(long funcOffset, out string funcName) {
            var address = DataBaseAddress + funcOffset;
            
            if (Data.functions.TryGetValue(address, out var fName)) {
                funcName = $"{fName}";
                return true;
            }

            foreach (var dataClass in Data.classes) {
                if (dataClass.Value is null || !dataClass.Value.funcs.TryGetValue(address, out var name))
                    continue;
                funcName = $"{dataClass.Key}.{name}";
                return true;
            }

            funcName = string.Empty;
            return false;
        }

        public static bool TryGetClassName(long classOffset, out string className) {
            className = string.Empty;
            if (m_ClassBlacklist.Contains(classOffset))
                return false;
            if (m_ClassCache.TryGetValue(classOffset, out className))
                return true;

            var address = DataBaseAddress + classOffset;
            className = Data.classes.FirstOrDefault(kv => kv.Value?.vtbl == address).Key;

            if (!string.IsNullOrEmpty(className)) {
                m_ClassCache[classOffset] = className;
                return true;
            }

            m_ClassBlacklist.Add(classOffset);
            return false;
        }
    }
}