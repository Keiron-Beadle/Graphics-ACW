using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace Labs.ACW
{
    class Framebuffer
    {
        private int fbo_ID, fboTexID, fbo_RBO, fbo_VAO, fbo_VBO;
        private int shaderID;
        int defaultLocation;
        int invertLocation;
        int blackAndWhiteLocation;
        public int GetFBO_ID() { return fbo_ID; }
        public int GetFBO_RBO() { return fbo_RBO; }
        public Framebuffer(int pShaderID)
        {
            shaderID = pShaderID;
            defaultLocation = GL.GetUniformLocation(shaderID, "defaultMode");
            invertLocation = GL.GetUniformLocation(shaderID, "invertMode");
            blackAndWhiteLocation = GL.GetUniformLocation(shaderID, "blackAndWhiteMode");

            InitialiseRenderBuffer(1600,1200);

            float[] screenQuadVerts = new float[]
            {   //vPos       //vTexCoords
                -1.0f, 1.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 0.0f,
                1.0f, -1.0f, 1.0f, 0.0f,

                -1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, -1.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f
            };
            GL.GenVertexArrays(1, out fbo_VAO);
            GL.GenBuffers(1, out fbo_VBO);
            GL.BindVertexArray(fbo_VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, fbo_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * screenQuadVerts.Length), screenQuadVerts, BufferUsageHint.StaticDraw);
            int vPosLocation = GL.GetAttribLocation(shaderID, "vPosition");
            int vTexCoordLocation = GL.GetAttribLocation(shaderID, "vTexCoords");
            GL.EnableVertexAttribArray(vPosLocation);
            GL.VertexAttribPointer(vPosLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vTexCoordLocation);
            GL.VertexAttribPointer(vTexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public void InitialiseRenderBuffer(int pClientWidth, int pClientHeight)
        {
            //Generate framebuffer
            GL.GenFramebuffers(1, out fbo_ID);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo_ID);
            //Generate texture that scene will be saved onto
            GL.GenTextures(1, out fboTexID); 
            GL.BindTexture(TextureTarget.Texture2D, fboTexID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, pClientWidth, pClientHeight, 0,
                            PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, fboTexID, 0);
            //Generate the renderbuffer that scene is drawn to
            GL.GenRenderbuffers(1, out fbo_RBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, fbo_RBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, pClientWidth, pClientHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, fbo_RBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void OnKeyDown(KeyboardKeyEventArgs e)
        {
            int[] renderMode = new int[] { 0, 0, 0 };
            if (e.Key == Key.C)
            {
                renderMode[0] = 1;
                UpdateRenderMode(renderMode);
            }
            else if (e.Key == Key.X)
            {
                renderMode[1] = 1;
                UpdateRenderMode(renderMode);
            }
            else if (e.Key == Key.Z)
            {
                renderMode[2] = 1;
                UpdateRenderMode(renderMode);
            }
        }

        public void UpdateRenderMode(int[] pRenderMode)
        {
            GL.Uniform1(defaultLocation, pRenderMode[0]);
            GL.Uniform1(invertLocation, pRenderMode[1]);
            GL.Uniform1(blackAndWhiteLocation, pRenderMode[2]);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo_ID);
            GL.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(shaderID);
            GL.BindVertexArray(fbo_VAO);
            GL.BindTexture(TextureTarget.Texture2D, fboTexID);
        }

        public void Delete()
        {
            GL.DeleteTexture(fboTexID);
            GL.DeleteFramebuffer(fbo_ID);
            GL.DeleteRenderbuffer(fbo_RBO);
            GL.DeleteVertexArray(fbo_VAO);
            GL.DeleteBuffer(fbo_VBO);
        }
    }
}
