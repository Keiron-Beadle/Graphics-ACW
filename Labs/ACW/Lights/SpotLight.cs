using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Lights
{
    class SpotLight : Light
    {
        public SpotLight(LightProperties pProperties, int pShaderID, float cutOffAngleRad)
            :base(pProperties, pShaderID)
        {

        }

        protected override void UpdateULight(Matrix4 pViewMat, int pIndex)
        {
            throw new NotImplementedException();
        }
    }
}
