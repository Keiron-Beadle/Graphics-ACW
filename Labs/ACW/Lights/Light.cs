using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Lights
{
    class Light
    {
        public struct LightProperties
        {
            public Vector4 Position;
            public Vector3 AmbientLight;
            public Vector3 DiffuseLight;
            public Vector3 SpecularLight;
            public float Constant;
            public float Linear;
            public float Quadratic;
            public float Cutoff;
            public Vector4 SpotLightDirection;
        }

        private int[] shaderIDs;
        private LightProperties properties;

        public Light(LightProperties pProperties, int[] pShaderIDs)
        {
            properties = pProperties;
            shaderIDs = pShaderIDs;
        }
        
        public void Update(Camera pActiveCam, int pIndex)
        {
            UpdateULight(pActiveCam.ViewMatrix, pIndex);
        }

        protected void UpdateULight(Matrix4 pViewMat, int pIndex)
        {
            for (int i = 0; i < shaderIDs.Length; i++)
            {
                Vector4 lightPos = Vector4.Transform(properties.Position, pViewMat);
                Vector4 spotLightDirection = Vector4.Transform(properties.SpotLightDirection, pViewMat);
                int uLightPosition = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].Position");
                GL.Uniform4(uLightPosition, lightPos);

                int uAmbientLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].AmbientLight");
                int uDiffuseLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].DiffuseLight");
                int uSpecularLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].SpecularLight");
                int uConstantLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].constant");
                int uLinearLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].linear");
                int uQuadraticLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].quadratic");
                int uCutoffLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].cutoff");
                int uSpotDirLocation = GL.GetUniformLocation(shaderIDs[i], "uLight[" + pIndex + "].spotLightDirection");
                GL.Uniform3(uAmbientLocation, properties.AmbientLight);
                GL.Uniform3(uDiffuseLocation, properties.DiffuseLight);
                GL.Uniform3(uSpecularLocation, properties.SpecularLight);
                GL.Uniform1(uConstantLocation, properties.Constant);
                GL.Uniform1(uLinearLocation, properties.Linear);
                GL.Uniform1(uQuadraticLocation, properties.Quadratic);
                GL.Uniform1(uCutoffLocation, properties.Cutoff);
                GL.Uniform4(uSpotDirLocation, spotLightDirection);
            }
        }

        public static LightProperties MakeLightPropertes(Vector4 pPosition, Vector3 pAmbientLight,
            Vector3 pDiffuseLight, Vector3 pSpecularLight)
        {
            return new LightProperties
            {
                Position = pPosition,
                AmbientLight = pAmbientLight,
                DiffuseLight = pDiffuseLight,
                SpecularLight = pSpecularLight,
                Constant = 1.0f,
                Linear = 0.35f,
                Quadratic = 0.84f,
                Cutoff = -1,
                SpotLightDirection = Vector4.Zero
            };
        }

        public static LightProperties MakeLightPropertes(Vector4 pPosition, Vector3 pAmbientLight,
            Vector3 pDiffuseLight, Vector3 pSpecularLight, float pCutOff, Vector4 pSpotDir)
        {
            return new LightProperties
            {
                Position = pPosition,
                AmbientLight = pAmbientLight,
                DiffuseLight = pDiffuseLight,
                SpecularLight = pSpecularLight,
                Constant = 1.0f,
                Linear = 0.35f,
                Quadratic = 0.84f,
                Cutoff = pCutOff,
                SpotLightDirection = pSpotDir
            };
        }

    }
}
