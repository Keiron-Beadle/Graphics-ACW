using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    class Camera
    {
        private Matrix4 mView;
        private Matrix4 mProjection;
        private Vector4 eyePosition;
        int uView;

        public Camera(Vector3 inPosition, float clientWidth, float clientHeight, int shaderProgramID)
        {
            eyePosition = new Vector4(inPosition,1);
            mProjection = Matrix4.CreatePerspectiveFieldOfView(1, clientWidth / clientHeight, 0.01f, 50f);
            mView = Matrix4.CreateTranslation(inPosition);
            uView = GL.GetUniformLocation(shaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);
        }

        public Matrix4 ProjectionMatrix { get { return mProjection; } }

        public void Update()
        {

        }
    }
}
