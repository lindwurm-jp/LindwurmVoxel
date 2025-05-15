using UnityEngine;

namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
        public const string LeftHandheldName = "LeftHandheld";
        public const string RightHandheldName = "RightHandheld";
        public const string LeftHandName = "LeftHand";
        public const string RightHandName = "RightHand";
        public static Transform GetBodyTransform(Transform root, string partsName)
        {
            if (root.name == partsName)
            {
                return root;
            }
            for (var i = 0; i <  root.childCount; i++)
            {
                var tr = GetBodyTransform(root.GetChild(i), partsName);
                if(tr != null)
                {
                    return tr;
                }
            }
            return null;
        }
    }
}
