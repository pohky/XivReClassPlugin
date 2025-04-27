using System.Runtime.InteropServices;
using XivReClassPlugin.Data;
using XivReClassPlugin.Game;
using XivReClassPlugin.Game.Memory;

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

    public static void Reload() {
        DataManager.Reload();
        Memory.Reload();
        Update();
    }

    public static void Update() {
        Memory.Update();
        Symbols.Update();
        Address.Update();

        AtkUnitManager.Update();
        AgentModule.Update();
        EventFramework.Update();
    }

    public static void CreateRemoteThread(nint address, nint arg = 0) {
        var h = OpenProcess(ProcessAllAccess, false, (int)Memory.Process.UnderlayingProcess.Id);
        if (h <= 0) return;
        var th = CreateRemoteThread(h, 0, 0, address, arg, 0, out _);

        try {
            WaitForSingleObject(th, 5000);
        } finally {
            if (th != 0)
                CloseHandle(th);
            if (h != 0)
                CloseHandle(h);
        }
    }

    private const uint ProcessAllAccess = 0x001FFFFF;

    [DllImport("kernel32", SetLastError = true)]
    private static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool CloseHandle(nint hObject);

    [DllImport("kernel32", SetLastError = true)]
    private static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, nint dwStackSize, nint lpStartAddress, nint lpParameter, uint dwCreationFlags,
        out int lpThreadId);

    [DllImport("kernel32", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint hHandle, nint dwMilliseconds);
}
