#version 330 core

// Texture uniform
uniform sampler2D Texture;

// Input variables
in vec2 FragTextCoord;
in vec4 FragVertexColor;

in vec4 FragOutlineColor;

in float FragContentMaskBit;
in float FragOutlineMaskBit;

// Color output
out vec4 FinalColor;

void main() 
{
	vec4 TextureColor = texture(Texture, FragTextCoord);

	float ContentMask, OutlineMask;
	if (int(FragContentMaskBit) == 1)
		ContentMask = TextureColor.r;
	else if (int(FragContentMaskBit) == 2)
		ContentMask = TextureColor.g;
	else if (int(FragContentMaskBit) == 3)
		ContentMask = TextureColor.b;
	if (int(FragOutlineMaskBit) == 1)
		OutlineMask = TextureColor.r;
	else if (int(FragOutlineMaskBit) == 2)
		OutlineMask = TextureColor.g;
	else if (int(FragOutlineMaskBit) == 3)
		OutlineMask = TextureColor.b;

	float TextOpacity = ContentMask * TextureColor.a;
	float OutlineOpacity = OutlineMask * TextureColor.a;

	OutlineOpacity *= FragOutlineColor.a * (int(FragOutlineMaskBit) < 0 ? 0 : 1);
	TextOpacity *= FragVertexColor.a * (int(FragContentMaskBit) < 0 ? 0 : 1);

	FinalColor.a = min(TextOpacity + OutlineOpacity, 1.0);

	if (FragContentMaskBit < 0 && FragOutlineMaskBit < 0)
		FinalColor = TextureColor;

	if (FinalColor.a <= 0)
		discard;

	FinalColor.rgb = (FragVertexColor.rgb * TextOpacity) + (FragOutlineColor.rgb * OutlineOpacity);
}