using Labs.ACW.Lights;
using Labs.ACW.Objects;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using static Labs.ACW.Lights.Light;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        private Camera staticCam, dynCam, ActiveCam;
        private int[] VAO_IDs = new int[11];
        private int[] texture_IDs = new int[2];
        private ShaderUtility shader, shader2;
        protected BitmapData TextureData;
        private int[] shaderIDs;
        private Cube cube, ground, wallL, wallR, wallF;
        private Tetrahedron tet;
        private Model werewolfModel;
        private Model[] orbitSpheres;
        List<Objects.Object> nonTexturedObjects = new List<Objects.Object>();
        List<Objects.Object> texturedObjects = new List<Objects.Object>();
        List<Light> lights = new List<Light>();

        private string[] textureFilePaths = { "ACW/Resources/woodTex.jpg", "ACW/Resources/stoneTex.jpg" };

        public ACWWindow()
            : base(
                1600, // Width
                1200, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        public struct Material
        {
            public Vector3 AmbientRef;
            public Vector3 DiffuseRef;
            public Vector3 SpecRef;
            public float Shininess;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            shader = new ShaderUtility(@"ACW/Shaders/vShader.vert", @"ACW/Shaders/fShader.frag");
            shader2 = new ShaderUtility(@"ACW/Shaders/vShaderNoTex.vert", @"ACW/Shaders/fShaderNoTex.frag");
            shaderIDs = new int[2];
            shaderIDs[0] = shader.ShaderProgramID;
            shaderIDs[1] = shader2.ShaderProgramID;
            orbitSpheres = new Model[4]; 
            GenerateCameras();

            GenerateLights();

            GL.GenVertexArrays(VAO_IDs.Length, VAO_IDs);

            GL.GenTextures(2, texture_IDs);
            LoadTexture(textureFilePaths[0], texture_IDs[0]);
            LoadTexture(textureFilePaths[1], texture_IDs[1]);
            GenerateEntities();

            GL.BindVertexArray(0);

            base.OnLoad(e);
        }

        private void GenerateEntities()
        {
            #region Materials
            Material groundMat = MakeMaterial(new Vector3(0f, 0.05f, 0f),
                new Vector3(0.3f, 0.3f, 0.3f),
                    new Vector3(0.05f, 0.05f, 0.05f), 0.078125f);

            Material cubeMat = MakeMaterial(new Vector3(0f, 0f, 0f),
                new Vector3(0.55f, 0.55f, 0.55f),
                    new Vector3(0.7f, 0.7f, 0.7f), 0.25f);
            #endregion

            #region NonTexturedObjects
            GL.UseProgram(shaderIDs[1]);

            cube = new Cube(new Vector3(0.5f, 0.25f, -0.2f), new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(1.1f, -0.1f, 1f), shaderIDs[1], VAO_IDs[0], cubeMat);
            cube.Updatable = true;
            nonTexturedObjects.Add(cube);

            tet = new Tetrahedron(new Vector3(-0.7f, 0f, 0f), new Vector3(0.2f, 0.2f, 0.2f),
                new Vector3(0f, 0, 0), shaderIDs[1], VAO_IDs[1], cubeMat);
            tet.Updatable = true;
            nonTexturedObjects.Add(tet);
            //mLocalTransform *= Matrix4.CreateScale(0.2f) * Matrix4.CreateRotationY(-1.55f);

            werewolfModel = new Model(new Vector3(-0.1f, 0.06f, 0f), new Vector3(0.2f), new Vector3(0,-1.65f,0), 
                        shaderIDs[1], VAO_IDs[2], "Utility/Models/model.bin", cubeMat);
            nonTexturedObjects.Add(werewolfModel);

            orbitSpheres[0] = new OrbitSphere(new Vector3(0.25f, 0f, 0f), new Vector3(0.05f), 
                shaderIDs[1], VAO_IDs[3], "Utility/Models/sphere.bin", cubeMat, werewolfModel);
            orbitSpheres[1] = new OrbitSphere(new Vector3(-0.25f, 0f, 0f), new Vector3(0.05f), 
                shaderIDs[1], VAO_IDs[4], "Utility/Models/sphere.bin", cubeMat, werewolfModel);
            orbitSpheres[2] = new OrbitSphere(new Vector3(0f, 0f, 0.25f), new Vector3(0.05f), 
                shaderIDs[1], VAO_IDs[5], "Utility/Models/sphere.bin", cubeMat, werewolfModel);
            orbitSpheres[3] = new OrbitSphere(new Vector3(0f, 0f, -0.25f), new Vector3(0.05f), 
                shaderIDs[1], VAO_IDs[6], "Utility/Models/sphere.bin", cubeMat, werewolfModel);
            for (int i = 0; i < orbitSpheres.Length; i++)
            {
                orbitSpheres[i].Updatable = true;
                nonTexturedObjects.Add(orbitSpheres[i]);
            }
            #endregion

            #region TexturedObjects
            GL.UseProgram(shaderIDs[0]);

            GL.ActiveTexture(TextureUnit.Texture0);
            ground = new Cube(new Vector3(0f, -0.2f, 0f), new Vector3(15f, 0.1f, 15f), 
                Vector3.Zero, shaderIDs[0], VAO_IDs[7], groundMat, texture_IDs[1]);
            texturedObjects.Add(ground);

            GL.ActiveTexture(TextureUnit.Texture1);
            wallL = new Cube(new Vector3(-1.1f, -0.6f, 0f), new Vector3(0.3f, 3f, 7f),
                new Vector3(0, 0, 0), shaderIDs[0], VAO_IDs[8], cubeMat, texture_IDs[0]);
            texturedObjects.Add(wallL);

            wallR = new Cube(new Vector3(1.1f, -0.6f, 0f), new Vector3(0.3f, 3f, 7f),
                 new Vector3(0, 0, 0), shaderIDs[0], VAO_IDs[9], cubeMat, texture_IDs[0]);
            texturedObjects.Add(wallR);

            wallF = new Cube(new Vector3(0f, -0.6f, -1f), new Vector3(7f, 3f, 0.3f),
                new Vector3(0, 0, 0), shaderIDs[0], VAO_IDs[10], cubeMat, texture_IDs[0]);
            texturedObjects.Add(wallF);
            #endregion
        }

        private void GenerateCameras()
        {
            staticCam = new Camera(new Vector3(-0.9f, 0.86f, -0.8f), new Vector3(0, 0, 0), this.Width, this.Height, shaderIDs);
            dynCam = new Camera(new Vector3(0, 0f, 2f), new Vector3(0,0,0), this.Width, this.Height, shaderIDs);
            dynCam.Active = true;
        }

        private void GenerateLights()
        {
            LightProperties p1 = MakeLightPropertes(new Vector4(-0.7f, 0.5f, -0.5f, 1), new Vector3(0.04f, 0.04f, 0.04f), 
                new Vector3(0.1804f, 0.85098f, 1f), new Vector3(0.045f, 0.2125f, 0.25f));
            LightProperties p2 = MakeLightPropertes(new Vector4(0f, 2f, -5f, 1), new Vector3(0f, 0f, 0f), 
                new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0f, 0, 0));
            LightProperties p3 = MakeLightPropertes(new Vector4(0.5f, 0.4f, 0.8f, 1), 
                new Vector3(0.04f, 0.04f, 0.04f), new Vector3(0.8219f, 0.03635f, 0.62274f), 
                new Vector3(0.055f, 0.06f, 0.25f));
            LightProperties spot = MakeLightPropertes(new Vector4(-0.1f,1,0,1), new Vector3(0.01f, 0.01f, 0.01f), 
                new Vector3(1f, 1f, 1f), new Vector3(0.01f,0.01f,0.01f), (float)Math.Cos(0.4187f), new Vector4(0,-1,0,0));
            lights.Add(new Light(p1, shaderIDs));
            lights.Add(new Light(p2, shaderIDs));
            lights.Add(new Light(p3, shaderIDs));
            lights.Add(new Light(spot, shaderIDs));
        }

        private void LoadTexture(string textureFilePath, int textureID)
        {
            Bitmap texBitmap;
            if (System.IO.File.Exists(textureFilePath))
            {
                texBitmap = new Bitmap(textureFilePath);
                texBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData texData = texBitmap.LockBits(new Rectangle(0, 0, texBitmap.Width, texBitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                TextureData = texData;
            }
            else
            {
                throw new Exception("Could not find file: " + textureFilePath);
            }
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            texBitmap.UnlockBits(TextureData);
            //texBitmap.Dispose();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            dynCam.OnKeyDown(e);
            if (e.Key == Key.O)
            {
                if (dynCam.Active) { dynCam.Active = false; staticCam.Active = true; }
                else { dynCam.Active = true; staticCam.Active = false; }
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);
            dynCam.Update();
            staticCam.Update();
            if (dynCam.Active) { ActiveCam = dynCam; }
            else { ActiveCam = staticCam; }
            for (int i = 0; i < lights.Count; i++) 
            {
                for (int j = 0; j < shaderIDs.Length; j++)
                {
                    GL.UseProgram(shaderIDs[j]);
                    lights[i].Update(ActiveCam, i);
                }
            }
            for (int i = 0; i < nonTexturedObjects.Count; i++)
            {
                if (nonTexturedObjects[i].Updatable)
                    nonTexturedObjects[i].Update(ActiveCam, e.Time);
            }
            for (int i = nonTexturedObjects.Count; i < texturedObjects.Count + nonTexturedObjects.Count; i++)
            {
                if (texturedObjects[i - nonTexturedObjects.Count].Updatable)
                    texturedObjects[i - nonTexturedObjects.Count].Update(ActiveCam, e.Time);
            }

            for (int i = 0; i < 900000; i++)
            {
                long j = i * i;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderIDs[1]);
            for (int i = 0; i < nonTexturedObjects.Count; i++)
            {
                GL.BindVertexArray(VAO_IDs[i]);
                nonTexturedObjects[i].RenderUpdate();
                nonTexturedObjects[i].Draw();
            }
            GL.UseProgram(shaderIDs[0]);
            for (int i = nonTexturedObjects.Count; i < texturedObjects.Count + nonTexturedObjects.Count; i++)
            {
                GL.BindVertexArray(VAO_IDs[i]);
                texturedObjects[i - nonTexturedObjects.Count].RenderUpdate();
                texturedObjects[i - nonTexturedObjects.Count].Draw();
            }

            GL.BindVertexArray(0);
            SwapBuffers();
        }

        private Material MakeMaterial(Vector3 pAmbientRef, Vector3 pDiffuseRef, Vector3 pSpecRef, float pShininess)
        {
            return new Material()
            {
                AmbientRef = pAmbientRef,
                DiffuseRef = pDiffuseRef,
                SpecRef = pSpecRef,
                Shininess = pShininess
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)Width / Height, 0.01f, 50f);
            for (int i = 0; i < shaderIDs.Length; i++)
            {
                int uProjectionLocation = GL.GetUniformLocation(shaderIDs[i], "uProjection");
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            for (int i = 0; i < nonTexturedObjects.Count; i++) { nonTexturedObjects[i].Dispose(); }
            for (int i = 0; i < texturedObjects.Count; i++) { texturedObjects[i].Dispose(); }
            GL.DeleteVertexArrays(VAO_IDs.Length, VAO_IDs);
            shader.Delete();
            shader2.Delete();
            base.OnUnload(e);
        }
    }
}
