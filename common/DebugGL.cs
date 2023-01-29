using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MyGame
{
    class DebugGL
    {
        private static DebugProc DebugCallBack = new DebugProc(DebugGL.Debug);
        public static bool DebugInitiated { get; private set; } = false;

        /// <summary>
        /// Inicia o debug;
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
        /// Função callback vai ser chamada toda vez que ocorre um erro.
        /// </summary>
        private static void Debug(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            // ignorar códigos de erro/aviso não significativos
            if (id == 131169 || id == 131185 || id == 131218 || id == 131204) return;

            Console.WriteLine("##################################################################");
            Console.WriteLine($"Debug message ( {id} ): {message} ");

            var stringSource = $"{source}";
            Console.WriteLine($"Sourcer: {stringSource[11..]}");

            var stringType = $"{type}";
            Console.WriteLine($"Type: {stringType[9..]}");

            var stringSeverity = $"{severity}";
            Console.WriteLine($"Severity: {stringSeverity[13..]}");

            Console.WriteLine("##################################################################");
            Console.WriteLine();
        }
    }
}
