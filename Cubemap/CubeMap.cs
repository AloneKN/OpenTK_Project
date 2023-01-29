using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace MyGame
{
    public enum CubeMapType
    {
        /// <summary>
        /// Texture Format square: 「 」
        /// </summary>
        Type0,

        /// <summary>
        /// Texture Format texture faces: X+ X- | Y+ Y- | Z+ Z-
        /// </summary>
        Type1,

        /// <summary>
        /// Texture format cross: -|--
        /// </summary>
        Type2,


        /// <summary>
        /// Texture Format: T
        /// </summary>
        Type3,


    }
    
    public class Faces
    {
        public string PathFaces = string.Empty;
        public List<string> Textures = new List<string>();
    }
    public struct TexturesCBMaps
    {
        public int Background;
        public int Irradiance;
        public int PreFilter;
    }
    public class CubeMap
    {
        public struct Handler
        {
            public int HDR_Texture;
            public int captureFrameBO;
            public int captureRenderBO;
            public int size;
            public PixelInternalFormat internalFormat;
            public CubeMapType type;
        }
        private static Matrix4 captureProjection = Matrix4.CreatePerspectiveFieldOfView((float)OpenTK.Mathematics.MathHelper.DegreesToRadians(90.0), 1.0f, 0.1f, 10.0f);
        private static Matrix4[] captureViews =
        {
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 1.0f,  0.0f,  0.0f),  new  Vector3(0.0f, -1.0f,  0.0f)), 
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f,  0.0f,  0.0f),  new  Vector3(0.0f, -1.0f,  0.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  1.0f,  0.0f),  new  Vector3(0.0f,  0.0f,  1.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f, -1.0f,  0.0f),  new  Vector3(0.0f,  0.0f, -1.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  0.0f,  1.0f),  new  Vector3(0.0f, -1.0f,  0.0f)),
            Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3( 0.0f,  0.0f, -1.0f),  new  Vector3(0.0f, -1.0f,  0.0f))
        };
        private ShaderProgram shaderRender = new ShaderProgram("Cubemap/shaders/renderFinal.vert", "Cubemap/shaders/renderFinal.frag");
        public TexturesCBMaps UseTextures = new TexturesCBMaps();
        public Handler handler;

        public CubeMap(Faces faces)
        {

            handler = new Handler()
            {
                size = 1920,
                internalFormat = PixelInternalFormat.Rgba32f,
                type = CubeMapType.Type1,
            };

            LoadFaces loadFaces = new LoadFaces(ref faces, ref handler, ref UseTextures);
            IrradianceMap irradianceMap = new IrradianceMap(ref handler, ref UseTextures);
            PreFilterMap preFilterMap = new PreFilterMap(ref handler, ref UseTextures, ref irradianceMap.shaderIrradiance);

            irradianceMap.Dispose();
            preFilterMap.Dispose();

            GL.DeleteFramebuffer(handler.captureFrameBO);
            GL.DeleteRenderbuffer(handler.captureRenderBO);

            GL.DeleteTexture(handler.HDR_Texture);
        }
        public CubeMap(string path, CubeMapType Type)
        {

            if(!File.Exists(path)) 
                throw new Exception($"Não foi possivel encontrar a Textura HDR: {path}");


            handler = new Handler()
            {
                size = 1920,
                internalFormat = PixelInternalFormat.Rgba32f,
                type = Type,
            };

            UseTextures = new TexturesCBMaps();

            LoadRectangularTexture rectangularTexture = new LoadRectangularTexture(path, ref handler);
            RetangularToCubemap retangularToCubemap = new RetangularToCubemap(ref handler, ref UseTextures);
            IrradianceMap irradianceMap = new IrradianceMap(ref handler, ref UseTextures);
            PreFilterMap preFilterMap = new PreFilterMap(ref handler, ref UseTextures, ref irradianceMap.shaderIrradiance);

            retangularToCubemap.Dispose();
            irradianceMap.Dispose();
            preFilterMap.Dispose();

            GL.DeleteFramebuffer(handler.captureFrameBO);
            GL.DeleteRenderbuffer(handler.captureRenderBO);

            GL.DeleteTexture(handler.HDR_Texture);
        }
        public void RenderFrame()
        {

            shaderRender.Use();
            shaderRender.SetUniform("projection", Camera.ProjectionMatrix);
            shaderRender.SetUniform("view", Camera.ViewMatrix);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, UseTextures.Background);
            shaderRender.SetUniform("environmentMap", 0);

            shaderRender.SetUniform("gamma", Values.gammaBackground);
            shaderRender.SetUniform("interpolation", Values.interpolatedBack);


            GL.Disable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Lequal);
            RenderCube(handler.type);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
        }
        private static void RenderCube(CubeMapType type)
        {
            if(type == CubeMapType.Type0 | type == CubeMapType.Type1)
            {
                CubemapDefault.RenderCube();
            }
            else if(type == CubeMapType.Type2)
            {
                CubeMapCross.RenderCube();
            }
            else if(type == CubeMapType.Type3)
            {
                CubemapT.RenderCube();
            }
        }
        public void Dispose()
        {
            shaderRender.Dispose();

           
            GL.DeleteTexture(UseTextures.Background);
            GL.DeleteTexture(UseTextures.Irradiance);
            GL.DeleteTexture(UseTextures.PreFilter);

            CubemapDefault.Dispose();
            CubemapT.Dispose();
            CubeMapCross.Dispose();
        }

        public struct LoadFaces
        {
            public LoadFaces(ref Faces faces, ref Handler handler, ref TexturesCBMaps useTextures)
            {
                handler.captureFrameBO = GL.GenFramebuffer();
                handler.captureRenderBO = GL.GenRenderbuffer();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handler.captureRenderBO);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, handler.size, handler.size);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, handler.captureRenderBO);


                useTextures.Background = GL.GenTexture();
                GL.BindTexture(TextureTarget.TextureCubeMap, useTextures.Background);

                StbImage.stbi_set_flip_vertically_on_load(1);

                for(int i = 0; i < 6; i++)
                {
                    var path = Path.Combine(faces.PathFaces, faces.Textures[i]);

                    if(!File.Exists(path)) 
                        throw new Exception($"Não foi possivel encontrar a Textura: {path}");

                    using(Stream stream = File.OpenRead(path))
                    {
                        var image = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);

                        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, handler.internalFormat,
                        image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Float, image.Data);
                    }
                }

                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            }
        }
        public struct LoadRectangularTexture
        {
            public LoadRectangularTexture(string path, ref Handler handler)
            {
                handler.captureFrameBO = GL.GenFramebuffer();
                handler.captureRenderBO = GL.GenRenderbuffer();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handler.captureRenderBO);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, handler.size, handler.size);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, handler.captureRenderBO);


                handler.HDR_Texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, handler.HDR_Texture);

                StbImage.stbi_set_flip_vertically_on_load(1);
                using(Stream stream = File.OpenRead(path))
                {
                    ImageResultFloat image = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, handler.internalFormat,
                        image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Float, image.Data);
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                
            }
        }
        public struct RetangularToCubemap
        {
            public ShaderProgram shader;
            public void Dispose() => shader.Dispose();
            public RetangularToCubemap(ref Handler handler, ref TexturesCBMaps CBMapsUse)
            {
                shader = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/rectangular_to_cubemap.frag");

                CBMapsUse.Background = GL.GenTexture();

                GL.BindTexture(TextureTarget.TextureCubeMap, CBMapsUse.Background);

                for(int i = 0; i < 6; i++)
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, handler.internalFormat,
                    handler.size, handler.size, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                }

                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

                
                shader.Use();
                shader.SetUniform("projection", captureProjection);
                shader.SetUniform("UseTexCoord", (int)handler.type);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, handler.HDR_Texture);
                shader.SetUniform("equirectangularMap", 0);

                GL.Viewport(0, 0, handler.size, handler.size);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);

                for(int i = 0; i < 6; i++)
                {
                    shader.SetUniform("view", captureViews[i]);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                        TextureTarget.TextureCubeMapPositiveX + i, CBMapsUse.Background, 0);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    
                    RenderCube(handler.type);
                }
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
        public struct IrradianceMap
        {
            public ShaderProgram shaderIrradiance;
            public void Dispose() => shaderIrradiance.Dispose();
            public IrradianceMap(ref Handler handler, ref TexturesCBMaps texturesCBMaps)
            {
                shaderIrradiance = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/irradiance.frag");

                texturesCBMaps.Irradiance = GL.GenTexture();
                GL.BindTexture(TextureTarget.TextureCubeMap, texturesCBMaps.Irradiance);

                for(int i = 0; i < 6; i++)
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, handler.internalFormat,
                    32, 32, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                }
                
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handler.captureRenderBO);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, 32, 32);

                shaderIrradiance.Use();
                shaderIrradiance.SetUniform("projection", captureProjection);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.TextureCubeMap, texturesCBMaps.Background);
                shaderIrradiance.SetUniform("environmentMap", 0);

                GL.Viewport(0, 0, 32, 32);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);
                for(int i = 0; i < 6; i++)
                {
                    shaderIrradiance.SetUniform("view", captureViews[i]);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                        TextureTarget.TextureCubeMapPositiveX + i, texturesCBMaps.Irradiance, 0);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    RenderCube(handler.type);
                    
                }
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
        public struct PreFilterMap
        {
            public ShaderProgram prefilterShader;
            public void Dispose() => prefilterShader.Dispose();
            public PreFilterMap(ref Handler handler, ref TexturesCBMaps texturesCBMaps, ref ShaderProgram shaderIrradiance)
            {
                prefilterShader = new ShaderProgram("Cubemap/shaders/cubemap.vert", "Cubemap/shaders/prefilter.frag");
                
                texturesCBMaps.PreFilter = GL.GenTexture();
                GL.BindTexture(TextureTarget.TextureCubeMap, texturesCBMaps.PreFilter);
                for(int i = 0; i < 6; i++)
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, handler.internalFormat,
                    128, 128, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                }


                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);


                prefilterShader.Use();
                prefilterShader.SetUniform("projection", captureProjection);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.TextureCubeMap, texturesCBMaps.Background);
                shaderIrradiance.SetUniform("environmentMap", 0);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handler.captureFrameBO);
                int maxMipLevels = 5;
                for(int mip = 0; mip < maxMipLevels; mip++)
                {
                    int mipWidth = (int)(128 * OpenTK.Mathematics.MathHelper.Pow(0.5, mip));
                    int mipHeight = (int)(128 * OpenTK.Mathematics.MathHelper.Pow(0.5, mip));

                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handler.captureRenderBO);
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, mipWidth, mipHeight);
                    GL.Viewport(0, 0, mipWidth, mipHeight);

                    float roughness = (float)mip / (float)(maxMipLevels - 1);
                    prefilterShader.SetUniform("roughness", roughness);
                    for(int i = 0; i < 6; i++)
                    {
                        prefilterShader.SetUniform("view", captureViews[i]);
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                                                TextureTarget.TextureCubeMapPositiveX + i, texturesCBMaps.PreFilter, mip);

                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                        RenderCube(handler.type);
                    }
                }
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
    }
}