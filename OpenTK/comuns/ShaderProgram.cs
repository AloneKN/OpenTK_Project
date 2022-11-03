using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Open_GLTK
{
    public class ShaderProgram
    {
        private readonly int Handle;
        private readonly Dictionary<string, int> uniformLocations;
        public ShaderProgram(string vertFile, string fragFile, bool isString = false)
        {

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            if(isString)
            {
                GL.ShaderSource(vertexShader, vertFile);
                GL.ShaderSource(fragmentShader, fragFile);
            }
            else
            {
                GL.ShaderSource(vertexShader, File.ReadAllText(vertFile));
                GL.ShaderSource(fragmentShader, File.ReadAllText(fragFile));
            }

            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            // Desanexar os shaders
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);


            // Em seguida, aloque o dicionário para armazenar os locais.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            
            uniformLocations = new Dictionary<string, int>();

            // Faz um loop em todos os uniformes,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                var location = GL.GetUniformLocation(Handle, key);

                uniformLocations.Add(key, location);
            }
        }
        private void CompileShader(int shader)
        {
            // Tenta compilar o shader
            GL.CompileShader(shader);

            // Verifica se há erros de compilação
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            
            if (code != (int)All.True)
            {
                throw new Exception($"Ocorreu um erro ao compilar o programa.{GL.GetShaderInfoLog(shader)}");
            }
        }
        private void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Ocorreu um erro ao vincular o programa ({program})");
            }
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        public void SetTexture(string nome, int dados)
        {
            GL.Uniform1(uniformLocations[nome], dados);
        }
        public void SetFloat(string nome, float dados)
        {
            GL.Uniform1(uniformLocations[nome], dados);
        }
        public void SetMatrix4(string nome, Matrix4 dados)
        {
            GL.UniformMatrix4(uniformLocations[nome], true, ref dados);
        }
        public void SetVector2(string nome, Vector2 dados)
        {
            GL.Uniform2(uniformLocations[nome], dados);
        }
        public void SetVector3(string nome, Vector3 dados)
        {
            GL.Uniform3(uniformLocations[nome], dados);
        }
        public void SetVector4(string nome, Vector4 dados)
        {
            GL.Uniform4(uniformLocations[nome], dados);
        }
        public void SetColor3(string nome, Color4 dados)
        {
            GL.Uniform3(uniformLocations[nome], new Vector3(dados.R, dados.G, dados.B));
        }
        public void SetColor4(string nome, Color4 dados)
        {
            GL.Uniform4(uniformLocations[nome], dados);
        }
        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}