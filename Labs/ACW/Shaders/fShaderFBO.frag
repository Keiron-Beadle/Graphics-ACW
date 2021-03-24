#version 330 core
out vec4 FragColour;

in vec2 oTexCoords;

uniform sampler2D screenTexture;

void main()
{
	FragColour = texture(screenTexture, oTexCoords);
}