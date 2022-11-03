using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace Open_GLTK
{
    public class Game : IDisposable
    {
        public CubeMap cube_map;
        private Fonte fonte;
        public static Vector3 LuzPosition = new Vector3(0.0f, 5.0f, 3.0f);
        private ViewPort aimCentro;
        private Outdoor lampada;
        private Models Unitron;
        private PostProcessing postProcessing;
        private ObjColor objColor;
        public Game()
        {
            fonte = new Fonte("Resources/Fonts/Wigners.otf");

            lampada = new Outdoor("Resources/img/lampada.png");
            aimCentro = new ViewPort("Resources/img/mira.png");
            objColor = new ObjColor();

            cube_map = new CubeMap("Resources/HDRI/Milkyway.hdr");

            Unitron = new Models("Resources/unitron/scene.gltf");
        

            postProcessing = new PostProcessing();
        }
        public void RenderFrame(FrameEventArgs frameEventArgs)
        {   
            postProcessing.ActiveBuffer();

            GL.Disable(EnableCap.CullFace);
            cube_map.RenderFrame();
            GL.Enable(EnableCap.CullFace);

            Unitron.RenderFrame();

            aimCentro.RenderFrame(0.05f, new Vector2(0.0f), 0.2f);

            // lampada.RenderFrame(LuzPosition, new Vector2(1.0f, 1.0f));
            objColor.RenderFrame();
            fonte.RenderText($"Frames: {TimerGL.Frames.ToString()}", new Vector2(10.0f, Program.Size.Y - 50.0f), 0.45f, Color4.Red);

            postProcessing.RenderFrame();

            
        }
        public void UpdateFrame()
        {
            var input = Program.window.IsKeyDown;
            float vel = 0.1f;

            if(input(Keys.R)) LuzPosition = new Vector3(0.0f, 5.0f, 3.0f);

            if(input(Keys.KeyPad8)) LuzPosition.Y += vel;
            if(input(Keys.KeyPad2)) LuzPosition.Y -= vel;
            if(input(Keys.KeyPad4)) LuzPosition.X -= vel;
            if(input(Keys.KeyPad6)) LuzPosition.X += vel;
            if(input(Keys.KeyPadSubtract)) LuzPosition.Z -= vel;
            if(input(Keys.KeyPadAdd)) LuzPosition.Z += vel;

            Unitron.UpdateFrame();
        }
        public void Resize(ResizeEventArgs resizeEventArgs)
        {
            postProcessing.ResizeFrame(resizeEventArgs.Size);
        }
        public void Dispose()
        {   
            Unitron.Dispose();
            fonte.Dispose();
            lampada.Dispose();
            cube_map.Dispose();

            postProcessing.Dispose();
        }
    }
}

