﻿using System;
using System.Linq;
using ReClassNET;
using ReClassNET.MemoryScanner;
using XivReClassPlugin.Data;

namespace XivReClassPlugin.Game.Memory;

public class AddressResolver {
    public nint Framework { get; private set; }
    public nint UiModule { get; private set; }
    public nint AgentModule { get; private set; }
    public nint AtkStage { get; private set; }
    public nint AtkUnitManager { get; private set; }
    public nint EventFramework { get; private set; }

    public nint ExcelSheetVtable { get; private set; }
    public nint ExcelSheetListVtable { get; private set; }

    public void Update() {
        Framework = GetFramework();
        UiModule = GetUiModule(Framework);
        AgentModule = GetAgentModule(UiModule);

        AtkStage = GetAtkStage();
        AtkUnitManager = GetAtkUnitManager(AtkStage);

        EventFramework = GetEventFramework();

        var sheet = GetExcelSheetVtables();
        ExcelSheetVtable = sheet.Sheet;
        ExcelSheetListVtable = sheet.LinkList;
    }

    private (nint Sheet, nint LinkList) GetExcelSheetVtables() {
        const string className = "Common::Component::Excel::ExcelSheet";
        const string listName = "Common::Component::Excel::LinkList<ExcelSheet>";
        const string interfaceName = "Common::Component::Excel::ExcelSheetInterface";
        var ifc = DataManager.Classes.FirstOrDefault(c => c.FullName == className && c.ParentClass?.FullName == interfaceName);
        var lc = DataManager.Classes.FirstOrDefault(c => c.FullName == className && c.ParentClass?.FullName == listName);
        if (ifc == null || lc == null) return (0, 0);
        var sheetAddr = Ffxiv.Memory.MainModule.Start + (int)ifc.Offset;
        var listAddr = Ffxiv.Memory.MainModule.Start + (int)lc.Offset;
        return (sheetAddr, listAddr);
    }

    private nint GetEventFramework() {
        if (!Ffxiv.Symbols.TryGetInstance("Client::Game::Event::EventFramework_Instance", out var efwPointer))
            efwPointer = ResolveSig("48 8B 2D ?? ?? ?? ?? 8B 50");
        return efwPointer == 0 ? 0 : Ffxiv.Memory.Read<nint>(efwPointer);
    }

    private nint GetFramework() {
        if (!Ffxiv.Symbols.TryGetInstance("Client::System::Framework::Framework_InstancePointer2", out var fwPointer))
            fwPointer = ResolveSig("48 8B 2D ?? ?? ?? ?? E9");
        return fwPointer == 0 ? 0 : Ffxiv.Memory.Read<nint>(fwPointer);
    }

    private nint GetAtkStage() {
        if (!Ffxiv.Symbols.TryGetInstance("Component::GUI::AtkStage_Instance", out var atkStagePointer))
            atkStagePointer = ResolveSig("4C 8B 15 ?? ?? ?? ?? 75");
        return atkStagePointer == 0 ? 0 : Ffxiv.Memory.Read<nint>(atkStagePointer);
    }

    private nint GetAtkUnitManager(nint atkStage) {
        if (atkStage == 0) return 0;
        return Ffxiv.Memory.Read<nint>(atkStage + 0x20);
    }

    private nint GetUiModule(nint framework) {
        if (framework == 0) return 0;
        var uiVf = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("Framework.GetUIModule", StringComparison.OrdinalIgnoreCase)).Key;
        if (uiVf == 0) uiVf = ResolveSig("E8 ?? ?? ?? ?? 44 0F B7 0F");
        if (uiVf == 0) return 0;
        var offset = Ffxiv.Memory.Read<int>(uiVf + 8 + 3);
        return offset <= 0 ? 0 : Ffxiv.Memory.Read<nint>(framework + offset);
    }

    private nint GetAgentModule(nint uiModule) {
        if (uiModule == 0) return 0;
        var agentVf = Ffxiv.Symbols.NamedAddresses.FirstOrDefault(kv => kv.Value.EndsWith("UiModule.GetAgentModule", StringComparison.OrdinalIgnoreCase)).Key;
        if (agentVf == 0) {
            var uiModuleVt = Ffxiv.Memory.Read<nint>(uiModule);
            agentVf = Ffxiv.Memory.Read<nint>(uiModuleVt + 37 * 8);
        }

        if (agentVf == 0) return 0;
        var offset = Ffxiv.Memory.Read<int>(agentVf + 3);
        return offset <= 0 ? 0 : uiModule + offset;
    }

    public nint ResolveSig(string sig) {
        var bp = BytePattern.Parse(sig);
        var result = PatternScanner.FindPattern(bp, Program.RemoteProcess, Ffxiv.Memory.MainModule);
        return result == IntPtr.Zero ? 0 : ResolveAddress(result);
    }

    private static nint ResolveAddress(nint address) {
        var mem = Program.RemoteProcess.ReadRemoteMemory(address, 8);
        if (mem == null) return 0;
        if (mem[0] == 0xE8 || mem[0] == 0xE9) {
            var offset = BitConverter.ToInt32(mem, 1);
            if (offset == 0) return 0;
            return address + 5 + offset;
        } else {
            var offset = BitConverter.ToInt32(mem, 3);
            if (offset == 0) return 0;
            return address + 7 + offset;
        }
    }
}
