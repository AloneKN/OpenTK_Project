using ImGuiNET;

using OpenTK.Mathematics;

using Vector3 = System.Numerics.Vector3;

namespace Open_GLTK
{
    public class ImGuiProgram
    {
        private static Vector3 color_light = Vector3.One;
        private static float hdr = 255f;
        public static void RenderFrame()
        {
            ImGui.Begin("Scene Details");
            ImGui.Text($"Frames: {TimerGL.Frames} | Time: {TimerGL.Time.ToString("0.0")}");

            ImGui.NewLine();
            ImGui.ColorEdit3("Color Light", ref color_light);
            Values.lightColor = new Color4(color_light.X * hdr, color_light.Y * hdr, color_light.Z * hdr, hdr);

            ImGui.SliderFloat("CubeMap Gamma",     ref Values.gammaBackground,   0.0f, 3.0f);
            ImGui.Text("Parameter Objects");
            ImGui.SliderFloat("Gamma Strenght",    ref Values.gammaObject,       0.5f, 1.5f);
            ImGui.SliderFloat("Luminous Strength", ref Values.luminousStrength,  0.0f, 5.0f);
            ImGui.SliderFloat("Specular Strength", ref Values.specularStrength,  0.0f, 1.0f);
            ImGui.SliderFloat("Emissive Strength", ref Values.emissiveStrength,  1.0f, 5.0f);

            ImGui.NewLine();
            ImGui.Text("Post Processing");
            ImGui.SliderFloat("Film Grain Amount", ref Values.FilmGrainAmount, -0.3f, 0.3f);
            ImGui.SliderFloat("Shine Intensity",   ref Values.shineValue,       0.0f, 1.0f);

            ImGui.End();
        }
    }
}