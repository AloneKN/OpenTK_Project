using OpenTK.Mathematics;
using Assimp;
using OpenTK.Graphics.OpenGL4;

namespace MyGame
{
    public class AssimpModel : IDisposable
    {
        private Scene scene;
        public List<Meshe> meshes { get; }
        public Meshe FirstMeshe => meshes[0];
        public List<double> PointsForCollision { get; }
        private string PathModel = string.Empty;

        public AssimpModel(string FilePath, bool FlipUVs = false)
        {
            if(!File.Exists(FilePath))
                throw new Exception($"ERROR::ASSIMP:: Arquivo nao encontrado: {FilePath}..");

            PathModel = Path.GetDirectoryName(FilePath)!;

            scene = new Scene();
            meshes = new List<Meshe>();
            PointsForCollision = new List<double>();

            using(var importer = new AssimpContext())
            {

                if(FlipUVs)
                    scene = importer.ImportFile(FilePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
                else
                    scene = importer.ImportFile(FilePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);
                    
            }

            processNodes(scene.RootNode);
        }
        private void processNodes(Node node)
        {
            for(int i = 0; i < node.MeshCount; i++)
            {
                ProcessMesh(scene.Meshes[node.MeshIndices[i]]);
                
            }
            for(int i = 0; i < node.ChildCount; i++)
            {
                processNodes(node.Children[i]);
            }
        }
        private void ProcessMesh(Mesh mesh)
        {
            var vertices = new List<Vertex>();
            var indices = new List<ushort>();

            for(int i = 0; i < mesh.VertexCount; i++)
            {
                var packed = new Vertex();

                PointsForCollision.Add(mesh.Vertices[i].X);
                PointsForCollision.Add(mesh.Vertices[i].Y);
                PointsForCollision.Add(mesh.Vertices[i].Z);

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

            for(int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for(int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((ushort)face.Indices[j]);
                }
            }
            
            ModelTexturesPath texturesPath = new ModelTexturesPath();

            if(mesh.MaterialIndex >= 0)
            {
                // Texturas
                Material material = scene.Materials[mesh.MaterialIndex];
                texturesPath = ProcessTextures(material.GetAllMaterialTextures());
            }

            meshes.Add( new Meshe(vertices, indices, texturesPath));

        }
        private ModelTexturesPath ProcessTextures(TextureSlot []slot)
        {
            ModelTexturesPath texturesPath = new ModelTexturesPath();

            foreach(var item in slot)           
            {
                if(item.FilePath != null)
                {
                    if(item.TextureType == TextureType.Diffuse)
                    {
                        texturesPath._DiffusePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Specular)
                    {
                        texturesPath._SpecularPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Normals)
                    {
                        texturesPath._NormalPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Height)
                    {
                        texturesPath._HeightPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Metalness)
                    {
                        texturesPath._MetallicPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Roughness)
                    {
                        texturesPath._RoughnnesPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Lightmap)
                    {
                        texturesPath._LightMap = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.Emissive)
                    {
                        texturesPath._EmissivePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if(item.TextureType == TextureType.AmbientOcclusion)
                    {
                        texturesPath._AmbientOcclusionPath = new string(Path.Combine(PathModel, item.FilePath));
                    }

                }
            }

            return texturesPath;
        }
        private void ProcessColors(Material material)
        {
            // colors
            if(material.HasColorAmbient)
            {
                Console.WriteLine(material.ColorAmbient);
            }
            if(material.HasColorDiffuse)
            {
                Console.WriteLine(material.ColorDiffuse);
            }
            if(material.HasColorEmissive)
            {
                Console.WriteLine(material.ColorEmissive);
            }
            if(material.HasColorReflective)
            {
                Console.WriteLine(material.ColorReflective);
            }
            if(material.HasColorSpecular)
            {
                Console.WriteLine(material.ColorSpecular);
            }
            if(material.HasColorTransparent)
            {
                Console.WriteLine(material.ColorTransparent);
            }
        }
        private static void ProcessValues(Material material)
        {
            // floats
            if(material.HasOpacity)
            {
                Console.WriteLine(material.Opacity);
            }
            if(material.HasTransparencyFactor)
            {
                Console.WriteLine(material.TransparencyFactor);
            }
            if(material.HasBumpScaling)
            {
                Console.WriteLine(material.BumpScaling);
            }
            if(material.HasShininess)
            {
                Console.WriteLine(material.Shininess);
            }
            if(material.HasShininessStrength)
            {
                Console.WriteLine(material.ShininessStrength);
            }
        }
        public void Dispose()
        {
            scene.Clear();
        }
    }
}