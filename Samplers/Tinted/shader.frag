#version 460 core

out vec4 FragColor;

uniform vec4 color;
uniform float ForceLight;

void main()
{
    
    FragColor = color * ForceLight;
}