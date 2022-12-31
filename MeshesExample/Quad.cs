using OpenTK.Graphics.OpenGL4;

namespace MyGame
{
    public struct Quad
    {
        private static VertexArrayObject ?Vao;
        private static BufferObject<float> ?vbo;
        public static void RenderQuad()
        {
            if (Vao == null)
            {
                float[] vertices = 
                {
                    // positions        // texture Coords
                    -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                     1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
                     1.0f, -1.0f, 0.0f, 1.0f, 0.0f,       
                };

                Vao = new VertexArrayObject();
                vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);
                Vao.LinkBufferObject(ref vbo);

                Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
                Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
            }
            // render Cube
            Vao.Bind();
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }
        public void Dispose()
        {

            Vao!.Dispose();
            vbo!.Dispose();
        }
    }
}