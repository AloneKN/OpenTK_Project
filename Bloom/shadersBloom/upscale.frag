#version 460 core

// This shader performs upsampling on a texture,
// as taken from Call Of Duty method, presented at ACM Siggraph 2014.

// Remember to add bilinear minification filter for this texture!
// Remember to use a floating-point texture format (for HDR)!
// Remember to use edge clamping for this texture!
uniform sampler2D srcTexture;
uniform float filterRadius;

in vec2 TexCoords;
layout (location = 0) out vec3 upsample;


// ghosts 
uniform sampler2D ghostTex;
uniform int uGhosts; // number of ghost samples
uniform float uGhostDispersal; // dispersion factor

vec3 Ghosts()
{
    vec2 texcoord = TexCoords + vec2(1.0);
    // vec2 texelSize = 1.0 / vec2(textureSize(ghostTex, 0));

    vec2 ghostVec = (vec2(0.5) - texcoord) * uGhostDispersal;

    vec3 result = vec3(0.0);
    for(int i = 0; i < uGhosts; i++)
    {
        vec2 offset = fract(texcoord + ghostVec * float(i));

        float weight = length(vec2(0.5) - offset) / length(vec2(0.5));
        weight = pow(1.0 - weight, 10.0);

        result += texture(ghostTex, offset).xyz * weight;
    }
    return result;
}

void main()
{
	// The filter kernel is applied with a radius, specified in texture
	// coordinates, so that the radius will vary across mip resolutions.
	float x = filterRadius;
	float y = filterRadius;

	// Take 9 samples around current texel:
	// a - b - c
	// d - e - f
	// g - h - i
	// === ('e' is the current texel) ===
	vec3 a = texture(srcTexture, vec2(TexCoords.x - x, TexCoords.y + y)).rgb;
	vec3 b = texture(srcTexture, vec2(TexCoords.x,     TexCoords.y + y)).rgb;
	vec3 c = texture(srcTexture, vec2(TexCoords.x + x, TexCoords.y + y)).rgb;

	vec3 d = texture(srcTexture, vec2(TexCoords.x - x, TexCoords.y)).rgb;
	vec3 e = texture(srcTexture, vec2(TexCoords.x,     TexCoords.y)).rgb;
	vec3 f = texture(srcTexture, vec2(TexCoords.x + x, TexCoords.y)).rgb;

	vec3 g = texture(srcTexture, vec2(TexCoords.x - x, TexCoords.y - y)).rgb;
	vec3 h = texture(srcTexture, vec2(TexCoords.x,     TexCoords.y - y)).rgb;
	vec3 i = texture(srcTexture, vec2(TexCoords.x + x, TexCoords.y - y)).rgb;

	// Apply weighted distribution, by using a 3x3 tent filter:
	//  1   | 1 2 1 |
	// -- * | 2 4 2 |
	// 16   | 1 2 1 |
	upsample = e*4.0;
	upsample += (b+d+f+h)*2.0;
	upsample += (a+c+g+i);
	upsample *= 1.0 / 16.0;
	upsample += Ghosts();
}