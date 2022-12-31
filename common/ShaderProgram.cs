using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    public class ShaderProgram : IDisposable
    {
        private readonly int Handle;
        private readonly Dictionary<string, int> uniformLocations;
        private string CurrentShader = string.Empty;
        public ShaderProgram(string vertFile, string fragFile, string geomFile = "")
        {
            CurrentShader = $"{vertFile} {fragFile} {geomFile}";

            Handle = GL.CreateProgram();

            int vertShader = CreateShader(vertFile, ShaderType.VertexShader);
            int fragShader = CreateShader(fragFile, ShaderType.FragmentShader);

            GL.AttachShader(Handle, vertShader);
            GL.AttachShader(Handle, fragShader);

            int geomShader = 0;
            if(geomFile != string.Empty)
            {
                geomShader = CreateShader(geomFile, ShaderType.GeometryShader);
                GL.AttachShader(Handle, geomShader);
            }

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Ocorreu um erro ao vincular o programa \n{CurrentShader} \n{GL.GetShaderInfoLog(Handle)}");
            }

            // Desanexar os shaders, e depois o apague-os
            DetachDeleteShaders(vertShader);
            DetachDeleteShaders(fragShader);
            if(geomShader != 0)
            {
                DetachDeleteShaders(geomShader);
            }

            // aloque o dicionário para armazenar todos os uniforms.GL.GetUniformLocation(Handle, nome)new Dictionary<string, int>();
            uniformLocations = new Dictionary<string, int>();

            // verfificamos a quantidade de uniforms que o shader possui
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            
            // realizamos um loop em todos os uniformes,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                string key = GL.GetActiveUniform(Handle, i, out var size, out _);

                if(size > 1)
                {
                    // opengl não armazena o nome dos uniforms de arrays, por isso devemos setar os nomes de arrays manualmente.
                    // vou tentar explicar de uma forma melhor.
                    // imagine o seguinte uniform colors[4], o opegl não ira armazenar todos esses arrays, ele ira armazenar apenas  
                    // uniform colors[0], por isso devemos manualmente incrementar color[1], color[2], color[3] e color[4] em nosso dicionario.

                    for(int j = 0; j < size; j++)
                    {
                        var keyArray = key.Substring(0, key.Length - 2) + $"{j}]";

                        var location = GL.GetUniformLocation(Handle, keyArray);
                        uniformLocations.Add(keyArray, location);
                    }
                }
                else
                {
                    var location = GL.GetUniformLocation(Handle, key);
                    uniformLocations.Add(key, location);
                    
                }
            }
        }
        private int CreateShader(string shaderCode, ShaderType type)
        {
            int shader = GL.CreateShader(type);

            if(File.Exists(shaderCode))
            {
                GL.ShaderSource(shader, File.ReadAllText(shaderCode));
            }
            else
            {
                GL.ShaderSource(shader, shaderCode);
            }

            // compila o shader
            GL.CompileShader(shader);

            // Verifica se há erros de compilação
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            
            if (code != (int)All.True)
            {
                throw new Exception($"Ocorreu um erro ao compilar o programa \n{CurrentShader} \n{GL.GetShaderInfoLog(shader)}");
            }


            return shader;
        }
        private void DetachDeleteShaders(int shader)
        {
            GL.DetachShader(Handle, shader);
            GL.DeleteShader(shader);
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        public void SetUniform(string nome, bool dados)
        {
            GL.Uniform1(uniformLocations[nome], (dados ? 1 : 0));
        }
        public void SetUniform(string nome, int dados)
        {
            GL.Uniform1(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, float dados)
        {
            GL.Uniform1(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, Matrix4 dados)
        {
            GL.UniformMatrix4(uniformLocations[nome], true, ref dados);
        }
        public void SetUniform(string nome, Vector2 dados)
        {
            GL.Uniform2(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, Vector3 dados)
        {
            GL.Uniform3(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, Vector4 dados)
        {
            GL.Uniform4(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, Color4 dados)
        {
            GL.Uniform4(uniformLocations[nome], dados);
        }
        public void SetUniform(string nome, System.Numerics.Vector4 dados)
        {
            GL.Uniform4(uniformLocations[nome], new Vector4(dados.X, dados.Y, dados.Z, dados.W));
        }
    }
}