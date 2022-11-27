using ImGuiNET;

using Vector4 = System.Numerics.Vector4;

namespace MyGame
{
    struct ConvertColor
    {
        public static OpenTK.Mathematics.Color4 Color4(System.Numerics.Vector4 color)
        {
            return new OpenTK.Mathematics.Color4(color.X, color.Y, color.Z, color.W);
        }    
    }
    public class ImGuiProgram
    {
        private static float hdr = 255f;
        private static Vector4 _lightColor = Vector4.One;
        public static void RenderFrame()
        {
            ImGui.Begin("Scene Details");
            ImGui.Text($"Frames: {Clock.Frames} | Time: {Clock.Time.ToString("0.0")}");
            ImGui.NewLine();
            ImGui.ColorEdit4("Color Text Display", ref Values.fpsColor);
            
            ImGui.NewLine();
            ImGui.ColorEdit4("Color CrossHair", ref Values.crosshairColor);

            ImGui.NewLine();
            ImGui.ColorEdit4("Stencil Color", ref Values.StencilColor);
            ImGui.SliderFloat("Stencil Size", ref Values.stencilSize, 1.0f, 1.1f);

            ImGui.NewLine();
            ImGui.ColorEdit4("Color Light", ref _lightColor);
            Values.lightColor  = new Vector4(_lightColor.X * hdr, _lightColor.Y * hdr, 
                                            _lightColor.Z * hdr, _lightColor.W * hdr);

            ImGui.NewLine();
            ImGui.SliderFloat("CubeMap Gamma",     ref Values.gammaBackground,   0.5f, 2.0f);
            
            ImGui.NewLine();
            ImGui.Text("Parameter Objects");
            ImGui.SliderFloat("Gamma Strenght",    ref Values.gammaObject,       0.5f, 2.0f);
            ImGui.SliderFloat("Luminous Strength", ref Values.luminousStrength,  0.0f, 2.0f);
            ImGui.SliderFloat("Specular Strength", ref Values.specularStrength,  0.0f, 0.3f);
            ImGui.SliderFloat("Emissive Strength", ref Values.emissiveStrength,  1.0f, 10.0f);

            ImGui.NewLine();
            ImGui.Text("Post Processing");
            ImGui.SliderFloat("Film Grain Amount", ref Values.FilmGrainAmount, -0.3f, 0.3f);
            ImGui.SliderFloat("Shine Intensity",   ref Values.shineValue,       0.0f, 1.0f);
            ImGui.SliderFloat("Lens Effect Interpolation", ref Values.lensInterpolation, 0.0f, 0.1f);

            ImGui.End();
        }
    }
}