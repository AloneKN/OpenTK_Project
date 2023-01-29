// #version 460 core

// out vec4 FragColor;

// in vec2 TexCoords;
// in vec3 WorldPos;
// in vec3 Normal;

// struct Maps
// {
//     sampler2D DiffuseMap;
//     sampler2D NormalMap;
// };


// uniform Maps maps;

// vec3 getNormalFromMap()
// {
//     vec3 tangentNormal = texture(maps.NormalMap, TexCoords).xyz * 2.0 - 1.0;

//     vec3 Q1  = dFdx(WorldPos);
//     vec3 Q2  = dFdy(WorldPos);
//     vec2 st1 = dFdx(TexCoords);
//     vec2 st2 = dFdy(TexCoords);

//     vec3 N   = normalize(Normal);
//     vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
//     vec3 B  = -normalize(cross(N, T));
//     mat3 TBN = mat3(T, B, N);

//     return normalize(TBN * tangentNormal);
// }

// vec4 elimineAlpha(vec4 tex)
// {
//     if(tex.a < 0.1)
//         discard;

//     return tex;
// }
// void main()
// {           
// 	vec3 difuse = elimineAlpha( texture(maps.DiffuseMap, TexCoords)).rgb;

//     FragColor = vec4(difuse, 1.0);
// }



#version 460 core

out vec4 FragColor;

in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

struct Maps
{
    sampler2D DiffuseMap;
};

// struct Light
// {
//     float Ambiente;
//     float Shininess;
//     float Specular;
//     float Diffuse;

// };
// uniform vec3 lightPos;
// uniform vec3 viewPos;

uniform Maps maps;
// uniform Light light;

uniform vec4 color;
uniform float LightScene;

// vec3 getNormalFromMap()
// {
//     vec3 tangentNormal = texture(maps.NormalMap, TexCoords).xyz * 2.0 - 1.0;

//     vec3 Q1  = dFdx(WorldPos);
//     vec3 Q2  = dFdy(WorldPos);
//     vec2 st1 = dFdx(TexCoords);
//     vec2 st2 = dFdy(TexCoords);

//     vec3 N   = normalize(Normal);
//     vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
//     vec3 B  = -normalize(cross(N, T));
//     mat3 TBN = mat3(T, B, N);

//     return normalize(TBN * tangentNormal);
// }

vec4 elimineAlpha(vec4 tex)
{
    if(tex.a < 0.1)
        discard;

    return tex;
}
void main()
{
    vec4 result = texture(maps.DiffuseMap, TexCoords);
    result.rgb = result.rgb + color.rgb;
    result.rgb = result.rgb * LightScene;
    FragColor = result;
}