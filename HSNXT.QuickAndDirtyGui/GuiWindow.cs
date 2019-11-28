using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ImGuiNET;
using JetBrains.Annotations;
using Veldrid;
using Veldrid.Sdl2;
using static HSNXT.QuickAndDirtyGui.ImportMeHelpers;

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
        private bool _enableIconsForAllFonts;
        
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

        public GuiWindowBuilder OnFirstUpdate(Action<GuiContainer> init)
        {
            _init = init;
            return this;
        }

        public GuiWindowBuilder OnUpdate(Action<GuiContainer> update)
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

        public GuiWindowBuilder EnableIconsForAllFonts()
        {
            _enableIconsForAllFonts = true;
            return this;
        }

        public static Task CreateAsync(Action<GuiContainer> onUpdate, Action<GuiContainer>? onFirstUpdate = null)
        {
            return new GuiWindowBuilder()
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
            var tcs = new TaskCompletionSource<bool>();

            new Thread(() =>
            {
                var guiWindow = new GuiWindow(
                    _update,
                    _init,
                    _x,
                    _y,
                    _width,
                    _height,
                    _state,
                    _title,
                    _fps,
                    _vsync,
                    _backgroundColor,
                    _enableIconsForAllFonts
                );
                guiWindow.Show();
                tcs.TrySetResult(true);
            }).Start();

            return tcs.Task;
        }
    }

    [PublicAPI]
    public sealed class GuiWindow : ImguiProgram
    {
        // ReSharper disable InconsistentNaming
        private const string Roboto_ThinItalic_Name = "Roboto-ThinItalic.otf";
        private const string Roboto_Black_Name = "Roboto-Black.otf";
        private const string Roboto_BlackItalic_Name = "Roboto-BlackItalic.otf";
        private const string Roboto_Bold_Name = "Roboto-Bold.otf";
        private const string Roboto_BoldItalic_Name = "Roboto-BoldItalic.otf";
        private const string Roboto_Italic_Name = "Roboto-Italic.otf";
        private const string Roboto_Light_Name = "Roboto-Light.otf";
        private const string Roboto_LightItalic_Name = "Roboto-LightItalic.otf";
        private const string Roboto_Medium_Name = "Roboto-Medium.otf";
        private const string Roboto_MediumItalic_Name = "Roboto-MediumItalic.otf";
        private const string RobotoMono_Bold_Name = "RobotoMono-Bold.otf";
        private const string RobotoMono_Light_Name = "RobotoMono-Light.otf";
        private const string RobotoMono_Medium_Name = "RobotoMono-Medium.otf";
        private const string RobotoMono_Regular_Name = "RobotoMono-Regular.otf";
        private const string Roboto_Regular_Name = "Roboto-Regular.otf";
        private const string Roboto_Thin_Name = "Roboto-Thin.otf";

        private readonly string NerdFont_Name = "Arimo Italic Nerd Font Complete Windows Compatible.ttf";
        // ReSharper restore InconsistentNaming

        private readonly Action<GuiContainer>? _onLayout;
        private readonly Action<GuiContainer>? _onInit;
        private bool _firstTime = true;
        private bool _iconsAllFonts;
        private GuiContainer _gui;

        internal GuiWindow(
            Action<GuiContainer>? onLayout, Action<GuiContainer>? onInit, int x, int y, int width, int height, WindowState state,
            string title, float fps = 60, bool vsync = true, Color? backgroundColor = null,
            bool enableIconsForAllFonts = false, bool resizable = true, bool forceHideBorder = false,
            float? opacity = null, bool? cursorVisible = null
        )
            : base(x, y, width, height, state, title, fps, vsync, backgroundColor)
        {
            _iconsAllFonts = enableIconsForAllFonts;
            Initialized += SetUpImGui;
            _onLayout = onLayout;
            _onInit = onInit;
            Window.Resizable = resizable;
            if (forceHideBorder) Window.BorderVisible = false;
            if (opacity != null) Window.Opacity = opacity.Value;
            if (cursorVisible != null) Window.CursorVisible = cursorVisible.Value;
            _gui = new GuiContainer(Window);
        }

        protected override void SubmitUI()
        {
            ImGui.PushFont(_gui.Roboto_Regular);
            {
                if (_firstTime)
                {
                    _onInit?.Invoke(_gui);
                    ImGui.SetNextWindowFocus();
                    _firstTime = false;
                }

                ImGui.SetNextWindowPos(new Vector2(0, 0));
                ImGui.SetNextWindowSize(new Vector2(Window.Width, Window.Height));
                ImGui.Begin("Main Window",
                    ImGuiWindowFlags.NoTitleBar
                    | ImGuiWindowFlags.NoResize
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoCollapse
                    | ImGuiWindowFlags.NoBringToFrontOnFocus);
                {
                    _onLayout?.Invoke(_gui);
                }
                ImGui.End();
            }
            ImGui.PopFont();
        }

        public void SetUpImGui()
        {
            var resourceAssembly = typeof(GuiWindow).Assembly;

            using var handles = new DisposableCollection<IDisposable>();

            using var nerdFontData = UnmanagedBlock.From(GetResource(NerdFont_Name));
            using var nerdFontCfg = new_ImFontConfigPtr();
            nerdFontCfg.V.Name.StringValue(NerdFont_Name);
            nerdFontCfg.V.OversampleH = 1;
            nerdFontCfg.V.OversampleV = 1;
            nerdFontCfg.V.MergeMode = true;

            const float fontSize = 18F;

            byte[] GetResource(string name)
            {
                var memoryStream = new MemoryStream();
                using (var stream = resourceAssembly.GetManifestResourceStream(typeof(GuiWindow), name))
                {
                    stream.CopyTo(memoryStream);
                }

                return memoryStream.ToArray();
            }

            void LoadNerdFont()
            {
                using var builderPtr = new_ImFontGlyphRangesBuilderPtr();

                using var ranges = new[] // null-terminated ImWchar*
                {
                    '\ue5fa', '\ue62b',
                    '\ue700', '\ue7c5',
                    '\uf000', '\uf2e0',
                    '\ue200', '\ue2a9',
                    '\uf500', '\ufd46',
                    '\ue300', '\ue3eb',
                    '\uf400', '\uf4a8',
                    '\ue0b4', '\ue0c8',
                    '\ue0cc', '\ue0d2',
                    '\u23fb', '\u23fe',
                    '\uf300', '\uf313',
                    '\ue000', '\ue00d',
                    '\0'
                }.ToArrayPtrWide();
                builderPtr.V.AddRanges(ranges.UnmanagedPointer);
                builderPtr.V.AddChar('\u2b58');
                builderPtr.V.AddChar('\ue0d4');
                builderPtr.V.AddChar('\u2665');
                builderPtr.V.AddChar('\u26A1');
                builderPtr.V.AddChar('\uf27c');
                builderPtr.V.AddChar('\ue0a3');
                builderPtr.V.BuildRanges(out var rangesVector);

                AddFont(nerdFontCfg, nerdFontData, fontSize, rangesVector.Data);
            }

            ImFontPtr GetFont(string resourceName, IntPtr? glyphRanges = null)
            {
                var data = GetResource(resourceName);
                var (font, handle) = AddFont(resourceName, data, fontSize, glyphRanges);
                handles.Add(handle);
                return font;
            }

            _gui.IO = ImGui.GetIO();

            _gui.IOFonts.AddFontDefault();
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_Regular = GetFont(Roboto_Regular_Name);
            LoadNerdFont();

            _gui.Roboto_Thin = GetFont(Roboto_Thin_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_Light = GetFont(Roboto_Light_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_Medium = GetFont(Roboto_Medium_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_Bold = GetFont(Roboto_Bold_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_Black = GetFont(Roboto_Black_Name);
            if (_iconsAllFonts) LoadNerdFont();


            _gui.Roboto_Italic = GetFont(Roboto_Italic_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_ThinItalic = GetFont(Roboto_ThinItalic_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_LightItalic = GetFont(Roboto_LightItalic_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_MediumItalic = GetFont(Roboto_MediumItalic_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_BoldItalic = GetFont(Roboto_BoldItalic_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.Roboto_BlackItalic = GetFont(Roboto_BlackItalic_Name);
            if (_iconsAllFonts) LoadNerdFont();


            _gui.RobotoMono_Light = GetFont(RobotoMono_Light_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.RobotoMono_Regular = GetFont(RobotoMono_Regular_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.RobotoMono_Medium = GetFont(RobotoMono_Medium_Name);
            if (_iconsAllFonts) LoadNerdFont();

            _gui.RobotoMono_Bold = GetFont(RobotoMono_Bold_Name);
            if (_iconsAllFonts) LoadNerdFont();

            /*unsafe
            {
                var ranges = stackalloc ushort[] // null-terminated ImWchar*
                {
                    '\ue5fa', '\ue62b',
                    '\ue700', '\ue7c5',
                    '\uf000', '\uf2e0',
                    '\ue200', '\ue2a9',
                    '\uf500', '\ufd46',
                    '\ue300', '\ue3eb',
                    '\uf400', '\uf4a8',
                    '\ue0b4', '\ue0c8',
                    '\ue0cc', '\ue0d2',
                    '\u23fb', '\u23fe',
                    '\uf300', '\uf313',
                    '\ue000', '\ue00d',
                    '\0'
                };

                var bld = new ImFontGlyphRangesBuilder();
                ImFontGlyphRangesBuilderPtr builderPtr = &bld;
                
                builderPtr.AddRanges(new IntPtr(ranges));
                builderPtr.AddChar('\u2b58');
                builderPtr.AddChar('\ue0d4');
                builderPtr.AddChar('\u2665');
                builderPtr.AddChar('\u26A1');
                builderPtr.AddChar('\uf27c');
                builderPtr.AddChar('\ue0a3');
                builderPtr.BuildRanges(out var rangesVector);

                GetFont(NerdFont_Name, new IntPtr(&rangesVector));
                IoFonts.Build(); // build atlas here so ranges doesn't get deleted too early
            }*/

            //IoFonts.TexDesiredWidth = 16384;
            var res = _gui.IOFonts.Build(); // build atlas here so ranges doesn't get deleted too early
            Console.WriteLine(res);

            // TODO fix memory leak
            unsafe
            {
                _gui.IO.NativePtr->IniFilename = null; // disable .ini save/load
            }

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

            var colors = style.GetColorList();
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

        /// <summary>
        /// Adds an OTF/TTF font to the current atlas from a file path.
        /// </summary>
        /// <param name="font">The file path of the font</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>The added font's handle</returns>
        public ImFontPtr AddFont(string font, float sizePixels, IntPtr? glyphRanges = null)
        {
            return _gui.IOFonts.AddFontFromFileTTF(font, sizePixels, glyphRanges ?? IntPtr.Zero);
        }

        /// <summary>
        /// Adds an OTF/TTF font from a byte array.
        /// </summary>
        /// <param name="name">The display name of the font</param>
        /// <param name="font">The contents of the font file</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>
        /// The added font's handle, and a handle that should be disposed after <see cref="ImFontAtlasPtr.Build"/> is
        /// called
        /// </returns>
        public (ImFontPtr, UnmanagedBlock) AddFont(string name, byte[] font, float sizePixels, IntPtr? glyphRanges = null)
        {
            var memory = UnmanagedBlock.From(font);
            return (
                AddFont(name, memory, sizePixels, glyphRanges),
                memory
            );
        }
        
        /// <summary>
        /// Adds an OTF/TTF font from a previously created <see cref="UnmanagedBlock"/>.
        /// </summary>
        /// <param name="name">The display name of the font</param>
        /// <param name="font">The contents of the font file</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>The added font's handle</returns>
        public ImFontPtr AddFont(string name, UnmanagedBlock font, float sizePixels, IntPtr? glyphRanges = null)
        {
            using var fontCfg = new_ImFontConfigPtr();
            fontCfg.V.Name.StringValue(name);

            return AddFont(fontCfg, font, sizePixels, glyphRanges);
        }

        /// <summary>
        /// Adds an OTF/TTF font from a previously created <see cref="UnmanagedBlock"/>.
        /// </summary>
        /// <param name="fontConfig">The font configuration settings.</param>
        /// <param name="font">The contents of the font file</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>The added font's handle</returns>
        public ImFontPtr AddFont(ImFontConfigPtr fontConfig, UnmanagedBlock font, float sizePixels, IntPtr? glyphRanges = null)
        {
            return _gui.IOFonts.AddFontFromMemoryTTF(font.UnmanagedPointer, font.Size, sizePixels, fontConfig, glyphRanges ?? IntPtr.Zero);
        }
    }
}