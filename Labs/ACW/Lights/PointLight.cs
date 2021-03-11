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

            int uAmbientPosition = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].AmbientLight");
            int uDiffusePosition = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].DiffuseLight");
            int uSpecularPosition = GL.GetUniformLocation(shaderProgramID, "uLight[" + pIndex + "].SpecularLight");
            GL.Uniform3(uAmbientPosition, properties.AmbientLight);
            GL.Uniform3(uDiffusePosition, properties.DiffuseLight);
            GL.Uniform3(uSpecularPosition, properties.SpecularLight);
        }
    }
}
