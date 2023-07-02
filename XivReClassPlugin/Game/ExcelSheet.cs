namespace XivReClassPlugin.Game; 

public class ExcelSheet {
	public nint Address { get; }

	public string Name => Ffxiv.Memory.ReadString(Ffxiv.Memory.Read<nint>(Address + 0x10));
	public int RowCount => Ffxiv.Memory.Read<int>(Address + 0x20);

	public ExcelSheet(nint address) {
		Address = address;
	}
}
