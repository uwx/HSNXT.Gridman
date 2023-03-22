using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using HSNXT.QuickAndDirtyGui.Extensions;
using ImGuiNET;
using JetBrains.Annotations;
using Veldrid;
using Veldrid.Sdl2;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed class GuiWindowBuilder
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Used to indicate that the window position should be centered.
        /// </summary>
        public const int SDL_WINDOWPOS_CENTERED = Sdl2Native.SDL_WINDOWPOS_CENTERED;

        /// <summary>
        /// Used to indicate that you don't care what the window position is.
        /// </summary>
        public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
        // ReSharper restore InconsistentNaming
        
        private int _x = SDL_WINDOWPOS_UNDEFINED;
        private int _y = SDL_WINDOWPOS_UNDEFINED;
        private int _width = 800;
        private int _height = 600;
        private WindowState _state = WindowState.Normal;
        private string _title = "GUI Window";
        private Action<GuiContainer>? _init;
        private Action<GuiContainer>? _update;
        private float _fps = 60;
        private bool _vsync = true;
        private Color? _backgroundColor = Color.FromArgb(255, 255, 255);
        
        public GuiWindowBuilder WithPosition(int x, int y)
        {
            _x = x;
            _y = y;
            return this;
        }

        public GuiWindowBuilder WithCentered()
        {
            _x = SDL_WINDOWPOS_CENTERED;
            _y = SDL_WINDOWPOS_CENTERED;
            return this;
        }

        public GuiWindowBuilder WithSize(int width, int height)
        {
            _width = width;
            _height = height;
            return this;
        }

        public GuiWindowBuilder IsFullScreen()
        {
            _state = WindowState.FullScreen;
            return this;
        }

        public GuiWindowBuilder IsMaximized()
        {
            _state = WindowState.Maximized;
            return this;
        }

        public GuiWindowBuilder IsMinimized()
        {
            _state = WindowState.Minimized;
            return this;
        }

        public GuiWindowBuilder IsBorderlessFullScreen()
        {
            _state = WindowState.BorderlessFullScreen;
            return this;
        }

        public GuiWindowBuilder IsHidden()
        {
            _state = WindowState.Hidden;
            return this;
        }

        public GuiWindowBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public GuiWindowBuilder OnFirstUpdate(Action<GuiContainer>? init)
        {
            _init = init;
            return this;
        }

        public GuiWindowBuilder OnUpdate(Action<GuiContainer>? update)
        {
            _update = update;
            return this;
        }

        public GuiWindowBuilder WithRefreshRate(float fps)
        {
            _fps = fps;
            return this;
        }

        public GuiWindowBuilder DisableWaitForVblank()
        {
            _vsync = false;
            return this;
        }

        public GuiWindowBuilder WithBackgroundColor(Color color)
        {
            _backgroundColor = color;
            return this;
        }

        public static async Task CreateAsync(Action<GuiContainer> onUpdate, Action<GuiContainer>? onFirstUpdate = null)
        {
            await new GuiWindowBuilder()
                .OnUpdate(onUpdate)
                .OnFirstUpdate(onFirstUpdate)
                .ShowAsync();
        }
        
        public static void Create(Action<GuiContainer> onUpdate, Action<GuiContainer>? onFirstUpdate = null)
        {
            new GuiWindowBuilder()
                .OnUpdate(onUpdate)
                .OnFirstUpdate(onFirstUpdate)
                .ShowAsync()
                .GetAwaiter()
                .GetResult();
        }

        public Task ShowAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var guiWindow = new GuiWindow(_x, _y, _width, _height, _state, _title)
                {
                    OnInit = _init,
                    OnLayout = _update,
                    Fps = _fps,
                    VSync = _vsync,
                    BackgroundColor = _backgroundColor,
                };
                guiWindow.Show();
            }, TaskCreationOptions.LongRunning);
        }
    }

    [PublicAPI]
    internal sealed class GuiWindow
    {
        internal Action<GuiContainer>? OnLayout { private get; set; }
        internal Action<GuiContainer>? OnInit { private get; set; }
        
        internal float Fps
        {
            set => _imguiWindow.Fps = value;
        }
        internal bool VSync
        {
            set => _imguiWindow.VSync = value;
        }
        internal Color? BackgroundColor
        {
            set
            {
                if (value != null) _imguiWindow.BackgroundColor = value.Value;
            }
        }

        internal bool Resizable { private get; set; } = true;
        internal bool ForceHideBorder { private get; set; } = false;
        internal float? Opacity { private get; set; } = null;
        internal bool? CursorVisible { private get; set; } = null;

        private bool _isFirstFrame = true;
        private GuiContainer? _gui;
        private ImguiWindow _imguiWindow;

        internal GuiWindow(int x, int y, int width, int height, WindowState state, string title)
        {
            _imguiWindow = new ImguiWindow(x, y, width, height, state, title);
            _imguiWindow.Initialized += SetUpImGui;
            _imguiWindow.SubmitUI = SubmitUI;
        }

        internal void Show()
        {
            _imguiWindow.Show();
        }

        private void SetUpImGui(Sdl2Window window)
        {
            _gui = new GuiContainer(this, window, _imguiWindow) {IO = ImGui.GetIO()};

            GuiFonts.EnsureFontsReady();

            SetWindowParameters(window);

            // TODO fix memory leak
            unsafe
            {
                _gui.IO.NativePtr->IniFilename = null; // disable .ini save/load
            }

            SetDefaultStyle();
        }

        private void SubmitUI(Sdl2Window window)
        {
            ImGui.PushFont(GuiFonts.Roboto_Regular);
            {
                if (_isFirstFrame)
                {
                    OnInit?.Invoke(_gui!);
                    ImGui.SetNextWindowFocus();
                    _isFirstFrame = false;
                }

                ImGui.SetNextWindowPos(new Vector2(0, 0));
                ImGui.SetNextWindowSize(new Vector2(window.Width, window.Height));
                ImGui.Begin("Main Window",
                    ImGuiWindowFlags.NoTitleBar
                    | ImGuiWindowFlags.NoResize
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoCollapse
                    | ImGuiWindowFlags.NoBringToFrontOnFocus);
                {
                    OnLayout?.Invoke(_gui!);
                    _gui!.AssertAllDefinesAreBalanced();
                }
                ImGui.End();
            }
            ImGui.PopFont();
        }

        private void SetWindowParameters(Sdl2Window window)
        {
            window.Resizable = Resizable;
            if (ForceHideBorder) window.BorderVisible = false;
            if (Opacity != null) window.Opacity = Opacity.Value;
            if (CursorVisible != null) window.CursorVisible = CursorVisible.Value;
        }

        private static void SetDefaultStyle()
        {
            var style = ImGui.GetStyle();
            style.FrameRounding = 2;
            style.WindowBorderSize = 0;
            style.PopupBorderSize = 0;
            style.FrameBorderSize = 1; // 3D look
            style.TabBorderSize = 0;
            style.WindowRounding = 2;
            style.FrameRounding = 2;
            style.ScrollbarRounding = 3;
            style.TabRounding = 2;
            style.WindowPadding = new Vector2(8, 8);
            style.ItemSpacing = new Vector2(6, 6);
            style.ItemInnerSpacing = new Vector2(6, 6);

            var colors = style.GetColors();
            colors[ImGuiCol.Text] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[ImGuiCol.TextDisabled] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
            colors[ImGuiCol.ChildBg] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            colors[ImGuiCol.WindowBg] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            colors[ImGuiCol.PopupBg] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            colors[ImGuiCol.Border] = new Vector4(0.12f, 0.12f, 0.12f, 0.71f);
            colors[ImGuiCol.BorderShadow] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
            colors[ImGuiCol.FrameBg] = new Vector4(0.42f, 0.42f, 0.42f, 0.54f);
            colors[ImGuiCol.FrameBgHovered] = new Vector4(0.42f, 0.42f, 0.42f, 0.40f);
            colors[ImGuiCol.FrameBgActive] = new Vector4(0.56f, 0.56f, 0.56f, 0.67f);
            colors[ImGuiCol.TitleBg] = new Vector4(0.19f, 0.19f, 0.19f, 1.00f);
            colors[ImGuiCol.TitleBgActive] = new Vector4(0.22f, 0.22f, 0.22f, 1.00f);
            colors[ImGuiCol.TitleBgCollapsed] = new Vector4(0.17f, 0.17f, 0.17f, 0.90f);
            colors[ImGuiCol.MenuBarBg] = new Vector4(0.335f, 0.335f, 0.335f, 1.000f);
            colors[ImGuiCol.ScrollbarBg] = new Vector4(0.24f, 0.24f, 0.24f, 0.53f);
            colors[ImGuiCol.ScrollbarGrab] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            colors[ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.52f, 0.52f, 0.52f, 1.00f);
            colors[ImGuiCol.ScrollbarGrabActive] = new Vector4(0.76f, 0.76f, 0.76f, 1.00f);
            colors[ImGuiCol.CheckMark] = new Vector4(0.65f, 0.65f, 0.65f, 1.00f);
            colors[ImGuiCol.SliderGrab] = new Vector4(0.52f, 0.52f, 0.52f, 1.00f);
            colors[ImGuiCol.SliderGrabActive] = new Vector4(0.64f, 0.64f, 0.64f, 1.00f);
            colors[ImGuiCol.Button] = new Vector4(0.54f, 0.54f, 0.54f, 0.35f);
            colors[ImGuiCol.ButtonHovered] = new Vector4(0.52f, 0.52f, 0.52f, 0.59f);
            colors[ImGuiCol.ButtonActive] = new Vector4(0.76f, 0.76f, 0.76f, 1.00f);
            colors[ImGuiCol.Header] = new Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            colors[ImGuiCol.HeaderHovered] = new Vector4(0.47f, 0.47f, 0.47f, 1.00f);
            colors[ImGuiCol.HeaderActive] = new Vector4(0.76f, 0.76f, 0.76f, 0.77f);
            colors[ImGuiCol.Separator] = new Vector4(0.000f, 0.000f, 0.000f, 0.137f);
            colors[ImGuiCol.SeparatorHovered] = new Vector4(0.700f, 0.671f, 0.600f, 0.290f);
            colors[ImGuiCol.SeparatorActive] = new Vector4(0.702f, 0.671f, 0.600f, 0.674f);
            colors[ImGuiCol.ResizeGrip] = new Vector4(0.26f, 0.59f, 0.98f, 0.25f);
            colors[ImGuiCol.ResizeGripHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.67f);
            colors[ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            colors[ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            colors[ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            colors[ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            colors[ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[ImGuiCol.TextSelectedBg] = new Vector4(0.73f, 0.73f, 0.73f, 0.35f);
            colors[ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);
            colors[ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[ImGuiCol.NavHighlight] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);

            colors[ImGuiCol.Tab] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            colors[ImGuiCol.TabHovered] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
            colors[ImGuiCol.TabActive] = new Vector4(0.33f, 0.33f, 0.33f, 1.00f);
            colors[ImGuiCol.TabUnfocused] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            colors[ImGuiCol.TabUnfocusedActive] = new Vector4(0.33f, 0.33f, 0.33f, 1.00f);

            colors[ImGuiCol.WindowBg] = 0x121212E7;
            colors[ImGuiCol.ChildBg] = new Vector4(1.00f, 1.00f, 1.00f, 0.03f);
            colors[ImGuiCol.PopupBg] = new Vector4(0.07f, 0.07f, 0.07f, 1.00f);
        }

    }
}