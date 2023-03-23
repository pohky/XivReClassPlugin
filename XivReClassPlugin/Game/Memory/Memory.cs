using System.Linq;
using Iced.Intel;
using ReClassNET.Memory;
using XivReClassPlugin.Game.Memory.Functions;

namespace XivReClassPlugin.Game.Memory;

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

	public int TryGetSizeFromDtor(nint dtorFunction) {
		if (dtorFunction <= 0x10_000) return 0;
		var func = new Function(dtorFunction);
		if (func.Size < 16) return 0;
		var tmpSize = 0ul;
		foreach (var instruction in func.Instructions) {
			if (IsEdxAssign(instruction) && instruction.InputObjects.FirstOrDefault() is ScalarObject obj)
				tmpSize = obj.Value;
			if (IsFreeMemCall(instruction))
				break;
		}

		return (int)tmpSize;

		static bool IsEdxAssign(InstructionData insn) {
			return insn.Mnemonic == Mnemonic.Mov &&
			       insn.OutputObjects.OfType<RegisterObject>().FirstOrDefault()?.Register == Register.EDX &&
			       insn.InputObjects.OfType<ScalarObject>().FirstOrDefault()?.Value > 0;
		}

		static bool IsFreeMemCall(InstructionData insn) {
			return insn.Mnemonic == Mnemonic.Call &&
			       insn.InputObjects.OfType<ScalarObject>().FirstOrDefault() is { Value: > 0x10_000 } obj &&
			       Ffxiv.Symbols.TryGetName((nint)obj.Value, out var name) &&
			       name.StartsWith("FreeMemory");
		}
	}
}
