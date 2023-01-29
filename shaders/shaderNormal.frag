#version 460 core

out vec4 FragColor;

in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

struct Maps
{
    sampler2D DiffuseMap;
    sampler2D NormalMap;
};
struct Light
{
    float Ambiente;
    float Shininess;
    float Diffuse;

};
uniform vec3 lightPos = vec3(0.0, 500.0, 0.0);
uniform vec3 viewPos;

uniform Maps maps;
uniform Light light;


vec3 getNormalFromMap()
{
    vec3 tangentNormal = texture(maps.NormalMap, TexCoords).xyz * 2.0 - 1.0;

    vec3 Q1  = dFdx(WorldPos);
    vec3 Q2  = dFdy(WorldPos);
    vec2 st1 = dFdx(TexCoords);
    vec2 st2 = dFdy(TexCoords);

    vec3 N   = normalize(Normal);
    vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
    vec3 B  = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
}

void main()
{           
	vec3 ambient = light.Ambiente * vec3(texture(maps.DiffuseMap, TexCoords));

    // Normal
    vec3 norm = getNormalFromMap();
    vec3 viewDirection = normalize(viewPos - WorldPos);
    vec3 R = reflect(-viewDirection, norm); 

    // Diffuse 
    vec3 lightDirection = normalize(lightPos - WorldPos);
    float diff = max(dot(R, lightDirection), 0.0);
    vec3 diffuse = light.Diffuse * diff * vec3(texture(maps.DiffuseMap, TexCoords));


    vec3 result = ambient + diffuse;
    FragColor = vec4(result * light.Shininess, 1.0);
}
