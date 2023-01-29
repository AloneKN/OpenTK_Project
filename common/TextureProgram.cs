using OpenTK.Graphics.OpenGL4;

using StbImageSharp;

namespace MyGame
{
    public class TextureProgram : IDisposable 
    {
        private int Handle;
        private Tuple<TextureUnit, int> ?Unit;
        private PixelInternalFormat PixelInternFormat = PixelInternalFormat.SrgbAlpha;
        private TextureUnit Textureunit = TextureUnit.Texture0;
        public TextureProgram(string path)
        {
            Init(path);
        }
        public TextureProgram(string path, PixelInternalFormat pixelformat)
        {
            PixelInternFormat = pixelformat;
            Init(path);
        }
        public TextureProgram(string path, TextureUnit unit)
        {
            Textureunit = unit;
            Init(path);
        }
        public TextureProgram(string path, PixelInternalFormat pixelformat, TextureUnit unit)
        {
            PixelInternFormat = pixelformat; Textureunit = unit;
            Init(path);
        }
        private void Init(string path)
        {
            if(!File.Exists(path))
                throw new Exception($"Não foi possivel para encontrar a Textura: {path}");

            Unit = new Tuple<TextureUnit, int>(Textureunit, getUnitInt(Textureunit));

            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            StbImage.stbi_set_flip_vertically_on_load(1);
            using(Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternFormat, 
                    image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        }
        public int Use { get => _Use(); }
        private int _Use()
        {

            GL.ActiveTexture(Unit!.Item1);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            return Unit.Item2;
            
        }
        private int getUnitInt(TextureUnit Textureunit)
        {
            string unitNum = $"{Textureunit}";
            switch(unitNum.Length)
            {
                case 8:
                    return int.Parse(unitNum[(unitNum.Length - 1)..]);
                
                case 9:
                    return int.Parse(unitNum[(unitNum.Length - 2)..]);

                default:
                    return 0;
            }
        }
        public void Dispose() => GL.DeleteTexture(Handle);
    }
}


