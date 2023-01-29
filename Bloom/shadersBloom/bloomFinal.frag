#version 460 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;

uniform float bloomStrength = 0.04f;
uniform float exposure;
uniform float gamma;

uniform float elapsedTime;
uniform float film_grain;

uniform float nitidezStrength;

uniform bool activeNegative = false;

uniform float vibrance = 1.0;




// Nitidez
vec3 Nitidez()
{
    float offset = nitidezStrength / 300.0;

    vec2 offsets[9] = vec2[](
        vec2(-offset,  offset), // top-left
        vec2( 0.0f,    offset), // top-center
        vec2( offset,  offset), // top-right
        vec2(-offset,  0.0f),   // center-left
        vec2( 0.0f,    0.0f),   // center-center
        vec2( offset,  0.0f),   // center-right
        vec2(-offset, -offset), // bottom-left
        vec2( 0.0f,   -offset), // bottom-center
        vec2( offset, -offset)  // bottom-right    
    );

    float kernel[9] = float[](
        -1, -1, -1,
        -1,  9, -1,
        -1, -1, -1
    );
    
    vec3 sampleTex[9];
    for(int i = 0; i < 9; i++)
    {
        vec3 hdrColor = vec3(texture(scene, TexCoords.st + offsets[i]));
        vec3 bloomColor = vec3(texture(bloomBlur, TexCoords.st + offsets[i]));

        sampleTex[i] = mix(hdrColor, bloomColor, bloomStrength); // linear interpolation

    }
    vec3 col = vec3(0.0);
    for(int i = 0; i < 9; i++)
        col += sampleTex[i] * kernel[i];
    
    return col;

}

vec3 GrayScale(vec3 texscreen)
{
    const vec3 grayChannels = vec3(1.0);
    float average = grayChannels.x * texscreen.r + grayChannels.y * texscreen.g + grayChannels.z * texscreen.b;
    vec3 resultTex = vec3(average, average, average);
    return mix(texscreen, resultTex, vibrance);
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
vec3 NegativeScreen(vec3 texscreen)
{
    const vec3 negative = vec3(1.0);

    float average = (negative.x - texscreen.r) + (negative.y - texscreen.g) + (negative.z - texscreen.b);
    return vec3(average, average, average);
}

void main()
{
    // to bloom or not to bloom
    vec3 result = Nitidez();

    result = GrayScale(result);

    if(activeNegative)
    {
        result = NegativeScreen(result);
    }
    // tone mapping
    result = vec3(1.0) - exp( -FilmGrain(result) * exposure);
    
    // also gamma correct while we're at it
    result = pow(result, vec3(1.0 / gamma));
    FragColor = vec4(result, 1.0);
}
