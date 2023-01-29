using BulletSharp;
using BulletSharp.Math;
using OpenTK.Graphics.OpenGL4;

using Vector3d = OpenTK.Mathematics.Vector3d;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    public class Demo
    {
        private ShaderProgram shader = new ShaderProgram("shaders/shaderNormal.vert", "shaders/shaderNormal.frag");
        private CharacterPhysic character;
        private TextureProgram DiffuseMap, NormalMap;
        private AssimpModel model;
        private Meshe meshe;
        public Demo()
        {
            model = new AssimpModel("Resources/xadrez_cimento/piso_cimento.obj");
            meshe = model.FirstMeshe;

            character = new CharacterPhysic("Piso", new Vector3d(50, 0.1, 50), new Vector3d(0, -30, 0), 0.0);
            DiffuseMap = new TextureProgram("Resources/xadrez_cimento/AlbedoMap.jpg", PixelInternalFormat.SrgbAlpha, TextureUnit.Texture0);
            NormalMap = new TextureProgram("Resources/xadrez_cimento/NormalMap.jpg", PixelInternalFormat.Rgba, TextureUnit.Texture1);
        }
        public void RenderFrame()
        {

            var monitoredBody = (RigidBody)PhysicsWorld.ObjectsArray[0];
            PhysicsWorld.World.ContactPairTest(PhysicsWorld.ObjectsArray[0], PhysicsWorld.ObjectsArray[1], new Contact(monitoredBody, "Collision"));

            shader.Use();
            shader.SetUniform("projection", Camera.ProjectionMatrix);
            shader.SetUniform("view", Camera.ViewMatrix);
            shader.SetUniform("model", character.Model);
            shader.SetUniform("viewPos", Camera.Position);


            shader.SetUniform("light.Ambiente", 1.0f);
            shader.SetUniform("light.Shininess", 1.0f);
            shader.SetUniform("light.Diffuse", 1.0f);

            shader.SetUniform("maps.DiffuseMap", DiffuseMap.Use);
            shader.SetUniform("maps.NormalMap", NormalMap.Use);

            meshe.RenderFrame();

        }
        private Random rand = new Random();
        public void UpdateFrame()
        {

            var input = Program.window.IsMouseButtonPressed;

            if(input(MouseButton.Button1))
            {
                PhysicsWorld.SphereShape("spheres", 1, new Vector3d(rand.Next(-10, 10), rand.Next(0, 50), rand.Next(-10, 10)), 100);
            }
        }
        public void Dispose()
        {
            shader.Dispose();
            DiffuseMap.Dispose();
            NormalMap.Dispose();
        }
    }
   
}