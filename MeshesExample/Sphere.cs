using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class Sphere : IDisposable
    {
        private static VertexArrayObject ?Vao;
        private static BufferObject<float> ?Vbo;
        private static BufferObject<int> ?Ebo;
        private static int indexCount;
        public static void RenderSphere(PrimitiveType type = PrimitiveType.Triangles)
        {
            if(Vao == null)
            {
                List<Vector3> positions = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();
                List<Vector3> normals = new List<Vector3>();
                List<int>    indices = new List<int>();

                const int X_SEGMENTS = 32;
                const int Y_SEGMENTS = 32;
                const float PI = 3.14159265359f;


                for (int x = 0; x <= X_SEGMENTS; ++x)
                {
                    for (int y = 0; y <= Y_SEGMENTS; ++y)
                    {
                        float xSegment = (float)x / (float)X_SEGMENTS;
                        float ySegment = (float)y / (float)Y_SEGMENTS;
                        float xPos = MathF.Cos(xSegment * 2.0f * PI) * MathF.Sin(ySegment * PI);
                        float yPos = MathF.Cos(ySegment * PI);
                        float zPos = MathF.Sin(xSegment * 2.0f * PI) * MathF.Sin(ySegment * PI);

                        positions.Add(new Vector3(xPos, yPos, zPos));
                        uv.Add(new Vector2(xSegment, ySegment));
                        normals.Add(new Vector3(xPos, yPos, zPos));
                    }
                }

                bool oddRow = false;
                for (int y = 0; y < Y_SEGMENTS; ++y)
                {
                    if (!oddRow) // even rows: y == 0, y == 2; and so on
                    {
                        for (int x = 0; x <= X_SEGMENTS; ++x)
                        {
                            indices.Add(y * (X_SEGMENTS + 1) + x);
                            indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
                        }
                    }
                    else
                    {
                        for (int x = X_SEGMENTS; x >= 0; --x)
                        {
                            indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
                            indices.Add(y * (X_SEGMENTS + 1) + x);
                        }
                    }
                    oddRow = !oddRow;
                }
                
                indexCount = indices.Count;

                List<float> data = new List<float>();

                for (int i = 0; i < positions.Count; ++i)
                {
                    data.Add(positions[i].X);
                    data.Add(positions[i].Y);
                    data.Add(positions[i].Z);
                    if (normals.Count > 0)
                    {
                        data.Add(normals[i].X);
                        data.Add(normals[i].Y);
                        data.Add(normals[i].Z);
                    }
                    if (uv.Count > 0)
                    {
                        data.Add(uv[i].X);
                        data.Add(uv[i].Y);
                    }
                }

                // vertexPosition + texCoords + nomalsPositions
                int stride = (3 + 2 + 3);

                Vao = new VertexArrayObject();
                Vbo = new BufferObject<float>(data.ToArray(), BufferTarget.ArrayBuffer);
                Vao.LinkBufferObject(ref Vbo);
                Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, stride * sizeof(float), 0 * sizeof(float));
                Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, stride * sizeof(float), 3 * sizeof(float));
                Vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, stride * sizeof(float), 6 * sizeof(float));

                Ebo = new BufferObject<int>(indices.ToArray(), BufferTarget.ElementArrayBuffer);
                Vao.LinkBufferObject(ref Ebo);
            }

            Vao!.Bind();
            GL.DrawElements(type, indexCount, DrawElementsType.UnsignedInt, 0);
        }
        public void Dispose() => Vao!.Dispose();
    }
}