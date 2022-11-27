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
            string vertex_shader = @"
                                    #version 460 core

                                    layout(location = 0) in vec3 aPos;
                                    layout(location = 1) in vec2 aTexCoords;

                                    out vec2 TexCoord;
                                    uniform mat4 model;
                                    uniform mat4 projection;

                                    void main()
                                    {
                                        gl_Position = vec4(aPos.xy, 0.0, 1.0) * model * projection;
                                        TexCoord = aTexCoords;
                                    }";
            string frag_shader = @"
                                    #version 460 core

                                    in vec2 TexCoord;

                                    out vec4 FragColor;

                                    uniform sampler2D mytexture;
                                    uniform vec4 color;

                                    void main()
                                    {

                                        vec4 tex = texture(mytexture, TexCoord);

                                        FragColor = color * tex;

                                        // if(FragColor.a < 0.1)
                                        //     discard;
                                    }";
            Shader = new ShaderProgram(vertex_shader, frag_shader);
            Sprite = TextureProgram.Load(fileTexture);
        }
        private float rotation = 0.0f;
        public void RenderFrame(Vector2 position, float scale = 1.0f)
        {
            Shader.Use();
            Sprite.Use(TextureUnit.Texture0);
            Shader.SetTexture("mytexture", 0);

            var model = Matrix4.Identity;

            rotation +=  Clock.ElapsedTime * 300.0f;
            model = model *  Matrix4.CreateScale(scale);
            model = model * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(rotation));
            model = model * Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));

            var Projection = Matrix4.CreateOrthographicOffCenter(-2.0f, 2.0f, -2.0f, 2.0f, 0.0f, 1.0f);

            Shader.SetMatrix4("model", model);
            Shader.SetMatrix4("projection", Projection);
            Shader.SetColor4("color", Values.crosshairColor);

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

