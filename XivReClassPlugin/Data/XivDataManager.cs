using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XivReClassPlugin.Data {
    public static class XivDataManager {
        public const long DataBaseAddress = 0x140000000;
        public static DataClass Data { get; private set; } = new();

        public static bool HasData => !string.IsNullOrEmpty(Data.version);

        private static readonly Dictionary<long, string> m_ClassCache = new(256);
        private static readonly HashSet<long> m_ClassBlacklist = new(256);
        private static readonly Dictionary<long, string> m_FunctionCache = new(256);
        private static readonly HashSet<long> m_FunctionBlacklist = new(256);
        private static readonly Dictionary<string, string[]> m_InheritanceCache = new(256);

        static XivDataManager() => Update();

        public static void Update() {
            m_ClassCache.Clear();
            m_ClassBlacklist.Clear();
            m_FunctionCache.Clear();
            m_FunctionBlacklist.Clear();
            m_InheritanceCache.Clear();
            
            var dataPath = XivReClassPluginExt.Settings.ClientStructsDataPath;
            if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath)) {
                Data = new DataClass();
                return;
            }

            var des = new YamlDotNet.Serialization.DeserializerBuilder()
                .IgnoreUnmatchedProperties().Build();
            Data = des.Deserialize<DataClass>(File.ReadAllText(dataPath));
        }

        public static bool TryGetFunctionName(long funcOffset, out string funcName) {
            funcName = string.Empty;
            if (m_FunctionBlacklist.Contains(funcOffset))
                return false;
            if (m_FunctionCache.TryGetValue(funcOffset, out funcName))
                return true;

            var address = DataBaseAddress + funcOffset;
            
            if (Data.functions.TryGetValue(address, out var fName)) {
                funcName = $"{fName}";
                m_FunctionCache[funcOffset] = funcName;
                return true;
            }

            foreach (var dataClass in Data.classes) {
                if (dataClass.Value is null || !dataClass.Value.funcs.TryGetValue(address, out var name))
                    continue;
                funcName = $"{dataClass.Key}.{name}";
                m_FunctionCache[funcOffset] = funcName;
                return true;
            }

            m_FunctionBlacklist.Add(funcOffset);
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

        public static string? GetClassInheritance(string className, bool includeNamespace) {
            if (!Data.classes.TryGetValue(className, out var xivClass))
                return null;
            if (xivClass == null || string.IsNullOrEmpty(xivClass.inherits_from)) return null;

            if (m_InheritanceCache.TryGetValue(className, out var array)) {
                return string.Join(" : ", includeNamespace ? array : array.Select(Utils.RemoveNamespace));
            }

            var sb = new StringBuilder();
            var nameList = new List<string>(5) { className };
            sb.Append($"{(includeNamespace ? className : Utils.RemoveNamespace(className))}");
            while (!string.IsNullOrEmpty(xivClass?.inherits_from)) {
                var name = xivClass?.inherits_from;
                if(name == null) continue;
                if (!Data.classes.TryGetValue(name, out xivClass) || xivClass is null)
                    continue;
                nameList.Add(name);
                sb.Append($" : {(includeNamespace ? name : Utils.RemoveNamespace(name))}");
            }

            m_InheritanceCache[className] = nameList.ToArray();
            return sb.ToString();
        }
    }
}