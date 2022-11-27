using OpenTK.Mathematics;
using Assimp;

namespace MyGame
{
    public class ModelProgram
    {
        private static Scene scene = new Scene();

        private static List<Meshe> ?malhas;

        private static List<Vertex> vertices = new List<Vertex>();
        private static List<ushort> indices = new List<ushort>();
        private static ModelTexturesPath texturesPath = new ModelTexturesPath();

        private static string PathModel = string.Empty;
        public static List<Meshe> Load(string filePath, bool FlipUVs = false)
        {
            if(!File.Exists(filePath))
                throw new Exception($"Arquivo nao encontrado: {filePath}..");

            PathModel = Path.GetDirectoryName(filePath)!;

            AssimpContext importer = new AssimpContext();

            if(FlipUVs)
            {
                scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            } else
            {
                scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);
            }
            malhas = new List<Meshe>();
            processNodes(scene.RootNode);
            
            return malhas;
        }
        private static void processNodes(Node node)
        {
            for(int i = 0; i < node.MeshCount; i++)
            {
                processMesh(scene.Meshes[node.MeshIndices[i]]);
                malhas!.Add(new Meshe(vertices, indices, texturesPath));
            }
            for(int i = 0; i < node.ChildCount; i++)
            {
                processNodes(node.Children[i]);
            }
        }
        private static void processMesh(Mesh mesh)
        {
            vertices = new List<Vertex>();
            for(int i = 0; i < mesh.VertexCount; i++)
            {
                var packed = new Vertex();

                packed.Positions = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                packed.Normals = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                if(mesh.HasTextureCoords(0))
                {
                    packed.TexCoords = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
                } else
                {
                    packed.TexCoords = new Vector2(0.0f, 0.0f);
                }
                packed.Tangents = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                packed.Bitangents = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);

                vertices.Add(packed);
            }
            indices = new List<ushort>();
            for(int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for(int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((ushort)face.Indices[j]);
                }
            }
            // textures
            texturesPath = new ModelTexturesPath();
            
            if(mesh.MaterialIndex >= 0)
            {
                // Texturas
                Material material = scene.Materials[mesh.MaterialIndex];
                ProcessTextures(material.GetAllMaterialTextures());

            }
        }
        private static void ProcessTextures(TextureSlot []slot)
        {
            for(int i = 0; i < slot.Length; i++)
            {
                if(slot[i].TextureType == TextureType.Diffuse)
                {
                    texturesPath._DiffusePath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Specular)
                {
                    texturesPath._SpecularPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Normals)
                {
                    texturesPath._NormalPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Height)
                {
                    texturesPath._HeightPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Metalness)
                {
                    texturesPath._MetallicPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Roughness)
                {
                    texturesPath._RoughnnesPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Lightmap)
                {
                    texturesPath._LightMap = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.Emissive)
                {
                    texturesPath._EmissivePath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
                else if(slot[i].TextureType == TextureType.AmbientOcclusion)
                {
                    texturesPath._AmbientOcclusionPath = new string(Path.Combine(PathModel, slot[i].FilePath));
                }
            }

        }
    }
}
// PathModel = new String(Path.GetDirectoryName(filePath));

// if(mesh.MaterialIndex >= 0)
// {
//     Material material = scene.Materials[mesh.MaterialIndex];

//         Console.WriteLine("Material de texturas...");
//         Console.WriteLine($"Material texture occlusion    : {material.HasTextureAmbientOcclusion}");
//         Console.WriteLine($"Material texture Ambiente     : {material.HasTextureAmbient}");
//         Console.WriteLine($"Material texture diffuse      : {material.HasTextureDiffuse}");
//         Console.WriteLine($"Material texture displacement : {material.HasTextureDisplacement}");
//         Console.WriteLine($"Material texture emissive     : {material.HasTextureEmissive}");
//         Console.WriteLine($"Material texture height       : {material.HasTextureHeight}");
//         Console.WriteLine($"Material texture lightmap     : {material.HasTextureLightMap}");
//         Console.WriteLine($"Material texture normal       : {material.HasTextureNormal}");
//         Console.WriteLine($"Material texture opacity      : {material.HasTextureOpacity}");
//         Console.WriteLine($"Material texture reflection   : {material.HasTextureReflection}");
//         Console.WriteLine($"Material texture specular     : {material.HasTextureSpecular} \n");

//         Console.WriteLine("Material Colors...");
//         Console.WriteLine($"Material color Ambinete      : {material.HasColorAmbient}");
//         Console.WriteLine($"Material color diffuse       : {material.HasColorDiffuse}");
//         Console.WriteLine($"Material color Emissive      : {material.HasColorEmissive}");
//         Console.WriteLine($"Material color Reflective    : {material.HasColorReflective}");
//         Console.WriteLine($"Material color Specular      : {material.HasColorSpecular}");
//         Console.WriteLine($"Material color Trasnparent   : {material.HasColorTransparent} \n");
        
//         Console.WriteLine("Valores FLoats...");
//         Console.WriteLine($"Material opacity            : {material.HasOpacity}");
//         Console.WriteLine($"Material reflective         : {material.HasReflectivity}");
//         Console.WriteLine($"Material shininnes          : {material.HasShininess}");
//         Console.WriteLine($"Material shininnes strengh  : {material.HasShininessStrength}");
//         Console.WriteLine($"Material transFactor        : {material.HasTransparencyFactor}");
//         Console.WriteLine($"Material bumpScalling       : {material.HasBumpScaling}\n");

//         Console.WriteLine("outers...");
//         Console.WriteLine($"Material Shaders            : {material.HasShaders}");
//         Console.WriteLine($"Material bledMode           : {material.HasBlendMode}");
//         Console.WriteLine($"Material ShadinMode         : {material.HasShadingMode}");
//         Console.WriteLine($"Material wireFrame          : {material.HasWireFrame}\n");

//     // Texturas
//     if(material.HasTextureAmbient)
//         ProcessTextures(material.TextureAmbient, TextureType.Ambient);

//     if(material.HasTextureAmbientOcclusion)
//         ProcessTextures(material.TextureAmbientOcclusion, TextureType.AmbientOcclusion);

//     if(material.HasTextureDiffuse)
//         ProcessTextures(material.TextureDiffuse, TextureType.Diffuse);

//     if(material.HasTextureDisplacement)
//         ProcessTextures(material.TextureDisplacement, TextureType.Displacement);

//     if(material.HasTextureEmissive)
//         ProcessTextures(material.TextureEmissive, TextureType.Emissive);

//     if(material.HasTextureHeight)
//         ProcessTextures(material.TextureHeight, TextureType.Height);

//     if(material.HasTextureLightMap)
//         ProcessTextures(material.TextureLightMap, TextureType.Lightmap);

//     if(material.HasTextureNormal)
//         ProcessTextures(material.TextureNormal, TextureType.Normals);

//     if(material.HasTextureOpacity)
//         ProcessTextures(material.TextureOpacity, TextureType.Opacity);

//     if(material.HasTextureReflection)
//         ProcessTextures(material.TextureReflection, TextureType.Reflection);

//     if(material.HasTextureSpecular)
//         ProcessTextures(material.TextureSpecular, TextureType.Specular);

//     // colors
//     if(material.HasColorAmbient)
//         ProcessColor(material.ColorAmbient, ColorType.ColorAmbient);
        
//     if(material.HasColorDiffuse)
//         ProcessColor(material.ColorDiffuse, ColorType.ColorAmbient);

//     if(material.HasColorEmissive)
//         ProcessColor(material.ColorEmissive, ColorType.ColorEmissive);

//     if(material.HasColorReflective)
//         ProcessColor(material.ColorReflective, ColorType.ColorReflective);

//     if(material.HasColorSpecular)
//         ProcessColor(material.ColorSpecular, ColorType.ColorSpecular);

//     if(material.HasColorTransparent)
//         ProcessColor(material.ColorTransparent, ColorType.ColorTransparent);

//     // floats
//     if(material.HasOpacity)
//         ProcessValues(material.Opacity, ValueType.Opacity);

//     if(material.HasReflectivity)
//         ProcessValues(material.Reflectivity, ValueType.Reflectivity);

//     if(material.HasShininess)
//         ProcessValues(material.Shininess, ValueType.Shininess);

//     if(material.HasShininessStrength)
//         ProcessValues(material.ShininessStrength, ValueType.ShininessStrength);

//     if(material.HasTransparencyFactor)
//         ProcessValues(material.TransparencyFactor, ValueType.TransparencyFactor);



//     Console.WriteLine("-------------------------------------------------------------------------------------------------\n \n");
    

// private static void ProcessTextures(TextureSlot slot, TextureType type)
//     {
//         Console.WriteLine($"Caminho da textura: {slot.FilePath} tipo {type.ToString()}");
//     }
//     private static void ProcessColor(Color4D color, ColorType type)
//     {
//         Console.WriteLine($"Color: {color} tipo: {type.ToString()}");
//     }
//     private static void ProcessValues(float value, ValueType type)
//     {
//         Console.WriteLine($" Valores float: {value} tipo: {type.ToString()}");
//     }

// public enum ColorType
//     {
//         ColorAmbient = 1,
//         ColorDiffuse = 2,
//         ColorEmissive = 3,
//         ColorReflective = 4,
//         ColorSpecular = 5,
//         ColorTransparent = 6
//     }
//     public enum ValueType
//     {
//         Opacity = 1,
//         Reflectivity = 2,
//         Shininess = 3,
//         ShininessStrength = 4,
//         TransparencyFactor = 5
//     }