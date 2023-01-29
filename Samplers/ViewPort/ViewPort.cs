using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class ViewPort : IDisposable
    {
        private ShaderProgram Shader;
        private TextureProgram Sprite; 
        public ViewPort(string fileTexture)
        {
            Shader = new ShaderProgram("Samplers/ViewPort/shader.vert", "Samplers/ViewPort/shader.frag");
            Sprite = new TextureProgram(fileTexture);
        }
        private float rotation = 0.0f;
        Matrix4 ProjectionDiplayCenter = Matrix4.CreateOrthographicOffCenter(-2.0f, 2.0f, -2.0f, 2.0f, 0.0f, 1.0f);
        public void RenderFrame(Vector2 position, float scale = 1.0f)
        {
            Shader.Use();

            var model = Matrix4.Identity;

            rotation += TimerGL.ElapsedTime * 300.0f;
            model = model *  Matrix4.CreateScale(scale);
            model = model * Matrix4.CreateRotationZ((float)OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation));
            model = model * Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));


            Shader.SetUniform("model", model);
            Shader.SetUniform("projection", ProjectionDiplayCenter);
            Shader.SetUniform("color", Values.crosshairColor);

            Shader.SetUniform("mytexture", Sprite.Use);
            Quad.RenderQuad();
        }
        public void UpdateFrame() {}
        public void Dispose()
        {
            Shader.Dispose();
            Sprite.Dispose();
        }
    }
}

