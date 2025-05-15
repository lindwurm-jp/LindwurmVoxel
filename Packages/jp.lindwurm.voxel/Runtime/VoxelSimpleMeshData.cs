using System.Collections.Generic;
using UnityEngine;

namespace Lindwurm.Voxel
{   public class SimpleMeshData
    {
        //public GameObject smgo;
        public GameObject armature;
        public Vector3 armaturePosition;    // サブスレッドで実行するときに使用する
        public Quaternion armatureInverseRotation;  // サブスレッドで実行するときに使用する
        public MeshFilter mf;
        public MeshRenderer mr;
        public VoxelMeshDisposer dp;
        public Mesh mesh;
        public List<Vector3> verts = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        //List<Color> colors = new List<Color>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<Color> colors = new List<Color>();
        //public List<byte> boneIndex = new List<byte>();
        public List<int> tris = new List<int>();
        public List<Vector2> uv2 = new List<Vector2>();
        public List<Dictionary<int, Vector2>> uvGroup = new List<Dictionary<int, Vector2>>();

        public SimpleMeshData(int gp = -1)
        {
            armature = null;
            var n = gp == -1 ? "" : $"{gp}";
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.name = $"Mesh{n}";
        }
        public void TrySetArmature(GameObject root, GameObject body)
        {
            if(armature == null)
            {
                armature = body;
                return;
            }
            var t = body.transform;
            while (t != null)
            {
                if(armature == t.gameObject)
                {
                    return;
                }
                if (t == root.transform)
                {
                    armature = body;
                    return;
                }
                t = t.parent;
            }

        }

        public void SetMesh(Material material, Texture2D tex)
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
            if(uv2.Count > 0)
            {
                mesh.SetUVs(1, uv2);
            }
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            if(mf == null)
            {
                mf = armature.AddComponent<MeshFilter>();
                mr = armature.AddComponent<MeshRenderer>();
                dp = armature.AddComponent<VoxelMeshDisposer>();
            }

            mf.sharedMesh = mesh;
            dp.targetMesh = mesh;
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
                mr.sharedMaterial = mat;
            }else
            {
                mr.sharedMaterial = material;
            }

        }
    };
}
