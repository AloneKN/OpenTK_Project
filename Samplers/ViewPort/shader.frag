#version 460 core

in vec2 TexCoord;

out vec4 FragColor;

uniform sampler2D mytexture;
uniform vec4 color;

void main()
{

    vec4 tex = texture(mytexture, TexCoord);

    FragColor = color * tex;

    if(FragColor.a < 0.1)
        discard;
}