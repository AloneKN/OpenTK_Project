using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MyGame
{
    public class Outdoor
    {
        private ShaderProgram shader;
        private TextureProgram texture; 
        public Outdoor(string fileTexture)
        {
            string vertex_shader = @"
                                    #version 460 core

                                    layout(location = 0) in vec3 aPos;
                                    layout(location = 1) in vec2 aTexCoords;

                                    out vec2 TexCoords;

                                    uniform vec3 CameraRight;
                                    uniform vec3 CameraUp;

                                    uniform mat4 projection;
                                    uniform mat4 view;
                                    uniform mat4 model;

                                    void main()
                                    {
                                        TexCoords = aTexCoords;

                                        vec3 vertexPosition = CameraRight * aPos.x + CameraUp * aPos.y;

                                        gl_Position = vec4(vertexPosition, 1.0) * model * view * projection;

                                    }";
            string frag_shader = @"
                                    #version 460 core

                                    in vec2 TexCoords;

                                    out vec4 FragColor;

                                    uniform sampler2D imagem;

                                    void main()
                                    {
                                        FragColor = texture( imagem, TexCoords );
                                        if(FragColor.a <= 0.1)
                                            discard;
                                    }";

            shader = new ShaderProgram(vertex_shader, frag_shader);
            texture = TextureProgram.Load(fileTexture);
        }
        public Vector3 position = Vector3.Zero;
        public Vector2 scale = Vector2.Zero;
        public void RenderFrame(Matrix4 model)
        {

            shader.Use();
            texture.Use(TextureUnit.Texture0);
            shader.SetUniform("imagem", 0);


            shader.SetUniform("CameraRight", Camera.Right);
            shader.SetUniform("CameraUp", Camera.Up);
            shader.SetUniform("model", model);

            shader.SetUniform("view", Camera.ViewMatrix);
            shader.SetUniform("projection", Camera.ProjectionMatrix);

            Quad.RenderQuad();
        }
    }
}