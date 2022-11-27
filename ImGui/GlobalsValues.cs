using OpenTK.Mathematics;

using Vector4 = System.Numerics.Vector4;

namespace MyGame
{
    // the values 
    public struct Values
    {
        public static float gammaBackground = 1.0f; 

        // Objects value
        public static float gammaObject         = 1.5f;
        public static float luminousStrength    = 1.0f;
	    public static float specularStrength    = 0.1f;
	    public static float emissiveStrength    = 1.0f;

        // post process Values
        public static float FilmGrainAmount     = 0.0f;
        public static float shineValue          = 0.0f;

        public static Vector4 lightColor = Vector4.One,
        fpsColor = Vector4.One,
        StencilColor = Vector4.One,
        crosshairColor = Vector4.One;

        public static float lensInterpolation = 0.0f;

        public static float stencilSize = 1.001f;

    }
}