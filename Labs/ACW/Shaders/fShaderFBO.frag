#version 330 core
out vec4 FragColour;

in vec2 oTexCoords;

uniform int defaultMode = 1;
uniform int invertMode;
uniform int blackAndWhiteMode;
uniform sampler2D screenTexture;

void main()
{
    if (defaultMode != 0)
	{
		FragColour = texture(screenTexture, oTexCoords); //normal	
	}
	else if (invertMode != 0)
	{
		FragColour = vec4(vec3(1.0 - texture(screenTexture, oTexCoords)), 1.0); //inverse
	}
	else if (blackAndWhiteMode != 0)
	{
		FragColour = texture(screenTexture, oTexCoords);
		float average = 0.2126 * FragColour.r + 0.7152 * FragColour.g + 0.0722 * FragColour.b; //Black and white
		FragColour = vec4(average, average, average, 1.0);
	}
	else{
		FragColour = texture(screenTexture, oTexCoords); //normal	
	}

}