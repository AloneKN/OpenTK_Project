using OpenTK.Mathematics;


namespace Open_GLTK
{
    // the values 
    public struct Values
    {
        public static float gammaBackground = 1.0f; 

        // Objects value
        public static float gammaObject         = 1.5f;
        public static float luminousStrength    = 2.0f;
	    public static float specularStrength    = 0.1f;
	    public static float emissiveStrength    = 1.0f;

        // post process Values
        public static float FilmGrainAmount     = 0.0f;
        public static float shineValue          = 0.0f;

        public static Color4 lightColor = Color4.White;
    }
}