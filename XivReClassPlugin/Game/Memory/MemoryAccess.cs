using System;
using System.Text;
using ReClassNET;
using ReClassNET.Extensions;
using ReClassNET.Memory;

namespace XivReClassPlugin.Game.Memory;

public abstract unsafe class MemoryAccess {
    public bool IsValid => Program.RemoteProcess.IsValid;
    public RemoteProcess Process => Program.RemoteProcess;

    public T Read<T>(nint address) where T : unmanaged {
        var data = Process.ReadRemoteMemory(address, sizeof(T));
        fixed (byte* ptr = data) return *(T*)ptr;
    }

    public T[] Read<T>(nint address, int count) where T : unmanaged {
        if (count <= 0) return Array.Empty<T>();
        var size = sizeof(T);
        var data = Process.ReadRemoteMemory(address, size * count);
        var array = new T[count];
        fixed (byte* ptr = data) {
            for (var i = 0; i < array.Length; i++)
                array[i] = *(T*)(ptr + size * i);
        }

        return array;
    }

    public string ReadString(nint address, int maxLength = 512) {
        return ReadString(address, Encoding.UTF8, maxLength);
    }

    public string ReadString(nint address, Encoding encoding, int maxLength = 512) {
        return Process.ReadRemoteStringUntilFirstNullCharacter(address, encoding, maxLength);
    }

    public bool Write<T>(nint address, T value) where T : unmanaged {
        var data = new byte[sizeof(T)];
        fixed (byte* ptr = data) *(T*)ptr = value;

        return Process.WriteRemoteMemory(address, data);
    }

    public bool Write<T>(nint address, T[] values) where T : unmanaged {
        var size = sizeof(T);
        var data = new byte[size * values.Length];
        fixed (byte* ptr = data) {
            for (var i = 0; i < values.Length; i++)
                *(T*)(ptr + size * i) = values[i];
        }

        return Process.WriteRemoteMemory(address, data);
    }

    public bool WriteString(nint address, string value) {
        return WriteString(address, value, Encoding.UTF8);
    }

    public bool WriteString(nint address, string value, Encoding encoding) {
        if (string.IsNullOrEmpty(value))
            return true;
        try {
            var data = encoding.GetBytes(value);
            return Process.WriteRemoteMemory(address, data);
        } catch {
            return false;
        }
    }
}
