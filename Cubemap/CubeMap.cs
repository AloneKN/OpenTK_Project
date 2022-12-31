using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace MyGame
{
    /// <sumary>
    /// Usada para criar cubemaps atraves de imagens comuns(retangulares) especialmente hdrs
    /// Formatos especias tambem podem ser criados 
    /// <sumary/>
    public struct TexturesCBMaps
    {
        public int Background_CB_Map { get; set; } 
        public int Irradiance_CB_Map { get; set; }
        public int PreFilter_CB_Map  { get; set; }
    }
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
            
            CreateMaps(path);
            
        }
        private int size = 1024;
        private void CreateMaps(string path)
        {
            LoadImage(path);
            CreateBackground();
            ConfigureIrradianceMap();
            PreFilter();
       
        }
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
        private TexturesCBMaps _textures_cb_maps;
        public TexturesCBMaps texturesCBMaps { get => _textures_cb_maps; }
        private void CreateBackground()
        {
            // creando o cubemap atraves da imagem que foi carregada
            _textures_cb_maps.Background_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.Background_CB_Map);

            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                size, size, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }

            SetParametersImage();

            
            equirectangularToCubemapShader.Use();
            equirectangularToCubemapShader.SetUniform("projection", captureProjection);
            equirectangularToCubemapShader.SetUniform("customTexture", CustomTexture);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, HDR_Texture);
            equirectangularToCubemapShader.SetUniform("equirectangularMap", 0);

            GL.Viewport(0, 0, size, size);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            for(int i = 0; i < 6; i++)
            {
                equirectangularToCubemapShader.SetUniform("view", captureViews[i]);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                    TextureTarget.TextureCubeMapPositiveX + i, _textures_cb_maps.Background_CB_Map, 0);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                RenderCube();
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        // aqui começamos configurar o cube map, de reflexão
        private void ConfigureIrradianceMap()
        {
            _textures_cb_maps.Irradiance_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.Irradiance_CB_Map);
            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                32, 32, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }
            
            SetParametersImage();


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRenderBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, 32, 32);

            irradianceShader.Use();
            irradianceShader.SetUniform("projection", captureProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.Background_CB_Map);
            irradianceShader.SetUniform("environmentMap", 0);

            GL.Viewport(0, 0, 32, 32);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFrameBO);
            for(int i = 0; i < 6; i++)
            {
                irradianceShader.SetUniform("view", captureViews[i]);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                    TextureTarget.TextureCubeMapPositiveX + i, _textures_cb_maps.Irradiance_CB_Map, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                RenderCube();
                 
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        // crie um mapa pre filtro de cubo
        // ---------------------------------------------
        private void PreFilter()
        {
            _textures_cb_maps.PreFilter_CB_Map = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.PreFilter_CB_Map);
            for(int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat,
                128, 128, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }

            SetParametersImage(TextureMinFilter.LinearMipmapLinear);


            prefilterShader.Use();
            prefilterShader.SetUniform("projection", captureProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.Background_CB_Map);
            irradianceShader.SetUniform("environmentMap", 0);

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
                prefilterShader.SetUniform("roughness", roughness);
                for(int i = 0; i < 6; i++)
                {
                    prefilterShader.SetUniform("view", captureViews[i]);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                                            TextureTarget.TextureCubeMapPositiveX + i, _textures_cb_maps.PreFilter_CB_Map, mip);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    RenderCube();
                }
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        private void SetParametersImage(TextureMinFilter param = TextureMinFilter.Linear)
        {
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)param);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // generate mipmaps for the cubemap so OpenGL automatically allocates the required memory.
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }
        public void RenderFrame()
        {

            backgroundShader.Use();
            backgroundShader.SetUniform("projection", Camera.ProjectionMatrix);
            backgroundShader.SetUniform("view", Camera.ViewMatrix);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _textures_cb_maps.Background_CB_Map);
            backgroundShader.SetUniform("environmentMap", 0);

            backgroundShader.SetUniform("gamma", Values.gammaBackground);


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
        public void Dispose()
        {
            equirectangularToCubemapShader.Dispose();
            backgroundShader.Dispose();

            irradianceShader.Dispose();
            prefilterShader.Dispose();

            GL.DeleteFramebuffer(captureFrameBO);
            GL.DeleteRenderbuffer(captureRenderBO);

            GL.DeleteTexture(HDR_Texture);
            GL.DeleteTexture(_textures_cb_maps.Background_CB_Map);
            GL.DeleteTexture(_textures_cb_maps.Irradiance_CB_Map);
            GL.DeleteTexture(_textures_cb_maps.PreFilter_CB_Map);

            DefaultCube.Dispose();
            CustomCube.Dispose();
        }
        private struct DefaultCube
        {
            private static VertexArrayObject ?Vao;
            private static BufferObject<float> ?vbo;
            public  static void RenderCube()
            {
                if (Vao == null)
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

                    Vao = new VertexArrayObject();
                    vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);

                    Vao.LinkBufferObject(ref vbo);
                    Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
                    Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
                    Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
                }
                // render DefaultCube
                Vao.Bind();
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            public static void Dispose()
            {
                vbo!.Dispose();
                Vao!.Dispose();
            }
        }
        private struct CustomCube
        {
            private static VertexArrayObject ?Vao;
            private static BufferObject<float> ?vbo;
            public static void RenderCube()
            {
                if (Vao == null)
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

                    Vao = new VertexArrayObject();
                    vbo = new BufferObject<float>(vertices, BufferTarget.ArrayBuffer);

                    Vao.LinkBufferObject(ref vbo);
                    Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
                    Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
                    Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

                }

                Vao.Bind();
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            public static void Dispose()
            {
                vbo!.Dispose();
                Vao!.Dispose();
            }
        }
    }
}