#version 330 core

// Texture uniform
uniform sampler2D Texture;

// Input variables
in vec2 FragTextCoord;
in vec4 FragVertexColor;

// Parameters
in float FragOpacity;
in float FragBrightness;
in float FragContrast;

// Color output
out vec4 FinalColor;

void main() 
{
	vec4 Col = texture(Texture, FragTextCoord);

	// Grab color
	vec4 PixelColor = Col;
	PixelColor.rgb /= PixelColor.a;

	// Apply contrast.
	PixelColor.rgb = ((PixelColor.rgb - 0.5f) * max(FragContrast, 0)) + 0.5f;

	// Apply brightness.
	PixelColor.rgb += FragBrightness;

	// Return final pixel color.
	PixelColor.rgb *= PixelColor.a;

	// Apply user opacity
	PixelColor.a *= FragOpacity;

	// Blend with a color
    FinalColor = PixelColor * FragVertexColor;

}