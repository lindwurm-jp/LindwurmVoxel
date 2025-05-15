
namespace Lindwurm.Voxel
{
	public partial class VoxelUtil
	{
		[System.Serializable]
		public struct VoxelFuncBlockInfo
		{
			public int id;	// jointID
			public string pr;	// シリアライズされたパラメータ
		}

		// JSONUtilityが配列を直に入れられないので。
		[System.Serializable]
		public class VoxelFuncBlockInfoSerialized
		{
			public VoxelFuncBlockInfo[] dat;
		}
	}
}