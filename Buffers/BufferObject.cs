using OpenTK.Graphics.OpenGL4;

namespace MyGame
{
    // Nossa abstração do objeto buffer.
    public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
    {
        private int Handle;
        private BufferTarget _bufferTarget;

        public unsafe BufferObject(Span<TDataType> data, BufferTarget bufferTarget)
        {
            _bufferTarget = bufferTarget;

            Handle = GL.GenBuffer();
            Bind();
            GL.BufferData(_bufferTarget, data.Length * sizeof(TDataType), data.ToArray(), BufferUsageHint.StaticDraw);
        }

        public void Bind()
        {
            GL.BindBuffer(_bufferTarget, Handle);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(Handle);
        }
    }
}