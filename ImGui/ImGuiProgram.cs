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
            ImGui.StyleColorsDark();
            ImGui.ShowDemoWindow();

            ImGui.Begin("Scene Details");
            ImGui.Text($"Frames: {TimerGL.FramesForSecond} | Time: {TimerGL.Time.ToString("0.0")}");
            ImGui.NewLine();
            ImGui.ColorEdit4("Color Text Display", ref Values.fpsColor);
            
            ImGui.NewLine();
            ImGui.ColorEdit4("Color CrossHair", ref Values.crosshairColor);

            ImGui.NewLine();
            ImGui.SliderFloat("CubeMap Gamma", ref Values.gammaBackground, 0.1818182f, 1.0f, "%.7f");
            ImGui.SliderFloat("CubeMap Contraste", ref Values.interpolatedBack, 0.0f, 1.0f);

            ImGui.NewLine();
            ImGui.ColorEdit4("Particles Color", ref Values.ParticlesColor);
            ImGui.SliderFloat("Particles Shine", ref Values.ParticlesLum, 1.0f, 50f);
            ImGui.SliderFloat("Particles Size", ref Values.ParticlesScale, 0.0f, 5f);


            ImGui.NewLine();
            ImGui.ColorEdit4("Color Light", ref _lightColor);
            ImGui.SliderFloat("Lights Scene Instencity", ref Values.ForceLightScene, 1.0f, 100f);
            Values.lightColor  = new Vector4(_lightColor.X * hdr, _lightColor.Y * hdr, 
                                            _lightColor.Z * hdr, _lightColor.W * hdr);
            
            ImGui.NewLine();
            ImGui.Text("Parameter Objects");
            ImGui.SliderFloat("Gamma Strenght",    ref Values.gammaObject,       0.5f, 2.0f);
            ImGui.SliderFloat("Luminous Strength", ref Values.luminousStrength,  0.0f, 5.0f);
            ImGui.SliderFloat("Specular Strength", ref Values.specularStrength,  0.0f, 2.0f);

            ImGui.SliderFloat("Rotation X-axis", ref Values.RotateX,  -360f, 360f, "%.1f");
            ImGui.SliderFloat("Rotation Y-axis", ref Values.RotateY,  -360f, 360f, "%.1f");
            ImGui.SliderFloat("Rotation Z-axis", ref Values.RotateZ,  -360f, 360f, "%.1f");

            ImGui.NewLine();
            if(ImGui.Button("Enable Bloom"))
                Values.Bloom.Enable = !Values.Bloom.Enable;
            
            if(Values.Bloom.Enable)
            {
                ImGui.SliderFloat("Bloom Exposure", ref Values.Bloom.Exposure, 0.0f, 1.0f);
                ImGui.SliderFloat("Bloom Strength", ref Values.Bloom.Strength, 0.0f, 1.0f, "%.7f");
                ImGui.SliderFloat("Bloom Gamma", ref Values.Bloom.Gamma, 0.0f, 1.0f, "%.7f");
                ImGui.SliderFloat("Bloom Spacing Filter", ref Values.Bloom.FilterRadius, 0.0f, 0.01f, "%.7f");
                ImGui.SliderFloat("Bloom Film Grain", ref Values.Bloom.FilmGrain, -0.1f, 0.1f, "%.7f");
                ImGui.SliderFloat("Bloom Nitidez Strength", ref Values.Bloom.Nitidez, 0.0f, 0.2f, "%.7f");
                ImGui.SliderInt("Bloom Vibrance", ref Values.Bloom.Vibrance, -255, 100);
                ImGui.Checkbox("Active Negative", ref Values.Bloom.Negative);

                ImGui.SliderInt("UGhost", ref Values.uGhost, 0, 50);
                ImGui.SliderFloat("Ghost Dispersal", ref Values.uGhostDispersal, 0.0f, 1.0f, "%.7f");
            }
            ImGui.End();

            ImGui.Begin("Physics World");
            ImGui.SliderFloat("Gravity", ref Values.PhysicsWorld.Gravity, 0f, 100f, "%.2f");
            ImGui.ListBox("Primitive Type", 
                ref Values.PhysicsWorld.PrimitiveTypeIndex, Values.PhysicsWorld.PrimitiveList, Values.PhysicsWorld.PrimitiveList.Length);

            ImGui.End();

        }
    }
}