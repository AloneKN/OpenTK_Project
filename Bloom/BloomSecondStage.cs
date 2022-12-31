using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace MyGame
{
    class Bloom : IDisposable
    {
        private Vector2i sizeWindow { get => Program.Size; }
        private ShaderProgram shaderNewBloomFinal;

        private int renderBuffer;
        private int frameBuffer;
        public int[] TexturesBuffer = new int[2];
        public int UseTexture { get => TexturesBuffer[1]; }

        // new bloom
        private BloomFirstStage FirstStage;

        public Bloom()
        {

            shaderNewBloomFinal = new ShaderProgram("Bloom/shadersBloom/new_bloom.vert", "Bloom/shadersBloom/newbloomfinal.frag");

            FirstStage = new BloomFirstStage(6);

            frameBuffer  = GL.GenFramebuffer();
            renderBuffer = GL.GenRenderbuffer();
            GL.GenTextures(2, TexturesBuffer);


            ResizedFrame();
        }
        private void ResizedFrame()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

            for(int i = 0; i < 2; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, TexturesBuffer[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f,
                    sizeWindow.X, sizeWindow.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, 
                    TextureTarget.Texture2D, TexturesBuffer[i], 0);
            }

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer , renderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, sizeWindow.X, sizeWindow.Y);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, renderBuffer);   

            DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 }; 
            GL.DrawBuffers(2, attachments);

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                    Console.WriteLine("Framebuffer Not complete!");



            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        public static bool EnableBloom { get => Values.isRenderBloom; }
        public void Active()
        {
            if(EnableBloom)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            }
        }
        public void ResizedFrameBuffer()
        {
            if(EnableBloom)
            {
                ResizedFrame();
                FirstStage.ResizedFrameBuffer();
            }
        }
        public void RenderFrame()
        {
            if(EnableBloom)
            {
                FirstStage.RenderBloomTexture(UseTexture, Values.filterRadius);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                shaderNewBloomFinal.Use();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, TexturesBuffer[0]);
                shaderNewBloomFinal.SetUniform("scene", 0);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, FirstStage.UseTexture);
                shaderNewBloomFinal.SetUniform("bloomBlur", 1);

                shaderNewBloomFinal.SetUniform("exposure", Values.new_bloom_exp);
                shaderNewBloomFinal.SetUniform("bloomStrength", Values.new_bloom_streng);
                shaderNewBloomFinal.SetUniform("gamma", Values.new_bloom_gama);
                shaderNewBloomFinal.SetUniform("film_grain", Values.new_bloom_filmGrain);
                shaderNewBloomFinal.SetUniform("elapsedTime", Clock.ElapsedTime);

                shaderNewBloomFinal.SetUniform("nitidezStrength", Values.nitidezStrengh);

                Quad.RenderQuad();
            }

        }
        public void Dispose()
        {
            FirstStage.Dispose();
            shaderNewBloomFinal.Dispose();

            GL.DeleteTextures(2, TexturesBuffer);
            GL.DeleteRenderbuffer(renderBuffer);
            GL.DeleteFramebuffer(frameBuffer);

        }
    }
}