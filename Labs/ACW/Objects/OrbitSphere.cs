using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Objects
{
    class OrbitSphere : Model
    {
        public OrbitSphere(Vector3 pPosition, Vector3 pScale, int pShaderID, int pVAO_ID, string pFileLoc, Material pMaterial, Object pParent) 
            :base(pPosition, pScale, Vector3.Zero, pShaderID, pVAO_ID, pFileLoc, pMaterial, pParent)
        {

        }

        public override void Update(Camera pActiveCam, double pDeltaTime)
        {
            Matrix4 translation = Matrix4.CreateTranslation(parent.LocalTransform.ExtractTranslation());
            Matrix4 rotation = Matrix4.CreateRotationY(1.14f * (float)pDeltaTime);
            Matrix4 backTranslation = Matrix4.CreateTranslation(-parent.LocalTransform.ExtractTranslation());
            mLocalTransform *= backTranslation * rotation * translation;
        }
    }
}
