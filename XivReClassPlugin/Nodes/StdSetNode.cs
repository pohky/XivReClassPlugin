using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.UI;
using System;
using System.Drawing;
using System.Linq;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin.Nodes;

public class StdSetNode : BaseWrapperArrayNode {
    public override int MemorySize => IntPtr.Size * 2;
    public long TotalSize { get; private set; }

    public StdSetNode() {
        IsReadOnly = true;
    }

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "StdSet<T>";
        icon = XivReClassResources.StdVectorIcon;
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

    public static StdSetNode Create(BaseNode innerNode) {
        var node = new StdSetNode();
        node.ChangeInnerNode(innerNode);
        return node;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        Count = Math.Max(context.Memory.ReadInt32(Offset + IntPtr.Size), 0);
        TotalSize = Count * InnerNode.MemorySize;
        var name = InnerNode switch {
            BaseClassWrapperNode wrapper => $"StdSet<{wrapper.InnerNode.Name}>",
            PointerNode { InnerNode: ClassInstanceNode pointerClass } => $"StdSet<{pointerClass.InnerNode.Name}*>",
            ArrayNode { InnerNode: ClassInstanceNode arrayClass } => $"StdSet<{arrayClass.InnerNode.GetType().Name.Replace("Node", string.Empty)}[]>",
            _ => $"StdSet<{InnerNode.GetType().Name.Replace("Node", string.Empty)}>"
        };
        return CustomDraw(context, x, y, name);
        //return Draw(context, x, y, "Vector");
    }

    private int CalculateAlignment(BaseNode node) {
        return node switch {
            PointerNode => IntPtr.Size,
            BaseHexNode => 0,
            BaseWrapperNode baseWrapperNode when baseWrapperNode.InnerNode != node => CalculateAlignment(baseWrapperNode.InnerNode),
            ClassNode classNode => classNode.Nodes.Select(CalculateAlignment).Max(),
            _ => node.MemorySize
        };
    }

    protected override Size DrawChild(DrawContext context, int x, int y) {
        var innerContext = context.Clone();

        var head = context.Memory.ReadIntPtr(Offset);
        var current = head;

        var valueOffset = IntPtr.Size * 3 + 2; // Left, Parent, Right, Color, IsNil
        var alignment = CalculateAlignment(InnerNode);
        var remainder = alignment > 0 ? valueOffset % alignment : 0;
        if (remainder != 0)
            valueOffset += alignment - remainder;

        nint GetLeft(nint node) {
            return context.Process.ReadRemoteIntPtr(node);
        }

        nint GetParent(nint node) {
            return context.Process.ReadRemoteIntPtr(node + 0x08);
        }

        nint GetRight(nint node) {
            return context.Process.ReadRemoteIntPtr(node + 0x10);
        }

        bool IsNil(nint node) {
            return context.Process.ReadRemoteUInt8(node + 0x19) == 1;
        }

        nint Next(nint node) {
            if (IsNil(node))
                throw new Exception("Tried to increment a head node.");

            if (IsNil(GetRight(node))) { // if (Right->IsNil)
                var ptr = node;
                while (!IsNil(node = GetParent(ptr)) && ptr == GetRight(node)) // while (!(node = ptr->Parent)->IsNil && ptr == node->Right)
                    ptr = node;
                return node;
            }

            var ret = GetRight(node); // var ret = Right;
            while (!IsNil(GetLeft(ret))) // while (!ret->Left->IsNil)
                ret = GetLeft(ret); // ret = ret->Left;
            return ret;
        }

        bool MoveNext() {
            if (current == IntPtr.Zero || current == GetRight(head))
                return false;
            current = current == head ? GetLeft(head) : Next(current);
            return current != IntPtr.Zero;
        }

        var index = 0;
        while (index < Count && MoveNext() && index++ < CurrentIndex) ;

        innerContext.Address = current + valueOffset;
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

        x = AddText(context, x, y, context.Settings.ValueColor, HotSpot.NoneId, $"<Size={TotalSize}>") + context.Font.Width;
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
