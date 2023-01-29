#version 460 core

layout (location = 0) in vec3 aPos;

out vec3 WorldPos;

uniform mat4 projection;
uniform mat4 view;


void main()
{
    WorldPos = aPos;
	mat4 rotView = mat4(mat3(view));
	vec4 clipPos = vec4(WorldPos, 1.0) * rotView * projection;

	gl_Position = clipPos.xyww;
}