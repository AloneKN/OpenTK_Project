#version 460 core

in vec2 TexCoords;

out vec4 FragColor;

uniform sampler2D imagem;

void main()
{
    FragColor = texture( imagem, TexCoords );
    if(FragColor.a <= 0.1)
        discard;
}