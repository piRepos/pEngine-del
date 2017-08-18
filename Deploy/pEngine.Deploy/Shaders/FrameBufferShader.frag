#version 330 core

// Texture uniform
uniform sampler2D Texture;

// Input variables
in vec2 FragTextCoord;
in vec4 FragVertexColor;

// Color output
out vec4 FinalColor;

void main() 
{
	vec4 Col = texture(Texture, FragTextCoord);

	// Blend with a color
    FinalColor = Col;

}