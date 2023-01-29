using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace MyGame
{
    public class Game : IDisposable
    {
        public CubeMap cubeMap;
        private ViewPort crossHair;
        private Bloom bloom;
        public static Vector3 LuzPosition = Vector3.UnitY * 5.0f;
        private Model Unitron;
        private Demo demo;
        public unsafe Game()
        {
            
            demo = new Demo();

            crossHair = new ViewPort("Resources/img/crosshair.png");

            cubeMap = new CubeMap("Resources/Cubemap/HDRI/Milkyway.hdr", CubeMapType.Type0);
            // cubeMap = new CubeMap("Resources/Cubemap/meadow.png", CubeMapType.Type2);
            // cubeMap = new CubeMap("Resources/Cubemap/mars.png", CubeMapType.Type3);
            // cubeMap = new CubeMap(new Faces()
            // {
            //     PathFaces = "Resources/Cubemap/asteroids",
            //     Textures = new List<string>()
            //     { "right.jpg", "left.jpg", "up.jpg", "down.jpg", "back.jpg", "front.jpg" }
            // });
            
            Unitron = new Model("Resources/unitron/scene.gltf");
            Unitron.UseTexCubemap = cubeMap.UseTextures;

            bloom = new Bloom();


        }
        public void RenderFrame()
        {
            bloom.BindBloom();

            cubeMap.RenderFrame();


            Unitron.RenderFrame();

            demo.RenderFrame();

            PhysicsWorld.RenderObjects();

            bloom.RenderFrame();


            crossHair.RenderFrame(Vector2.Zero, 0.03f);


            Text.RenderText($"Frames: {TimerGL.FramesForSecond.ToString()}", new Vector2(10.0f, Program.Size.Y - 50.0f),
                            0.45f, Values.fpsColor);

            Text.RenderText($"Camera Position {Camera.Position}", new Vector2(100f, 40f), 0.5f, Color4.AliceBlue);

        }
        public void ResizedFrame()
        {
            bloom.ResizedFrame();
        }
        public void UpdateFrame()
        {
            Unitron.UpdateFrame();
            
            demo.UpdateFrame();
        }
        public void Dispose()
        {   
            demo.Dispose();
            Unitron.Dispose();
        }
    }
}

