using System.Collections.Generic;
using UnityEngine;

namespace Lindwurm.Voxel
{   public class SkinnedMeshData
    {
        public GameObject smgo;
        public SkinnedMeshRenderer smr;
        public Mesh mesh;
        public VoxelMeshDisposer dp;
        public List<Vector3> verts = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<Color> colors = new List<Color>();
        public List<byte> boneIndex = new List<byte>();
        public List<int> tris = new List<int>();
        public Matrix4x4 localToWorldMatrix;// worldToLocalMatrix;
        public Vector3 smrPosition;

        public SkinnedMeshData(GameObject root, GameObject go, int gp = -1)
        {
            smgo = new GameObject();
            smgo.transform.SetParent(root.transform);
            smgo.transform.position = Vector3.zero;
            smgo.transform.rotation = Quaternion.identity;
            smgo.transform.localScale = Vector3.one;
            var n = gp == -1 ? "" : $"{gp}";
            smgo.name = $"Renderer{n}";

            smr = smgo.AddComponent<SkinnedMeshRenderer>();
            smr.rootBone = go.transform;
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.name = $"Mesh{n}";
            dp = smgo.AddComponent<VoxelMeshDisposer>();
            dp.targetMesh = mesh;
        }

    public void SetMesh(Material material, Texture2D tex, List<Transform> boneTransforms, List<Matrix4x4> poses)
        {
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            if(colors.Count > 0)
            {
                mesh.SetColors(colors);
            }
            if(uvs.Count > 0)
            {
                mesh.SetUVs(0, uvs);
            }
            mesh.SetTriangles(tris, 0);
            var bwno = new Unity.Collections.NativeArray<byte>(boneIndex.Count, Unity.Collections.Allocator.Temp);
            var boneWeights = new Unity.Collections.NativeArray<BoneWeight1>(boneIndex.Count, Unity.Collections.Allocator.Temp);
            for (var i = 0; i < bwno.Length; i++)
            {
                bwno[i] = 1;
                boneWeights[i] = new BoneWeight1() { boneIndex = boneIndex[i], weight = 1.0f };
            }
            mesh.SetBoneWeights(bwno, boneWeights);
            mesh.bindposes = poses.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            smr.bones = boneTransforms.ToArray();
            smr.sharedMesh = mesh;
            if (tex != null)
            {
                var mat = GameObject.Instantiate(material);
                if (mat.HasProperty("_MainTex"))
                {
                    mat.SetTexture("_MainTex", tex);
                }
                else if (mat.HasProperty("_BaseMap"))
                {
                    mat.SetTexture("_BaseMap", tex);
                }
                smr.sharedMaterial = mat;
            }else
            {
                smr.sharedMaterial = material;
            }

        }
    };
}
