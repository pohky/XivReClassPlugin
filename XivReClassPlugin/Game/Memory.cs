using ReClassNET.Memory;

namespace XivReClassPlugin.Game; 

public class Memory : MemoryAccess {
	private static readonly Module EmptyModule = new() { Name = string.Empty, Path = string.Empty };
	public Module MainModule { get; private set; } = EmptyModule;

	public void Update() {
		MainModule = EmptyModule;
		if (!Process.IsValid) 
			return;
		if (Process.EnumerateRemoteSectionsAndModules(out _, out var modules))
			MainModule = modules.Find(m => m.Name.Equals(Process.UnderlayingProcess.Name));
		else MainModule = EmptyModule;
	}

	public nint GetMainModuleOffset(nint staticAddress) {
		if (staticAddress >= MainModule.Start && staticAddress < MainModule.End)
			return staticAddress - MainModule.Start;
		return 0;
	}
}
