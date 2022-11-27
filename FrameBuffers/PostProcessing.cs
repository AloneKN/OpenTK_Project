using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class PostProcessing
    {
        private ShaderProgram shader;
        private FrameBuffer post_process;
        private TextureProgram lensEffect;
        public PostProcessing()
        {
            shader = new ShaderProgram("FrameBuffers/PostProcessing-shaders/post_process.vert", "FrameBuffers/PostProcessing-shaders/post_process.frag");
            lensEffect = TextureProgram.Load("Resources/lens/lf_lensDirt-3.png");
            post_process = new FrameBuffer();
        }
        /// <summary>
        /// Ativa o framebuffer, deve ser chamado antes de fazer as chamadas de desenho.
        /// </summary>
        public void Active()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, post_process.frameBufferObject);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }
        /// <summary>
        /// Renderiza o framebuffer.
        /// </summary>
        public void RenderFrame()
        {
            shader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, post_process.textureColorBuffer);
            shader.SetTexture("screenTexture", 0);

            lensEffect.Use(TextureUnit.Texture1);
            shader.SetTexture("lensTexture", 1);
            shader.SetFloat("interp", Values.lensInterpolation);

            shader.SetFloat("elapsedTime", Clock.ElapsedTime);
            shader.SetFloat("grainAmount", Values.FilmGrainAmount);
            shader.SetFloat("shineValue", Values.shineValue);

            // ativa o buffer padrao para desenhar para ele 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Disable(EnableCap.DepthTest);
            Quad.RenderQuad();
            GL.Enable(EnableCap.DepthTest);

        }
        public void ResizedFrame()
        {
            post_process.ResizeFrameBuffer(Program.Size);
        }
        public void Dispose()
        {
            shader.Dispose();
            lensEffect.Dispose();
            post_process.Dispose();
        }
    }
}