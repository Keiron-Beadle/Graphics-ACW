#version 330

uniform vec4 uEyePosition;
uniform sampler2D uTextureSampler;

in vec4 oNormal;
in vec4 oSurfacePosition;
in vec2 oTexCoords;

out vec4 FragColour;

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
	float constant;
	float linear;
	float quadratic;
	vec4 spotLightDirection;
	float cutoff;
};

uniform LightProperties uLight[4];

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

void RunPointLight(int i, vec4 eyeDir){
		vec4 texColour = texture(uTextureSampler, oTexCoords);

		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, oNormal);

		float specularFactor = pow(max(dot(reflectedVector, eyeDir), 0.0), uMaterial.Shininess * 128.0);
		float diffuseFactor = max(dot(oNormal,lightDir), 0);
		float ambientFactor = 0.05;

		vec3 lightPos = vec3(uLight[i].Position);
		vec3 surfacePos = vec3(oSurfacePosition);
		float dist = length(lightPos - surfacePos);
		float attenuation = 1.0 / (uLight[i].constant + (uLight[i].linear * dist) + (uLight[i].quadratic * (dist * dist)));
		
		vec3 ambientTot = uLight[i].AmbientLight * uMaterial.AmbientReflectivity * texColour.xyz;
		vec3 diffuseTot = uLight[i].DiffuseLight * attenuation * uMaterial.DiffuseReflectivity * diffuseFactor * texColour.xyz;
		vec3 specularTot = uLight[i].SpecularLight * attenuation * uMaterial.SpecularReflectivity * specularFactor;
		FragColour = FragColour + vec4(ambientTot + diffuseTot + specularTot,1);

}

void RunSpotLight(int i, vec4 eyeDir){
	vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);

	float theta = dot(lightDir, normalize(-uLight[i].spotLightDirection));
	vec4 texColour = texture(uTextureSampler, oTexCoords);

	if (theta > uLight[i].cutoff){
	    //FragColour  = vec4(1,1,1,1);
		RunPointLight(i, eyeDir);
	}
	else{
		//FragColour = vec4(1,0,1,1);
		FragColour = FragColour + vec4(uLight[i].AmbientLight * uMaterial.AmbientReflectivity * texColour.xyz,1);
	}
}

void main()
{
	vec4 eyeDir = normalize(uEyePosition - oSurfacePosition);
	for (int i = 0; i < 4; ++i)
	{
		if (uLight[i].cutoff > 0.0)
		{
			//FragColour = vec4(1,0,0,1);
			RunSpotLight(i, eyeDir);
		}
		else
		{
			//FragColour = vec4(1,1,1,1);
			RunPointLight(i, eyeDir);
		}

	}
}