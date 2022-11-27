using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MyGame
{
    public class Outdoor : IDisposable
    {
        private ShaderProgram shader;
        private TextureProgram texture; 
        public Outdoor(string fileTexture)
        {
            string vertex_shader = @"
                                    #version 460 core

                                    layout(location = 0) in vec3 squareVertices;
                                    layout(location = 1) in vec2 aTexCoords;

                                    out vec2 TexCoords;

                                    uniform vec3 CameraRight;
                                    uniform vec3 CameraUp;
                                    uniform vec3 Position;
                                    uniform vec2 Size;

                                    uniform mat4 projection;
                                    uniform mat4 view;

                                    void main()
                                    {
                                        vec3 particleCenter_wordspace = Position;
                                        
                                        vec3 vertexPosition_worldspace = 
                                            particleCenter_wordspace
                                            + CameraRight * squareVertices.x * Size.x
                                            + CameraUp * squareVertices.y * Size.y;


                                        gl_Position = vec4(vertexPosition_worldspace, 1.0) * view * projection;

                                        TexCoords = aTexCoords;
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
        public void RenderFrame(Vector3 position, Vector2 scale)
        {

            shader.Use();
            texture.Use(TextureUnit.Texture0);
            shader.SetTexture("imagem", 0);


            shader.SetVector3("CameraRight", Camera.Right);
            shader.SetVector3("CameraUp", Camera.Up);

            shader.SetVector3("Position", position);
            shader.SetVector2("Size", scale);

            shader.SetMatrix4("view", Camera.ViewMatrix);
            shader.SetMatrix4("projection", Camera.ProjectionMatrix);

            Quad.RenderQuad();
        }
        public void Dispose()
        {

            shader.Dispose();
            texture.Dispose();
        }
    }
}