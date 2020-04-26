using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a model which stores multiple meshes <see cref="STGenericMesh"/>
    /// and multiple materials <see cref="STGenericMaterial"/> and
    /// a <see cref="STSkeleton"/>.
    /// </summary>
    public class STGenericModel
    {
        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of <see cref="STGenericMesh"/> used for rendering and exporting mesh data.
        /// editing, and exporting meshes.
        /// </summary>
        public List<STGenericMesh> Meshes = new List<STGenericMesh>();

        /// <summary>
        /// A list of <see cref="STGenericMaterial"/> used for rendering and exporting material data. 
        /// editing, and exporting materials.
        /// </summary>
        public List<STGenericMaterial> Materials = new List<STGenericMaterial>();

        /// <summary>
        /// A list of <see cref="STGenericTexture"/> used for rendering and exporting textures.
        /// </summary>
        public List<STGenericTexture> Textures = new List<STGenericTexture>();

        /// <summary>
        /// The skeleton of the model used to store a list of <see cref="STBone"/>.
        /// Used for rendering and exporting bone data.
        /// </summary>
        public STSkeleton Skeleton = new STSkeleton();

        public STGenericModel(string name) {
            Name = name;
        }
    }
}
