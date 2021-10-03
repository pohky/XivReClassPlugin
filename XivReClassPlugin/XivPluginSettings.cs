using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ReClassNET.Util;

namespace XivReClassPlugin {
    [Serializable]
    public class XivPluginSettings {
        private static string SettingsPath => Path.Combine(PathUtil.SettingsFolderPath, "XivReClassPlugin.xml");

        public string ClientStructsDataPath = "C:\\Users\\Pohky\\Documents\\GitHub\\FFXIVClientStructs\\ida\\data.yml";
        public bool FallbackModuleOffset = true;
        public bool ShowNamespaces = true;
        public bool ShowFullInheritance = false;

        public static XivPluginSettings Load() {
            if (!File.Exists(SettingsPath))
                return new XivPluginSettings();
            var tr = new StringReader(File.ReadAllText(SettingsPath, Encoding.UTF8));
            var ser = new XmlSerializer(typeof(XivPluginSettings));
            var obj = ser.Deserialize(tr);
            if (obj is XivPluginSettings settings)
                return settings;
            return new XivPluginSettings();
        }

        public void Save() {
            using var sw = new StringWriter();
            var ser = new XmlSerializer(typeof(XivPluginSettings));
            ser.Serialize(sw, this);
            File.WriteAllText(SettingsPath, sw.ToString(), Encoding.UTF8);
        }
    }
}