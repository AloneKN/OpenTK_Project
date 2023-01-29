#version 460 core

in vec2 TexCoords;
in vec4 Colors;

out vec4 FragColor;

uniform sampler2D imagem;
uniform float LightDiffuse;

// uniform vec4 color = vec4(1.0);

void main()
{
    vec4 result = texture( imagem, TexCoords ) * Colors;

    if(result.a == 0.0)
        discard;

    FragColor = result * LightDiffuse;
    
}