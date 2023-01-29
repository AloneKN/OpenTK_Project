#version 460 core

out vec4 FragColor;
in vec3 WorldPos;

uniform samplerCube environmentMap;

uniform float gamma = 1.0;
uniform float interpolation= 0.9;

vec3 Luminous(vec3 texbase)
{
    vec3 envColor = pow(texbase, vec3(1.0/ (gamma + 1.0) ));
    vec3 envColorLum = pow(texbase, vec3(1.0/gamma));

    return mix(envColor, envColorLum, interpolation);
}
void main()
{
    lowp vec3 base = texture(environmentMap, WorldPos).rgb;

    FragColor = vec4(Luminous(base), 1.0);

}