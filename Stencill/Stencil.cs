using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace MyGame
{
    public class Stencil
    {
        private const int MASK_MAX = 0xff; 
        private const int MASK_MIN = 0x00; 
        private ShaderProgram shaderStencil;
        public Stencil()
        {
            string vert_code = @"#version 460 core

                                layout (location = 0) in vec3 aPos;


                                uniform mat4 projection;
                                uniform mat4 view;
                                uniform mat4 model;


                                void main()
                                {
                                    gl_Position = vec4(aPos, 1.0) * model * view * projection;
                                }";

            string frag_code = @"#version 460 core

                                out vec4 FragColor;
                                uniform vec4 color; 

                                void main()
                                {
                                    FragColor = color;
                                    
                                }";

            shaderStencil = new ShaderProgram(vert_code, frag_code);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, MASK_MAX);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        }
        public static bool RenderStencil { get => !Bloom.EnableBloom; }
        public void Active()
        {
            GL.StencilFunc(StencilFunction.Always, 1, MASK_MAX);
            GL.StencilMask(MASK_MAX);
        }
        public void RenderFrame(Matrix4 model, System.Numerics.Vector4 color) 
        {
            if(RenderStencil)
            {
                GL.StencilFunc(StencilFunction.Notequal, 1, MASK_MAX);
                GL.StencilMask(MASK_MIN);

                model = model * Matrix4.CreateScale(Values.stencilSize);

                shaderStencil.Use();
                shaderStencil.SetUniform("model", model);
                shaderStencil.SetUniform("view", Camera.ViewMatrix);
                shaderStencil.SetUniform("projection", Camera.ProjectionMatrix);
                shaderStencil.SetUniform("color", color);
            }
        }
        public void Deactive()
        {
            if(RenderStencil)
            {
                GL.StencilMask(MASK_MAX);
                GL.StencilFunc(StencilFunction.Always, 0, MASK_MAX);
            }

        }
    }
}