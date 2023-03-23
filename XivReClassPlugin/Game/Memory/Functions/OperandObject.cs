using Iced.Intel;

namespace XivReClassPlugin.Game.Memory.Functions;

public abstract class OperandObject { }

public class ScalarObject : OperandObject {
	public ulong Value { get; }
	public ScalarObject(ulong value) => Value = value;
}

public class RegisterObject : OperandObject {
	public Register Register { get; }
	public RegisterObject(Register reg) => Register = reg;
}

public class MemoryObject : OperandObject {
	public ulong Address { get; }
	public MemoryObject(ulong address) => Address = address;
}
