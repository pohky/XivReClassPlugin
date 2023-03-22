using System;
using System.Linq;

namespace XivReClassPlugin.Game;

public class AddressResolver {
	public nint Framework { get; private set; }
	public nint UiModule { get; private set; }
	public nint AgentModule { get; private set; }

	public void Update() {
		Framework = GetFramework();
		UiModule = GetUiModule(Framework);
		AgentModule = GetAgentModule(UiModule);
	}

	private nint GetFramework() {
		if (!Ffxiv.Symbols.TryGetInstance("Client::System::Framework::Framework_InstancePointer2", out var fwPointer))
			return 0;
		return Ffxiv.Memory.Read<nint>(fwPointer);
	}

	private nint GetUiModule(nint framework) {
		if (framework == 0) return 0;
		var uiVf = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("Framework.GetUIModule", StringComparison.OrdinalIgnoreCase));
		var offset = Ffxiv.Memory.Read<int>(uiVf.Key + 8 + 3);
		return offset <= 0 ? 0 : Ffxiv.Memory.Read<nint>(framework + offset);
	}

	private nint GetAgentModule(nint uiModule) {
		if (uiModule == 0) return 0;
		var agentVf = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("UiModule.GetAgentModule", StringComparison.OrdinalIgnoreCase));
		var offset = Ffxiv.Memory.Read<int>(agentVf.Key + 3);
		return offset <= 0 ? 0 : uiModule + offset;
	}
}