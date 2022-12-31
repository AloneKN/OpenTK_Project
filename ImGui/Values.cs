using OpenTK.Mathematics;

using Vector4 = System.Numerics.Vector4;

namespace MyGame
{
    // the values 
    public struct Values
    {
        public static Vector4 lightColor = Vector4.One,
        fpsColor = Vector4.One,
        StencilColor = Vector4.One,
        crosshairColor = Vector4.One;
        public static float gammaBackground = 1.0f; 

        public static float LightDiffuse = 15.0f;

        // Objects value
        public static bool rotate = true;
        public static float gammaObject         = 1.5f;
        public static float luminousStrength    = 1.0f;
	    public static float specularStrength    = 0.1f;



        public static float lensInterpolation = 0.0f;

        public static float stencilSize = 1.001f;

        // bloom
        public static bool isRenderBloom = true; 
        public static float new_bloom_exp = 0.8f;
        public static float new_bloom_streng = 0.3f;
        public static float new_bloom_gama = 0.6f;
        public static float filterRadius = 0.002f;
        public static float new_bloom_filmGrain = -0.03f;
        public static float nitidezStrengh = 0.0f;


    }
}