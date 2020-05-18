using System.Numerics;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui
{
    public readonly struct ImguiColors
    {
        private readonly RangeAccessor<Vector4> _range;

        public Vector4Color this[ImGuiCol color]
        {
            get => _range[(int) color];
            set => _range[(int) color] = value;
        }

        public ImguiColors(RangeAccessor<Vector4> range)
        {
            _range = range;
        }
    }
}