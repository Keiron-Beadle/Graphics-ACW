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
    class PointLight : Light
    {
        public PointLight(LightProperties pProperties, int pShaderID) : base(pProperties, pShaderID)
        {

        }

        protected override void UpdateULight(Matrix4 pViewMat, int pIndex)
        {
            Vector4 lightPos = Vector4.Transform(properties.Position, pViewMat);
            int uLightPosition = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].Position");
            GL.Uniform4(uLightPosition, lightPos);

            int uAmbientLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].AmbientLight");
            int uDiffuseLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].DiffuseLight");
            int uSpecularLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].SpecularLight");
            int uConstantLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].constant");
            int uLinearLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].linear");
            int uQuadraticLocation = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].quadratic");
            GL.Uniform3(uAmbientLocation, properties.AmbientLight);
            GL.Uniform3(uDiffuseLocation, properties.DiffuseLight);
            GL.Uniform3(uSpecularLocation, properties.SpecularLight);
            GL.Uniform1(uConstantLocation, properties.Constant);
            GL.Uniform1(uLinearLocation, properties.Linear);
            GL.Uniform1(uQuadraticLocation, properties.Quadratic);
        }
    }
}
