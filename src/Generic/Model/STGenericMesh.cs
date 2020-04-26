using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using System.Linq;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a generic mesh used for rendering, exporting and editing geoemetry.
    /// These can optionally be attatched to a <see cref="STGenericModel"/>.
    /// </summary>
    public class STGenericMesh
    {
        /// <summary>
        /// Gets or sets the name of the mesh.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the total amount of faces in all polygon groups
        /// </summary>
        public List<uint> Faces
        {
            get
            {
                List<uint> faces = new List<uint>();
                for (int i = 0; i < PolygonGroups.Count; i++)
                    faces.AddRange(PolygonGroups[i].Faces);
                return faces;
            }
        }

        /// <summary>
        /// A list of polygon groups which store the faces and material indices of a mesh.
        /// </summary>
        public List<STPolygonGroup> PolygonGroups = new List<STPolygonGroup>();

        /// <summary>
        /// A list of <see cref="STVertex"/> which determine the 
        /// points of a mesh is rendered and displayed
        /// </summary>
        public List<STVertex> Vertices = new List<STVertex>();

        /// <summary>
        /// The bone index used for binding a bone to a mesh.
        /// </summary>
        public int BoneIndex { get; set; }

        /// <summary>
        /// The total amount of skinning a single vertex can use.
        /// </summary>
        public uint VertexSkinCount { get; set; }

        #region methods

        /// <summary>
        /// Gets a list of materials used by all the polygon groups.
        /// </summary>
        /// <returns></returns>
        public List<STGenericMaterial> GetMaterials()
        {
            List<STGenericMaterial> materials = new List<STGenericMaterial>();
            foreach (var group in PolygonGroups)
                if (group.Material != null)
                    materials.Add(group.Material);
            return materials;
        }

        /// <summary>
        /// Optmizes the indices by regenerating them from the given polygon group.
        /// </summary>
        /// <param name="group"></param>
        public void Optmize(STPolygonGroup group)
        {
            Dictionary<STVertex, int> verticesNew = new Dictionary<STVertex, int>();

            group.Faces.Clear();

            foreach (var v in Vertices)
            {
                if (!verticesNew.ContainsKey(v))
                    verticesNew.Add(v, verticesNew.Count);

                if (verticesNew.ContainsKey(v))
                    group.Faces.Add((uint)verticesNew[v]);
            }

            Vertices.Clear();
            Vertices.AddRange(verticesNew.Keys);
        }

        /// <summary>
        /// Calculates the tangents and bitangents 
        /// for <see cref="Vertices"/> from texture coordinates.
        /// </summary>
        /// <param name="uvSet"></param>
        public void CalculateTangentBitangent(int uvSet)
        {
            if (Vertices.Count < 3)
                return;

            if (Vertices.Any(x => x.TexCoords?.Length <= uvSet)) {
                throw new Exception($"Invalid UV set {uvSet} given for calculating tangents.");
            }

            Vector3[] tanArray = new Vector3[Vertices.Count];
            Vector3[] bitanArray = new Vector3[Vertices.Count];

            CalculateTanBitanArrays(Faces, tanArray, bitanArray, uvSet);
            ApplyTanBitanArray(tanArray, bitanArray);
        }

        private void ApplyTanBitanArray(Vector3[] tanArray, Vector3[] bitanArray)
        {
            if (Vertices.Count < 3)
                return;

            for (int i = 0; i < Vertices.Count; i++)
            {
                STVertex v = Vertices[i];
                Vector3 newTan = tanArray[i];
                Vector3 newBitan = bitanArray[i];

                // The tangent and bitangent should be orthogonal to the normal. 
                // Bitangents are not calculated with a cross product to prevent flipped shading  with mirrored normal maps.
                v.Tangent = new Vector4(Vector3.Normalize(newTan - v.Normal * Vector3.Dot(v.Normal, newTan)), 1);
                v.Bitangent = new Vector4(Vector3.Normalize(newBitan - v.Normal * Vector3.Dot(v.Normal, newBitan)), 1);
                v.Bitangent *= -1;
            }
        }

        private void CalculateTanBitanArrays(List<uint> faces, Vector3[] tanArray, Vector3[] bitanArray, int uvSet = 0)
        {
            if (Vertices.Count < 3 || faces.Count <= 0)
                return;

            for (int i = 0; i < faces.Count; i += 3)
            {
                STVertex v1 = Vertices[(int)faces[i]];
                STVertex v2 = Vertices[(int)faces[i + 1]];
                STVertex v3 = Vertices[(int)faces[i + 2]];

                float x1 = v2.Position.X - v1.Position.X;
                float x2 = v3.Position.X - v1.Position.X;
                float y1 = v2.Position.Y - v1.Position.Y;
                float y2 = v3.Position.Y - v1.Position.Y;
                float z1 = v2.Position.Z - v1.Position.Z;
                float z2 = v3.Position.Z - v1.Position.Z;

                float s1, s2, t1, t2;

                s1 = v2.TexCoords[uvSet].X - v1.TexCoords[uvSet].X;
                s2 = v3.TexCoords[uvSet].X - v1.TexCoords[uvSet].X;
                t1 = v2.TexCoords[uvSet].Y - v1.TexCoords[uvSet].Y;
                t2 = v3.TexCoords[uvSet].Y - v1.TexCoords[uvSet].Y;

                float div = (s1 * t2 - s2 * t1);
                float r = 1.0f / div;

                // Fix +/- infinity from division by 0.
                if (r == float.PositiveInfinity || r == float.NegativeInfinity)
                    r = 1.0f;

                float sX = t2 * x1 - t1 * x2;
                float sY = t2 * y1 - t1 * y2;
                float sZ = t2 * z1 - t1 * z2;
                Vector3 s = new Vector3(sX, sY, sZ) * r;

                float tX = s1 * x2 - s2 * x1;
                float tY = s1 * y2 - s2 * y1;
                float tZ = s1 * z2 - s2 * z1;
                Vector3 t = new Vector3(tX, tY, tZ) * r;

                // Prevents black tangents or bitangents due to having vertices with the same UV coordinates. 
                float delta = 0.00075f;
                bool sameU, sameV;

                sameU = (Math.Abs(v1.TexCoords[uvSet].X - v2.TexCoords[uvSet].X) < delta) &&
                        (Math.Abs(v2.TexCoords[uvSet].X - v3.TexCoords[uvSet].X) < delta);

                sameV = (Math.Abs(v1.TexCoords[uvSet].Y - v2.TexCoords[uvSet].Y) < delta) &&
                        (Math.Abs(v2.TexCoords[uvSet].Y - v3.TexCoords[uvSet].Y) < delta);

                if (sameU || sameV)
                {
                    // Let's pick some arbitrary tangent vectors.
                    s = new Vector3(1, 0, 0);
                    t = new Vector3(0, 1, 0);
                }

                // Average tangents and bitangents.
                tanArray[faces[i]] += s;
                tanArray[faces[i + 1]] += s;
                tanArray[faces[i + 2]] += s;

                bitanArray[faces[i]] += t;
                bitanArray[faces[i + 1]] += t;
                bitanArray[faces[i + 2]] += t;
            }
        }

        #endregion
    }
}
