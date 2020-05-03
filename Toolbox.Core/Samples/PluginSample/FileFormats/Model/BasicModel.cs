using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Core.IO;
using Toolbox.Core;
using Toolbox.Core.OpenGL;
using OpenTK;

namespace PluginSample
{
    public class BasicModel : IFileFormat, IModelFormat
    {
        public bool CanSave { get; set; } = false;

        public string[] Description { get; set; } = new string[] { "Basic Model" };
        public string[] Extension { get; set; } = new string[] { "*.mdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public File_Info FileInfo { get; set; }

        public bool Identify(File_Info fileInfo, Stream stream)
        {
            return fileInfo.Extension == ".mdl";
        }

        public void Load(Stream stream)
        {
            using (var reader = new FileReader(stream))
            {

            }
        }

        public void Save(Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {

            }
        }

        public ModelRenderer Renderer => new ModelRenderer(ToGeneric());

        public STGenericModel ToGeneric()
        {
            STGenericModel model = new STGenericModel(FileInfo.FileName);

            //Make a new mesh and material and add to the model
            var mesh = new STGenericMesh() { Name = "Mesh_0" };
            var mat = new STGenericMaterial() { Name = "Mat_0" };
            model.Meshes.Add(mesh);
            model.Materials.Add(mat);

            //Bone example
            model.Skeleton = new STSkeleton();
            model.Skeleton.Bones.Add(new STBone(model.Skeleton)
            {
                Position = new Vector3(0, 0, 0),
                Scale = new Vector3(1, 1, 1),
                EulerRotation = new Vector3(0, 0, 0),
            });
            //Reset the skeleton to update the transforms for rendering
            model.Skeleton.Reset();

            //Meshes use polygon groups to map faces
            var group = new STPolygonGroup();
            group.Faces.AddRange(new uint[] { 0,1,2,3,4,5,6,7 });
            //Materials can link either directly or by index
            group.Material = mat;

            //Add vertices
            //Note this all won't draw a model yet, just a quick setup
            for (int i = 0; i < 8; i++)
            {
                mesh.Vertices.Add(new STVertex()
                {
                    Position = new Vector3(0,0,0),
                    Normal = new Vector3(1, 0, 0),

                    //Multiple of these can be added for extra channels
                    TexCoords = new Vector2[1] { new Vector2(0, 0) },
                    Colors = new Vector4[1] { Vector4.One },

                    //Rigging
                    BoneIndices = new List<int>() { 0 }, //Index of bone to rig to. -1 will have no bone rig
                    BoneWeights = new List<float>() { 1 } //0 - 1 for weight
                });
            }

            //Add a new texture map and make it diffuse
            mat.TextureMaps.Add(new STGenericTextureMap()
            {
                Name = "Grass_Alb",
                Type = STTextureType.Diffuse,
                MagFilter = STTextureMagFilter.Linear,
                MinFilter = STTextureMinFilter.Linear,
                WrapU = STTextureWrapMode.Repeat,
                WrapV = STTextureWrapMode.Repeat,
            });

            return model;
        }
    }
}
