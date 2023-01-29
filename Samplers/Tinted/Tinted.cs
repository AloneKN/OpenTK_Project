
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MyGame
{
    public class Tinted : IDisposable
    {
        public class Ball
        {
            public Vector3 Position;
            public Vector3 Direction;

        }
        private List<Ball> ball = new List<Ball>();
        private ShaderProgram Shader;
        public Tinted()
        {
            Shader = new ShaderProgram("Samplers/Tinted/shader.vert", "Samplers/Tinted/shader.frag");
        }
        public void RenderFrame()
        {
            Shader.Use();
            
            Shader.SetUniform("view", Camera.ViewMatrix);
            Shader.SetUniform("projection", Camera.ProjectionMatrix);

            Shader.SetUniform("color", Values.lightColor / 255f);
            Shader.SetUniform("ForceLight", Values.ForceLightScene);


            for( int i = 0; i < ball.Count; i++)
            {
                ball[i].Position += ball[i].Direction;

                if(Delete(ball[i].Position.X) || Delete(ball[i].Position.Y) || Delete(ball[i].Position.Z))
                {
                    ball.RemoveAt(i);
                    break;
                }

                Matrix4 model = Matrix4.Identity;
                model = model * Matrix4.CreateScale(0.5f);
                model = model * Matrix4.CreateTranslation(ball[i].Position);

                Shader.SetUniform("model", model);
                MyGame.Sphere.RenderSphere();
            }

        }
        private bool Delete(float num)
        {
            if(num > Camera.DistanceOfView || num < -Camera.DistanceOfView)
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }
        public void UpdateFrame()
        {
            var input = Program.window.IsKeyDown;
            float vel =  TimerGL.ElapsedTime * 30.0f;

            if(Program.window.IsMouseButtonPressed(MouseButton.Button1))
            {
                ball.Add( new Ball()
                {
                    Position = Camera.Position,
                    Direction = Vector3.Normalize(Camera.Front),

                });
            }

        }
        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}