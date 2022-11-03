using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Open_GLTK
{
    struct BuffersVertex
    {
        public int vertexArrayObject, verticesBuffer, elementBuffer;
        public BuffersVertex()
        {
            vertexArrayObject = GL.GenVertexArray();
            verticesBuffer = GL.GenBuffer();
            elementBuffer = GL.GenBuffer();
        }
        public void Dispose()
        {
            GL.DeleteBuffer(verticesBuffer);
            GL.DeleteBuffer(elementBuffer);

            GL.DeleteVertexArray(vertexArrayObject);
        }
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
    
    public class Meshe
    {
        private BuffersVertex buffers;
        private int indicesCount;
        public string DiffusePath, SpecularPath, NormalPath, HeightMap, MetallicPath, RoughnnesPath, LightMap, EmissivePath, AmbientOcclusionPath;
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

            buffers = new BuffersVertex();

            GL.BindVertexArray(buffers.vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, buffers.verticesBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.ToArray().Length * sizeof(Vertex), Vertices.ToArray(), BufferUsageHint.StaticDraw);

            // positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), 0);

            // Normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Normals"));

            // Tecoods
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "TexCoords"));

            // Tangents
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tangents"));

            // Bitangents
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Bitangents"));


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffers.elementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.ToArray().Length * sizeof(ushort), Indices.ToArray(), BufferUsageHint.StaticDraw);

            Vertices.Clear();
            Indices.Clear();
        }
        public void RenderFrame()
        {
            GL.BindVertexArray(buffers.vertexArrayObject);
            GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedShort, 0);
        }
        public void Dispose()
        {
            buffers.Dispose();
        }
        public void PrintTexturesMap()
        {
            Console.WriteLine();
            Console.WriteLine($"DiffusePath          : {DiffusePath}");
            Console.WriteLine($"SpecularPath         : {SpecularPath}");
            Console.WriteLine($"NormalPath           : {NormalPath}");
            Console.WriteLine($"HeightMap            : {HeightMap}");
            Console.WriteLine($"MetallicPath         : {MetallicPath}");
            Console.WriteLine($"RoughnnesPath        : {RoughnnesPath}");
            Console.WriteLine($"LightMap             : {LightMap}");
            Console.WriteLine($"EmissivePath         : {EmissivePath}");
            Console.WriteLine($"AmbientOcclusionPath : {AmbientOcclusionPath}");
            Console.WriteLine("\n -------------------------------------------------------------- \n");
        }
    }
}