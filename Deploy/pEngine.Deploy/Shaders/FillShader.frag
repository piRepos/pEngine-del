#version 330 core

// Texture uniform
uniform sampler2D Texture;

// Input variables
in vec4 VertexColor;
in vec2 FragTexCoord;

// Color output
out vec4 FinalColor;

void main() 
{
    FinalColor = VertexColor;
}