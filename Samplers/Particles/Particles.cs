using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class Particles : IDisposable
    {
        private ShaderProgram shader;
        private TextureProgram texture;
        private VertexArrayObject Vao;
        private BufferObject<float> vbo;
        private BufferObject<Vector3> vboPositions;
        private BufferObject<Vector2> vboScales;
        private BufferObject<Color4> vboColor;
        private int amountParticles;
        public unsafe Particles(string fileTexture)
        {
            shader = new ShaderProgram("Samplers/Particles/shader.vert", "Samplers/Particles/shader.frag");
            texture = new TextureProgram(fileTexture);


            float[] vertices = 
            {
                // positions        // texture Coords
                -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                 1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
                 1.0f, -1.0f, 0.0f, 1.0f, 0.0f,       
            };

            

            var rand = new System.Random();

            var positions = new List<Vector3>();
            var scale = new List<Vector2>();
            var colors = new List<Color4>();


            const int SEGMENTS = 12;
            const float PI = 3.14159265359f;
            float radius = Camera.DistanceOfView - 300f;

            for (int x = 0; x <= SEGMENTS; ++x)
            {
                for (int y = 0; y <= SEGMENTS; ++y)
                {

                    float xSegment = (float)x / (float)SEGMENTS;
                    float ySegment = (float)y / (float)SEGMENTS;
                    var pos = new Vector3()
                    {
                        X = MathF.Cos(xSegment * 2.0f * PI) * MathF.Sin(ySegment * PI) * radius,
                        Y = MathF.Cos(ySegment * PI) * radius,
                        Z = MathF.Sin(xSegment * 2.0f * PI) * MathF.Sin(ySegment * PI) * radius,
                    };

                    pos.X += (float)rand.Next(0, 200);
                    pos.Y += (float)rand.Next(0, 200);
                    pos.Z += (float)rand.Next(0, 200);

                    positions.Add(pos);
                    scale.Add( new Vector2( (float)rand.Next(0, 50) + (float)rand.NextDouble() ));

                    var color = new Color4()
                    {
                        R = (float)rand.Next(50, 255) / 255f,
                        G = (float)rand.Next(50, 255) / 255f,
                        B = (float)rand.Next(50, 255) / 255f,
                        A = 1.0f,
                    };

                    colors.Add(color);

                }
            }

            amountParticles = positions.Count;

            Vao = new VertexArrayObject();

            vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);
            Vao.LinkBufferObject(ref vbo);
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float), 0);
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            vboPositions = new BufferObject<Vector3>(positions.ToArray(), BufferTarget.ArrayBuffer);
            Vao.LinkBufferObject(ref vboPositions);
            Vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, sizeof(Vector3), 0);
            GL.VertexAttribDivisor(2, 1);


            vboScales = new BufferObject<Vector2>(scale.ToArray(), BufferTarget.ArrayBuffer);
            Vao.LinkBufferObject(ref vboScales);
            Vao.VertexAttributePointer(3, 2, VertexAttribPointerType.Float, sizeof(Vector2), 0);
            GL.VertexAttribDivisor(3, 1);

            vboColor = new BufferObject<Color4>(colors.ToArray(), BufferTarget.ArrayBuffer);
            Vao.LinkBufferObject(ref vboColor);
            Vao.VertexAttributePointer(4, 4, VertexAttribPointerType.Float, sizeof(Color4), 0);
            GL.VertexAttribDivisor(4, 1);

        }
        // private Vector3 position = Camera.Position;
        // private float vel = 0.01f;
        public void RenderFrame()
        {
            // float teste = MathF.Sin(Clock.Time) / 0.003f + 400f; 50 ate 650


            shader.Use();
            shader.SetUniform("imagem", texture.Use);

            shader.SetUniform("CameraRight", Camera.Right);
            shader.SetUniform("CameraUp", Camera.Up);

            shader.SetUniform("view", Camera.ViewMatrix);
            shader.SetUniform("projection", Camera.ProjectionMatrix);
            
            shader.SetUniform("LightDiffuse", Values.ParticlesLum);
            // shader.SetUniform("color", Values.ParticlesColor);

            // position.X += vel * 0.9f;
            // position.Y += vel * 0.2f;
            // position.Z += vel * 0.7f;

            // if(position.X > 50f || position.X < -50f)
            // {
            //     vel = vel * -1f;
            // }

            shader.SetUniform("moving", Camera.Position);


            GL.Enable(EnableCap.Blend);
            Vao.Bind();
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, amountParticles);
            GL.Disable(EnableCap.Blend);
        }
        public void Dispose()
        {

            shader.Dispose();
            texture.Dispose();
        }
    }
}