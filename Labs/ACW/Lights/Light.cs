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
    abstract class Light
    {
        protected int shaderProgramID;
        protected Vector4 lightPosition;
        protected LightProperties properties;

        public Light(LightProperties pProperties, int pShaderID)
        {
            properties = pProperties;
            shaderProgramID = pShaderID;
        }
        
        public void Update(Camera pActiveCam, int pIndex)
        {
            UpdateULight(pActiveCam.ViewMatrix, pIndex);
        }

        protected abstract void UpdateULight(Matrix4 pViewMat, int pIndex);

    }
}
