using OpenTK.Graphics.OpenGL4;

namespace MyGame
{
    public class CubemapDefault
    {
        private static VertexArrayObject ?Vao;
        private static BufferObject<float> ?vbo;
        public static void RenderCube()
        {
            if (Vao == null)
            {
                float[] vertices = 
                {
                    
                    -1.0f, -1.0f, -1.0f,  0.0f, 0.0f, 
                     1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 
                     1.0f, -1.0f, -1.0f,  1.0f, 0.0f,          
                     1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 
                    -1.0f, -1.0f, -1.0f,  0.0f, 0.0f, 
                    -1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 
                    -1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 
                     1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 
                     1.0f,  1.0f,  1.0f,  1.0f, 1.0f, 
                     1.0f,  1.0f,  1.0f,  1.0f, 1.0f, 
                    -1.0f,  1.0f,  1.0f,  0.0f, 1.0f, 
                    -1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 
                    -1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 
                    -1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 
                    -1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                    -1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                    -1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 
                    -1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 
                     1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 
                     1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                     1.0f,  1.0f, -1.0f,  1.0f, 1.0f,          
                     1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                     1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 
                     1.0f, -1.0f,  1.0f,  0.0f, 0.0f,      
                    -1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                     1.0f, -1.0f, -1.0f,  1.0f, 1.0f, 
                     1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 
                     1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 
                    -1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 
                    -1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 
                    -1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 
                     1.0f,  1.0f , 1.0f,  1.0f, 0.0f, 
                     1.0f,  1.0f, -1.0f,  1.0f, 1.0f,      
                     1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 
                    -1.0f,  1.0f, -1.0f,  0.0f, 1.0f, 
                    -1.0f,  1.0f,  1.0f,  0.0f, 0.0f       
                };

                Vao = new VertexArrayObject();
                vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);

                Vao.LinkBufferObject(ref vbo);
                Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float), 0 * sizeof(float));
                Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));
            }
            // render DefaultCube
            Vao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        public static void Dispose() => Vao!.Dispose();
    }
    public class CubemapT
    {
        private static VertexArrayObject ?Vao;
        private static BufferObject<float> ?vbo;
        public static void RenderCube()
        {
            if (Vao == null)
            {
                float[] vertices = 
                {

                    -1.0f,  1.0f, -1.0f,   0.999914f, 0.750482f,
                     1.0f,  1.0f,  1.0f,   0.667046f, 0.999732f,
                     1.0f,  1.0f, -1.0f,   0.666272f, 0.750489f,
                     1.0f,  1.0f,  1.0f,   0.666289f, 0.000426f,
                    -1.0f, -1.0f,  1.0f,   0.334182f, 0.249989f,
                     1.0f, -1.0f,  1.0f,   0.334184f, 0.000426f,
                    -1.0f,  1.0f,  1.0f,   0.666288f, 0.249506f,
                    -1.0f, -1.0f, -1.0f,   0.334186f, 0.499505f,
                    -1.0f, -1.0f,  1.0f,   0.334182f, 0.249989f,
                     1.0f, -1.0f, -1.0f,   0.334179f, 0.750536f,
                    -1.0f, -1.0f,  1.0f,   0.000838f, 0.999708f,
                    -1.0f, -1.0f, -1.0f,   0.000958f, 0.750539f,
                     1.0f,  1.0f, -1.0f,   0.666272f, 0.750489f,
                     1.0f, -1.0f,  1.0f,   0.333746f, 0.999729f,
                     1.0f, -1.0f, -1.0f,   0.334179f, 0.750536f,
                    -1.0f,  1.0f, -1.0f,   0.666291f, 0.499505f,
                     1.0f, -1.0f, -1.0f,   0.334179f, 0.750536f,
                    -1.0f, -1.0f, -1.0f,   0.334186f, 0.499505f,
                    -1.0f,  1.0f, -1.0f,   0.999914f, 0.750482f,
                    -1.0f,  1.0f,  1.0f,   0.999908f, 0.999732f,
                     1.0f,  1.0f,  1.0f,   0.667046f, 0.999732f,
                     1.0f,  1.0f,  1.0f,   0.666289f, 0.000426f,
                    -1.0f,  1.0f,  1.0f,   0.666288f, 0.249506f,
                    -1.0f, -1.0f,  1.0f,   0.334182f, 0.249989f,
                    -1.0f,  1.0f,  1.0f,   0.666288f, 0.249506f,
                    -1.0f,  1.0f, -1.0f,   0.666291f, 0.499505f,
                    -1.0f, -1.0f, -1.0f,   0.334186f, 0.499505f,
                     1.0f, -1.0f, -1.0f,   0.334179f, 0.750536f,
                     1.0f, -1.0f,  1.0f,   0.333746f, 0.999729f,
                    -1.0f, -1.0f,  1.0f,   0.000838f, 0.999708f,
                     1.0f,  1.0f, -1.0f,   0.666272f, 0.750489f,
                     1.0f,  1.0f,  1.0f,   0.667046f, 0.999732f,
                     1.0f, -1.0f,  1.0f,   0.333746f, 0.999729f,
                    -1.0f,  1.0f, -1.0f,   0.666291f, 0.499505f,
                     1.0f,  1.0f, -1.0f,   0.666272f, 0.750489f,
                     1.0f, -1.0f, -1.0f,   0.334179f, 0.750536f,

                };

                Vao = new VertexArrayObject();
                vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);

                Vao.LinkBufferObject(ref vbo);
                Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float), 0 * sizeof(float));
                Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            }

            Vao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        public static void Dispose() => Vao!.Dispose();
    }
    public class CubeMapCross
    {
        private static VertexArrayObject ?Vao;
        private static BufferObject<float> ?vbo;
        public static void RenderCube()
        {
            if (Vao == null)
            {
                float[] vertices = 
                {

                    -1.0f,  1.0f, -1.0f,     0.499858f, 0.999974f,
                     1.0f,  1.0f,  1.0f,     0.250174f, 0.666554f,
                     1.0f,  1.0f, -1.0f,     0.499842f, 0.666538f,
                     1.0f,  1.0f,  1.0f,     0.250174f, 0.666554f,
                    -1.0f, -1.0f,  1.0f,     0.000161f, 0.333634f,
                     1.0f, -1.0f,  1.0f,     0.250158f, 0.333594f,
                    -1.0f,  1.0f,  1.0f,     0.999946f, 0.666505f,
                    -1.0f, -1.0f, -1.0f,     0.750488f, 0.333561f,
                    -1.0f, -1.0f,  1.0f,     0.99993f,  0.333545f,
                     1.0f, -1.0f, -1.0f,     0.499826f, 0.333578f,
                    -1.0f, -1.0f,  1.0f,     0.250142f, 0.000504f,
                    -1.0f, -1.0f, -1.0f,     0.49981f,  0.000488f,
                     1.0f,  1.0f, -1.0f,     0.499842f, 0.666538f,
                     1.0f, -1.0f,  1.0f,     0.250158f, 0.333594f,
                     1.0f, -1.0f, -1.0f,     0.499826f, 0.333578f,
                    -1.0f,  1.0f, -1.0f,     0.750504f, 0.666521f,
                     1.0f, -1.0f, -1.0f,     0.499826f, 0.333578f,
                    -1.0f, -1.0f, -1.0f,     0.750488f, 0.333561f,
                    -1.0f,  1.0f, -1.0f,     0.499858f, 0.999974f,
                    -1.0f,  1.0f,  1.0f,     0.250191f, 0.99999f,
                     1.0f,  1.0f,  1.0f,     0.250174f, 0.666554f,
                     1.0f,  1.0f,  1.0f,     0.250174f, 0.666554f,
                    -1.0f,  1.0f,  1.0f,     0.000139f, 0.666522f,
                    -1.0f, -1.0f,  1.0f,     0.000161f, 0.333634f,
                    -1.0f,  1.0f,  1.0f,     0.999946f, 0.666505f,
                    -1.0f,  1.0f, -1.0f,     0.750504f, 0.666521f,
                    -1.0f, -1.0f, -1.0f,     0.750488f, 0.333561f,
                     1.0f, -1.0f, -1.0f,     0.499826f, 0.333578f,
                     1.0f, -1.0f,  1.0f,     0.250158f, 0.333594f,
                    -1.0f, -1.0f,  1.0f,     0.250142f, 0.000504f,
                     1.0f,  1.0f, -1.0f,     0.499842f, 0.666538f,
                     1.0f,  1.0f,  1.0f,     0.250174f, 0.666554f,
                     1.0f, -1.0f,  1.0f,     0.250158f, 0.333594f,
                    -1.0f,  1.0f, -1.0f,     0.750504f, 0.666521f,
                     1.0f,  1.0f, -1.0f,     0.499842f, 0.666538f,
                     1.0f, -1.0f, -1.0f,     0.499826f, 0.333578f,

                };

                Vao = new VertexArrayObject();
                vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);

                Vao.LinkBufferObject(ref vbo);
                Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float), 0 * sizeof(float));
                Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            }

            Vao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        public static void Dispose() => Vao!.Dispose();
    }
}