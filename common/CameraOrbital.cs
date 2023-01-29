using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    class CameraOrbital
    {
        // current vaues
        private float Raio = 25f;
        private float Theta = 90f;
        private float Phi = 90f;
        private static float _fov = OpenTK.Mathematics.MathHelper.PiOver2;
        private const float PI = OpenTK.Mathematics.MathHelper.Pi;
        // ---------------------------------------------------
        private float Fov
        {
            get => OpenTK.Mathematics.MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = OpenTK.Mathematics.MathHelper.Clamp(value, 1f, 90f);
                _fov = OpenTK.Mathematics.MathHelper.DegreesToRadians(angle);
            }
        }
        public static Vector3 Position;
        public static Vector3 Look;
        private static float AspectRatio = Program.Size.X / (float)Program.Size.Y;
        public static Matrix4 ProjectionMatrix 
        { 
            get => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f); 
        }
        public static Matrix4 ViewMatrix
        {
            get => Matrix4.LookAt(Position, Look, Vector3.UnitY * 1f);
        }
        public CameraOrbital()
        { 
            Order(); 
        }
        public void UpdateFrame()
        {

            UpdateDistance();
            UpdateOrbit();
        }
        public void Resized(Vector2 size)
        {
            AspectRatio = size.X / (float)size.Y;
        }
        private void UpdateDistance()
        {

            var input = Program.window.IsKeyDown;

            if(input(Keys.W))
                Forward();

            if(input(Keys.S))
                Back();

            Order();
        }
        private bool firstMove = true;
        private Vector2 lastPos;
        private void UpdateOrbit()
        {


            if(Program.window.IsMouseButtonDown(MouseButton.Button2))
            {
                
                var mouse = Program.window.MouseState;

                Program.window.CursorState = CursorState.Grabbed;

                if (firstMove) 
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    Theta -= mouse.X - lastPos.X;
                    Phi -= mouse.Y - lastPos.Y;

                    Order();

                    lastPos = mouse.Position;
                }

            }
            else
            {
                Program.window.CursorState = CursorState.Normal;
            }

        }
        private void Forward()
        {
            Position += Vector3.Normalize(Look) * new Vector3(0.25f) * (TimerGL.ElapsedTime * 500);
            Raio = MathF.Sqrt(Position.X * Position.X + Position.Y * Position.Y + Position.Z * Position.Z);
        }
        private void Back()
        {
            Position += Vector3.Normalize(Look) * new Vector3(-0.25f) * (TimerGL.ElapsedTime * 500);
            Raio = MathF.Sqrt(Position.X * Position.X + Position.Y * Position.Y + Position.Z * Position.Z);
        }
        public void Order()
        {
            if(Phi < 1f)
                Phi = 1f;

            if(Phi > 150f)
                Phi = 150f;

            float aux = Raio * MathF.Sin(Phi * PI / 180f);
            Position = new Vector3()
            {
                X = aux  * MathF.Sin(Theta * PI / 180f),
                Y = Raio * MathF.Cos(Phi * PI / 180f),
                Z = aux  * MathF.Cos(Theta *  PI / 180f),
            };

            Look = Position * -1f;

        }
    }
}