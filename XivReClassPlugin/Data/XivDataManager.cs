using System.IO;
using System.Text;

namespace XivReClassPlugin.Data {
    public static class XivDataManager {
        public const long DataBaseAddress = 0x140000000;
        public static DataClass Data { get; private set; } = new();
        private static string DataPath => XivReClassPluginExt.Settings?.ClientStructsDataPath ?? string.Empty;

        static XivDataManager() => Update();

        public static void Update() {
            if (!File.Exists(DataPath)) {
                Data = new DataClass();
                return;
            }

            var des = new YamlDotNet.Serialization.DeserializerBuilder()
                .IgnoreUnmatchedProperties().Build();
            
            Data = des.Deserialize<DataClass>(File.ReadAllText(DataPath));
        }

        public static bool TryGetFunctionName(long funcOffset, out string funcName) {
            var address = DataBaseAddress + funcOffset;
            if (Data.functions.TryGetValue(address, out var fName) && !string.IsNullOrEmpty(fName)) {
                funcName = fName!;
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
            var address = DataBaseAddress + classOffset;
            foreach (var dataClass in Data.classes) {
                if (dataClass.Value is null || dataClass.Value.vtbl != address)
                    continue;
                className = dataClass.Key;
                return true;
            }
            className = string.Empty;
            return false;
        }

        public static string? GetClassInheritance(string className, bool includeNamespace) {
            if (!Data.classes.TryGetValue(className, out var xivClass))
                return null;
            if (xivClass == null || string.IsNullOrEmpty(xivClass.inherits_from)) return null;

            var sb = new StringBuilder();
            sb.Append($"{(includeNamespace ? className : RemoveNamespace(className))}");
            while (!string.IsNullOrEmpty(xivClass?.inherits_from)) {
                var name = xivClass?.inherits_from;
                if(name == null) continue;
                if (!Data.classes.TryGetValue(name, out xivClass) || xivClass is null)
                    continue;
                sb.Append($" : {(includeNamespace ? name : RemoveNamespace(name))}");
            }

            return sb.ToString();

            static string RemoveNamespace(string str) {
                var idx = str.LastIndexOf(':');
                return idx > 0 && idx < str.Length ? str.Substring(idx + 1) : str;
            }
        }
    }
}