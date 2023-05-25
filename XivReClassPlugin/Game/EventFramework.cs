namespace XivReClassPlugin.Game; 

public static class EventFramework {
	public static nint Address => Ffxiv.Address.EventFramework;
	public static DirectorModule DirectorModule { get; } = new();

	public static void Update() {
		if (Address == 0) return;
	}
}

public class DirectorModule {
	public nint Address => EventFramework.Address == 0 ? 0 : EventFramework.Address + 0xC0;
	public nint ActiveContentDirector => Address == 0 ? 0 : Ffxiv.Memory.Read<nint>(Address + 0x98);
}
