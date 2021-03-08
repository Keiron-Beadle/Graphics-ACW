#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

in vec4 vPosition;
in vec3 vNormal;

out vec3 oNormal;
out vec4 oSurfacePosition;

void main()
{
	gl_Position = vPosition * uModel * uView * uProjection;
	oSurfacePosition = vPosition * uModel * uView;
	oNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
}