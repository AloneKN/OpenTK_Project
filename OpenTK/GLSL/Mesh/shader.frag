#version 460 core

out vec4 FragColor;

in Saida {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} saida;

uniform sampler2D DiffuseMap;
uniform sampler2D NormalMap;


void main()
{           
     // obtain normal from normal map in range [0,1]
    vec3 normal = texture(NormalMap, saida.TexCoords).rgb;
    // transform normal vector to range [-1,1]
    normal = normalize(normal * 2.0 - 1.0);  // this normal is in tangent space
   
    // get diffuse color
	vec4 diffu = texture(DiffuseMap, saida.TexCoords);
	if(diffu.a <= 0.1)
		discard;

	float gamma = 1.0;
    vec3 color = pow(diffu.rgb, vec3(gamma));

    // ambient
    vec3 ambient = 0.1 * color;
    // diffuse
    vec3 lightDir = normalize(saida.TangentLightPos - saida.TangentFragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;
    // specular
    vec3 viewDir = normalize(saida.TangentViewPos - saida.TangentFragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

    vec3 specular = vec3(0.2) * spec;
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}