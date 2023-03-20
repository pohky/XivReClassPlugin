using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Nodes;
using ReClassNET.UI;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin.Nodes; 

public class AtkValueNode : BaseWrapperNode {
	public override int MemorySize => 0x10;
	protected override bool PerformCycleCheck => false;

	private readonly Dictionary<AtkValueType, BaseNode> m_NodeMap = new() {
		{ AtkValueType.None, new Hex64Node { Name = "UnknownValue", Offset = 0x08 } },
		{ AtkValueType.Bool, new BoolNode { Name = "BoolValue", Offset = 0x08 } },
		{ AtkValueType.Int, new Int32Node { Name = "IntValue", Offset = 0x08 } },
		{ AtkValueType.UInt, new UInt32Node { Name = "UIntValue", Offset = 0x08 } },
		{ AtkValueType.Float, new FloatNode { Name = "FloatValue", Offset = 0x08 } },
		{ AtkValueType.String, new Utf8TextPtrNode { Name = "StringValue", Offset = 0x08 } },
		{ AtkValueType.String8, new Utf8TextPtrNode { Name = "String8Value", Offset = 0x08 } },
		{ AtkValueType.AllocatedString, new Utf8TextPtrNode { Name = "AllocatedStringValue", Offset = 0x08 } },
		{ AtkValueType.Texture, new Hex64Node { Name = "TextureValue", Offset = 0x08 } }
	};

	public AtkValueNode() {
		var vectorNode = VectorNode.Create(new Hex64Node());
		var pointerNode = new PointerNode { Offset = 8 };
		pointerNode.ChangeInnerNode(vectorNode);
		m_NodeMap.Add(AtkValueType.Vector, pointerNode);
		m_NodeMap.Add(AtkValueType.AllocatedVector, pointerNode);
	}

	public override bool CanChangeInnerNodeTo(BaseNode node) {
		return true;
	}

	public override void GetUserInterfaceInfo(out string name, out Image icon) {
		name = "AtkValue";
		icon = XivReClassResources.AtkValueIcon;
	}

	public override string GetToolTipText(HotSpot spot) {
		return InnerNode.GetToolTipText(spot);
	}

	private BaseNode GetNodeForType(AtkValueType type) {
		if (!m_NodeMap.TryGetValue(type, out var node))
			node = m_NodeMap[AtkValueType.None];
		node.Offset = Offset + 8;
		return node;
	}

	private AtkValueType ReadType(DrawContext context) {
		return (AtkValueType)context.Memory.ReadInt32(Offset);
	}

	private string ReadValueString(DrawContext context, AtkValueType type) {
		return type switch {
			AtkValueType.None => string.Empty,
			AtkValueType.Int => $"{context.Memory.ReadInt32(Offset + 8)}",
			AtkValueType.Bool => $"{context.Memory.ReadInt8(Offset + 8) != 0}",
			AtkValueType.UInt => $"{context.Memory.ReadUInt32(Offset + 8)}",
			AtkValueType.Float => $"{context.Memory.ReadFloat(Offset + 8)}",
			AtkValueType.String => $"'{context.Process.ReadRemoteString(context.Memory.ReadIntPtr(Offset + 8), Encoding.UTF8, 256)}'",
			AtkValueType.String8 => $"'{context.Process.ReadRemoteString(context.Memory.ReadIntPtr(Offset + 8), Encoding.UTF8, 256)}'",
			AtkValueType.AllocatedString => $"'{context.Process.ReadRemoteString(context.Memory.ReadIntPtr(Offset + 8), Encoding.UTF8, 256)}'",
			_ => $"0x{context.Memory.ReadIntPtr(Offset + 8).ToInt64():X8}"
		};
	}

	public override Size Draw(DrawContext context, int x, int y) {
		if (IsHidden && !IsWrapped)
			return DrawHidden(context, x, y);

		var origX = x;
		var origY = y;

		AddSelection(context, x, y, context.Font.Height);

		var type = ReadType(context);
		ChangeInnerNode(GetNodeForType(type));

		x = InnerNode != null ? AddOpenCloseIcon(context, x, y) : AddIconPadding(context, x);
		x = AddIconPadding(context, x);

		var tx = x;
		x = AddAddressOffset(context, x, y);

		x = AddText(context, x, y, context.Settings.PluginColor, HotSpot.NoneId, "AtkValue") + context.Font.Width;
		if (!IsWrapped)
			x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;

		var typeString = Enum.IsDefined(typeof(AtkValueType), type) && type != 0 ? $"<{type}>" : $"<{(int)type}>";
		x = AddText(context, x, y, context.Settings.TypeColor, HotSpot.NoneId, typeString) + context.Font.Width;
		if (!LevelsOpen[context.Level]) {
			var valueString = ReadValueString(context, type);
			x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, valueString) + context.Font.Width;
		}
			
		DrawInvalidMemoryIndicatorIcon(context, y);
		AddContextDropDownIcon(context, y);
		AddDeleteIcon(context, y);

		y += context.Font.Height;

		var size = new Size(x - origX, y - origY);

		if (!LevelsOpen[context.Level] || InnerNode == null)
			return size;

		var innerSize = InnerNode.Draw(context, tx, y);
		size.Width = Math.Max(size.Width, innerSize.Width + tx - origX);
		size.Height += innerSize.Height;
		return size;
	}

	public override int CalculateDrawnHeight(DrawContext context) {
		return IsHidden && !IsWrapped ? HiddenHeight : context.Font.Height;
	}
}

public enum AtkValueType {
	None = 0x00,
	Int = 0x03,
	Bool = 0x02,
	UInt = 0x04,
	Float = 0x05,
	String = 0x06,
	String8 = 0x08,
	Vector = 0x09,
	Texture = 0x0A,
	AllocatedString = 0x26,
	AllocatedVector = 0x29
}