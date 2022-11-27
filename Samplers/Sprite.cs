using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class Sprite : IDisposable
    {
        private ShaderProgram shader;
        private TextureProgram DiffuseMap, NormalMap;
        private Matrix4 model = Matrix4.Identity;
        private int Vao, Vbo;
        public Sprite(string DiffuseTex, string NormalTex)
        {
            shader = new ShaderProgram("GLSL/Mesh/shader.vert", "GLSL/Mesh/shader.frag");
            DiffuseMap = TextureProgram.Load(DiffuseTex);
            NormalMap = TextureProgram.Load(NormalTex);


            // isso é uma forma de usar um sprite sem a nescesiade de chamar a clase do assimp
            Vector3 pos1 = new Vector3(-1.0f,  1.0f, 0.0f);
            Vector3 pos2 = new Vector3(-1.0f, -1.0f, 0.0f);
            Vector3 pos3 = new Vector3( 1.0f, -1.0f, 0.0f);
            Vector3 pos4 = new Vector3( 1.0f,  1.0f, 0.0f);

            Vector2 uv1 = new Vector2(0.0f, 1.0f);
            Vector2 uv2 = new Vector2(0.0f, 0.0f);
            Vector2 uv3 = new Vector2(1.0f, 0.0f);
            Vector2 uv4 = new Vector2(1.0f, 1.0f);

            Vector3 nm = new Vector3(0.0f, 0.0f, 1.0f);

            Vector3 tangent1, bitangent1;
            Vector3 tangent2, bitangent2;

            // triangle 1
            // ----------
            Vector3 edge1 = pos2 - pos1;
            Vector3 edge2 = pos3 - pos1;
            Vector2 deltaUV1 = uv2 - uv1;
            Vector2 deltaUV2 = uv3 - uv1;

            float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

            tangent1.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
            tangent1.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
            tangent1.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);

            bitangent1.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
            bitangent1.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
            bitangent1.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);

            // triangle 2
            // ----------
            edge1 = pos3 - pos1;
            edge2 = pos4 - pos1;
            deltaUV1 = uv3 - uv1;
            deltaUV2 = uv4 - uv1;

            f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

            tangent2.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
            tangent2.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
            tangent2.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);


            bitangent2.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
            bitangent2.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
            bitangent2.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);

            float []vertices = 
            {
                // positions            // normal         // texcoords  // tangent                          // bitangent
                pos1.X, pos1.Y, pos1.Z, nm.X, nm.Y, nm.Z, uv1.X, uv1.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,
                pos2.X, pos2.Y, pos2.Z, nm.X, nm.Y, nm.Z, uv2.X, uv2.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,
                pos3.X, pos3.Y, pos3.Z, nm.X, nm.Y, nm.Z, uv3.X, uv3.Y, tangent1.X, tangent1.Y, tangent1.Z, bitangent1.X, bitangent1.Y, bitangent1.Z,

                pos1.X, pos1.Y, pos1.Z, nm.X, nm.Y, nm.Z, uv1.X, uv1.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z,
                pos3.X, pos3.Y, pos3.Z, nm.X, nm.Y, nm.Z, uv3.X, uv3.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z,
                pos4.X, pos4.Y, pos4.Z, nm.X, nm.Y, nm.Z, uv4.X, uv4.Y, tangent2.X, tangent2.Y, tangent2.Z, bitangent2.X, bitangent2.Y, bitangent2.Z
            };
                
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));

        }
        public void RenderFrame()
        {
            shader.Use();

            model = Matrix4.Identity;
            model = model * Matrix4.CreateScale(1.1f, 1.0f, 1.0f);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", Camera.ViewMatrix);
            shader.SetMatrix4("projection", Camera.ProjectionMatrix);

            shader.SetVector3("lightPos", Game.LuzPosition);
            shader.SetVector3("viewPos", Camera.Position);

            DiffuseMap.Use(TextureUnit.Texture0);
            shader.SetTexture("DiffuseMap", 0);

            NormalMap.Use(TextureUnit.Texture1);
            shader.SetTexture("NormalMap", 1);
            
            GL.BindVertexArray(Vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
        public void UpdateFrame()
        {
            
        }
        public void Dispose()
        {
            shader.Dispose();
            DiffuseMap.Dispose();
            NormalMap.Dispose();
            
        }
    }
}