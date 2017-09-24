#version 330 core

// - Inputs
layout(location = 1) in vec2 Vertex;
layout(location = 2) in vec2 TexCoord;
layout(location = 3) in vec4 Color;

// - Layer mask operations
uniform int Operations[15];
uniform int TextureCount;

uniform mat4 ModelView;

// - Outputs
flat out int FragOperations[15];
flat out int FragTextureCount;
out vec4 FragVertexColor;
out vec2 FragTextCoord;

void main() 
{
	FragVertexColor = Color;
	FragTextCoord = TexCoord;
	FragOperations = Operations;
	FragTextureCount = TextureCount;

	gl_Position = ModelView * vec4(Vertex, 1, 1);
}