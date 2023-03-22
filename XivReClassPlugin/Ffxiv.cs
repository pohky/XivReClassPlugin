using XivReClassPlugin.Game;

namespace XivReClassPlugin; 

public static class Ffxiv {
	public static XivPluginSettings Settings { get; }
	public static Symbols Symbols { get; }
	public static Memory Memory { get; }
	public static AddressResolver Address { get; }

	static Ffxiv() {
		Settings = XivPluginSettings.Load();
		Symbols = new Symbols();
		Memory = new Memory();
		Address = new AddressResolver();
	}

	public static void Update() {
		Memory.Update();
		Symbols.Update();
		Address.Update();
	}
}
