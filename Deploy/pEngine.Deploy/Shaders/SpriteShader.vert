#version 330 core

// - Inputs
layout(location = 1) in vec2 Vertex;
layout(location = 2) in vec2 TexCoord;
layout(location = 3) in vec4 Color;

// - Visibility & effects
uniform float Opacity;
uniform float Brightness;
uniform float Contrast;

uniform mat4 ModelView;

// - Outputs
out vec4 FragVertexColor;
out vec2 FragTextCoord;
out float FragOpacity;
out float FragBrightness;
out float FragContrast;

void main() 
{
	FragVertexColor = Color;
	FragTextCoord = TexCoord;
	FragOpacity = Opacity;
	FragBrightness = Brightness;
	FragContrast = Contrast;

	gl_Position = ModelView * vec4(Vertex, 1, 1);
}