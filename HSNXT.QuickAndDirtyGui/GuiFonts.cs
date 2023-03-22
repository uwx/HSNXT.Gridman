using System;
using System.Runtime.InteropServices;
using System.Threading;
using HSNXT.QuickAndDirtyGui.Extensions;
using ImGuiNET;
using JetBrains.Annotations;
using static HSNXT.QuickAndDirtyGui.ImguiStructHelpers;

namespace HSNXT.QuickAndDirtyGui
{
    public static class GuiFonts
    {
        public static bool IsInitialized => _isInitialized != 0;
        private static int _isInitialized;

        public static bool EnableIconsForAllFonts
        {
            set
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("The ImGui font atlas has already been initialized");
                }

                _enableIconsForAllFonts = value;
            }
        }

        public static bool EnableNerdFontIcons
        {
            set
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("The ImGui font atlas has already been initialized");
                }

                _enableNerdFontIcons = value;
            }
        }

        private static readonly object AtlasLock = new object();
        private static readonly IntPtr FontAtlasId = (IntPtr)1;

        private static ImGuiIOPtr _io;
        private static bool _enableIconsForAllFonts;
        private static bool _enableNerdFontIcons = true;
        private static (NativeArray pixels, int width, int height, int bytesPerPixel)? _fontAtlas;

        // ReSharper disable InconsistentNaming
        public static ImFontAtlasPtr IOFonts => _io.Fonts; // TODO share font atlas statically between all contexts

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
        private const string NerdFont_Name = "Arimo-Regular-NerdFont-WindowsCompat.ttf";

        [PublicAPI] public static ImFontPtr Roboto_ThinItalic { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Black { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_BlackItalic { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Bold { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_BoldItalic { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Italic { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Light { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_LightItalic { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Medium { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_MediumItalic { get; private set; }
        [PublicAPI] public static ImFontPtr RobotoMono_Bold { get; private set; }
        [PublicAPI] public static ImFontPtr RobotoMono_Light { get; private set; }
        [PublicAPI] public static ImFontPtr RobotoMono_Medium { get; private set; }
        [PublicAPI] public static ImFontPtr RobotoMono_Regular { get; private set; }
        [PublicAPI] public static ImFontPtr Roboto_Regular { get; private set; }

        [PublicAPI] public static ImFontPtr Roboto_Thin { get; private set; }
        // ReSharper restore InconsistentNaming

        internal static void EnsureFontsReady()
        {
            if (Interlocked.CompareExchange(ref _isInitialized, 1, 1) == 1)
            {
                return;
            }

            lock (AtlasLock)
            {
                EnsureFontsReadyLocked();
            }
        }

        private static void EnsureFontsReadyLocked()
        {
            using var handles = new DisposableList<IDisposable>();

            using var nerdFontData = GetResource("Fonts." + NerdFont_Name);
            using var nerdFontCfg = new_ImFontConfigPtr();
            nerdFontCfg.Value.Name.StringValue(NerdFont_Name);
            nerdFontCfg.Value.OversampleH = 1;
            nerdFontCfg.Value.OversampleV = 1;
            nerdFontCfg.Value.MergeMode = true;

            const float fontSize = 18F;

            void InjectNerdFont()
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
                builderPtr.Value.AddRanges(ranges.UnmanagedPointer);
                builderPtr.Value.AddChar('\u2b58');
                builderPtr.Value.AddChar('\ue0d4');
                builderPtr.Value.AddChar('\u2665');
                builderPtr.Value.AddChar('\u26A1');
                builderPtr.Value.AddChar('\uf27c');
                builderPtr.Value.AddChar('\ue0a3');
                builderPtr.Value.BuildRanges(out var rangesVector);

                AddFont(nerdFontCfg, nerdFontData, fontSize, rangesVector.Data);
            }

            ImFontPtr GetFont(string resourceName, IntPtr? glyphRanges = null)
            {
                var handle = GetResource("Fonts." + resourceName);
                var font = AddFont(resourceName, handle, fontSize, glyphRanges);
                handles.Add(handle);
                return font;
            }

            ImFontPtr GetFontAndInject(string resourceName, bool overrideInject = false)
            {
                var resultFont = GetFont(resourceName);
                if (overrideInject || _enableIconsForAllFonts)
                {
                    InjectNerdFont();
                }

                return resultFont;
            }

            _io = ImGui.GetIO();

            //
            IOFonts.AddFontDefault();
            if (_enableIconsForAllFonts) InjectNerdFont();
            //

            Roboto_Regular = GetFontAndInject(Roboto_Regular_Name, _enableNerdFontIcons);
            Roboto_Thin = GetFontAndInject(Roboto_Thin_Name);
            Roboto_Light = GetFontAndInject(Roboto_Light_Name);
            Roboto_Medium = GetFontAndInject(Roboto_Medium_Name);
            Roboto_Bold = GetFontAndInject(Roboto_Bold_Name);
            Roboto_Black = GetFontAndInject(Roboto_Black_Name);

            Roboto_Italic = GetFontAndInject(Roboto_Italic_Name);
            Roboto_ThinItalic = GetFontAndInject(Roboto_ThinItalic_Name);
            Roboto_LightItalic = GetFontAndInject(Roboto_LightItalic_Name);
            Roboto_MediumItalic = GetFontAndInject(Roboto_MediumItalic_Name);
            Roboto_BoldItalic = GetFontAndInject(Roboto_BoldItalic_Name);
            Roboto_BlackItalic = GetFontAndInject(Roboto_BlackItalic_Name);

            RobotoMono_Light = GetFontAndInject(RobotoMono_Light_Name);
            RobotoMono_Regular = GetFontAndInject(RobotoMono_Regular_Name);
            RobotoMono_Medium = GetFontAndInject(RobotoMono_Medium_Name);
            RobotoMono_Bold = GetFontAndInject(RobotoMono_Bold_Name);

            var res = IOFonts.Build(); // build atlas here so ranges doesn't get deleted too early
            Console.WriteLine(res);
        }

        // Gets font atlas and assures _fontAtlas is set
        internal static unsafe NativeArray GetFontAtlas(
            out int width,
            out int height,
            out int bytesPerPixel)
        {
            EnsureFontsReady();

            // we can use the same lock here, so long as we don't call EnsureFontsReady from inside it 
            lock (AtlasLock)
            {
                if (_fontAtlas != null)
                {
                    width = _fontAtlas.Value.width;
                    height = _fontAtlas.Value.height;
                    bytesPerPixel = _fontAtlas.Value.bytesPerPixel;
                    return _fontAtlas.Value.pixels;
                }

                // Build
                IOFonts.GetTexDataAsRGBA32(out IntPtr pixels, out width, out height, out bytesPerPixel);
                Console.WriteLine($"Got ImGui font texture data: {pixels} px {width}x{height} {bytesPerPixel * 8}bpp");
                // Store our identifier
                IOFonts.SetTexID(FontAtlasId);

                // take a local copy of the graphics data, make sure we never accidentally release it
                var pixelsCopy = NativeArray.From(pixels.ToPointer(), bytesPerPixel * width * height);

                IOFonts.ClearTexData();

                _fontAtlas = (pixelsCopy, width, height, bytesPerPixel);

                return pixelsCopy;
            }
        }

        
        public static void DestroyFonts()
        {
            if (Interlocked.CompareExchange(ref _isInitialized, 0, 0) == 0)
            {
                throw new InvalidOperationException("Cannot destroy font data, there is nothing to destroy");
            }

            lock (AtlasLock)
            {
                // if GetFontAtlas has not been called yet, it's likely that the fonts are being destroyed before the
                // window is finished initializing. this isn't an issue as the initialization process will execute again
                // just fine, but that means this operation will have no meaningful result and the atlas will be
                // rendered twice.
                if (_fontAtlas == null)
                {
                    Console.WriteLine("Warning: Destroying font data while the atlas is only owned by ImGui. This is usually not dangerous but ill-advised.");
                }
                else
                {
                    _fontAtlas.Value.pixels.Dispose();
                    _fontAtlas = null;
                }

                IOFonts.Clear();
            }
        }

        private static NativeArray GetResource(string name)
        {
            using var stream = typeof(GuiWindow).Assembly.GetManifestResourceStream(typeof(GuiWindow), name);
            if (stream == null)
            {
                throw new InvalidOperationException($"Missing required resource: {typeof(GuiWindow).Assembly}:{name}");
            }

            Span<byte> bytes = stackalloc byte[(int) stream.Length];
            stream.Read(bytes);
            return NativeArray.From(bytes);
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
        public static ImFontPtr AddFont(string font, float sizePixels, IntPtr? glyphRanges = null)
        {
            return IOFonts.AddFontFromFileTTF(font, sizePixels, glyphRanges ?? IntPtr.Zero);
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
        public static (ImFontPtr font, NativeArray array) AddFont(string name, byte[] font, float sizePixels,
            IntPtr? glyphRanges = null)
        {
            var memory = NativeArray.From(font);
            return (
                AddFont(name, memory, sizePixels, glyphRanges),
                memory
            );
        }

        /// <summary>
        /// Adds an OTF/TTF font from a previously created <see cref="NativeArray"/>.
        /// </summary>
        /// <param name="name">The display name of the font</param>
        /// <param name="font">The contents of the font file</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>The added font's handle</returns>
        public static ImFontPtr AddFont(string name, NativeArray font, float sizePixels, IntPtr? glyphRanges = null)
        {
            using var fontCfg = new_ImFontConfigPtr();
            fontCfg.Value.Name.StringValue(name);

            return AddFont(fontCfg, font, sizePixels, glyphRanges);
        }

        /// <summary>
        /// Adds an OTF/TTF font from a previously created <see cref="NativeArray"/>.
        /// </summary>
        /// <param name="fontConfig">The font configuration settings.</param>
        /// <param name="font">The contents of the font file</param>
        /// <param name="sizePixels">The size, in pixels, of the font</param>
        /// <param name="glyphRanges">
        /// A pointer to a null-terminated wchar list (ushort[]) containing the to-from character ranges to map the font
        /// to. If <c>null</c>, the all characters will be used.
        /// </param>
        /// <returns>The added font's handle</returns>
        public static ImFontPtr AddFont(ImFontConfigPtr fontConfig, NativeArray font, float sizePixels,
            IntPtr? glyphRanges = null)
        {
            return IOFonts.AddFontFromMemoryTTF(font.UnmanagedPointer, font.Size, sizePixels, fontConfig,
                glyphRanges ?? IntPtr.Zero);
        }
    }
}