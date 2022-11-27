using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;


namespace MyGame
{
    public class Program
    {
        public static GameWindow window = new GameWindow(Window.gws, Window.nws);
        public static Camera Camera = new Camera();
        public static Vector2i Size = new Vector2i(window.Size.X, window.Size.Y);
        private static ImGuiController ?Imgui_controller;
        private static Game ?game;
        public static bool vsync = true;
        public static bool fullscreen = false;
        private static void Main()
        {
            window.Load += delegate
            {
                DebugGL.StartGlobal();

                DebugGL.InitDebug();

                Imgui_controller = new ImGuiController(window.Size);
                game = new Game();
            };
            window.RenderFrame += delegate(FrameEventArgs frameEventArgs)
            {
                
                Imgui_controller!.Update(Program.window, (float)frameEventArgs.Time);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                game!.RenderFrame();

                // imgui NaveBars
                ImGuiProgram.RenderFrame();

                Imgui_controller.Render();
                ImGuiController.CheckGLError("End of frame");
                window.Context.SwapBuffers();
            };

  
            window.UpdateFrame += delegate(FrameEventArgs eventArgs)
            {
                var input = window.IsKeyPressed;

                if (input(Keys.Escape))
                    window.Close();



                if(input(Keys.F))
                {
                    fullscreen = fullscreen ? false : true;
                    window.WindowState = fullscreen ? WindowState.Normal : WindowState.Fullscreen;
                }

                if(input(Keys.V))
                {
                    vsync = vsync ? false : true;
                    window.VSync = vsync ? VSyncMode.On : VSyncMode.Off;
                }

                Clock.TimerUpdateFrame(eventArgs);

                // update game
                Camera.UpdateCamera();
                game!.UpdateFrame();


            };
            window.Resize += delegate(ResizeEventArgs resizeEventArgs)
            {
                Size = resizeEventArgs.Size;

                GL.Viewport(0, 0, resizeEventArgs.Size.X, resizeEventArgs.Size.Y);
                Camera.UpdateSize(resizeEventArgs.Size);

                Imgui_controller!.WindowResized(resizeEventArgs.Size);
                    
                game!.ResizedFrame();


            };
            window.MouseWheel += delegate(MouseWheelEventArgs mouseWheelEventArgs)
            {
                Camera.UpdateFov(ref mouseWheelEventArgs);

            };
            window.TextInput += delegate(TextInputEventArgs textInputEventArgs)
            {
                Imgui_controller!.PressChar((char)textInputEventArgs.Unicode);
            };
            window.Unload += delegate
            {
                game!.Dispose();
            };

            window.Run();
        }
        
    }
}