using System.Numerics;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed partial class GuiContainer
    {
        private WindowElementContext? _autogenWindow;
        public WindowElementContext Window => _autogenWindow ??= new WindowElementContext();

        public readonly struct WindowElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 0;

            internal WindowElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // Begin(string name) => ImGui.Begin(name);
            public bool Begin(string name) => _gui.PushDefine(ImGui.Begin(name), Identity);
            // Begin(string name, ref bool pOpen) => ImGui.Begin(name, ref pOpen);
            public bool Begin(string name, ref bool pOpen) => _gui.PushDefine(ImGui.Begin(name, ref pOpen), Identity);
            // Begin(string name, ref bool pOpen, ImGuiWindowFlags flags) => ImGui.Begin(name, ref pOpen, flags);
            public bool Begin(string name, ref bool pOpen, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.Begin(name, ref pOpen, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.End();
                }
            }
        }
        private ChildElementContext? _autogenChild;
        public ChildElementContext Child => _autogenChild ??= new ChildElementContext();

        public readonly struct ChildElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 1;

            internal ChildElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginChild(string strId) => ImGui.BeginChild(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginChild(strId), Identity);
            // BeginChild(string strId, Vector2 size) => ImGui.BeginChild(strId, size);
            public bool Begin(string strId, Vector2 size) => _gui.PushDefine(ImGui.BeginChild(strId, size), Identity);
            // BeginChild(string strId, Vector2 size, bool border) => ImGui.BeginChild(strId, size, border);
            public bool Begin(string strId, Vector2 size, bool border) => _gui.PushDefine(ImGui.BeginChild(strId, size, border), Identity);
            // BeginChild(string strId, Vector2 size, bool border, ImGuiWindowFlags flags) => ImGui.BeginChild(strId, size, border, flags);
            public bool Begin(string strId, Vector2 size, bool border, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.BeginChild(strId, size, border, flags), Identity);
            // BeginChild(uint id) => ImGui.BeginChild(id);
            public bool Begin(uint id) => _gui.PushDefine(ImGui.BeginChild(id), Identity);
            // BeginChild(uint id, Vector2 size) => ImGui.BeginChild(id, size);
            public bool Begin(uint id, Vector2 size) => _gui.PushDefine(ImGui.BeginChild(id, size), Identity);
            // BeginChild(uint id, Vector2 size, bool border) => ImGui.BeginChild(id, size, border);
            public bool Begin(uint id, Vector2 size, bool border) => _gui.PushDefine(ImGui.BeginChild(id, size, border), Identity);
            // BeginChild(uint id, Vector2 size, bool border, ImGuiWindowFlags flags) => ImGui.BeginChild(id, size, border, flags);
            public bool Begin(uint id, Vector2 size, bool border, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.BeginChild(id, size, border, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndChild();
                }
            }
        }
        private ChildFrameElementContext? _autogenChildFrame;
        public ChildFrameElementContext ChildFrame => _autogenChildFrame ??= new ChildFrameElementContext();

        public readonly struct ChildFrameElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 2;

            internal ChildFrameElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginChildFrame(uint id, Vector2 size) => ImGui.BeginChildFrame(id, size);
            public bool Begin(uint id, Vector2 size) => _gui.PushDefine(ImGui.BeginChildFrame(id, size), Identity);
            // BeginChildFrame(uint id, Vector2 size, ImGuiWindowFlags flags) => ImGui.BeginChildFrame(id, size, flags);
            public bool Begin(uint id, Vector2 size, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.BeginChildFrame(id, size, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndChildFrame();
                }
            }
        }
        private ComboSelectorElementContext? _autogenComboSelector;
        public ComboSelectorElementContext ComboSelector => _autogenComboSelector ??= new ComboSelectorElementContext();

        public readonly struct ComboSelectorElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 3;

            internal ComboSelectorElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginCombo(string label, string previewValue) => ImGui.BeginCombo(label, previewValue);
            public bool Begin(string label, string previewValue) => _gui.PushDefine(ImGui.BeginCombo(label, previewValue), Identity);
            // BeginCombo(string label, string previewValue, ImGuiComboFlags flags) => ImGui.BeginCombo(label, previewValue, flags);
            public bool Begin(string label, string previewValue, ImGuiComboFlags flags) => _gui.PushDefine(ImGui.BeginCombo(label, previewValue, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndCombo();
                }
            }
        }
        private DragDropSourceElementContext? _autogenDragDropSource;
        public DragDropSourceElementContext DragDropSource => _autogenDragDropSource ??= new DragDropSourceElementContext();

        public readonly struct DragDropSourceElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 4;

            internal DragDropSourceElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginDragDropSource() => ImGui.BeginDragDropSource();
            public bool Begin() => _gui.PushDefine(ImGui.BeginDragDropSource(), Identity);
            // BeginDragDropSource(ImGuiDragDropFlags flags) => ImGui.BeginDragDropSource(flags);
            public bool Begin(ImGuiDragDropFlags flags) => _gui.PushDefine(ImGui.BeginDragDropSource(flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndDragDropSource();
                }
            }
        }
        private DragDropTargetElementContext? _autogenDragDropTarget;
        public DragDropTargetElementContext DragDropTarget => _autogenDragDropTarget ??= new DragDropTargetElementContext();

        public readonly struct DragDropTargetElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 5;

            internal DragDropTargetElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginDragDropTarget() => ImGui.BeginDragDropTarget();
            public bool Begin() => _gui.PushDefine(ImGui.BeginDragDropTarget(), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndDragDropTarget();
                }
            }
        }
        private GroupElementContext? _autogenGroup;
        public GroupElementContext Group => _autogenGroup ??= new GroupElementContext();

        public readonly struct GroupElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 6;

            internal GroupElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginGroup() => ImGui.BeginGroup();
            public bool Begin() { ImGui.BeginGroup(); return _gui.PushDefine(true, Identity); }

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndGroup();
                }
            }
        }
        private MainMenuBarElementContext? _autogenMainMenuBar;
        public MainMenuBarElementContext MainMenuBar => _autogenMainMenuBar ??= new MainMenuBarElementContext();

        public readonly struct MainMenuBarElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 7;

            internal MainMenuBarElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginMainMenuBar() => ImGui.BeginMainMenuBar();
            public bool Begin() => _gui.PushDefine(ImGui.BeginMainMenuBar(), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndMainMenuBar();
                }
            }
        }
        private MenuElementContext? _autogenMenu;
        public MenuElementContext Menu => _autogenMenu ??= new MenuElementContext();

        public readonly struct MenuElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 8;

            internal MenuElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginMenu(string label) => ImGui.BeginMenu(label);
            public bool Begin(string label) => _gui.PushDefine(ImGui.BeginMenu(label), Identity);
            // BeginMenu(string label, bool enabled) => ImGui.BeginMenu(label, enabled);
            public bool Begin(string label, bool enabled) => _gui.PushDefine(ImGui.BeginMenu(label, enabled), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndMenu();
                }
            }
        }
        private MenuBarElementContext? _autogenMenuBar;
        public MenuBarElementContext MenuBar => _autogenMenuBar ??= new MenuBarElementContext();

        public readonly struct MenuBarElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 9;

            internal MenuBarElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginMenuBar() => ImGui.BeginMenuBar();
            public bool Begin() => _gui.PushDefine(ImGui.BeginMenuBar(), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndMenuBar();
                }
            }
        }
        private PopupElementContext? _autogenPopup;
        public PopupElementContext Popup => _autogenPopup ??= new PopupElementContext();

        public readonly struct PopupElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 10;

            internal PopupElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginPopup(string strId) => ImGui.BeginPopup(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginPopup(strId), Identity);
            // BeginPopup(string strId, ImGuiWindowFlags flags) => ImGui.BeginPopup(strId, flags);
            public bool Begin(string strId, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.BeginPopup(strId, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndPopup();
                }
            }
        }
        private PopupContextItemElementContext? _autogenPopupContextItem;
        public PopupContextItemElementContext PopupContextItem => _autogenPopupContextItem ??= new PopupContextItemElementContext();

        public readonly struct PopupContextItemElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 11;

            internal PopupContextItemElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginPopupContextItem() => ImGui.BeginPopupContextItem();
            public bool Begin() => _gui.PushDefine(ImGui.BeginPopupContextItem(), Identity);
            // BeginPopupContextItem(string strId) => ImGui.BeginPopupContextItem(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginPopupContextItem(strId), Identity);
            // BeginPopupContextItem(string strId, int mouseButton) => ImGui.BeginPopupContextItem(strId, mouseButton);
            public bool Begin(string strId, int mouseButton) => _gui.PushDefine(ImGui.BeginPopupContextItem(strId, mouseButton), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndPopup();
                }
            }
        }
        private PopupContextVoidElementContext? _autogenPopupContextVoid;
        public PopupContextVoidElementContext PopupContextVoid => _autogenPopupContextVoid ??= new PopupContextVoidElementContext();

        public readonly struct PopupContextVoidElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 12;

            internal PopupContextVoidElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginPopupContextVoid() => ImGui.BeginPopupContextVoid();
            public bool Begin() => _gui.PushDefine(ImGui.BeginPopupContextVoid(), Identity);
            // BeginPopupContextVoid(string strId) => ImGui.BeginPopupContextVoid(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginPopupContextVoid(strId), Identity);
            // BeginPopupContextVoid(string strId, int mouseButton) => ImGui.BeginPopupContextVoid(strId, mouseButton);
            public bool Begin(string strId, int mouseButton) => _gui.PushDefine(ImGui.BeginPopupContextVoid(strId, mouseButton), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndPopup();
                }
            }
        }
        private PopupContextWindowElementContext? _autogenPopupContextWindow;
        public PopupContextWindowElementContext PopupContextWindow => _autogenPopupContextWindow ??= new PopupContextWindowElementContext();

        public readonly struct PopupContextWindowElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 13;

            internal PopupContextWindowElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginPopupContextWindow() => ImGui.BeginPopupContextWindow();
            public bool Begin() => _gui.PushDefine(ImGui.BeginPopupContextWindow(), Identity);
            // BeginPopupContextWindow(string strId) => ImGui.BeginPopupContextWindow(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginPopupContextWindow(strId), Identity);
            // BeginPopupContextWindow(string strId, int mouseButton) => ImGui.BeginPopupContextWindow(strId, mouseButton);
            public bool Begin(string strId, int mouseButton) => _gui.PushDefine(ImGui.BeginPopupContextWindow(strId, mouseButton), Identity);
            // BeginPopupContextWindow(string strId, int mouseButton, bool alsoOverItems) => ImGui.BeginPopupContextWindow(strId, mouseButton, alsoOverItems);
            public bool Begin(string strId, int mouseButton, bool alsoOverItems) => _gui.PushDefine(ImGui.BeginPopupContextWindow(strId, mouseButton, alsoOverItems), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndPopup();
                }
            }
        }
        private PopupModalElementContext? _autogenPopupModal;
        public PopupModalElementContext PopupModal => _autogenPopupModal ??= new PopupModalElementContext();

        public readonly struct PopupModalElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 14;

            internal PopupModalElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginPopupModal(string name) => ImGui.BeginPopupModal(name);
            public bool Begin(string name) => _gui.PushDefine(ImGui.BeginPopupModal(name), Identity);
            // BeginPopupModal(string name, ref bool pOpen) => ImGui.BeginPopupModal(name, ref pOpen);
            public bool Begin(string name, ref bool pOpen) => _gui.PushDefine(ImGui.BeginPopupModal(name, ref pOpen), Identity);
            // BeginPopupModal(string name, ref bool pOpen, ImGuiWindowFlags flags) => ImGui.BeginPopupModal(name, ref pOpen, flags);
            public bool Begin(string name, ref bool pOpen, ImGuiWindowFlags flags) => _gui.PushDefine(ImGui.BeginPopupModal(name, ref pOpen, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndPopup();
                }
            }
        }
        private TabBarElementContext? _autogenTabBar;
        public TabBarElementContext TabBar => _autogenTabBar ??= new TabBarElementContext();

        public readonly struct TabBarElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 15;

            internal TabBarElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginTabBar(string strId) => ImGui.BeginTabBar(strId);
            public bool Begin(string strId) => _gui.PushDefine(ImGui.BeginTabBar(strId), Identity);
            // BeginTabBar(string strId, ImGuiTabBarFlags flags) => ImGui.BeginTabBar(strId, flags);
            public bool Begin(string strId, ImGuiTabBarFlags flags) => _gui.PushDefine(ImGui.BeginTabBar(strId, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndTabBar();
                }
            }
        }
        private TabItemElementContext? _autogenTabItem;
        public TabItemElementContext TabItem => _autogenTabItem ??= new TabItemElementContext();

        public readonly struct TabItemElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 16;

            internal TabItemElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginTabItem(string label) => ImGui.BeginTabItem(label);
            public bool Begin(string label) => _gui.PushDefine(ImGui.BeginTabItem(label), Identity);
            // BeginTabItem(string label, ref bool pOpen) => ImGui.BeginTabItem(label, ref pOpen);
            public bool Begin(string label, ref bool pOpen) => _gui.PushDefine(ImGui.BeginTabItem(label, ref pOpen), Identity);
            // BeginTabItem(string label, ref bool pOpen, ImGuiTabItemFlags flags) => ImGui.BeginTabItem(label, ref pOpen, flags);
            public bool Begin(string label, ref bool pOpen, ImGuiTabItemFlags flags) => _gui.PushDefine(ImGui.BeginTabItem(label, ref pOpen, flags), Identity);

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndTabItem();
                }
            }
        }
        private TooltipElementContext? _autogenTooltip;
        public TooltipElementContext Tooltip => _autogenTooltip ??= new TooltipElementContext();

        public readonly struct TooltipElementContext
        {
            private readonly GuiContainer _gui;
            private const uint Identity = 17;

            internal TooltipElementContext(GuiContainer gui)
            {
                _gui = gui;
            }

            // BeginTooltip() => ImGui.BeginTooltip();
            public bool Begin() { ImGui.BeginTooltip(); return _gui.PushDefine(true, Identity); }

            public void End()
            {
                if (_gui.PopDefine(Identity))
                {
                    _gui.EndTooltip();
                }
            }
        }

        private static readonly string[] ElementContextIdentityToName =
        {
            "WindowElementContext",
            "ChildElementContext",
            "ChildFrameElementContext",
            "ComboSelectorElementContext",
            "DragDropSourceElementContext",
            "DragDropTargetElementContext",
            "GroupElementContext",
            "MainMenuBarElementContext",
            "MenuElementContext",
            "MenuBarElementContext",
            "PopupElementContext",
            "PopupContextItemElementContext",
            "PopupContextVoidElementContext",
            "PopupContextWindowElementContext",
            "PopupModalElementContext",
            "TabBarElementContext",
            "TabItemElementContext",
            "TooltipElementContext",
        };
    }
}