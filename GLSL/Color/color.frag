#version 460 core

out vec4 FragColor;

uniform vec4 color;

uniform float LightDiffuse;

void main()
{
    vec3 diffuse = LightDiffuse * color.rgb;

    FragColor = vec4(color.rgb + diffuse, 1.0);
}