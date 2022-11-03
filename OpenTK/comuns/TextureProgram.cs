using OpenTK.Graphics.OpenGL4;

using StbImageSharp;

namespace Open_GLTK
{
    public class TextureProgram // Uma classe auxiliar, muito parecida com Shader, destinada a simplificar o carregamento de texturas.
    {
        private readonly int Handle;
        public static TextureProgram Load(string path, PixelInternalFormat pixelFormat = PixelInternalFormat.Rgba)
        {
            if(!File.Exists(path))
                throw new Exception($"Não foi possivel para encontrar a Textura: {path}");

            int handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);
            using(Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    
                GL.TexImage2D(TextureTarget.Texture2D, 0, pixelFormat, 
                    image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                // GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha, 
                //     image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new TextureProgram(handle);
        }
        public TextureProgram(int glHandle)
        {
            Handle = glHandle;
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}