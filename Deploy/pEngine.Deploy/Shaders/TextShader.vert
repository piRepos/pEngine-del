#version 330 core

// - Inputs
in vec2 Vertex;
in vec4 Color;
in vec2 TexCoord;

uniform vec4 OutlineColor;

uniform float ContentMaskBit;
uniform float OutlineMaskBit;

uniform mat4 ModelView;

// - Outputs
out vec2 FragTextCoord;
out vec4 FragVertexColor;
out vec4 FragTextColor;
out vec4 FragOutlineColor;
out float FragContentMaskBit;
out float FragOutlineMaskBit;

void main() 
{
	FragTextCoord = TexCoord;
	FragVertexColor = Color;

	FragOutlineColor = OutlineColor;

	FragContentMaskBit = ContentMaskBit;
	FragOutlineMaskBit = OutlineMaskBit;

	gl_Position = ModelView * vec4(Vertex, 1, 1);
}