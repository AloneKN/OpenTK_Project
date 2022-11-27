using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    class DebugGL
    {
        public static void StartGlobal()
        {
            // ativa o tete de profundidade(z_buffer), assim o OpenGL renderiza tudo de forma correta
            // por padrao e gl_less
            GL.Enable(EnableCap.DepthTest);
            // GL.DepthFunc(DepthFunction.Less);
            // GL.DepthFunc(DepthFunction.Lequal);

            // nao renderiza o que esta atrás
            // GL.Enable(EnableCap.CullFace);
            
            // ativando o espaço Srgb sobre texturas
            GL.Enable(EnableCap.FramebufferSrgb);

            // antialising
            GL.Enable(EnableCap.Multisample);

            // filtra as costuras dos cube maps
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            // habilita o point size, isso tem efeito apenas no shader de geometria
            GL.Enable(EnableCap.ProgramPointSize);

            // ativando a opacidade no fragment shader
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            GL.ClearColor(Color4.Black);
        }
        private static DebugProc DebugCallBack = new DebugProc(DebugGL.Debug);
        public static bool DebugInitiated { get; private set; } = false;

        /// <summary>
        /// inicia o debug, ira printar no console o status atual.
        /// </summary>
        public static void InitDebug()
        {
            int []context_flags = { int.MinValue };

            GL.GetInteger(GetPName.ContextFlags, context_flags);

            if(context_flags[0] == (int)ContextFlagMask.ContextFlagDebugBit)
            {
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);

                GL.DebugMessageCallback(DebugCallBack, IntPtr.Zero);
                int []ids = { int.MinValue };
                GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, ids , true);

                DebugInitiated = true;
                Console.WriteLine("Debug context created sucessfully!");
            }
            else
            {
                Console.WriteLine("Debug context failed to create!");
            }
            
        }
        /// <summary>
        /// função callback vai ser chamada toda vez que o opengl dar error
        /// </summary>
        private static void Debug(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            // ignorar códigos de erro/aviso não significativos
            if (id == 131169 || id == 131185 || id == 131218 || id == 131204) return;

            Console.WriteLine($"Debug message ( {id} ): {message} ");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            switch (source)
            {
                case DebugSource.DebugSourceApi:             Console.WriteLine("Source: API");              break;
                case DebugSource.DebugSourceWindowSystem:    Console.WriteLine("Source: Window System");    break;
                case DebugSource.DebugSourceShaderCompiler:  Console.WriteLine("Source: Shader Compiler");  break;
                case DebugSource.DebugSourceThirdParty:      Console.WriteLine("Source: Third Party");      break;
                case DebugSource.DebugSourceApplication:     Console.WriteLine("Source: Application");      break;
                case DebugSource.DebugSourceOther:           Console.WriteLine("Source: Other");            break;

            };

            switch (type)
            {
                case DebugType.DebugTypeError:               Console.WriteLine("Type: Error");                  break;
                case DebugType.DebugTypeDeprecatedBehavior:  Console.WriteLine("Type: Deprecated Behaviour");   break;
                case DebugType.DebugTypeUndefinedBehavior:   Console.WriteLine("Type: Undefined Behaviour");    break;
                case DebugType.DebugTypePortability:         Console.WriteLine("Type: Portability");            break;
                case DebugType.DebugTypePerformance:         Console.WriteLine("Type: Performance");            break;
                case DebugType.DebugTypeMarker:              Console.WriteLine("Type: Marker");                 break;
                case DebugType.DebugTypePushGroup:           Console.WriteLine("Type: Push Group");             break;
                case DebugType.DebugTypePopGroup:            Console.WriteLine("Type: Pop Group");              break;
                case DebugType.DebugTypeOther:               Console.WriteLine("Type: Other");                  break;

            };

            switch (severity)
            {
                case DebugSeverity.DebugSeverityHigh:         Console.WriteLine("Severity: high");                  break;
                case DebugSeverity.DebugSeverityMedium:       Console.WriteLine("Severity: medium");                break;
                case DebugSeverity.DebugSeverityLow:          Console.WriteLine("Severity: low");                   break;
                case DebugSeverity.DebugSeverityNotification: Console.WriteLine("Severity: notification");          break;

            }

            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
