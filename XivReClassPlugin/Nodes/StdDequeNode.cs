using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.UI;
using System;
using System.Drawing;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin.Nodes;

public class StdDequeNode : BaseWrapperArrayNode {
    public override int MemorySize => IntPtr.Size * 5;

    public StdDequeNode() {
        IsReadOnly = true;
    }

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "StdDeque<T>";
        icon = XivReClassResources.StdDequeIcon;
    }

    //public override void CopyFromNode(BaseNode node) {
    //	base.CopyFromNode(node);
    //	if (InnerNode is not BaseWrapperNode inner)
    //		return;
    //	if (node is not BaseWrapperNode wrapper || wrapper.ResolveMostInnerNode() is not { } innerNode)
    //		return;
    //	if (inner.CanChangeInnerNodeTo(innerNode) && InnerNode is BaseClassWrapperNode classWrapper) {
    //		var old = classWrapper.InnerNode;
    //		inner.ChangeInnerNode(innerNode);
    //		if (old is ClassNode classNode && classNode.Name.StartsWith("N00"))
    //			Program.MainForm.CurrentProject.Remove(classNode);
    //	}
    //}

    public override void Initialize() {
        //var node = new ClassInstanceNode();
        //node.Initialize();
        //ChangeInnerNode(node);
        ChangeInnerNode(new Hex64Node());
    }

    public static StdDequeNode Create(BaseNode innerNode) {
        var node = new StdDequeNode();
        node.ChangeInnerNode(innerNode);
        return node;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        Count = (int)context.Memory.ReadInt64(Offset + 0x20);
        if (CurrentIndex > Count)
            CurrentIndex = Count > 0 ? Count - 1 : 0;
        var name = InnerNode switch {
            BaseClassWrapperNode wrapper => $"StdDeque<{wrapper.InnerNode.Name}>",
            PointerNode { InnerNode: ClassInstanceNode pointerClass } => $"StdDeque<{pointerClass.InnerNode.Name}*>",
            ArrayNode { InnerNode: ClassInstanceNode arrayClass } => $"StdDeque<{arrayClass.InnerNode.GetType().Name.Replace("Node", string.Empty)}[]>",
            _ => $"StdDeque<{InnerNode.GetType().Name.Replace("Node", string.Empty)}>"
        };
        return CustomDraw(context, x, y, name);
        //return Draw(context, x, y, "Deque");
    }

    protected override Size DrawChild(DrawContext context, int x, int y) {
        var innerContext = context.Clone();

        var valueSize = InnerNode.MemorySize;
        var blockSize =
            valueSize <= 1 ? 16 :
            valueSize <= 2 ? 8 :
            valueSize <= 4 ? 4 :
            valueSize <= 8 ? 2 :
            1;

        var map = (nint)context.Memory.ReadIntPtr(Offset + 0x8);
        var mapSize = context.Memory.ReadUInt64(Offset + 0x10);
        var myOff = context.Memory.ReadUInt64(Offset + 0x18);

        var idx = myOff + (ulong)CurrentIndex;
        var block = (idx / (ulong)blockSize) & (mapSize - 1);
        var offset = idx % (ulong)blockSize;
        var valueAddress = context.Process.ReadRemoteIntPtr((IntPtr)((ulong)map + (ulong)IntPtr.Size * block)) + (nint)offset * valueSize; // map[block][offset]

        innerContext.Address = valueAddress;
        innerContext.Memory = new MemoryBuffer {
            Size = InnerNode.MemorySize,
            Offset = 0
        };
        innerContext.Memory.UpdateFrom(context.Process, innerContext.Address);

        return InnerNode.Draw(innerContext, x, y);
    }

    private Size CustomDraw(DrawContext context, int x, int y, string type) {
        if (IsHidden && !IsWrapped)
            return DrawHidden(context, x, y);

        var origX = x;

        AddSelection(context, x, y, context.Font.Height);

        x = AddOpenCloseIcon(context, x, y);
        x = AddIcon(context, x, y, context.IconProvider.Array, HotSpot.NoneId, HotSpotType.None);

        var tx = x;
        x = AddAddressOffset(context, x, y);

        x = AddText(context, x, y, context.Settings.PluginColor, HotSpot.NoneId, type) + context.Font.Width;
        if (!IsWrapped)
            x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name);

        x = AddText(context, x, y, context.Settings.IndexColor, HotSpot.NoneId, "[");
        x = AddText(context, x, y, context.Settings.IndexColor, IsReadOnly ? HotSpot.NoneId : 0, Count.ToString());
        x = AddText(context, x, y, context.Settings.IndexColor, HotSpot.NoneId, "]");

        x = AddIcon(context, x, y, context.IconProvider.LeftArrow, 2, HotSpotType.Click);
        x = AddText(context, x, y, context.Settings.IndexColor, HotSpot.NoneId, "(");
        x = AddText(context, x, y, context.Settings.IndexColor, 1, CurrentIndex.ToString());
        x = AddText(context, x, y, context.Settings.IndexColor, HotSpot.NoneId, ")");
        x = AddIcon(context, x, y, context.IconProvider.RightArrow, 3, HotSpotType.Click) + context.Font.Width;

        //x = AddText(context, x, y, context.Settings.ValueColor, HotSpot.NoneId, $"<Size={TotalSize}>") + context.Font.Width;
        x = AddIcon(context, x + 2, y, context.IconProvider.Change, 4, HotSpotType.ChangeWrappedType);

        x += context.Font.Width;
        x = AddComment(context, x, y);

        DrawInvalidMemoryIndicatorIcon(context, y);
        AddContextDropDownIcon(context, y);
        AddDeleteIcon(context, y);

        y += context.Font.Height;

        var size = new Size(x - origX, context.Font.Height);

        if (!LevelsOpen[context.Level])
            return size;

        var childSize = DrawChild(context, tx, y);
        size.Width = Math.Max(size.Width, childSize.Width + tx - origX);
        size.Height += childSize.Height;
        return size;
    }
}
