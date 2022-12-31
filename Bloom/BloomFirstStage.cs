using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{

    public class BloomFirstStage
    {
        public struct BloomMip
        {
            public Vector2 size;
            public int texture;
        }
        private BloomMip[] mMipChain;
        private Vector2i sizeWindow { get => Program.Size; }
        private int mipFrameBuffer;
        public int UseTexture { get => mMipChain[0].texture; }
        //----------------------------------------------------------

	    private ShaderProgram mDownsampleShader;
	    private ShaderProgram mUpsampleShader;

        private bool mKarisAverageOnDownsample = true;

        public BloomFirstStage(int NumBlomMips)
        {
            mDownsampleShader = new ShaderProgram("Bloom/shadersBloom/new_bloom.vert", "Bloom/shadersBloom/downscale.frag");
            mUpsampleShader = new ShaderProgram("Bloom/shadersBloom/new_bloom.vert", "Bloom/shadersBloom/upscale.frag");

            mMipChain = new BloomMip[NumBlomMips];

            mipFrameBuffer = GL.GenFramebuffer();
            for(int i = 0; i < NumBlomMips; i++)
                mMipChain[i].texture = GL.GenTexture();


            ResizedFrameBuffer();
        }
        public void ResizedFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mipFrameBuffer);
            Vector2 mipSize = sizeWindow;
            Vector2i mipIntSize = sizeWindow;

            for(int i = 0; i < mMipChain.Length; i++)
            {
                mipSize *= 0.5f;
                mipIntSize /= 2;
                
                mMipChain[i].size = mipSize;
      
                GL.BindTexture(TextureTarget.Texture2D, mMipChain[i].texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R11fG11fB10f, 
                    mipIntSize.X, mipIntSize.Y, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                TextureTarget.Texture2D, mMipChain[0].texture, 0);

            DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0}; 
            GL.DrawBuffers(1, attachments);

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("Framebuffer Not complete!");
                    
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        private void RenderDownSamples(int srcTexture)
        {
            mDownsampleShader.Use();
            mDownsampleShader.SetUniform("srcResolution", sizeWindow);
            if(mKarisAverageOnDownsample)
            {
                mDownsampleShader.SetUniform("mipLevel", 0);
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, srcTexture);
            mDownsampleShader.SetUniform("srcTexture", 0);

            for(int i = 0; i < mMipChain.Length; i++)
            {
                BloomMip mip = mMipChain[i];
                GL.Viewport(0, 0, (int)mip.size.X, (int)mip.size.Y);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, mip.texture, 0);

                Quad.RenderQuad();

                mDownsampleShader.SetUniform("srcResolution", mip.size);
                GL.BindTexture(TextureTarget.Texture2D, mip.texture);

                if(i == 0)
                {
                    mDownsampleShader.SetUniform("mipLevel", 1);
                }
            }
        }
        private void RenderUpSamples(float FilterRadius)
        {

            mUpsampleShader.Use();
            mUpsampleShader.SetUniform("filterRadius", FilterRadius);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            
            for(int i = mMipChain.Length - 1; i > 0; i--)
            {
                BloomMip mip = mMipChain[i];
                BloomMip nextMip = mMipChain[i - 1];

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mip.texture);
                mUpsampleShader.SetUniform("srcTexture", 0);

                GL.Viewport(0, 0, (int)nextMip.size.X, (int)nextMip.size.Y);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, nextMip.texture, 0);

                Quad.RenderQuad();
            }

            GL.Disable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        }
        public void RenderBloomTexture(int srcTexture, float FilterRadius)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mipFrameBuffer);

            RenderDownSamples(srcTexture);
            RenderUpSamples(FilterRadius);

            GL.Viewport(0, 0, sizeWindow.X, sizeWindow.Y);
        }
        
        public void Dispose()
        {
             GL.DeleteFramebuffer(mipFrameBuffer);

            foreach(var item in mMipChain)
                GL.DeleteTexture(item.texture);

            mDownsampleShader.Dispose();
            mUpsampleShader.Dispose();
        }
    }
}