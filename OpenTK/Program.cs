using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using ImGuiNET;


namespace Open_GLTK
{
    public class Program
    {
        public static GameWindow window = new GameWindow(Window.gws, Window.nws);
        public static Camera Camera = new Camera(Vector3.UnitZ * 20.0f);
        public static Vector2i Size = new Vector2i(window.Size.X, window.Size.Y);
        private static Game ?game;
        private static ImGuiController ?Imgui_controller;
        private static void Main()
        {
            
            bool vsync = true;
            bool fullscreen = false;

            window.Load += delegate
            {
                InitGLobalsOpenGL();
                
                Imgui_controller = new ImGuiController(Program.Size.X, Program.Size.Y);
                game = new Game();
            };
            window.RenderFrame += delegate(FrameEventArgs frameEventArgs)
            {
                
                Imgui_controller!.Update(Program.window, (float)frameEventArgs.Time);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                game!.RenderFrame(frameEventArgs);

                // imgui NaveBars
                ImGuiProgram.RenderFrame();

                Imgui_controller.Render();
                ImGuiController.CheckGLError("End of frame");
                window.Context.SwapBuffers();
            };
            window.UpdateFrame += delegate(FrameEventArgs eventArgs)
            {
                TimerGL.TimerUpdateFrame(eventArgs);


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


                // update game
                Camera.UpdateCamera();
                game!.UpdateFrame();


            };
            window.Resize += delegate(ResizeEventArgs resizeEventArgs)
            {
                GL.Viewport(0, 0, resizeEventArgs.Size.X, resizeEventArgs.Size.Y);
                Size = resizeEventArgs.Size;
                Camera.AspectRatio = resizeEventArgs.Size.X / (float)resizeEventArgs.Size.Y;

                Imgui_controller!.WindowResized(resizeEventArgs.Size.X, resizeEventArgs.Size.Y);
                game!.Resize(resizeEventArgs);
            };
            window.MouseWheel += delegate(MouseWheelEventArgs mouseWheelEventArgs)
            {
                Camera.UpdateFov(mouseWheelEventArgs);

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
        private static void InitGLobalsOpenGL()
        {
            // ativa o tete de profundidade(z_buffer), assim o OpenGL renderiza tudo de forma correta
                // por padrao e gl_less
                GL.Enable(EnableCap.DepthTest);
                // GL.DepthFunc(DepthFunction.Less);
                // GL.DepthFunc(DepthFunction.Lequal);

                // nao renderiza o que esta atrás
                // GL.Enable(EnableCap.CullFace);
                
                // ativando o espaço Srgb sobre texturas
                GL.Enable(EnableCap.FramebufferSrgb);

                // antialising
                GL.Enable(EnableCap.Multisample);

                // filtra as costuras dos cube maps
                GL.Enable(EnableCap.TextureCubeMapSeamless);

                // habilita o point size, isso tem efeito apenas no shader de geometria
                GL.Enable(EnableCap.ProgramPointSize);

                // ativando a opacidade no fragment shader
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                
                GL.ClearColor(Color4.Black);
        }
    }
}