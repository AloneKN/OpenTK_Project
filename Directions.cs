using OpenTK.Mathematics;

namespace MyGame
{
    public class Directions
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
        private Movimento[] movimento = new Movimento[6];
        public Directions(float max, float min, float up, float down)
        {
            for(int i = 0; i < movimento.Length; i++)
            {
                movimento[i] = new Movimento(new Vector2(max, min), new Vector2(up, down));
            }
        }
        // Moving X
        public float X_positiveUp { get => (TimerGL.ElapsedTime * 500) * movimento[0].upMove(); } 
        public float X_positiveDowm { get => (TimerGL.ElapsedTime * 500) * movimento[0].downMove(); } 
        public float X_negativeUp { get => (TimerGL.ElapsedTime * 500) * movimento[1].upMove(); } 
        public float X_negativeDowm { get => (TimerGL.ElapsedTime * 500) * movimento[1].downMove(); } 
        // Moving X
        public float Y_positiveUp { get => (TimerGL.ElapsedTime * 500) * movimento[2].upMove(); } 
        public float Y_positiveDowm { get => (TimerGL.ElapsedTime * 500) * movimento[2].downMove(); } 
        public float Y_negativeUp { get => (TimerGL.ElapsedTime * 500) * movimento[3].upMove(); } 
        public float Y_negativeDowm { get => (TimerGL.ElapsedTime * 500) * movimento[3].downMove(); } 
        // Moving X
        public float Z_positiveUp { get => (TimerGL.ElapsedTime * 500) * movimento[4].upMove(); } 
        public float Z_positiveDowm { get => (TimerGL.ElapsedTime * 500) * movimento[4].downMove(); } 
        public float Z_negativeUp { get => (TimerGL.ElapsedTime * 500) * movimento[5].upMove(); } 
        public float Z_negativeDowm { get => (TimerGL.ElapsedTime * 500) * movimento[5].downMove(); } 
        

    }
}