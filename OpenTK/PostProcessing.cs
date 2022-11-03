using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_GLTK
{
    public class PostProcessing : IDisposable
    {
        ShaderProgram shader;
        private int textureColorBuffer, renderBufferObject, frameBufferObject;
        public PostProcessing()
        {
            shader = new ShaderProgram("GLSL/Post Processing/post_process.vert", "GLSL/Post Processing/post_process.frag");

            textureColorBuffer = GL.GenTexture();
            renderBufferObject = GL.GenRenderbuffer();
            frameBufferObject = GL.GenFramebuffer();

            ResizeFrame(Program.Size);
        }
        public void ResizeFrame(Vector2i Size)
        {

            GL.BindTexture(TextureTarget.Texture2D, textureColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha,
                    Size.X, Size.Y, 0, PixelFormat.Rgb, PixelType.Byte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer , renderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.X, Size.Y);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferObject);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureColorBuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderBufferObject);   

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
            

        }
        public void ActiveBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferObject);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public void RenderFrame()
        {
            shader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureColorBuffer);
            shader.SetTexture("screenTexture", 0);

            shader.SetFloat("elapsedTime", TimerGL.ElapsedTime);
            shader.SetFloat("grainAmount", Values.FilmGrainAmount);
            shader.SetFloat("shineValue", Values.shineValue);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.DepthTest);
            Quad.RenderQuad();
            GL.Enable(EnableCap.DepthTest);

        }
        public void Dispose()
        {
            GL.DeleteTexture(textureColorBuffer);
            GL.DeleteRenderbuffer(renderBufferObject);
            GL.DeleteFramebuffer(frameBufferObject);
        }
    }
}