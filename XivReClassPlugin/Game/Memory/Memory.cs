using System.Linq;
using System.Windows.Forms;
using Iced.Intel;
using ReClassNET.Memory;

namespace XivReClassPlugin.Game.Memory;

public class Memory : MemoryAccess {
    private static readonly Module EmptyModule = new() { Name = string.Empty, Path = string.Empty };
    public Module MainModule { get; private set; } = EmptyModule;

    public nint FreeMemoryFunc { get; private set; }
    public nint FreeMemory2Func { get; private set; }

    public void Update() {
        if (!Process.IsValid) {
            MainModule = EmptyModule;
            FreeMemoryFunc = 0;
            FreeMemory2Func = 0;
            return;
        }

        if (MainModule != EmptyModule)
            return;

        Reload();
    }

    public void Reload() {
        if (Process.EnumerateRemoteSectionsAndModules(out _, out var modules))
            MainModule = modules.Find(m => m.Name.Equals(Process.UnderlayingProcess.Name));
        else MainModule = EmptyModule;

        FreeMemoryFunc = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("FreeMemory")).Key;
        if (FreeMemoryFunc == 0) {
            FreeMemoryFunc = Ffxiv.Address.ResolveSig("E8 ?? ?? ?? ?? 4D 89 AE");
            if (FreeMemoryFunc == 0)
                FreeMemoryFunc = Ffxiv.Address.ResolveSig("E8 ?? ?? ?? ?? 48 63 2E");
        }

        FreeMemory2Func = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("FreeMemory_2")).Key;
        if (FreeMemory2Func == 0) {
            FreeMemory2Func = Ffxiv.Address.ResolveSig("E8 ?? ?? ?? ?? 48 8B C3 48 83 C4 ?? 5F 5D");
            if (FreeMemory2Func == 0)
                FreeMemory2Func = Ffxiv.Address.ResolveSig("E8 ?? ?? ?? ?? 48 8B C7 48 83 C4 ?? 41 5C");
        }
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
        if (FreeMemoryFunc == 0 && FreeMemory2Func == 0) return 0;

        var data = Read<byte>(function, 512);

        if (data[0] == 0x48 && data[1] == 0x81 && data[2] == 0xE9 && data[7] == 0xE9)
            return -1; // sub rcx, jmp, possible subclass dtor, safer to just return

        var reader = new ByteArrayCodeReader(data);
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
                if ((nint)insn.NearBranchTarget == FreeMemoryFunc || (nint)insn.NearBranchTarget == FreeMemory2Func)
                    mightBeValid = true;
                else mightBeValid = false;
            }
        }

        return mightBeValid ? (int)size : 0;
    }
}
