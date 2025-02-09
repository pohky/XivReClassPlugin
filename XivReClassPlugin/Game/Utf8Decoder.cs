using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Linq;

namespace XivReClassPlugin.Game;

public static class Utf8Decoder {
    public static readonly Dictionary<byte, string> MacroCodeMap = [];

    static Utf8Decoder() {
        foreach (var value in Enum.GetValues(typeof(MacroCodes)).Cast<MacroCodes>())
            MacroCodeMap.Add((byte)value, value.ToString().ToLower());
        MacroCodeMap.Add(0x16, "-");
        MacroCodeMap.Add(0x1F, "--");
    }

    public static string DecodeString(byte[] data) {
        if (data.Length == 0 || data[0] == 0)
            return string.Empty;
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        return DecodeString(reader);
    }

    public static string DecodeString(BinaryReader reader) {
        if (!reader.BaseStream.CanRead || reader.BaseStream.Length == 0)
            return string.Empty;
        var text = new StringBuilder();
        while (reader.BaseStream.Position < reader.BaseStream.Length) {
            text.Append(DecodeMacro(reader));
        }
        return text.ToString();
    }

    private static string DecodeMacro(BinaryReader reader) {
        var init = reader.ReadByte();
        var text = new StringBuilder();
        if (init != 0x02) {
            var stream = new MemoryStream();
            stream.WriteByte(init);
            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                var value = reader.ReadByte();
                if (value == 0x00) break;
                if (value == 0x02) {
                    reader.BaseStream.Position--;
                    break;
                }
                stream.WriteByte(value);
            }
            text.Append(Encoding.UTF8.GetString(stream.ToArray()));
        } else {
            var type = reader.ReadByte();
            var length = DecodeNumber(reader, reader.ReadByte());
            var data = reader.ReadBytes(length);
            if (data.Length != length)
                throw new ArgumentException("Unexpected end of Input (EOF).");
            
            var end = reader.ReadByte();
            if (end != 0x03)
                throw new ArgumentException($"Missing Macro End Marker (0x03). Found 0x{end:X2} instead.");

            if (MacroCodeMap.TryGetValue(type, out var macro))
                text.Append($"<{macro}");
            else
                text.Append($"<UndefinedMacro_{type:X2}");

            if (data.Length > 0) {
                var paramList = new List<string>();
                using var paramStream = new MemoryStream(data);
                using var paramReader = new BinaryReader(paramStream);
                while (paramStream.Position < paramStream.Length)
                    paramList.Add(DecodeParameter(paramReader));
                if (paramList.Count > 0)
                    text.Append($"({string.Join(",", paramList)})");
            }

            text.Append('>');
        }
        return text.ToString();
    }

    private static string DecodeParameter(BinaryReader reader) {
        var type = reader.ReadByte();
        if (type == 0x00)
            throw new ArgumentException("Unexpected end of input (null character).");
        
        if (type is < 0xD0 or >= 0xF0 and <= 0xFE)
            return DecodeNumber(reader, type).ToString();

        if (type is >= 0xE0 and <= 0xE5)
            return DecodeConditional(reader, type);

        if (type is >= 0xE8 and <= 0xEC or >= 0xD8 and <= 0xDF) {
            return DecodePlaceholder(reader, type);
        }

        if (type == 0xFF) {
            var length = DecodeNumber(reader, reader.ReadByte());
            var buffer = reader.ReadBytes(length);
            if (buffer.Length != length)
                throw new ArgumentException("Unexpected end of input (EOF).");
            if (buffer.Any(b => b == 0))
                throw new ArgumentException("Unexpected end of input (null character).");
            if (buffer.Length == 0)
                return string.Empty;
            using var subStream = new MemoryStream(buffer);
            using var subReader = new BinaryReader(subStream);
            return DecodeString(subReader);
        }

        throw new NotImplementedException($"Parameter Type 0x{type:X2} is not implemented.");
    }

    private static string DecodePlaceholder(BinaryReader reader, byte type) {
        return type switch {
            0xE8 => $"lnum{DecodeParameter(reader)}",
            0xE9 => $"gnum{DecodeParameter(reader)}",
            0xEA => $"lstr{DecodeParameter(reader)}",
            0xEB => $"gstr{DecodeParameter(reader)}",
            0xEC => "stackcolor",
            0xD8 => "t_msec",
            0xD9 => "t_sec",
            0xDA => "t_min",
            0xDB => "t_hour",
            0xDC => "t_day",
            0xDD => "t_wday",
            0xDE => "t_mon",
            0xDF => "t_year",
            _ => throw new NotImplementedException($"Type 0x{type:X2} is not a supported placeholder Parameter.")
        };
    }

    private static string DecodeConditional(BinaryReader reader, byte type) {
        var left = DecodeParameter(reader);
        var right = DecodeParameter(reader);
        return type switch {
            0xE0 => $"[{left}>={right}]",
            0xE1 => $"[{left}>{right}]",
            0xE2 => $"[{left}<={right}]",
            0xE3 => $"[{left}<{right}]",
            0xE4 => $"[{left}=={right}]",
            0xE5 => $"[{left}!={right}]",
            _ => throw new NotImplementedException($"Type 0x{type} is not a supported conditional Parameter.")
        };
    }

    private static int DecodeNumber(BinaryReader reader, byte type) {
        switch (type) {
            case > 0 and < 0xD0:
                return type - 1;
            case >= 0xF0 and <= 0xFE: {
                type += 1;
                var value = 0;
                if ((type & 8) != 0) value |= (reader.ReadByte() << 24);
                if ((type & 4) != 0) value |= (reader.ReadByte() << 16);
                if ((type & 2) != 0) value |= (reader.ReadByte() << 8);
                if ((type & 1) != 0) value |= (reader.ReadByte() << 0);
                return value;
            }
            default:
                throw new ArgumentException($"Type 0x{type:X2} does indicate a number parameter.", nameof(type));
        }
    }

    public enum MacroCodes : byte {
        Setresettime = 0x06,
        Settime = 0x07,
        If = 0x08,
        Switch = 0x09,
        Pcname = 0x0A,
        Ifpcgender = 0x0B,
        Ifpcname = 0x0C,
        Josa = 0x0D,
        Josaro = 0x0E,
        Ifself = 0x0F,
        Br = 0x10,
        Wait = 0x11,
        Icon = 0x12,
        Color = 0x13,
        Edgecolor = 0x14,
        Shadowcolor = 0x15,
        // - = 0x16,
        Key = 0x17,
        Scale = 0x18,
        Bold = 0x19,
        Italic = 0x1A,
        Edge = 0x1B,
        Shadow = 0x1C,
        Nbsp = 0x1D,
        Icon2 = 0x1E,
        // -- = 0x1F,
        Num = 0x20,
        Hex = 0x21,
        Kilo = 0x22,
        Byte = 0x23,
        Sec = 0x24,
        Time = 0x25,
        Float = 0x26,
        Link = 0x27,
        Sheet = 0x28,
        String = 0x29,
        Caps = 0x2A,
        Head = 0x2B,
        Split = 0x2C,
        Headall = 0x2D,
        Fixed = 0x2E,
        Lower = 0x2F,
        Janoun = 0x30,
        Ennoun = 0x31,
        Denoun = 0x32,
        Frnoun = 0x33,
        Chnoun = 0x34,
        Lowerhead = 0x40,
        Colortype = 0x48,
        Edgecolortype = 0x49,
        Ruby = 0x4A, // not in the game
        Digit = 0x50,
        Ordinal = 0x51,
        Sound = 0x60,
        Levelpos = 0x61
    }
}
