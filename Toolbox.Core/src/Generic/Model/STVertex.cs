using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a single vertex point capable of storing vertex information such as 
    /// position, normal, texture coordinates, and more.
    /// </summary>
    public class STVertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2[] TexCoords { get; set; }
        public Vector4[] Colors { get; set; }
        public Vector4 Tangent { get; set; }
        public Vector4 Bitangent { get; set; }

        public List<float> BoneWeights = new List<float>();
        public List<int> BoneIndices = new List<int>();
        public List<string> BoneNames = new List<string>();

        public STVertex()
        {
            TexCoords = new Vector2[0];
            Colors = new Vector4[0]; 
        }

        public float DistanceTo(STVertex vertex)
        {
            float deltaX = vertex.Position.X - Position.X;
            float deltaY = vertex.Position.Y - Position.Y;
            float deltaZ = vertex.Position.Z - Position.Z;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
    }

    /// <summary>
    /// Represents a bone weight used for a vertex
    /// storing bone and weight rigging information.
    /// </summary>
    public class BoneWeight
    {
        public string Bone { get; set; }
        public int Index { get; set; }
        public float Weight { get; set; }
    }
}
