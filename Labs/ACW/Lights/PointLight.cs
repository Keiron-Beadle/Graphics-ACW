using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW.Lights
{
    class PointLight : Light
    {
        public PointLight(Vector4 pPosition, int pShaderID) : base(pPosition, pShaderID)
        {

        }

        protected override void UpdateULight(Matrix4 pViewMat)
        {
            Vector4 lightPos = Vector4.Transform(lightPosition, pViewMat);
            int uLightPosition = GL.GetUniformLocation(shaderProgramID, "uLight.Position");
            GL.Uniform4(uLightPosition, lightPos);

            Vector3 ambient = new Vector3(0.0215f, 0.1745f, 0.0215f);
            Vector3 diffuse = new Vector3(0.07568f, 0.61424f, 0.07568f);
            Vector3 specular = new Vector3(0.633f, 0.727811f, 0.633f);
            int uAmbientPosition = GL.GetUniformLocation(shaderProgramID, "uLight.AmbientLight");
            int uDiffusePosition = GL.GetUniformLocation(shaderProgramID, "uLight.DiffuseLight");
            int uSpecularPosition = GL.GetUniformLocation(shaderProgramID, "uLight.SpecularLight");
            GL.Uniform3(uAmbientPosition, ambient);
            GL.Uniform3(uDiffusePosition, diffuse);
            GL.Uniform3(uSpecularPosition, specular);
        }
    }
}
