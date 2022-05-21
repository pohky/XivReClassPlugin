using System.Drawing;
using System.Text;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.UI;
using XivReClassPlugin.Resources;

namespace XivReClassPlugin.Nodes {
    public class Utf8StringNode : BaseNode {
        public override int MemorySize => 0x68;
        public static Encoding Encoding => Encoding.UTF8;

        public override int CalculateDrawnHeight(DrawContext context) {
            return IsHidden && !IsWrapped ? HiddenHeight : context.Font.Height;
        }

        public override void GetUserInterfaceInfo(out string name, out Image icon) {
            name = "Utf8String";
            icon = XivReClassResources.Utf8StringIcon;
        }

        public override string? GetToolTipText(HotSpot spot) {
            return ReadString(spot.Process, spot.Memory, short.MaxValue);
        }

        private string? ReadString(IRemoteMemoryReader proc, MemoryBuffer mem, int maxLength) {
            var ptr = mem.ReadIntPtr(Offset);
            if (!ptr.MayBeValid()) return null;
            var length = mem.ReadInt64(Offset + 0x10);
            if (length > maxLength && maxLength > 0)
                return proc.ReadRemoteString(ptr, Encoding, maxLength);
            return length <= 1 ? null : proc.ReadRemoteString(ptr, Encoding, (int)(length - 1));
        }

        public override Size Draw(DrawContext context, int x, int y) {
            if (IsHidden && !IsWrapped)
                return DrawHidden(context, x, y);

            var text = ReadString(context.Process, context.Memory, 256) ?? string.Empty;

            var origX = x;

            AddSelection(context, x, y, context.Font.Height);

            x = AddIconPadding(context, x);
            x = AddIcon(context, x, y, context.IconProvider.Text, HotSpot.NoneId, HotSpotType.None);
            x = AddAddressOffset(context, x, y);

            x = AddText(context, x, y, context.Settings.PluginColor, HotSpot.NoneId, "UTF8") + context.Font.Width;
            if (!IsWrapped)
                x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;

            x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "= '");
            x = AddText(context, x, y, context.Settings.TextColor, HotSpot.ReadOnlyId, text);
            x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "'") + context.Font.Width;

            DrawInvalidMemoryIndicatorIcon(context, y);
            AddContextDropDownIcon(context, y);
            AddDeleteIcon(context, y);

            return new Size(x - origX, context.Font.Height);
        }
    }
}