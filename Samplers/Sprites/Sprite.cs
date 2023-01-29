using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    
    public class Sprite : IDisposable
    {
        private Meshe plane;
        private ShaderProgram shader;
        private TextureProgram DiffuseMap;
        public Sprite(string DiffuseTex)
        {
            shader = new ShaderProgram("Samplers/Sprites/shader.vert", "Samplers/Sprites/shader.frag");
            DiffuseMap = new TextureProgram(DiffuseTex, TextureUnit.Texture0);

            AssimpModel assimpModel = new AssimpModel("Resources/ship/retangle.obj");
            plane = assimpModel.FirstMeshe;
            

        }
        private Matrix4 Projection2D { get => Matrix4.CreateOrthographicOffCenter(0f, Program.Size.X, 0f, Program.Size.Y, -500f, 500); }
        public void RenderFrame(Matrix4 model)
        {

            shader.Use();
            shader.SetUniform("projection", Projection2D);

            shader.SetUniform("maps.DiffuseMap", DiffuseMap.Use);
            shader.SetUniform("LightScene", Values.ForceLightScene);
            shader.SetUniform("color", Color4.Red);
            
            shader.SetUniform("model", model);
            
            GL.Enable(EnableCap.Blend);
            plane.RenderFrame();
            GL.Disable(EnableCap.Blend);

        }
        public void Dispose()
        {
            shader.Dispose();
            DiffuseMap.Dispose();
            
        }
    }
}