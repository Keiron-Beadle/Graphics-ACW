#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

in vec4 vPosition;
in vec3 vColour;

out vec4 oColour;

void main()
{
	gl_Position = vPosition * uModel * uView * uProjection;
	oColour = vec4(vColour, 1);
}