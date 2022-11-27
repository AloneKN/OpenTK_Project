using FreeTypeSharp.Native;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MyGame
{
    public class Text
    {  
        private struct Character
        {
            public int TextureID;
            public Vector2 Size;
            public Vector2i Bearing;
            public int Advance;
        }
        private int Vao, Vbo;
        private ShaderProgram shaderFonts;
        private Dictionary<uint, Character> characteres = new Dictionary<uint, Character>();
        public Text(string fontePath)
        {
            string vert_shader = @" #version 460 core

                                    layout (location = 0) in vec4 vertex;

                                    out vec2 TexCoords;

                                    uniform mat4 projection;

                                    void main()
                                    {
                                        gl_Position = vec4(vertex.xy, 0.0, 1.0) * projection;
                                        TexCoords = vertex.zw;
                                    }";

            string frag_shader = @" #version 460 core

                                    in vec2 TexCoords;
                                    out vec4 FragColor;

                                    uniform sampler2D text;
                                    uniform vec4 textColor;

                                    void main()
                                    {    
                                        vec4 sampled = vec4(1.0, 1.0, 1.0, texture(text, TexCoords).r);
                                        FragColor = textColor * sampled;
                                        if(FragColor.a <= 0.1)
                                            discard;
                                    }";
                                                
            shaderFonts = new ShaderProgram(vert_shader, frag_shader);

            FreeTypeClassApi fts = new FreeTypeClassApi(fontePath);
            
            fts.SetPixelSizes(0, 48);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for(uint c = 0; c < 128; c++)
            {
                fts.LoadChar(c);

                FT_Bitmap bitmap = fts.GlyphBitmap;

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.R8, 
                (int)bitmap.width, 
                (int)bitmap.rows, 
                0, 
                PixelFormat.Red, 
                PixelType.UnsignedByte, 
                bitmap.buffer); 


                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                
                Character character = new Character();
                character.TextureID = texture;
                character.Size    = new Vector2i((int)bitmap.width, (int)bitmap.rows);
                character.Bearing = new Vector2i(fts.GlyphBitmapLeft, fts.GlyphBitmapTop);
                character.Advance = fts.GlyphMetricHorizontalAdvance;

                characteres.Add(c, character);
            }

            fts.DoneFreetype();
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        }
        public void RenderText(string text, Vector2 position, float scale, Color4 color)
        {
            GL.BindVertexArray(Vao);
            shaderFonts.Use();
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, Program.Size.X, 0.0f, Program.Size.Y, 0.0f, 1.0f);
            shaderFonts.SetMatrix4("projection", projection);
            shaderFonts.SetColor4("textColor", color);
            

            foreach(var c in text)
            {
                Character ch = characteres[c];
                if(characteres.ContainsKey(c) == false)
                    continue;

                float xpos = position.X + ch.Bearing.X * scale;
                float ypos = position.Y - (ch.Size.Y - ch.Bearing.Y) * scale;

                position.X += ch.Advance * scale;

                float w = ch.Size.X * scale;
                float h = ch.Size.Y * scale;

                float[,] vertices = new float[6, 4]
                {
                    { xpos,     ypos + h,   0.0f, 0.0f },
                    { xpos,     ypos,       0.0f, 1.0f },
                    { xpos + w, ypos,       1.0f, 1.0f },

                    { xpos,     ypos + h,   0.0f, 0.0f },
                    { xpos + w, ypos,       1.0f, 1.0f },
                    { xpos + w, ypos + h,   1.0f, 0.0f }
                };
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                shaderFonts.SetTexture("text", 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);


                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                
            }
        }
        public void Dispose()
        {
            GL.DeleteTextures(characteres.Count, characteres.Keys.ToArray());

            shaderFonts.Dispose();
            GL.DeleteBuffer(Vbo);
            GL.DeleteVertexArray(Vao);
        }
    }
    
}