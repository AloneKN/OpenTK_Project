#version 460 core

out vec4 FragColor;

in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

in vec3 vertexPos;

uniform vec4 colorPositive;
uniform vec4 colorNegative;

uniform float Light;
void main()
{
    if(vertexPos.x < 0.0 || vertexPos.y < 0.0 || vertexPos.z < 0.0)
    {
        FragColor = colorNegative * Light;
    }
    else
    {
        FragColor = colorPositive * Light;
    }
}