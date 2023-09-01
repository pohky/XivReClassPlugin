namespace XivReClassPlugin.Game; 

public class ExcelSheet {
	public nint Address { get; }

    public string Name => Ffxiv.Memory.ReadString(Ffxiv.Memory.Read<nint>(Address + m_NameOffset));
	public int RowCount => Ffxiv.Memory.Read<int>(Address + m_RowCntOffset);
    public readonly bool IsLinkedList;
    private readonly int m_NameOffset = 0x10;
    private readonly int m_RowCntOffset = 0x20;

	public ExcelSheet(nint address) {
		Address = address;
        if (Ffxiv.Memory.MightBeClass(address + 0x18)) {
            IsLinkedList = true;
            const int offset = 0x18;
            m_NameOffset += offset;
            m_RowCntOffset += offset;
        }
    }
}
