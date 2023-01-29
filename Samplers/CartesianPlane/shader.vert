#version 460 core

layout (location = 0) in vec3 aPos;

uniform mat4 projection;
uniform mat4 model;
uniform mat4 view;

out vec3 vertexPos;

void main()
{
    vertexPos = aPos;
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
}