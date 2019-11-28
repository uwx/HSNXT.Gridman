using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiNET
{
    public abstract class ImguiProgram
    {
        /// <summary>
        /// Called when the imgui context is entered but before textures are built
        /// </summary>
        public event Action Initialized;
        public event Action Closed;

        private readonly float _fps;
        private readonly RgbaFloat _clearColor = new RgbaFloat(0.45F, 0.55F, 0.6F, 1F);
        private protected readonly GraphicsDevice GraphicsDevice;
        private protected readonly Sdl2Window Window;

        protected ImguiProgram(int x, int y, int width, int height, WindowState state, string title, float fps = 60F, bool vsync = true, Color? backgroundColor = null)
        {
            _fps = fps;
            if (backgroundColor != null)
            {
                var col = backgroundColor.Value;
                _clearColor = new RgbaFloat(col.R / 256F, col.G / 256F, col.B / 256F, 1F);
            }

            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(x, y, width, height, state, title),
                new GraphicsDeviceOptions(true, null, vsync),
                out Window,
                out GraphicsDevice);
        }

        public void Show()
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            Initialized?.Invoke();

            using var device = GraphicsDevice;
            using var controller = new ImGuiController(device, device.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height, context);
            using var commandList = device.ResourceFactory.CreateCommandList();

            Window.Resized += () =>
            {
                device.MainSwapchain.Resize((uint) Window.Width, (uint) Window.Height);
                controller.WindowResized(Window.Width, Window.Height);
            };
                
            // Main application loop
            while (Window.Exists)
            {
                var snapshot = Window.PumpEvents();
                if (!Window.Exists)
                {
                    break;
                }

                // Feed the input events to our ImGui controller, which passes them through to ImGui.
                controller.Update(1f / _fps, snapshot);

                SubmitUI();

                commandList.Begin();
                commandList.SetFramebuffer(device.MainSwapchain.Framebuffer);
                commandList.ClearColorTarget(0, _clearColor);
                controller.Render(device, commandList);
                commandList.End();
                device.SubmitCommands(commandList);
                device.SwapBuffers(device.MainSwapchain);
            }

            // Clean up Veldrid resources
            device.WaitForIdle();

            Closed?.Invoke();
        }

        protected abstract void SubmitUI();
    }
}
