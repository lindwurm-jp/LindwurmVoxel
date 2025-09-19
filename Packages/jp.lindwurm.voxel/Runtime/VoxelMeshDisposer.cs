using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lindwurm.Voxel
{
    public class VoxelMeshDisposer : MonoBehaviour
    {
        static Dictionary<Mesh, int> meshRefCount = new Dictionary<Mesh, int>();

        [HideInInspector]
        public Mesh mesh = null;
        public Mesh targetMesh {
            get
            {
                return mesh;
            }
            set
            {
                //Debug.Log("targetMesh Add");
                var m = value;
                if(mesh == m)
                {
                    return;
                }
                if (mesh != null && meshRefCount.ContainsKey(mesh))
                {
                    meshRefCount[mesh]--;
                    if (meshRefCount[mesh] <= 0)
                    {
                        meshRefCount.Remove(mesh);
                        Destroy(mesh);
                    }
                }
                if(meshRefCount.ContainsKey(m))
                {
                    meshRefCount[m]++;
                }else
                {
                    meshRefCount[m] = 1;
                }
                mesh = m;
            }
        }

        private void Awake()
        {
            //Debug.Log($"new Disposer Awake {gameObject.name},{targetMesh==null}");
            if(mesh != null)
            {
                var mf = GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != mesh)
                {
                    // instantiateÇµÇΩéûì_Ç≈ä«óùÇ≥ÇÍÇƒÇ¢ÇÈmeshÇ∆àŸÇ»Ç¡ÇΩÇÁñ{êlÇ™ä«óùÇ∑ÇÈ
                    mesh = null;
                    return;
                }
                if (meshRefCount.ContainsKey(mesh))
                {
                    meshRefCount[mesh]++;
                }
                else
                {
                    meshRefCount[mesh] = 1;
                }
                //Debug.Log($"Count:[{mesh.GetHashCode()}] {meshRefCount[mesh]}");
            }
        }


        private void OnDestroy()
        {
            if (mesh != null && meshRefCount.ContainsKey(mesh))
            {
                meshRefCount[mesh]--;
                //Debug.Log(meshRefCount[targetMesh]);
                if (meshRefCount[mesh] <= 0)
                {
                    meshRefCount.Remove(mesh);
                    Destroy(mesh);
                }
            }else
            {
                //Debug.Log("Unknown mesh");
            }
        }
    }
}
