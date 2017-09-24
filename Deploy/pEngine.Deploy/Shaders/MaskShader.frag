#version 330 core

// Texture uniform
uniform sampler2D Textures[16];

// Input variables
in vec2 FragTextCoord;
in vec4 FragVertexColor;

// Parameters
flat in int FragOperations[15];
flat in int FragTextureCount;

// Color output
out vec4 OutputColor;

void main() 
{
	vec4 finalColor = texture(Textures[0], FragTextCoord);

	for (int i = 1; i < FragTextureCount + 1; i++)
	{
		vec4 MaskColor = texture(Textures[i], FragTextCoord);

		float alphaChannel = MaskColor.a;

		if (int(FragOperations[i - 1]) == 0)
			continue;
		else if (int(FragOperations[i - 1]) == 2)
			alphaChannel = 1 - alphaChannel;

		finalColor.a *= alphaChannel;
	}

	if (finalColor.a <= 0)
		discard;

    OutputColor = finalColor;

}