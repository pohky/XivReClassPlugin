using System.Collections.Generic;
using Iced.Intel;

namespace XivReClassPlugin.Game.Memory.Functions;

public class InstructionData {
	private readonly InstructionInfo m_Info;
	private readonly Instruction m_Instruction;
	public Mnemonic Mnemonic => m_Instruction.Mnemonic;
	public int Length => m_Instruction.Length;
	public List<OperandObject> InputObjects { get; } = new();
	public List<OperandObject> OutputObjects { get; } = new();

	public InstructionData(in Instruction instruction, in InstructionInfo info) {
		m_Instruction = instruction;
		m_Info = info;

		for (var i = 0; i < instruction.OpCount; i++) {
			var kind = instruction.GetOpKind(i);
			var access = info.GetOpAccess(i);

			OperandObject? obj = kind switch {
				OpKind.Memory => new MemoryObject(instruction.MemoryDisplacement64),
				OpKind.Register => new RegisterObject(instruction.GetOpRegister(i)),
				OpKind.Immediate8 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate16 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate32 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate64 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate8to16 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate8to32 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate8to64 => new ScalarObject(instruction.GetImmediate(i)),
				OpKind.Immediate32to64 => new ScalarObject(instruction.GetImmediate(i)),
				_ => null
			};

			if (obj == null) continue;

			switch (access) {
				case OpAccess.Read:
					InputObjects.Add(obj);
					break;
				case OpAccess.Write:
					OutputObjects.Add(obj);
					break;
			}
		}
	}

	public override string ToString() => m_Instruction.ToString();
}
