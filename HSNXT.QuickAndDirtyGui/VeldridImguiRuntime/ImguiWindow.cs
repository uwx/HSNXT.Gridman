using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using HSNXT.QuickAndDirtyGui;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiNET
{
    internal class ImguiWindow
    {
        /// <summary>
        /// Called when the imgui context is entered but before textures are built
        /// </summary>
        public event Action<Sdl2Window>? Initialized;
        public event Action? Closed;
        [DisallowNull] public Action<Sdl2Window>? SubmitUI;

        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly WindowState _state;
        private readonly string _title;
        private RgbaFloat _clearColor = new RgbaFloat(0.45F, 0.55F, 0.6F, 1F);
        
        public float Fps { private get; set; } = 60F;
        public bool VSync { private get; set; } = true;

        public Color BackgroundColor
        {
            set => _clearColor = new RgbaFloat(value.R / 256F, value.G / 256F, value.B / 256F, 1F);
        }

        private GraphicsDevice? _graphicsDevice;
        private ImguiRenderer? _renderer;
        private List<Texture>? _loadedOwnedTextures;

        public ImguiWindow(int x, int y, int width, int height, WindowState state, string title)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _state = state;
            _title = title;
        }

        public IntPtr CreateTexture<T>(uint width, uint height, PixelFormat format, T[] source) where T : unmanaged
        {
            if (_graphicsDevice == null || _renderer == null)
            {
                throw new InvalidOperationException($"Program disposed or uninitialized. In case of the latter call {nameof(Show)} first and only invoke this method in or after {nameof(Initialized)}");
            }

            var texture = Create2DTexture(width, height, format, out var description);

            _graphicsDevice.UpdateTexture(
                texture,
                source,
                0, 0, 0,
                description.Width, description.Height, description.Depth,
                0,
                0);

            return CreateTexture(texture);
        }

        public IntPtr CreateTexture(uint width, uint height, PixelFormat format, NativeArray source)
        {
            if (_graphicsDevice == null || _renderer == null)
            {
                throw new InvalidOperationException($"Program disposed or uninitialized. In case of the latter call {nameof(Show)} first and only invoke this method in or after {nameof(Initialized)}");
            }

            var texture = Create2DTexture(width, height, format, out var description);

            _graphicsDevice.UpdateTexture(
                texture,
                source.UnmanagedPointer,
                (uint) source.Size,
                0, 0, 0,
                description.Width, description.Height, description.Depth,
                0,
                0);

            return CreateTexture(texture);
        }

        public IntPtr CreateTexture(Texture texture)
        {
            if (_graphicsDevice == null || _renderer == null)
            {
                throw new InvalidOperationException($"Program disposed or uninitialized. In case of the latter call {nameof(Show)} first and only invoke this method in or after {nameof(Initialized)}");
            }

            _loadedOwnedTextures ??= new List<Texture>();
            _loadedOwnedTextures.Add(texture);

            return _renderer.GetOrCreateImGuiBinding(_graphicsDevice.ResourceFactory, texture);
        }

        private Texture Create2DTexture(uint width, uint height, PixelFormat format, out TextureDescription description)
        {
            if (_graphicsDevice == null || _renderer == null)
            {
                throw new InvalidOperationException($"Program disposed or uninitialized. In case of the latter call {nameof(Show)} first and only invoke this method in or after {nameof(Initialized)}");
            }

            description = TextureDescription.Texture2D(
                width,
                height,
                1,
                1,
                format,
                TextureUsage.Sampled);

            var texture = _graphicsDevice.ResourceFactory.CreateTexture(description);
            
            _loadedOwnedTextures ??= new List<Texture>();
            _loadedOwnedTextures.Add(texture);

            return texture;
        }

        public void Show()
        {
            if (_graphicsDevice == null || _renderer == null)
            {
                throw new InvalidOperationException("Program initialized twice before the first execution returned to caller");
            }

            if (SubmitUI == null)
            {
                throw new InvalidOperationException($"{nameof(SubmitUI)} must be assigned before call to {nameof(Show)}");
            }

            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(_x, _y, _width, _height, _state, _title),
                new GraphicsDeviceOptions(true, null, VSync),
                out var window,
                out _graphicsDevice);

            var commandList = _graphicsDevice.ResourceFactory.CreateCommandList();

            void OnWindowResized()
            {
                // ReSharper disable AccessToDisposedClosure
                _graphicsDevice.MainSwapchain.Resize((uint) window.Width, (uint) window.Height);
                _renderer.WindowResized(window.Width, window.Height);
                // ReSharper restore AccessToDisposedClosure
            }

            try
            {
                var context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);

                Initialized?.Invoke(window);

                _renderer = new ImguiRenderer(_graphicsDevice, _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height, context);

                window.Resized += OnWindowResized;

                // Main application loop
                while (window.Exists)
                {
                    var snapshot = window.PumpEvents();
                    if (!window.Exists)
                    {
                        break;
                    }

                    // Feed the input events to our ImGui controller, which passes them through to ImGui.
                    _renderer.Update(1f / Fps, snapshot);

                    SubmitUI(window);

                    commandList.Begin();
                    commandList.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
                    commandList.ClearColorTarget(0, _clearColor);
                    _renderer.Render(_graphicsDevice, commandList);
                    commandList.End();
                    _graphicsDevice.SubmitCommands(commandList);
                    _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
                }
            }
            finally
            {
                // Prevent execution of an event on a non-existent GraphicsDevice
                window.Resized -= OnWindowResized;

                // Dispose of ImguiRenderer-owned resources
                _renderer.Dispose();

                // Dispose of all created textures. This may not be necessary but I don't know how TextureViews work and
                // I want to make sure it's all gone even if the API surface for ImguiRenderer changes.
                if (_loadedOwnedTextures != null)
                {
                    foreach (var texture in _loadedOwnedTextures)
                    {
                        texture.Dispose();;
                    }
                }

                // Clean up Veldrid resources
                _graphicsDevice.Dispose();

                // clear state so the rest of the object knows it's empty
                _graphicsDevice = null;
                _renderer = null;
            }
        }

        public void Dispose()
        {
            _graphicsDevice = null;
            _renderer = null;

            Closed?.Invoke();
        }
    }
}
