using OpenTK.Mathematics;

namespace MyGame
{
    class Directions
    {
        private struct Movimento
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
        //-------------------------------------------------------------

        private Movimento _PosX;
        private Movimento _NegX;
        private Movimento _PosY;
        private Movimento _NegY;
        private Movimento _PosZ;
        private Movimento _NegZ;
        public Directions(float velmax, float velmin, float velup, float veldown)
        {
            _PosX = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
            _NegX = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
            _PosY = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
            _NegY = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
            _PosZ = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
            _NegZ = new Movimento(new Vector2(velmax, velmin), new Vector2(velup, veldown)); 
        }
        public float PositiveX()
        {
            return _PosX.upMove();
        } 
        public float NegativeX()
        {
            return _NegX.downMove();
        } 
        public float PositiveY()
        {
            return _PosY.upMove();
        } 
        public float NegativeY()
        {
            return _NegY.downMove();
        } 
        public float PositiveZ()
        {
            return _PosZ.upMove();
        } 
        public float NegativeZ()
        {
            return _NegZ.downMove();
        } 
    }
}