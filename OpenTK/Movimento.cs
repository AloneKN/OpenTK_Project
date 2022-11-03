using OpenTK.Mathematics;

namespace Open_GLTK
{
    class Movimento
    {
        private float vel;
        
        private float up;
        private float down;

        private float velMax;
        private float velMin;
        public Movimento(Vector2 velMaxMin, Vector2 upDown)
        {
            vel = velMaxMin.X;

            velMin = velMaxMin.X;
            velMax = velMaxMin.Y;

            up = upDown.X;
            down = upDown.Y;
        }
        // Ganho de movimento
        public float upMove()
        {
            if(vel < velMax)
            {
                vel += up;
            }
            return funcaoExponencial(vel);
        }
        // perda de velocidade
        public float downMove()
        {
            if(vel > velMin)
            {
                vel -= down;
                return funcaoExponencial(vel);
            }
            return 0;
        }
        // ganho de velocidade
        private float funcaoExponencial(float x)
        {
            return (float)Math.Pow(2, x);
        }
    }
}