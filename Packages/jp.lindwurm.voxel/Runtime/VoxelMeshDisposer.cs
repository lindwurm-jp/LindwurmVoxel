using UnityEngine;

namespace Lindwurm.Voxel
{
    public class VoxelMeshDisposer : MonoBehaviour
    {
        public Mesh targetMesh = null;
        private void OnDestroy()
        {
            if (targetMesh != null)
            {
                Destroy(targetMesh);
            }
        }
    }
}
