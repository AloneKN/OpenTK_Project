using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace MyGame
{
    public struct Vertex
    {
        public Vector3 Positions;
        public Vector3 Normals;
        public Vector2 TexCoords;
        public Vector3 Tangents;
        public Vector3 Bitangents;
    }
    public class ModelTexturesPath
    {
        public string _DiffusePath = string.Empty;
	    public string _SpecularPath = string.Empty;
	    public string _NormalPath = string.Empty;
	    public string _HeightPath = string.Empty;
	    public string _MetallicPath = string.Empty;
	    public string _RoughnnesPath = string.Empty;
        public string _LightMap = string.Empty;
	    public string _EmissivePath = string.Empty;
        public string _AmbientOcclusionPath = string.Empty;
    }
    public class Meshe : IDisposable
    {
        // private BuffersVertex buffers;
        private int indicesCount;
        public string DiffusePath, SpecularPath, NormalPath, HeightMap, MetallicPath, RoughnnesPath, LightMap, EmissivePath, AmbientOcclusionPath;
        
        private VertexArrayObject Vao;
        private BufferObject<Vertex> Vbo;
        private BufferObject<ushort> Ebo;
        public unsafe Meshe(List<Vertex> Vertices, List<ushort> Indices, ModelTexturesPath texturesPath)
        {

            indicesCount = Indices.Count;


            DiffusePath             =   texturesPath._DiffusePath;
	        SpecularPath            =   texturesPath._SpecularPath;
	        NormalPath              =   texturesPath._NormalPath;
            HeightMap               =   texturesPath._HeightPath;
	        MetallicPath            =   texturesPath._MetallicPath;
	        RoughnnesPath           =   texturesPath._RoughnnesPath;
	        LightMap                =   texturesPath._LightMap;
	        EmissivePath            =   texturesPath._EmissivePath;
            AmbientOcclusionPath    =   texturesPath._AmbientOcclusionPath;

            Vao = new VertexArrayObject();
            Vbo = new BufferObject<Vertex>(Vertices.ToArray(), BufferTarget.ArrayBuffer);
            Vao.LinkBufferObject(ref Vbo);

            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, sizeof(Vertex), IntPtr.Zero);
            Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Normals"));
            Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "TexCoords"));
            Vao.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tangents"));
            Vao.VertexAttributePointer(4, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Bitangents"));

            Ebo = new BufferObject<ushort>(Indices.ToArray(), BufferTarget.ElementArrayBuffer);
            Vao.LinkBufferObject(ref Ebo);

            Vertices.Clear();
            Indices.Clear();

        }
        public void RenderFrame()
        {
            Vao.Bind();

            GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedShort, 0);
        }
        public void Dispose() => Vao.Dispose();
        public void PrintTexturesMap()
        {
            Console.WriteLine("Meshes Maps Contains...");
            _Print("DiffusePath", DiffusePath);
            _Print("SpecularPath", SpecularPath);
            _Print("NormalPath", NormalPath);
            _Print("HeightMap", HeightMap);
            _Print("MetallicPath", MetallicPath);
            _Print("RoughnnesPath", RoughnnesPath);
            _Print("LightMap", LightMap);
            _Print("EmissivePath", EmissivePath);
            _Print("AmbientOcclusionPath", AmbientOcclusionPath);
            Console.WriteLine("\n -------------------------------------------------------------- \n");
        }
        private void _Print(string TypeTexture, string pathTex)
        {
            if(pathTex != string.Empty)
                Console.WriteLine($"{TypeTexture} : {pathTex}");
        }
    }
}