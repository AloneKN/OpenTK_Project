using OpenTK.Mathematics;

namespace MyGame
{
    public class Game : IDisposable
    {
        
        public CubeMap cube_map;
        private Text text;
        private ViewPort crossHair;
        private Models Unitron;
        private PostProcessing postProcessing;
        private ObjColor objColor;
        private Stencil stencil;
        public static Vector3 LuzPosition = new Vector3(0.0f, 5.0f, 0.0f);
        public Game()
        {

            text = new Text("Resources/Fonts/Wigners.otf");

            crossHair = new ViewPort("Resources/img/crosshair.png");
            objColor = new ObjColor();

            // HDR images are also supported, I didn't put any here, they are too big
            // cube_map = new CubeMap("Resources/Cubemap/HDRI/Milkyway.hdr", false);
            
            // cube_map = new CubeMap("Resources/Cubemap/nebulle.jpg", false);
            cube_map = new CubeMap("Resources/Cubemap/mars.png", true);
            Unitron = new Models("Resources/unitron/scene.gltf");

            postProcessing = new PostProcessing();
            stencil = new Stencil();

        }
        public void RenderFrame()
        {
            postProcessing.Active();
            
            cube_map.RenderFrame();

            objColor.RenderFrame();

            stencil.Active();
            Unitron.RenderFrame();

            stencil.RenderFrame(Unitron.model, ConvertColor.Color4(Values.StencilColor));
            Unitron.RenderForStencil();
            stencil.Deactive();

            crossHair.RenderFrame(Vector2.Zero, 0.03f);

            text.RenderText($"Frames: {Clock.Frames.ToString()}", new Vector2(10.0f, Program.Size.Y - 50.0f),
                            0.45f, ConvertColor.Color4(Values.fpsColor));

            text.RenderText("OpenTK APP", new Vector2(Program.Size.X - 200, 50.0f), 0.45f, Color4.Aquamarine);

            postProcessing.RenderFrame();
        }
        public void ResizedFrame()
        {
            postProcessing.ResizedFrame();
        }
        public void UpdateFrame()
        {
            objColor.UpdateFrame();
        }
        public void Dispose()
        {   

        }
    }
}

