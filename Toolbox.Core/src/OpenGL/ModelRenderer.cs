using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Core.OpenGL
{
    /// <summary>
    /// Represents a renderable model.
    /// </summary>
    public class ModelRenderer
    {
        /// <summary>
        /// The main shader to use when rendering the model.
        /// </summary>
        public ShaderProgram ShaderProgram;

        public List<MeshRender> Meshes = new List<MeshRender>();

        public STGenericModel Model;

        public ModelRenderer(STGenericModel model) {
            Model = model;

            foreach (var mesh in Model.Meshes)
                Meshes.Add(new MeshRender(mesh));
        }

        public virtual void OnRender(Camera camera)
        {
            if (ShaderProgram == null)
                PrepareShaders();

            Matrix4 mtxMdl = camera.ModelMatrix;
            Matrix4 mtxCam = camera.ViewMatrix * camera.ProjectionMatrix;

            ShaderProgram.Enable();
            ShaderProgram.SetVector4("highlight_color", Vector4.Zero);
            ShaderProgram.SetMatrix4x4("mtxMdl", ref mtxMdl);
            ShaderProgram.SetMatrix4x4("mtxCam", ref mtxCam);
            Console.WriteLine($"OnRender");
        }

        public virtual void ReloadUniforms(ShaderProgram shader)
        {

        }

        public virtual void DrawModel(ShaderProgram shader)
        {
            foreach (var drawableMesh in Meshes)
            {
                foreach (var group in drawableMesh.Mesh.PolygonGroups)
                {
                    RenderMaterials(shader, drawableMesh.Mesh, group, group.Material);
                    OnMeshDraw(drawableMesh, group);
                }
            }
        }

        private void SetDefaultUniforms(ShaderProgram shader)
        {
            shader.SetBoolToInt("hasDiffuse", false);
        }

        public virtual void RenderMaterials(ShaderProgram shader, 
            STGenericMesh mesh,  STPolygonGroup group, STGenericMaterial material)
        {
            SetDefaultUniforms(shader);

            int textureUintID = 1;
            foreach (var textureMap in material.TextureMaps)
            {
                var tex = textureMap.GetTexture();
                if (textureMap.Type == STTextureType.Diffuse) {
                    shader.SetBoolToInt("hasDiffuse", true);
                }

                textureUintID++;
            }
        }

        public virtual void OnMeshDraw(MeshRender msh, STPolygonGroup group)
        {
            msh.vao.Enable();
            msh.vao.Use();
            GL.DrawElements(BeginMode.Triangles,
                group.Faces.Count,
                DrawElementsType.UnsignedInt,
                group.FaceOffset);
        }

        public virtual void PrepareShaders()
        {
            if (ShaderProgram != null)
                return;

            ShaderProgram = new ShaderProgram(
                new VertexShader(VertexShaderBasic),
                new FragmentShader(FragmentShaderBasic));
        }

        private static string FragmentShaderBasic = @"
            #version 330

            uniform vec4 highlight_color;

            //Samplers
            uniform sampler2D tex_Diffuse;

            uniform int hasDiffuse;

            in vec2 f_texcoord0;
            in vec3 fragPosition;

            in vec4 vertexColor;
            in vec3 normal;

            out vec4 fragOutput;

            void main(){
                vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
                float hc_a   = highlight_color.w;

                vec4 color = vec4(1);
                if (hasDiffuse == 1)
                    color = texture(tex_Diffuse,f_texcoord0);

                float halfLambert = max(displayNormal.y,0.5);
                vec4 colorComb = vec4(color.rgb * (1-hc_a) + highlight_color.rgb * hc_a, color.a);

                 fragOutput = vec4(displayNormal, 1) * colorComb * vertexColor;
            }";

        private static string VertexShaderBasic = @"
            #version 330

            layout(location = 0) in vec3 vPosition;
            layout(location = 1) in vec3 vNormal;
            layout(location = 2) in vec2 vTexCoord;
            layout(location = 3) in vec4 vColor;
            layout(location = 4) in vec4 vBone;
            layout(location = 5) in vec4 vWeight;

            uniform mat4 mtxMdl;
            uniform mat4 mtxCam;

            out vec2 f_texcoord0;
            out vec4 vertexColor;
            out vec3 normal;

            void main(){
                f_texcoord0 = vTexCoord;
                vertexColor = vColor;
                normal = vNormal;

                gl_Position = mtxCam*mtxMdl*vec4(vPosition.xyz, 1);
            }";
    }

    public class MeshRender
    {
        public VertexArrayObject vao;
        public STGenericMesh Mesh;

        int indexBuffer;
        int vaoBuffer;

        public MeshRender(STGenericMesh mesh) {
            Mesh = mesh;
        }

        public void Initialize()
        {
            int[] buffers = new int[2];
            GL.GenBuffers(2, buffers);

            indexBuffer = buffers[0];
            vaoBuffer = buffers[1];

            UpdateVertexBuffer();

            vao = new VertexArrayObject(vaoBuffer, indexBuffer);
            vao.AddAttribute(0, 3, VertexAttribPointerType.Float, false, 68, 0);
            vao.AddAttribute(1, 3, VertexAttribPointerType.Float, false, 68, 12);
            vao.AddAttribute(2, 2, VertexAttribPointerType.Float, false, 68, 24);
            vao.AddAttribute(3, 4, VertexAttribPointerType.UnsignedByte, true, 68, 32);
            vao.AddAttribute(4, 4, VertexAttribPointerType.Int, false, 68, 36);
            vao.AddAttribute(4, 4, VertexAttribPointerType.Float, false, 68, 52);

            vao.Initialize();
        }

        private void UpdateVertexBuffer()
        {
            int[] indexData = CreateIndexBuffer(Mesh);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexData.Length * sizeof(int), indexData, BufferUsageHint.StaticDraw);

            float[] data = CreateVertexBuffer(Mesh);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vaoBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

        }

        private int[] CreateIndexBuffer(STGenericMesh mesh)
        {
            List<int> indices = new List<int>();
            foreach (var poly in mesh.PolygonGroups) {
                for (int f = 0; f < poly.Faces.Count; f++)
                    indices.Add((int)poly.Faces[f]);
            }
            return indices.ToArray();
        }

        private float[] CreateVertexBuffer(STGenericMesh mesh)
        {
            List<float> list = new List<float>();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                list.Add(mesh.Vertices[i].Position.X);
                list.Add(mesh.Vertices[i].Position.Y);
                list.Add(mesh.Vertices[i].Position.Z);
                list.Add(mesh.Vertices[i].Normal.X);
                list.Add(mesh.Vertices[i].Normal.Y);
                list.Add(mesh.Vertices[i].Normal.Z);
                for (int t = 0; t < 1; t++)
                {
                    if (mesh.Vertices[i].TexCoords.Length > t) {
                        list.Add(mesh.Vertices[i].TexCoords[t].X);
                        list.Add(mesh.Vertices[i].TexCoords[t].Y);
                    }
                    else {
                        list.Add(0);
                        list.Add(0);
                    }
                }

                Vector4 color = new Vector4(255,255,255,255);
                list.Add(BitConverter.ToSingle(new byte[4]
                {
                    (byte)color.X,
                    (byte)color.Y,
                    (byte)color.Z,
                    (byte)color.W
                }, 0));

                for (int j = 0; j < 4; j++)
                {
                    if (mesh.Vertices[i].BoneIndices.Count > j)
                        list.Add(mesh.Vertices[i].BoneIndices[j]);
                    else
                        list.Add(-1);
                }
                for (int j = 0; j < 4; j++)
                { 
                    if (mesh.Vertices[i].BoneWeights.Count > j)
                        list.Add(mesh.Vertices[i].BoneWeights[j]);
                    else
                        list.Add(0);
                }
            }
            return list.ToArray();
        }
    }
}
