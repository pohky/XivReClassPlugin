using System.Collections.Generic;
using Iced.Intel;

namespace XivReClassPlugin.Game.Memory.Functions; 

public class Function {
	public nint Address { get; }
	public List<InstructionData> Instructions { get; } = new();
	public int Size { get; }

	public Function(nint address) {
		Address = address;
		var data = Ffxiv.Memory.Read<byte>(Address, 8192);
		var reader = new ByteArrayCodeReader(data);
		var decoder = Decoder.Create(64, reader, (ulong)Address);
		var factory = new InstructionInfoFactory();
		var size = 0;
		while (reader.CanReadByte) {
			decoder.Decode(out var instruction);
			if (instruction.IsInvalid || instruction.Mnemonic == Mnemonic.Int3) break;
			size += instruction.Length;
			var info = factory.GetInfo(instruction);
			Instructions.Add(new InstructionData(instruction, info));
		}
		Size = size;
	}
}
