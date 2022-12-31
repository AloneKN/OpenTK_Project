using OpenTK.Mathematics;

namespace MyGame
{
    public class Game : IDisposable
    {
        
        public CubeMap cubeMap;
        private Text text;
        private ViewPort crossHair;
        private Bloom bloom;
        private ObjColor Light;
        private Stencil stencil;
        public static Vector3 LuzPosition = Vector3.UnitY * 5.0f;
        private Model Unitron;
        public Game()
        {
            text = new Text("Resources/Fonts/Wigners.otf");

            crossHair = new ViewPort("Resources/img/crosshair.png");
            Light = new ObjColor();

            cubeMap = new CubeMap("Resources/Cubemap/HDRI/Milkyway.hdr", false);
            // cubeMap = new CubeMap("Resources/Cubemap/nebulle.jpg", false);
            // cubeMap = new CubeMap("Resources/Cubemap/mars.png", true);
            
            Unitron = new Model(AssimpModel.Load("Resources/unitron/scene.gltf"));
            Unitron.texturesCubemaps = cubeMap.texturesCBMaps;

            bloom = new Bloom();
            stencil = new Stencil();

        }
        public void RenderFrame()
        {
 
            bloom.Active();

            cubeMap.RenderFrame();

            Light.RenderFrame();

            stencil.Active();
            Unitron.RenderFrame();


            stencil.RenderFrame(Unitron.modelMatrix, Values.StencilColor);
            Unitron.RenderForStencil();
            stencil.Deactive();

            crossHair.RenderFrame(Vector2.Zero, 0.03f);

            text.RenderText($"Frames: {Clock.FramesForSecond.ToString()}", new Vector2(10.0f, Program.Size.Y - 50.0f),
                            0.45f, Values.fpsColor);

            bloom.RenderFrame();

        }
        public void ResizedFrame()
        {
            bloom.ResizedFrameBuffer();
        }
        public void UpdateFrame()
        {
            Light.UpdateFrame();
        }
        public void Dispose()
        {   
        }
    }
}

