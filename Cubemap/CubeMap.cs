using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace MyGame
{
    public struct TexturesCBMaps
    {
        public static int Background_CB_Map, Irradiance_CB_Map, PreFilter_CB_Map;

    }
    /// <sumary>
    /// Usada para criar cubemaps atraves de imagens comuns(retangulares) especialmente hdrs
    /// Formatos especias tambem podem ser criados 
    /// <sumary/>
    public class CubeMap
    {
        ShaderProgram equirectangularToCubemapShader;
        ShaderProgram backgroundShader, irradianceShader, prefilterShader;
        private int captureFrameBO, captureRenderBO;
        private int HDR_Texture;
        private Matrix4 captureProjection = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(90.0), 1.0f, 0.1f, 10.0f);
        private Matrix4[] captureViews =
        {
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 1.0f,  0.0f,  0.0f),  new  Vector3(0.0f, -1.0f,  0.0f)), 
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f,  0.0f,  0.0f),  new  Vector3(0.0f, -1.0f,  0.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  1.0f,  0.0f),  new  Vector3(0.0f,  0.0f,  1.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f, -1.0f,  0.0f),  new  Vector3(0.0f,  0.0f, -1.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  0.0f,  1.0f),  new  Vector3(0.0f, -1.0f,  0.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  0.0f, -1.0f),  new  Vector3(0.0f, -1.0f,  0.0f))
        };
        private PixelInternalFormat internalFormat;
        private bool CustomTexture = false;
        public CubeMap(string path, bool customTextureCubemap)
        {

            if(!File.Exists(path)) 
                throw new Exception($"Não foi possivel encontrar a Textura HDR: {path}");

            bool isHdr = false;

            if(path.Substring(path.Length - 4) == ".hdr")
            {
                isHdr = true;
            }
            internalFormat = (isHdr ? PixelInternalFormat.Rgba32f : PixelInternalFormat.Srgb);
            
            CustomTexture = customTextureCubemap;


            equirectangularToCubemapShader = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/equirectangular_to_cubemap.frag");

            backgroundShader = new ShaderProgram("Cubemap/shaders/background.vert", "Cubemap/shaders/background.frag");
            irradianceShader = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/irradiance_convolution.frag");
            prefilterShader = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/prefilter.frag");
            
            LoadImage(path);
            CreateBackground();
            ConfigureIrradianceMap();
            PreFilter();
            
        }
        private int size = 1024;
        private void LoadImage(string path)
        {
            captureFrameBO = GL.GenFramebuffer();
            captureRenderBO = GL.GenRenderbuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRenderBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, size, size);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, captureRenderBO);


            HDR_Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, HDR_Texture);

            StbImage.stbi_set_flip_vertically_on_load(1);
            using(Stream stream = File.OpenRead(path))
            {
                ImageResultFloat image = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);

                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat,
                    image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Float, image.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        private void CreateBackground()
        {
            // creando o cubemap atraves da imagem que foi carregada
            TexturesCBMaps.Background_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Background_CB_Map);

            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                size, size, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            equirectangularToCubemapShader.Use();
            equirectangularToCubemapShader.SetMatrix4("projection", captureProjection);
            equirectangularToCubemapShader.SetBool("customTexture", CustomTexture);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, HDR_Texture);
            equirectangularToCubemapShader.SetTexture("equirectangularMap", 0);

            GL.Viewport(0, 0, size, size);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            for(int i = 0; i < 6; i++)
            {
                equirectangularToCubemapShader.SetMatrix4("view", captureViews[i]);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                    TextureTarget.TextureCubeMapPositiveX + i, TexturesCBMaps.Background_CB_Map, 0);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                RenderCube();
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        // aqui começamos configurar o cube map, de reflexão
        private void ConfigureIrradianceMap()
        {
            TexturesCBMaps.Irradiance_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Irradiance_CB_Map);
            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                32, 32, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRenderBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, 32, 32);

            irradianceShader.Use();
            irradianceShader.SetMatrix4("projection", captureProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Background_CB_Map);
            irradianceShader.SetTexture("environmentMap", 0);

            GL.Viewport(0, 0, 32, 32);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            for(int i = 0; i < 6; i++)
            {
                irradianceShader.SetMatrix4("view", captureViews[i]);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                    TextureTarget.TextureCubeMapPositiveX + i, TexturesCBMaps.Irradiance_CB_Map, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                RenderCube();
                 
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        // crie um mapa pre filtro de cubo
        // ---------------------------------------------
        private void PreFilter()
        {
            TexturesCBMaps.PreFilter_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.PreFilter_CB_Map);
            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                128, 128, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // generate mipmaps for the cubemap so OpenGL automatically allocates the required memory.
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            prefilterShader.Use();
            prefilterShader.SetMatrix4("projection", captureProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Background_CB_Map);
            irradianceShader.SetTexture("environmentMap", 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            int maxMipLevels = 5;
            for(int mip = 0; mip < maxMipLevels; mip++)
            {
                int mipWidth = (int)(128 * MathHelper.Pow(0.5, mip));
                int mipHeight = (int)(128 * MathHelper.Pow(0.5, mip));

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRenderBO);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, mipWidth, mipHeight);
                GL.Viewport(0, 0, mipWidth, mipHeight);

                float roughness = (float)mip / (float)(maxMipLevels - 1);
                prefilterShader.SetFloat("roughness", roughness);
                for(int i = 0; i < 6; i++)
                {
                    prefilterShader.SetMatrix4("view", captureViews[i]);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                                            TextureTarget.TextureCubeMapPositiveX + i, TexturesCBMaps.PreFilter_CB_Map, mip);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    RenderCube();
                }
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        public void RenderFrame()
        {

            backgroundShader.Use();
            backgroundShader.SetMatrix4("projection", Camera.ProjectionMatrix);
            backgroundShader.SetMatrix4("view", Camera.ViewMatrix);
            
            backgroundShader.SetFloat("gamma", Values.gammaBackground);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Background_CB_Map);
            backgroundShader.SetTexture("environmentMap", 0);

            backgroundShader.SetBool("customTexture", CustomTexture);

            GL.Disable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Lequal);
            RenderCube();
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
        }
        private void RenderCube()
        {
            if(CustomTexture)
            {
                CustomCube.RenderCube();
            }
            else
            {
                DefaultCube.RenderCube();
            }
        }
        public void UpdateFrame() { }
        public void Dispose()
        {
            equirectangularToCubemapShader.Dispose();
            backgroundShader.Dispose();

            irradianceShader.Dispose();
            prefilterShader.Dispose();

            GL.DeleteFramebuffer(captureFrameBO);
            GL.DeleteRenderbuffer(captureRenderBO);

            GL.DeleteTexture(HDR_Texture);
            GL.DeleteTexture(TexturesCBMaps.Background_CB_Map);
            GL.DeleteTexture(TexturesCBMaps.Irradiance_CB_Map);
            GL.DeleteTexture(TexturesCBMaps.PreFilter_CB_Map);

            DefaultCube.Dispose();
            CustomCube.Dispose();
        }
        private struct DefaultCube
        {
            private static int cubeVAO;
            private static int cubeVBO;
            public  static void RenderCube()
            {
                if (cubeVAO == 0)
                {
                    float[] vertices = 
                    {
                        
                        -1.0f, -1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 0.0f, 0.0f, 
                         1.0f,  1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 1.0f, 1.0f, 
                         1.0f, -1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 1.0f, 0.0f,          
                         1.0f,  1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 1.0f, 1.0f, 
                        -1.0f, -1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 0.0f, 0.0f, 
                        -1.0f,  1.0f, -1.0f,    0.0f,  0.0f,    -1.0f, 0.0f, 1.0f, 
                        -1.0f, -1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 0.0f, 0.0f, 
                         1.0f, -1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 1.0f, 0.0f, 
                         1.0f,  1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 1.0f, 1.0f, 
                         1.0f,  1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 1.0f, 1.0f, 
                        -1.0f,  1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 0.0f, 1.0f, 
                        -1.0f, -1.0f,  1.0f,    0.0f,  0.0f,     1.0f, 0.0f, 0.0f, 
                        -1.0f,  1.0f,  1.0f,   -1.0f,  0.0f,     0.0f, 1.0f, 0.0f, 
                        -1.0f,  1.0f, -1.0f,   -1.0f,  0.0f,     0.0f, 1.0f, 1.0f, 
                        -1.0f, -1.0f, -1.0f,   -1.0f,  0.0f,     0.0f, 0.0f, 1.0f, 
                        -1.0f, -1.0f, -1.0f,   -1.0f,  0.0f,     0.0f, 0.0f, 1.0f, 
                        -1.0f, -1.0f,  1.0f,   -1.0f,  0.0f,     0.0f, 0.0f, 0.0f, 
                        -1.0f,  1.0f,  1.0f,   -1.0f,  0.0f,     0.0f, 1.0f, 0.0f, 
                         1.0f,  1.0f,  1.0f,    1.0f,  0.0f,     0.0f, 1.0f, 0.0f, 
                         1.0f, -1.0f, -1.0f,    1.0f,  0.0f,     0.0f, 0.0f, 1.0f, 
                         1.0f,  1.0f, -1.0f,    1.0f,  0.0f,     0.0f, 1.0f, 1.0f,          
                         1.0f, -1.0f, -1.0f,    1.0f,  0.0f,     0.0f, 0.0f, 1.0f, 
                         1.0f,  1.0f,  1.0f,    1.0f,  0.0f,     0.0f, 1.0f, 0.0f, 
                         1.0f, -1.0f,  1.0f,    1.0f,  0.0f,     0.0f, 0.0f, 0.0f,      
                        -1.0f, -1.0f, -1.0f,    0.0f, -1.0f,     0.0f, 0.0f, 1.0f, 
                         1.0f, -1.0f, -1.0f,    0.0f, -1.0f,     0.0f, 1.0f, 1.0f, 
                         1.0f, -1.0f,  1.0f,    0.0f, -1.0f,     0.0f, 1.0f, 0.0f, 
                         1.0f, -1.0f,  1.0f,    0.0f, -1.0f,     0.0f, 1.0f, 0.0f, 
                        -1.0f, -1.0f,  1.0f,    0.0f, -1.0f,     0.0f, 0.0f, 0.0f, 
                        -1.0f, -1.0f, -1.0f,    0.0f, -1.0f,     0.0f, 0.0f, 1.0f, 
                        -1.0f,  1.0f, -1.0f,    0.0f,  1.0f,     0.0f, 0.0f, 1.0f, 
                         1.0f,  1.0f , 1.0f,    0.0f,  1.0f,     0.0f, 1.0f, 0.0f, 
                         1.0f,  1.0f, -1.0f,    0.0f,  1.0f,     0.0f, 1.0f, 1.0f,      
                         1.0f,  1.0f,  1.0f,    0.0f,  1.0f,     0.0f, 1.0f, 0.0f, 
                        -1.0f,  1.0f, -1.0f,    0.0f,  1.0f,     0.0f, 0.0f, 1.0f, 
                        -1.0f,  1.0f,  1.0f,    0.0f,  1.0f,     0.0f, 0.0f, 0.0f       
                    };

                    cubeVAO = GL.GenVertexArray();
                    GL.BindVertexArray(cubeVAO);

                    cubeVBO = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                }
                // render DefaultCube
                GL.BindVertexArray(cubeVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            public static void Dispose()
            {
                GL.DeleteVertexArray(cubeVAO);
                GL.DeleteBuffer(cubeVBO);
            }
        }
        private struct CustomCube
        {
            private static int cubeVAO;
            private static int cubeVBO;
            public static void RenderCube()
            {
                if (cubeVAO == 0)
                {
                    float[] vertices = 
                    {

                        -1.0f,  1.0f,  -1.0f,    -0.0f,   1.0f,  -0.0f,     0.999914f,  0.750482f,
                         1.0f,  1.0f,   1.0f,    -0.0f,   1.0f,  -0.0f,     0.667046f,  0.999732f,
                         1.0f,  1.0f,  -1.0f,    -0.0f,   1.0f,  -0.0f,     0.666272f,  0.750489f,
                         1.0f,  1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.666289f,  0.000426f,
                        -1.0f, -1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.334182f,  0.249989f,
                         1.0f, -1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.334184f,  0.000426f,
                        -1.0f,  1.0f,   1.0f,    -1.0f,  -0.0f,  -0.0f,     0.666288f,  0.249506f,
                        -1.0f, -1.0f,  -1.0f,    -1.0f,  -0.0f,  -0.0f,     0.334186f,  0.499505f,
                        -1.0f, -1.0f,   1.0f,    -1.0f,  -0.0f,  -0.0f,     0.334182f,  0.249989f,
                         1.0f, -1.0f,  -1.0f,    -0.0f,  -1.0f,  -0.0f,     0.334179f,  0.750536f,
                        -1.0f, -1.0f,   1.0f,    -0.0f,  -1.0f,  -0.0f,     0.000838f,  0.999708f,
                        -1.0f, -1.0f,  -1.0f,    -0.0f,  -1.0f,  -0.0f,     0.000958f,  0.750539f,
                         1.0f,  1.0f,  -1.0f,     1.0f,  -0.0f,  -0.0f,     0.666272f,  0.750489f,
                         1.0f, -1.0f,   1.0f,     1.0f,  -0.0f,  -0.0f,     0.333746f,  0.999729f,
                         1.0f, -1.0f,  -1.0f,     1.0f,  -0.0f,  -0.0f,     0.334179f,  0.750536f,
                        -1.0f,  1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.666291f,  0.499505f,
                         1.0f, -1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.334179f,  0.750536f,
                        -1.0f, -1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.334186f,  0.499505f,
                        -1.0f,  1.0f,  -1.0f,    -0.0f,   1.0f,  -0.0f,     0.999914f,  0.750482f,
                        -1.0f,  1.0f,   1.0f,    -0.0f,   1.0f,  -0.0f,     0.999908f,  0.999732f,
                         1.0f,  1.0f,   1.0f,    -0.0f,   1.0f,  -0.0f,     0.667046f,  0.999732f,
                         1.0f,  1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.666289f,  0.000426f,
                        -1.0f,  1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.666288f,  0.249506f,
                        -1.0f, -1.0f,   1.0f,    -0.0f,  -0.0f,   1.0f,     0.334182f,  0.249989f,
                        -1.0f,  1.0f,   1.0f,    -1.0f,  -0.0f,  -0.0f,     0.666288f,  0.249506f,
                        -1.0f,  1.0f,  -1.0f,    -1.0f,  -0.0f,  -0.0f,     0.666291f,  0.499505f,
                        -1.0f, -1.0f,  -1.0f,    -1.0f,  -0.0f,  -0.0f,     0.334186f,  0.499505f,
                         1.0f, -1.0f,  -1.0f,    -0.0f,  -1.0f,  -0.0f,     0.334179f,  0.750536f,
                         1.0f, -1.0f,   1.0f,    -0.0f,  -1.0f,  -0.0f,     0.333746f,  0.999729f,
                        -1.0f, -1.0f,   1.0f,    -0.0f,  -1.0f,  -0.0f,     0.000838f,  0.999708f,
                         1.0f,  1.0f,  -1.0f,     1.0f,  -0.0f,  -0.0f,     0.666272f,  0.750489f,
                         1.0f,  1.0f,   1.0f,     1.0f,  -0.0f,  -0.0f,     0.667046f,  0.999732f,
                         1.0f, -1.0f,   1.0f,     1.0f,  -0.0f,  -0.0f,     0.333746f,  0.999729f,
                        -1.0f,  1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.666291f,  0.499505f,
                         1.0f,  1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.666272f,  0.750489f,
                         1.0f, -1.0f,  -1.0f,    -0.0f,  -0.0f,  -1.0f,     0.334179f,  0.750536f,

                    };

                    cubeVAO = GL.GenVertexArray();
                    GL.BindVertexArray(cubeVAO);

                    cubeVBO = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                }
                // render CustomCube
                GL.BindVertexArray(cubeVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            public static void Dispose()
            {
                GL.DeleteVertexArray(cubeVAO);
                GL.DeleteBuffer(cubeVBO);
            }
        }
    }
}