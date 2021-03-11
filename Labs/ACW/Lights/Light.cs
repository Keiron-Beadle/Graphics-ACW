using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW.Lights
{
    abstract class Light
    {
        protected int shaderProgramID;
        protected Vector4 lightPosition;

        public Light(Vector4 pPosition, int pShaderID)
        {
            lightPosition = pPosition;
            shaderProgramID = pShaderID;
        }
        
        public void Update(Camera pActiveCam)
        {
            UpdateULight(pActiveCam.ViewMatrix);
        }

        protected abstract void UpdateULight(Matrix4 pViewMat);

    }
}
