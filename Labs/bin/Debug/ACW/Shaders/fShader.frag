#version 330

uniform vec4 uLightPosition;
uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLightPosition - oSurfacePosition);
	vec4 eyeDir = normalize(uEyePosition - oSurfacePosition);
	vec4 reflectedVector = reflect(-lightDir, oNormal);
	float specularFactor = pow(max(dot(reflectedVector, eyeDir), 0.0), 60);
	float diffuseFactor = max(dot(oNormal,lightDir), 0);
	float ambientFactor = 0.05;
	FragColour = vec4(vec3(ambientFactor + diffuseFactor + specularFactor),1);
}