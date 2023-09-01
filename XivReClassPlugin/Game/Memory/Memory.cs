using Iced.Intel;
using ReClassNET.Memory;

namespace XivReClassPlugin.Game.Memory;

public class Memory : MemoryAccess {
	private static readonly Module EmptyModule = new() { Name = string.Empty, Path = string.Empty };
	public Module MainModule { get; private set; } = EmptyModule;

	public void Update() {
		if (!Process.IsValid) {
			MainModule = EmptyModule;
            return;
        }
		
        if (MainModule != EmptyModule)
            return;

		if (Process.EnumerateRemoteSectionsAndModules(out _, out var modules)) {
            MainModule = modules.Find(m => m.Name.Equals(Process.UnderlayingProcess.Name));
        } else MainModule = EmptyModule;
    }

	public nint GetMainModuleOffset(nint staticAddress) {
		if (staticAddress >= MainModule.Start && staticAddress < MainModule.End)
			return staticAddress - MainModule.Start;
		return 0;
	}
    
    public bool MightBeClass(nint address) {
        if (address <= 0x10_000) return false;

        var vtable = Ffxiv.Memory.Read<nint>(address);
        if (Process.GetSectionToPointer(vtable) is not { Name: ".rdata" })
            return false;
        var vf0 = Ffxiv.Memory.Read<nint>(vtable);
        return Process.GetSectionToPointer(vf0) is { Name: ".text" };
    }

    public int TryGetSizeFromFunction(nint function) {
        if (function <= 0x10_000) return 0;

        var reader = new ByteArrayCodeReader(Read<byte>(function, 512));
        var decoder = Decoder.Create(64, reader, (ulong)function);
        var size = 0ul;
        var mightBeValid = false;
        while (reader.CanReadByte) {
            decoder.Decode(out var insn);
            if (insn.IsInvalid)
                break;

            if (insn.FlowControl is FlowControl.Return or FlowControl.UnconditionalBranch or FlowControl.Interrupt)
                break;

            if (insn is { Mnemonic: Mnemonic.Mov, Op0Kind: OpKind.Register, Op0Register: Register.EDX, Op1Kind: OpKind.Immediate32 or OpKind.Immediate16 }) {
                size = insn.GetImmediate(1);
                mightBeValid = false;
            }
            
            if (insn.IsCallNear && size >= 8 && size % 2 == 0) {
                if (Ffxiv.Symbols.TryGetName((nint)insn.NearBranchTarget, out var name) && name.StartsWith("FreeMemory")) {
                    mightBeValid = true;
                } else mightBeValid = false;
            }
        }

        return mightBeValid ? (int)size : 0;
    }
}
