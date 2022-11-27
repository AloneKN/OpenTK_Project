using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    public class Models : IDisposable
    {
        private List<Meshe> meshes;
        private ShaderProgram ShaderPBR;
        private Dictionary<string, TextureProgram> TexturesMap = new Dictionary<string, TextureProgram>(); 
        public Models(string model3D)
        {
            this.meshes = new List<Meshe>(ModelProgram.Load(model3D));
            ShaderPBR = new ShaderProgram("GLSL/PBR/PBR_Shader.vert", "GLSL/PBR/PBR_Shader.frag");
            
            foreach(var index in meshes)
            {
                LoadTextures(index.DiffusePath         , PixelInternalFormat.SrgbAlpha);
                LoadTextures(index.SpecularPath        , PixelInternalFormat.Rgba);
                LoadTextures(index.NormalPath          , PixelInternalFormat.Rgba);
                LoadTextures(index.HeightMap           , PixelInternalFormat.Rgba);
                LoadTextures(index.MetallicPath        , PixelInternalFormat.Rgba);
                LoadTextures(index.RoughnnesPath       , PixelInternalFormat.Rgba);
                LoadTextures(index.LightMap            , PixelInternalFormat.Rgba);
                LoadTextures(index.EmissivePath        , PixelInternalFormat.SrgbAlpha);
                LoadTextures(index.AmbientOcclusionPath, PixelInternalFormat.Rgba);
            }
        }
        public Matrix4 model { get; private set; }
        public void RenderFrame()
        {
            model = Matrix4.Identity;
            model = model * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(-90.0));
            model = model * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(Clock.Time * 5.0));
            
            ShaderPBR.Use();
            ShaderPBR.SetMatrix4("model", model);
            ShaderPBR.SetMatrix4("view", Camera.ViewMatrix);
            ShaderPBR.SetMatrix4("projection", Camera.ProjectionMatrix);

            ShaderPBR.SetVector3("viewPos", Camera.Position);
            ShaderPBR.SetVector3("lightPositions", Game.LuzPosition);
            ShaderPBR.SetColor4("lightColors", Values.lightColor);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Irradiance_CB_Map);
            ShaderPBR.SetTexture("irradianceMap", 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, TexturesCBMaps.Background_CB_Map);
            ShaderPBR.SetTexture("prefilterMap", 1);

            ShaderPBR.SetFloat("gamma", Values.gammaObject);
            ShaderPBR.SetFloat("luminousStrength", Values.luminousStrength);
            ShaderPBR.SetFloat("specularStrength", Values.specularStrength);
            ShaderPBR.SetFloat("emissiveStrength", Values.emissiveStrength);

            
            for(int i = 0; i < meshes.Count; i++)
            {
                TexturesMap[meshes[i].DiffusePath].Use(TextureUnit.Texture2);
                ShaderPBR.SetTexture("AlbedoMap", 2);

                TexturesMap[meshes[i].NormalPath].Use(TextureUnit.Texture3);
                ShaderPBR.SetTexture("NormalMap", 3);

                TexturesMap[meshes[i].LightMap].Use(TextureUnit.Texture4);
                ShaderPBR.SetTexture("AmbienteRoughnessMetallic", 4);

                TexturesMap[meshes[i].EmissivePath].Use(TextureUnit.Texture5);
                ShaderPBR.SetTexture("EmissiveMap", 5);

                meshes[i].RenderFrame();
            }
        }
        public void RenderForStencil()
        {
            foreach(var item in meshes)
            {
                item.RenderFrame();
            }
        }
        public void UpdateFrame()
        {
        }
        public void Dispose()
        {
            for(int i = 0; i < meshes.Count; i++)
                meshes[i].Dispose();
                
            foreach(var index in TexturesMap.Keys) 
                TexturesMap[index].Dispose();

            ShaderPBR.Dispose();
        }
        private void LoadTextures(string tex_path, PixelInternalFormat pixelFormat)
        {
            if(!TexturesMap.ContainsKey(tex_path))
            {
                if(tex_path != string.Empty)
                {
                    TextureProgram _texture_map = TextureProgram.Load(tex_path, pixelFormat);
                    TexturesMap.Add(tex_path, _texture_map);
                }
            }

        }
    }
}