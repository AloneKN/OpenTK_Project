using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace MyGame
{
    public class Model : IDisposable
    {
        public List<Meshe> meshes;
        private ShaderProgram ShaderPBR;
        private Dictionary<string, TextureProgram> TexturesMap = new Dictionary<string, TextureProgram>(); 
  
        public Model(List<Meshe> meshes)
        {
            // this.meshes = new List<Meshe>(meshes);
            this.meshes = new List<Meshe>(meshes);
            
            ShaderPBR = new ShaderProgram("GLSL/PBR/PBR_Shader.vert", "GLSL/PBR/PBR_Shader.frag");
            
            foreach(var index in meshes)
            {
                LoadTextures(index.DiffusePath          , PixelInternalFormat.SrgbAlpha);
                LoadTextures(index.SpecularPath         , PixelInternalFormat.Rgba);
                LoadTextures(index.NormalPath           , PixelInternalFormat.Rgba);
                LoadTextures(index.HeightMap            , PixelInternalFormat.Rgba);
                LoadTextures(index.MetallicPath         , PixelInternalFormat.Rgba);
                LoadTextures(index.RoughnnesPath        , PixelInternalFormat.Rgba);
                LoadTextures(index.LightMap             , PixelInternalFormat.Rgba);
                LoadTextures(index.EmissivePath         , PixelInternalFormat.SrgbAlpha);
                LoadTextures(index.AmbientOcclusionPath , PixelInternalFormat.Rgba);
            }
        }
        public Matrix4 modelMatrix;
        public TexturesCBMaps texturesCubemaps;
        private float rotate = 0;
        public void RenderFrame()
        {
            modelMatrix = Matrix4.Identity;
            modelMatrix = modelMatrix * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(-90.0));
            if(Values.rotate == true)
            {
                rotate += 5.0f * Clock.ElapsedTime;
            }
            modelMatrix = modelMatrix * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(rotate));
            
            ShaderPBR.Use();
            ShaderPBR.SetUniform("model", modelMatrix);
            ShaderPBR.SetUniform("view", Camera.ViewMatrix);
            ShaderPBR.SetUniform("projection", Camera.ProjectionMatrix);

            ShaderPBR.SetUniform("viewPos", Camera.Position);
            ShaderPBR.SetUniform("lightPositions", Game.LuzPosition);
            ShaderPBR.SetUniform("lightColors", Values.lightColor);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texturesCubemaps.Irradiance_CB_Map);
            ShaderPBR.SetUniform("irradianceMap", 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, texturesCubemaps.Background_CB_Map);
            ShaderPBR.SetUniform("prefilterMap", 1);

            ShaderPBR.SetUniform("gamma", Values.gammaObject);
            ShaderPBR.SetUniform("luminousStrength", Values.luminousStrength);
            ShaderPBR.SetUniform("specularStrength", Values.specularStrength);
            ShaderPBR.SetUniform("emissiveStrength", Values.LightDiffuse);

            
            foreach(var item in meshes)
            {
                TexturesMap[item.DiffusePath].Use(TextureUnit.Texture2);
                ShaderPBR.SetUniform("AlbedoMap", 2);

                TexturesMap[item.NormalPath].Use(TextureUnit.Texture3);
                ShaderPBR.SetUniform("NormalMap", 3);

                TexturesMap[item.LightMap].Use(TextureUnit.Texture4);
                ShaderPBR.SetUniform("AmbienteRoughnessMetallic", 4);

                TexturesMap[item.EmissivePath].Use(TextureUnit.Texture5);
                ShaderPBR.SetUniform("EmissiveMap", 5);

                item.RenderFrame();
            }
        }
        public void RenderForStencil()
        {
            if(Stencil.RenderStencil)
            {
                foreach(var item in meshes)
                {
                    item.RenderFrame();
                }

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