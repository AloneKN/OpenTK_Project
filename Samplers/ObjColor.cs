
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    public class ObjColor : IDisposable
    {
        private ShaderProgram Shader;
        public ObjColor()
        {
            Shader = new ShaderProgram("GLSL/Color/color.vert", "GLSL/Color/color.frag");
        }
        public void RenderFrame()
        {
            Shader.Use();
            
            Matrix4 model = Matrix4.Identity;
            model = model * Matrix4.CreateScale(1.0f);
            model = model * Matrix4.CreateTranslation(Game.LuzPosition);

            Shader.SetUniform("model", model);
            Shader.SetUniform("view", Camera.ViewMatrix);
            Shader.SetUniform("projection", Camera.ProjectionMatrix);

            Shader.SetUniform("color", Values.lightColor / 255f);
            Shader.SetUniform("LightDiffuse", Values.LightDiffuse);

            Sphere.RenderSphere();
            
        }
        public void UpdateFrame()
        {
            var input = Program.window.IsKeyDown;
            float vel =  Clock.ElapsedTime * 30.0f;

            if(input(Keys.R)) Game.LuzPosition = new Vector3(0.0f, 5.0f, 0.0f);

            if(input(Keys.KeyPad8)) Game.LuzPosition.Y += vel;
            if(input(Keys.KeyPad2)) Game.LuzPosition.Y -= vel;
            if(input(Keys.KeyPad4)) Game.LuzPosition.X -= vel;
            if(input(Keys.KeyPad6)) Game.LuzPosition.X += vel;
            if(input(Keys.KeyPadSubtract)) Game.LuzPosition.Z -= vel;
            if(input(Keys.KeyPadAdd)) Game.LuzPosition.Z += vel;


        }
        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}