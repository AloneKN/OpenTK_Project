using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MyGame
{
    // Esta é a classe da câmera.
    // É importante observar que existem algumas maneiras de configurar esta câmera.
    // Esta é apenas uma das muitas maneiras pelas quais poderíamos configurar a câmera.
    public class Camera
    {
        // --------------------------------------------------------------------------------------------------------------------------------
        // Esses vetores são direções apontando para fora da câmera para definir como é o seu giro
        private static Vector3 _front   = -Vector3.UnitZ;
        private static Vector3 _up      = Vector3.UnitY;
        private static Vector3 _right   = Vector3.UnitX;

        // Esses valores compôem o sistema de rotação 
        // Rotação ao redor do eixo X em (radianos)
        private static float _pitch;
        // Rotação em torno do eixo Y em (radianos). Sem isso, você começaria a girar 90 graus para a direita.
        private static float _yaw = -OpenTK.Mathematics.MathHelper.PiOver2;
        // O campo de visão da câmera em (radianos)
        private static float _fov = OpenTK.Mathematics.MathHelper.PiOver2;

        // Convertemos de graus para radianos assim que a propriedade é definida para melhorar o desempenho.
        public float Pitch
        {
            get => OpenTK.Mathematics.MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // Fixamos o valor do pitch entre -89 e 89 para evitar que a câmera fique de cabeça para baixo 
                // e um monte de "bugs" estranhos surgem quando você está usando ângulos de euler para rotação.
                // Se você quiser ler mais sobre isso você pode tentar pesquisar um tópico chamado gimbal lock
                var angle = OpenTK.Mathematics.MathHelper.Clamp(value, -89f, 89f);
                _pitch = OpenTK.Mathematics.MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }
        // Convertemos de graus para radianos assim que a propriedade é definida para melhorar o desempenho.
        private float Yaw
        {
            get => OpenTK.Mathematics.MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = OpenTK.Mathematics.MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        // O campo de visão (FOV) é o ângulo vertical da visão da câmera.
        // Convertemos de graus para radianos assim que a propriedade é definida para melhorar o desempenho.
        private float Fov
        {
            get => OpenTK.Mathematics.MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = OpenTK.Mathematics.MathHelper.Clamp(value, 1f, 90f);
                _fov = OpenTK.Mathematics.MathHelper.DegreesToRadians(angle);
            }
        }
        // Esta função irá atualizar os vértices de direção usando alguns calculos matematicos.
        private void UpdateVectors()
        {
            // Primeiro, a matriz frontal é calculada usando alguma trigonometria básica.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // Precisamos ter certeza de que os vetores estão todos normalizados, caso contrário, obteríamos alguns resultados estranhos.
            _front = Vector3.Normalize(_front);

            // Calcula o vetor direito e o vetor para cima usando o produto vetorial.
            // Observe que estamos calculando a direita do global para cima; esse comportamento pode
            // não seja o que você precisa para todas as câmeras, então tenha isso em mente se você não quiser uma câmera FPS.
            _right  = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up     = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        // -------------------------------------------------------------------------------------------------------------------------------
        // Esta é simplesmente a proporção da janela de visualização, usada para ordenar a matriz de projeção.
        private static float AspectRatio = Program.window.Size.X / (float)Program.window.Size.Y;
        // -------------------------------------------------------------------------------------------------------------------------------

        // Movimentação da camera no mundo
        // São as posições da câmera, e velocidade, todas são estaticas pois so vamos ter uma camera,
        // e também porque podemos acessar esses valores de qualquer lugar
        public static float CameraSpeed { get; set;      } = 6.5f;
        public static float MouseSensitivity  { get; set; } = 0.2f;
        public static float DistanceOfView { get; private set; } = 1000f;
        public static Vector3 Position  { get; private set; }
        public static Vector3 Front     { get => _front; }
        public static Vector3 Up        { get => _up;    }
        public static Vector3 Right     { get => _right; }

        // Matrizes de visualizao e projeção da camera ssao elas que sero enviadas para os nossos shaders
        // isso define a posicao da camera e para onde ela está olhando

        public static Matrix4 ViewMatrix
        {
            get => Matrix4.LookAt(Position, Position + _front, _up);
        }
        // isso serve para definir um limite da distancia da perspectiva do plano 3D
        // quanto mais alto valor mais longe a visao ira alcançar, más lembre-se
        // nao extrapole muito pois pode influençiar na performançe
        // ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 600f);
        public static Matrix4 ProjectionMatrix  
        { 
            get => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, DistanceOfView);
        }
        private void KeysUpdatePosition()
        {
            var input = Program.window.IsKeyDown;

            // update a movimentação da camera
            if(input(Keys.W)) Position += Front * CameraSpeed  * (float)TimerGL.ElapsedTime;
            if(input(Keys.S)) Position -= Front * CameraSpeed  * (float)TimerGL.ElapsedTime;
            if(input(Keys.A)) Position -= Right * CameraSpeed  * (float)TimerGL.ElapsedTime;
            if(input(Keys.D)) Position += Right * CameraSpeed  * (float)TimerGL.ElapsedTime;

            if(input(Keys.LeftShift) && input(Keys.W)) Position += Front * (CameraSpeed * 10 )* (float)TimerGL.ElapsedTime;
            if(input(Keys.LeftShift) && input(Keys.S)) Position -= Front * (CameraSpeed * 10 )* (float)TimerGL.ElapsedTime;

            if(input(Keys.Space)) Position += Up * CameraSpeed * (float)TimerGL.ElapsedTime;
            if(input(Keys.C))     Position -= Up * CameraSpeed * (float)TimerGL.ElapsedTime;

            
        }
        // --------------------------------------------------------------------------------------------------------------------------------
        // Movimentação da camera
        // View camera update
        private bool firstMove = true;
        private Vector2 lastPos;
        private bool activeView = false;
        private void MouseUpdateView()
        {
            var mouse = Program.window.MouseState;

            if(Program.window.IsMouseButtonPressed(MouseButton.Button2))
            {
                activeView = activeView ? false : true;
                Program.window.CursorState = activeView ? CursorState.Grabbed : CursorState.Normal;
                
            }
            if(activeView)
            {
                if (firstMove) 
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y);

                    Yaw += deltaX * MouseSensitivity;
                    Pitch -= deltaY * MouseSensitivity;
                }
            }
        }
        public Camera(Vector3 position = new Vector3())
        {

            if(position == Vector3.Zero)
            {
                Position = new Vector3(0.0f, 10.0f, 20.0f);
            }
            else
            {
                Position = position;
            }

        }
        public void UpdateCamera()
        {
            if (!Program.window.IsFocused)
                return;

            MouseUpdateView();
            KeysUpdatePosition();
        }
        public void UpdateSize(Vector2i size)
        {
            AspectRatio = size.X / (float)size.Y;
        }
        public void UpdateFov(ref MouseWheelEventArgs mouseWheelEventArgs)
        {
            if(activeView)
            {
                Fov -= mouseWheelEventArgs.OffsetY;
            }
        }
    }
}