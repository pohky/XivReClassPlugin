using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ReClassNET.Util;

namespace XivReClassPlugin; 

[Serializable]
public class XivPluginSettings {
	private static string SettingsPath => Path.Combine(PathUtil.SettingsFolderPath, "XivReClassPlugin.xml");
        
	public string ClientStructsDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GitHub\\FFXIVClientStructs\\ida\\data.yml");

	public bool FallbackModuleOffset = true;
	public bool ShowNamespaces = false;
	public bool ShowNamespacesOnPointer = false;
	public bool ShowInheritanceOnPointer = false;
	public bool UseNamedAddresses = true;
	public bool ShowInheritance = true;
    public bool ShowExcelSheetNames = true;

    public bool GuessClassSizes = true;
    public bool TryGetSizeForEventInterfaces = false;

    public bool DecodeUtf8Strings = true;
	
	public static XivPluginSettings Load() {
		if (!File.Exists(SettingsPath))
			return new XivPluginSettings();
		using var reader = new StringReader(File.ReadAllText(SettingsPath, Encoding.UTF8));
		var ser = new XmlSerializer(typeof(XivPluginSettings));
		return ser.Deserialize(reader) as XivPluginSettings ?? new XivPluginSettings();
	}

	public void Save() {
		using var writer = new StringWriter();
		var ser = new XmlSerializer(typeof(XivPluginSettings));
		ser.Serialize(writer, this);
		File.WriteAllText(SettingsPath, writer.ToString(), Encoding.UTF8);
	}
}