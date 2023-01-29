using OpenTK.Windowing.Common;

namespace MyGame
{
    public struct TimerGL
    {
        public static float Time        { get; private set; } = 0.0f;
        public static float ElapsedTime { get; private set; } = 0.0f;
        public static float FramesForSecond      { get; private set; } = 0.0f;
        
        private static double previousTime = 0.0, frameCount = 0.0;
        public static void TimerUpdateFrame(FrameEventArgs eventArgs)
        {
            ElapsedTime = (float)eventArgs.Time;
            Time += (float)eventArgs.Time;

            frameCount++;
            if(Time - previousTime >= 1.0)
            {
                FramesForSecond = (float)frameCount;
                frameCount = 0;
                previousTime = Time;
            }
        }
    }
}