using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed partial class GuiContainer
    {
        private Stack<HorizontalLayout> _layouts = new Stack<HorizontalLayout>();
        private int _globallyIncrementingId;

        public bool Button(string label, bool disabled)
        {
            if (!disabled) return Button(label);

            PushStyleVar(ImGuiStyleVar.Alpha, Style.Alpha * 0.5F);
            Button(label);
            PopStyleVar();
            return false;
        }

        public bool ButtonDisabled(string label) => Button(label, true);
        
        public bool Button(string label, Vector2 size, bool disabled)
        {
            if (!disabled) return Button(label);

            PushStyleVar(ImGuiStyleVar.Alpha, Style.Alpha * 0.5F);
            Button(label, size);
            PopStyleVar();
            return false;
        }
        
        public bool ButtonDisabled(string label, Vector2 size) => Button(label, size, true);

        public bool Combo(string label, ref int currentItem, params string[] items) => Combo(label, ref currentItem, items, items.Length);
        public bool Combo(string label, ref int currentItem, int popupMaxHeightInItems, params string[] items) => Combo(label, ref currentItem, items, items.Length, popupMaxHeightInItems);

        // Always call a matching EndChild() for each BeginChild() call, regardless of its return value [as with Begin:
        // this is due to legacy reason and inconsistent with most BeginXXX functions apart from the regular Begin()
        // which behaves like BeginChild().]

        /// <summary>
        /// Begins a layout in which child elements are all present on the same line and determine their width based on
        /// a percentage of the available space in the parent element (or in <paramref name="bounds"/> if specified) 
        /// </summary>
        /// <param name="inSubPanel">
        /// If <c>true</c>, creates a child panel to place subsequent elements in. The panel is closed when the layout
        /// ends.
        /// </param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool BeginHorizontalLayout(bool inSubPanel = false, Vector2? bounds = null)
        {
            _layouts.Push(new HorizontalLayout(inSubPanel, bounds ?? GetContentRegionAvail()));
            if (inSubPanel)
            {
                return BeginChild($"~HorL{_globallyIncrementingId++}", bounds ?? default);
            }

            return true;
        }

        // fillX is a percentage between 0F and 1F

        public bool BeginHorizontalChild(string strId, float fillX, float? sizeY = null, bool border = false, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        {
            SameLine();
            return BeginChild(strId, _layouts.Peek().CalculateSize(fillX, sizeY), border, flags);
        }
        public bool BeginHorizontalChild(uint id, float fillX, float? sizeY = null, bool border = false, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        {
            SameLine();
            return BeginChild(id, _layouts.Peek().CalculateSize(fillX, sizeY), border, flags);
        }

        public void EndHorizontalLayout()
        {
            var layout = _layouts.Pop();
            if (layout.InSubPanel)
            {
                EndChild();
            }
        }
    }

    internal readonly struct HorizontalLayout
    {
        public readonly bool InSubPanel;
        public readonly Vector2 Bounds;

        public HorizontalLayout(bool inSubPanel, Vector2 bounds)
        {
            InSubPanel = inSubPanel;
            Bounds = bounds;
        }

        public Vector2 CalculateSize(float fillX, float? sizeY)
        {
            return new Vector2(Bounds.X * fillX, sizeY ?? Bounds.Y);
        }
    }
}