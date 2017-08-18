#version 330 core

// - Inputs
layout(location = 1) in vec2 Vertex;
layout(location = 2) in vec2 TexCoord;
layout(location = 3) in vec4 Color;

uniform mat4 ModelView;

// - Outputs
out vec4 FragVertexColor;
out vec2 FragTextCoord;

void main() 
{
	FragVertexColor = Color;
	FragTextCoord = TexCoord;

	gl_Position = ModelView * vec4(Vertex, 1, 1);
}