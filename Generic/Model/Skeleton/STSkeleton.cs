using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a skeleton which stores multiple <see cref="STBone"/>.
    /// This is used for rendering, editing and exporting a skeleton with its bones.
    /// </summary>
    public class STSkeleton
    {
        /// <summary>
        /// A list of bones attatched to the skeleton.
        /// </summary>
        public List<STBone> Bones = new List<STBone>();

        /// <summary>
        /// Resets the current pose back to the original state.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                Bones[i].AnimationController.Position = new Vector3(
                    Bones[i].Position.X,
                    Bones[i].Position.Y,
                    Bones[i].Position.Z);
                Bones[i].AnimationController.Rotation = new Quaternion(
                    Bones[i].Rotation.X,
                    Bones[i].Rotation.Y,
                    Bones[i].Rotation.Z,
                    Bones[i].Rotation.W);
                Bones[i].AnimationController.Scale = new Vector3(
                    Bones[i].Scale.X,
                    Bones[i].Scale.Y,
                    Bones[i].Scale.Z);
            }

            Update();

            for (int i = 0; i < Bones.Count; i++)
            {
                try
                {
                    Bones[i].Inverse = Matrix4.Invert(Bones[i].Transform);
                }
                catch (InvalidOperationException)
                {
                    Bones[i].Inverse = Matrix4.Zero;
                }
            }
        }

        private bool Updated = false;

        /// <summary>
        /// Updates the current pose.
        /// </summary>
        public void Update()
        {
            Updated = true;
            foreach (STBone Bone in Bones)
                Bone.Transform = GetMatrix(Bone);
        }

        private Matrix4 GetMatrix(STBone bone) {
            var transform =
                Matrix4.CreateScale(bone.AnimationController.Scale) *
                Matrix4.CreateFromQuaternion(bone.AnimationController.Rotation) *
                Matrix4.CreateTranslation(bone.AnimationController.Position);

            Console.WriteLine($"Bone {bone.Name} parent {bone.ParentIndex} transform {transform}");

            if (bone.ParentIndex != -1)
                return transform * GetMatrix(bone.Parent);
            else
                return transform;
        }

        /// <summary>
        /// Gets the bone transform in world space by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Matrix4 GetBoneTransform(int index)
        {
            return GetBoneTransform(Bones[index]);
        }

        /// <summary>
        /// Gets the bone transform in world space.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public Matrix4 GetBoneTransform(STBone bone)
        {
            if (bone == null)
                return Matrix4.Identity;
            if (bone.ParentIndex == -1)
                return bone.GetTransform();
            else
                return bone.GetTransform() * GetBoneTransform(Bones[bone.ParentIndex]);
        }

        /// <summary>
        /// Searches for the bone by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public STBone SearchBone(string name)
        {
            for (int i = 0; i < Bones.Count; i++)
                if (Bones[i].Name == name)
                    return Bones[i];

            return null;
        }
    }
}
