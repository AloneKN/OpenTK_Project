using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace MyGame
{
    public class FrameBuffer : IDisposable
    {
        public int textureColorBuffer { get; private set; }
        public int frameBufferObject { get; private set; }
        private int renderBufferObject;
        private Vector2i sizeWindow = Program.Size; 
        public FrameBuffer()
        {
            textureColorBuffer = GL.GenTexture();
            renderBufferObject = GL.GenRenderbuffer();
            frameBufferObject = GL.GenFramebuffer();

            ConfigFrameBuffer();
        }
        private void ConfigFrameBuffer()
        {
            GL.BindTexture(TextureTarget.Texture2D, textureColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha,
                    sizeWindow.X, sizeWindow.Y, 0, PixelFormat.Rgb, PixelType.Byte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer , renderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, sizeWindow.X, sizeWindow.Y);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferObject);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureColorBuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderBufferObject);   

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete && !DebugGL.DebugInitiated)
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        public void ResizeFrameBuffer(Vector2i Size)
        {
            sizeWindow = Size;
            ConfigFrameBuffer();
        }
        public void Dispose()
        {
            GL.DeleteTexture(textureColorBuffer);
            GL.DeleteRenderbuffer(renderBufferObject);
            GL.DeleteFramebuffer(frameBufferObject);
        }
    }
}