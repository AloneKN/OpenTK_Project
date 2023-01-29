#version 460 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoords;
layout(location = 2) in vec3 aPositions;
layout(location = 3) in vec2 aScales;
layout(location = 4) in vec4 aColors;

out vec2 TexCoords;
out vec4 Colors;

uniform vec3 CameraRight;
uniform vec3 CameraUp;

uniform mat4 projection;
uniform mat4 view;
uniform vec3 moving;

void main()
{
    TexCoords = aTexCoords;
    Colors = aColors;

    vec3 vertexPosition = aPositions + CameraRight * aPos.x * aScales.x + CameraUp * aPos.y * aScales.y;

    gl_Position = vec4(vertexPosition + moving, 1.0) * view * projection;

}