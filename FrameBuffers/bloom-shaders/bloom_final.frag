#version 460 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;

uniform float exposure;
uniform float gamma;

uniform float elapsedTime;
uniform float film_grain;


vec3 FilmGrain(vec3 hrdcolor);

vec3 bloom()
{
    vec3 hdrColor = texture(scene, TexCoords).rgb;      
    vec3 bloomColor = texture(bloomBlur, TexCoords).rgb;
    hdrColor += bloomColor; // additive blending
    return hdrColor;
}
void main()
{             
    
    vec3 result = bloom();
    // tone mapping

    result = vec3(1.0) - exp(-FilmGrain(result) * exposure);
    // also gamma correct while we're at it       
    result = pow(result, vec3(1.0 / gamma));
    FragColor = vec4(result, 1.0);
}

vec3 FilmGrain(vec3 hrdcolor)
{
    float amount = film_grain;
    const float PI = 3.14159265359;


    float randomIntensity =
    fract ( 10000 * sin ( ( (TexCoords.x * gl_FragCoord.x) + (TexCoords.y * gl_FragCoord.y)* elapsedTime) * PI) );

    amount *= randomIntensity;

    hrdcolor.rgb += amount;

    return hrdcolor;
}