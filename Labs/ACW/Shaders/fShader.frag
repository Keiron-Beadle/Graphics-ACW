﻿#version 330

uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

out vec4 FragColour;

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform LightProperties uLight;

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

void main()
{
	vec4 lightDir = normalize(uLight.Position - oSurfacePosition);

	vec4 eyeDir = normalize(uEyePosition - oSurfacePosition);

	vec4 reflectedVector = reflect(-lightDir, oNormal);

	float specularFactor = pow(max(dot(reflectedVector, eyeDir), 0.0), uMaterial.Shininess * 128.0);
	float diffuseFactor = max(dot(oNormal,lightDir), 0);
	float ambientFactor = 0.05;

	FragColour = vec4(uLight.AmbientLight * uMaterial.AmbientReflectivity +
				uLight.DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor +
				uLight.SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);
}