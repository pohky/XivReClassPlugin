namespace XivReClassPlugin.Game;

public readonly ref struct ExcelSheet {
    public readonly nint Address;
    public readonly bool IsValid;
    public readonly bool IsLinkedList;
    private readonly int m_NameOffset = 0x10;
    private readonly int m_RowCntOffset = 0x20;

    public string Name => Ffxiv.Memory.ReadString(Ffxiv.Memory.Read<nint>(Address + m_NameOffset));
    public int RowCount => Ffxiv.Memory.Read<int>(Address + m_RowCntOffset);

    public ExcelSheet(nint address) {
        Address = address;
        var vtable = Ffxiv.Memory.Read<nint>(address);
        if (vtable == Ffxiv.Address.ExcelSheetVtable) {
            IsLinkedList = false;
            IsValid = true;
        } else if (vtable == Ffxiv.Address.ExcelSheetListVtable) {
            IsLinkedList = true;
            IsValid = true;
            m_NameOffset += 0x18;
            m_RowCntOffset += 0x18;
        }
    }
}
