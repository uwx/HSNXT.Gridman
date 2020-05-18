using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using JetBrains.Annotations;
using Veldrid;
using Veldrid.Sdl2;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;

namespace HSNXT.QuickAndDirtyGui
{
    [PublicAPI]
    public sealed partial class GuiContainer
    {
        // ReSharper disable once InconsistentNaming
        public ImGuiIOPtr IO { get; internal set; }
        public ImGuiStylePtr Style => ImGui.GetStyle();
        
        public int X { get => _window.X; set => _window.X = value; }
        public int Y { get => _window.Y; set => _window.Y = value; }
        public int Width { get => _window.Width; set => _window.Width = value; }
        public int Height { get => _window.Height; set => _window.Height = value; }
        public float Opacity { get => _window.Opacity; set => _window.Opacity = value; }
        public string Title { get => _window.Title; set => _window.Title = value; }
        public WindowState WindowState { get => _window.WindowState; set => _window.WindowState = value; }
        public bool Visible { get => _window.Visible; set => _window.Visible = value; }
        public bool CursorVisible { get => _window.CursorVisible; set => _window.CursorVisible = value; }
        public bool BorderVisible { get => _window.BorderVisible; set => _window.BorderVisible = value; }
        public bool Resizable { get => _window.Resizable; set => _window.Resizable = value; }

        private readonly GuiWindow _guiWindow;
        private readonly Sdl2Window _window;
        private readonly ImguiWindow _imguiWindow;

        internal GuiContainer(GuiWindow guiWindow, Sdl2Window window, ImguiWindow imguiWindow)
        {
            _guiWindow = guiWindow;
            _window = window;
            _imguiWindow = imguiWindow;
        }

        #if DEBUG
        public void Debug()
        {
            ImGui.Text("Bap " + NerdFonts.CustomCpp);
            ImGui.ShowStyleEditor();
            ImGui.Text("Testa");
        }
        #endif

        public IntPtr CreateTexture<T>(uint width, uint height, Veldrid.PixelFormat format, T[] source) where T : unmanaged
        {
            return _imguiWindow.CreateTexture(width, height, format, source);
        }

        public IntPtr CreateTexture(string imagePath)
        {
            using var image = SD.Image.FromFile(imagePath);
            if (!(image is Bitmap bitmap))
            {
                throw new ArgumentException("Image does not point to a bitmap", nameof(imagePath));
            }

            var data = bitmap.LockBits(new SD.Rectangle(SD.Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, SDI.PixelFormat.Format32bppArgb);
            
            unsafe
            {
                var width = data.Width;
                var height = data.Height;
                var stride = data.Stride;

                if (stride != width)
                {
                    throw new ArgumentException($"Unsupported image byte algignment, width {width} != stride {stride}", nameof(imagePath));
                }

                using var resultArray = NativeArray.From(data.Scan0.ToPointer(), height * stride);
                // TODO i'm not good at endianness. RGBA or BGRA, or neither?
                return _imguiWindow.CreateTexture((uint) bitmap.Width, (uint) bitmap.Height, Veldrid.PixelFormat.R8_G8_B8_A8_UNorm, resultArray);
            }
        }

        public IntPtr CreateTexture(Texture texture)
        {
            return _imguiWindow.CreateTexture(texture);
        }
    }
}