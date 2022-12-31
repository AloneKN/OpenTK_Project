#version 460 core

out vec4 FragColor;
in vec3 WorldPos;

uniform samplerCube environmentMap;

uniform float gamma;


void main()
{
    lowp vec3 base = texture(environmentMap, WorldPos).rgb;

    vec3 envColor = pow(base, vec3(1.0/ (gamma + 2.0) ));
    vec3 envColorLum = pow(base, vec3(1.0/gamma));

    
    FragColor = vec4(mix(envColor, envColorLum, 0.8), 1.0);


}