#version 460 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform float elapsedTime;

uniform float grainAmount;
uniform float shineValue;

uniform sampler2D lensTexture;
uniform float interp;


vec4 FilmGrain();
vec4 Shine();

void main()
{
    vec4 result = FilmGrain();
    vec4 lens = texture(lensTexture, TexCoords);
    result = mix(result, lens, interp);
    FragColor = result;
        
}
vec4 Shine()
{
    vec2 texSize   = textureSize(screenTexture, 0).xy;

    float neighbor = shineValue * -1.0;
    float center   = shineValue *  8.0 + 1.0;

    vec3 color =
          texture(screenTexture, (gl_FragCoord.xy + vec2( 0,  1)) / texSize).rgb
        * neighbor

        + texture(screenTexture, (gl_FragCoord.xy + vec2(-1,  0)) / texSize).rgb
        * neighbor
        + texture(screenTexture, (gl_FragCoord.xy + vec2( 0,  0)) / texSize).rgb
        * center
        + texture(screenTexture, (gl_FragCoord.xy + vec2( 1,  0)) / texSize).rgb
        * neighbor

        + texture(screenTexture, (gl_FragCoord.xy + vec2( 0, -1)) / texSize).rgb
        * neighbor
        ;

    return vec4(color, texture(screenTexture, TexCoords).a);
}
vec4 FilmGrain()
{

    float amount = grainAmount;
    const float PI = 3.14159265359;
    
    vec4 color = Shine();

    float randomIntensity =
    fract ( 10000 * sin ( ( (TexCoords.x * gl_FragCoord.x) + (TexCoords.y * gl_FragCoord.y)* elapsedTime) * PI) );

    amount *= randomIntensity;

    color.rgb += amount;

    return color;
}