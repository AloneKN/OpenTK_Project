#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec3 WorldPos;

uniform mat4 projection;
uniform mat4 view;

out vec2 TexCoord;

void main()
{
    WorldPos = aPos;
    gl_Position = vec4(WorldPos, 1.0) * view * projection;
    TexCoord = aTexCoords;
}