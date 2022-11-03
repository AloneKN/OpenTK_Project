using OpenTK.Graphics.OpenGL4;

namespace Open_GLTK
{
    public struct Quad : IDisposable
    {
        private static int quadVAO;
        private static int quadVBO;
        public static void RenderQuad()
        {
            if (quadVAO == 0)
            {
                float[] vertices = 
                {
                    // positions        // texture Coords
                    -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                     1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
                     1.0f, -1.0f, 0.0f, 1.0f, 0.0f,       
                };

                quadVAO = GL.GenVertexArray();
                GL.BindVertexArray(quadVAO);

                quadVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            }
            // render Cube
            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }
        public void Dispose()
        {

            GL.DeleteVertexArray(quadVAO);
            GL.DeleteBuffer(quadVBO);
        }
    }
}