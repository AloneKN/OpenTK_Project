using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    // A abstração do objeto array de vértices.
    public class VertexArrayObject : IDisposable
    {
        private int Handle;
        private List<int> buffersLinked;
        public VertexArrayObject()
        {
            Handle = GL.GenVertexArray();
            buffersLinked = new List<int>();
        }
        public void LinkBufferObject<TDataType>(ref BufferObject<TDataType> bufferObject) where TDataType : unmanaged
        {
            Bind();
            bufferObject.Bind();
            buffersLinked.Add(bufferObject.Handle);
        }
        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, int vertexSize, int offSet)
        {
            GL.VertexAttribPointer(index, count, type, false, vertexSize, offSet);
            GL.EnableVertexAttribArray(index);
        }
        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, int vertexSize, IntPtr offSet)
        {
            GL.VertexAttribPointer(index, count, type, false, vertexSize, (int)offSet);
            GL.EnableVertexAttribArray(index);
        }
        public void Bind() => GL.BindVertexArray(Handle);
        
        public void Dispose()
        {
            GL.DeleteVertexArray(Handle);
            GL.DeleteBuffers(buffersLinked.Count, buffersLinked.ToArray());
        }
    }
}