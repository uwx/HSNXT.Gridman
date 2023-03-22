using System;
using System.Numerics;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed partial class GuiContainer
    {
        private static readonly Action ActionPopId = ImGui.PopID;
        private static readonly Action ActionPopAllowKeyboardFocus = ImGui.PopAllowKeyboardFocus;
        private static readonly Action ActionPopButtonRepeat = ImGui.PopButtonRepeat;
        private static readonly Action ActionPopClipRect = ImGui.PopClipRect;
        private static readonly Action ActionPopFont = ImGui.PopFont;
        private static readonly Action ActionPopItemWidth = ImGui.PopItemWidth;
        private static readonly Action ActionPopStyleColor = ImGui.PopStyleColor;
        private static readonly Action ActionPopStyleVar = ImGui.PopStyleVar;
        private static readonly Action ActionPopTextWrapPos = ImGui.PopTextWrapPos;

        public GuiHandle WithId(string strId)
        {
            PushId(strId);
            return new GuiHandle(ActionPopId);
        }

        public GuiHandle WithId(IntPtr ptrId)
        {
            PushId(ptrId);
            return new GuiHandle(ActionPopId);
        }

        public GuiHandle WithId(int intId)
        {
            PushId(intId);
            return new GuiHandle(ActionPopId);
        }

        public GuiHandle WithAllowKeyboardFocus(bool allowKeyboardFocus)
        {
            PushAllowKeyboardFocus(allowKeyboardFocus);
            return new GuiHandle(ActionPopAllowKeyboardFocus);
        }

        public GuiHandle WithButtonRepeat(bool repeat)
        {
            PushButtonRepeat(repeat);
            return new GuiHandle(ActionPopButtonRepeat);
        }

        public GuiHandle WithClipRect(Vector2 clipRectMin, Vector2 clipRectMax, bool intersectWithCurrentClipRect)
        {
            PushClipRect(clipRectMin, clipRectMax,  intersectWithCurrentClipRect);
            return new GuiHandle(ActionPopClipRect);
        }

        public GuiHandle WithFont(ImFontPtr font)
        {
            PushFont(font);
            return new GuiHandle(ActionPopFont);
        }

        public GuiHandle WithItemWidth(float itemWidth)
        {
            PushItemWidth(itemWidth);
            return new GuiHandle(ActionPopItemWidth);
        }

        public GuiHandle WithStyleColor(ImGuiCol idx, uint col)
        {
            PushStyleColor(idx, col);
            return new GuiHandle(ActionPopStyleColor);
        }

        public GuiHandle WithStyleColor(ImGuiCol idx, Vector4 col)
        {
            PushStyleColor(idx, col);
            return new GuiHandle(ActionPopStyleColor);
        }

        public GuiHandle WithStyleVar(ImGuiStyleVar idx, float val)
        {
            PushStyleVar(idx, val);
            return new GuiHandle(ActionPopStyleVar);
        }

        public GuiHandle WithStyleVar(ImGuiStyleVar idx, Vector2 val)
        {
            PushStyleVar(idx, val);
            return new GuiHandle(ActionPopStyleVar);
        }

        public GuiHandle WithTextWrapPos()
        {
            PushTextWrapPos();
            return new GuiHandle(ActionPopTextWrapPos);
        }

        public GuiHandle WithTextWrapPos(float wrapLocalPosX)
        {
            PushTextWrapPos(wrapLocalPosX);
            return new GuiHandle(ActionPopTextWrapPos);
        }
    }

    public readonly struct GuiHandle : IDisposable
    {
        private readonly Action _endAction;

        internal GuiHandle(Action endAction)
        {
            _endAction = endAction;
        }

        public void Dispose()
        {
            _endAction();
        }
    }
}