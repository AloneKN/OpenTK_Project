using MyGame.BloomStages;

namespace MyGame
{
    class Bloom
    {
        public static bool EnableBloom { get => Values.Bloom.Enable; }
        private BloomSecondStage bloomSecondStage;
        public Bloom(int NumBlomMips = 6)
        {
            bloomSecondStage = new BloomSecondStage(NumBlomMips);
        }
        public void BindBloom()
        {
            if(EnableBloom)
            {
                bloomSecondStage.Active();
            }
        }
        public void ResizedFrame()
        {
            if(EnableBloom)
            {
                bloomSecondStage.ResizedFrameBuffer();
            }
        }
        public void RenderFrame()
        {
            if(EnableBloom)
            {
                bloomSecondStage.RenderFrame();
            }
        }
        public void Dispose()
        {
            bloomSecondStage.Dispose();
        }
    }
}