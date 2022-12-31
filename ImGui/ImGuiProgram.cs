using ImGuiNET;

using Vector4 = System.Numerics.Vector4;

namespace MyGame
{
    public class ImGuiProgram
    {
        private static float hdr = 255f;
        private static Vector4 _lightColor = Vector4.One;
        public static void RenderFrame()
        {
            ImGui.ShowDemoWindow();
            ImGui.StyleColorsClassic();

            ImGui.Begin("Scene Details");
            ImGui.Text($"Frames: {Clock.FramesForSecond} | Time: {Clock.Time.ToString("0.0")}");
            ImGui.NewLine();
            ImGui.ColorEdit4("Color Text Display", ref Values.fpsColor);
            
            ImGui.NewLine();
            ImGui.ColorEdit4("Color CrossHair", ref Values.crosshairColor);

            ImGui.NewLine();
            ImGui.SliderFloat("CubeMap Gamma",     ref Values.gammaBackground, 0.1818182f, 1.0f, "%.7f");

            if(Stencil.RenderStencil)
            {
                ImGui.NewLine();
                ImGui.ColorEdit4("Stencil Color", ref Values.StencilColor);
                ImGui.SliderFloat("Stencil Size", ref Values.stencilSize, 1.0f, 1.1f);
            }

            ImGui.NewLine();
            ImGui.ColorEdit4("Color Light", ref _lightColor);
            ImGui.SliderFloat("Lights Scene Instencity", ref Values.LightDiffuse, 1.0f, 100f);
            Values.lightColor  = new Vector4(_lightColor.X * hdr, _lightColor.Y * hdr, 
                                            _lightColor.Z * hdr, _lightColor.W * hdr);

            
            
            ImGui.NewLine();
            ImGui.Text("Parameter Objects");
            ImGui.Checkbox("Rotate Object", ref Values.rotate);
            ImGui.SliderFloat("Gamma Strenght",    ref Values.gammaObject,       0.5f, 2.0f);
            ImGui.SliderFloat("Luminous Strength", ref Values.luminousStrength,  0.0f, 2.0f);
            ImGui.SliderFloat("Specular Strength", ref Values.specularStrength,  0.0f, 0.3f);

            ImGui.NewLine();
            ImGui.Checkbox("Enable Bloom", ref Values.isRenderBloom);

            if(Values.isRenderBloom)
            {
                ImGui.SliderFloat("Bloom Exposure", ref Values.new_bloom_exp, 0.0f, 1.0f);
                ImGui.SliderFloat("Bloom Strength", ref Values.new_bloom_streng, 0.0f, 1.0f, "%.7f");
                ImGui.SliderFloat("Bloom Gamma", ref Values.new_bloom_gama, 0.0f, 1.0f, "%.7f");
                ImGui.SliderFloat("Bloom Spacing Filter", ref Values.filterRadius, 0.0f, 0.01f, "%.7f");
                ImGui.SliderFloat("Bloom Film Grain", ref Values.new_bloom_filmGrain, -0.1f, 0.1f, "%.7f");
                ImGui.SliderFloat("Bloom Nitidez Strength", ref Values.nitidezStrengh, 0.0f, 0.2f, "%.7f");
            }


            ImGui.End();
        }
    }
}