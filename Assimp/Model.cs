using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    public class Model : IDisposable
    {
        public AssimpModel assimpModel;
        public List<Meshe> meshes;
        private ShaderProgram ShaderPBR;
        private Dictionary<string, TextureProgram> TexturesMap = new Dictionary<string, TextureProgram>();
        private CharacterPhysic characterPhysic;
        public Model(string modelPath)
        {
            assimpModel = new AssimpModel(modelPath);
            meshes = new List<Meshe>(assimpModel.meshes);
            
            ShaderPBR = new ShaderProgram("Assimp/PBR/PBR_Shader.vert", "Assimp/PBR/PBR_Shader.frag");
            
            foreach(var index in meshes)
            {
                LoadTextures(index.DiffusePath          , PixelInternalFormat.SrgbAlpha, TextureUnit.Texture4);
                LoadTextures(index.NormalPath           , PixelInternalFormat.Rgba,      TextureUnit.Texture5);
                LoadTextures(index.LightMap             , PixelInternalFormat.Rgba,      TextureUnit.Texture6);
                LoadTextures(index.EmissivePath         , PixelInternalFormat.SrgbAlpha, TextureUnit.Texture7);
                LoadTextures(index.SpecularPath         , PixelInternalFormat.Rgba,      TextureUnit.Texture8);
                LoadTextures(index.HeightMap            , PixelInternalFormat.Rgba,      TextureUnit.Texture9);
                LoadTextures(index.MetallicPath         , PixelInternalFormat.Rgba,      TextureUnit.Texture10);
                LoadTextures(index.RoughnnesPath        , PixelInternalFormat.Rgba,      TextureUnit.Texture11);
                LoadTextures(index.AmbientOcclusionPath , PixelInternalFormat.Rgba,      TextureUnit.Texture12);
            }
            
            

            characterPhysic = new CharacterPhysic("Unitron", assimpModel.PointsForCollision.ToArray(), new Vector3(0f, 10f, 0f), 1);
            characterPhysic.RotationX_Axis = -90;


        }
        public TexturesCBMaps UseTexCubemap;
        public void RenderFrame()
        {

            ShaderPBR.Use();
            ShaderPBR.SetUniform("model", characterPhysic.Model);
            ShaderPBR.SetUniform("view", Camera.ViewMatrix);
            ShaderPBR.SetUniform("projection", Camera.ProjectionMatrix);

            ShaderPBR.SetUniform("viewPos", Camera.Position);
            ShaderPBR.SetUniform("lightPositions", Game.LuzPosition);
            ShaderPBR.SetUniform("lightColors", Values.lightColor);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, UseTexCubemap.Irradiance);
            ShaderPBR.SetUniform("irradianceMap", 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, UseTexCubemap.Background);
            ShaderPBR.SetUniform("backgroundMap", 1);

            ShaderPBR.SetUniform("gammaCubemap", Values.gammaBackground);
            ShaderPBR.SetUniform("interpolation", Values.interpolatedBack);

            ShaderPBR.SetUniform("emissiveStrength", Values.ForceLightScene);
            
            ShaderPBR.SetUniform("gamma", Values.gammaObject);
            ShaderPBR.SetUniform("luminousStrength", Values.luminousStrength);
            ShaderPBR.SetUniform("specularStrength", Values.specularStrength);


            GL.Enable(EnableCap.CullFace);
            
            foreach(var item in meshes)
            {

                ShaderPBR.SetUniform("AlbedoMap", TexturesMap[item.DiffusePath].Use);
                ShaderPBR.SetUniform("NormalMap", TexturesMap[item.NormalPath].Use);
                ShaderPBR.SetUniform("AmbienteRoughnessMetallic", TexturesMap[item.LightMap].Use);
                ShaderPBR.SetUniform("EmissiveMap", TexturesMap[item.EmissivePath].Use);


                item.RenderFrame();
            }
            GL.Disable(EnableCap.CullFace);



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
            var input = Program.window.IsKeyDown;

            
            if(input(Keys.Up))
            {
                var position = characterPhysic.Position;
                characterPhysic.Position = new Vector3d(position.X, position.Y + 3.0f, position.Z);
            }
           
        }
        public void Dispose()
        {
            for(int i = 0; i < meshes.Count; i++)
                meshes[i].Dispose();
                
            foreach(var index in TexturesMap.Keys) 
                TexturesMap[index].Dispose();

            ShaderPBR.Dispose();
        }
        private void LoadTextures(string tex_path, PixelInternalFormat pixelFormat, TextureUnit unit)
        {
            if(!TexturesMap.ContainsKey(tex_path))
            {
                if(tex_path != string.Empty)
                {
                    TextureProgram _texture_map = new TextureProgram(tex_path, pixelFormat, unit);
                    TexturesMap.Add(tex_path, _texture_map);
                }
            }

        }
    }
}