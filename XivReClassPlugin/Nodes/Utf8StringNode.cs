using System.Drawing;
using System.Text;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.UI;

namespace XivReClassPlugin.Nodes {
    public class Utf8StringNode : BaseNode {
        public override int MemorySize => 0x68;
        public static Encoding Encoding => Encoding.UTF8;
        
        public override int CalculateDrawnHeight(DrawContext context) {
            return IsHidden && !IsWrapped ? HiddenHeight : context.Font.Height;
        }

        public override void GetUserInterfaceInfo(out string name, out Image icon) {
            name = "Utf8String";
            icon = Resources.XivReClassResources.Utf8StringIcon;
        }

        public override string? GetToolTipText(HotSpot spot) {
            return ReadString(spot.Process, spot.Memory);
            //var ptr = spot.Memory.ReadIntPtr(Offset);
            //if (!ptr.MayBeValid()) return null;
            //var length = spot.Memory.ReadInt64(Offset + 0x10);
            //if (length <= 1) return null;
            //var text = spot.Process.ReadRemoteString(ptr, Encoding, (int)(length - 1));
            //return string.IsNullOrEmpty(text) ? null : text;
        }

        private string? ReadString(IRemoteMemoryReader proc, MemoryBuffer mem) {
            var ptr = mem.ReadIntPtr(Offset);
            if (!ptr.MayBeValid()) return null;
            var length = mem.ReadInt64(Offset + 0x10);
            return length <= 1 ? null : proc.ReadRemoteString(ptr, Encoding, (int)(length - 1));
        }
        
        public override Size Draw(DrawContext context, int x, int y) {
            if (IsHidden && !IsWrapped) {
                return DrawHidden(context, x, y);
            }

            //var ptr = context.Memory.ReadIntPtr(Offset);
            //var length = context.Memory.ReadInt64(Offset + 0x10);
            //var text = context.Process.ReadRemoteString(ptr, Encoding, (int)(length > 0 ? length - 1 : 0));
            var text = ReadString(context.Process, context.Memory) ?? string.Empty;
            
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