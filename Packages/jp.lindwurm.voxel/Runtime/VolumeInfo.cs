using UnityEngine;

namespace Lindwurm.Voxel
{
	[System.Serializable]
	public class VolumeInfo
	{
		public int meshGroupNo; // Meshグループ
		public GameObject gameObject;   // ターゲットのゲームオブジェクト
		public Collider collider;   // コライダーオブジェクト(nullあり)
		public int singleBlocks;    // 普通ブロックの数
		public int quaterBlocks;    // 1/4ブロックの数
		public float volume;    // 体積
		public float size;      // 普通ブロック一つの大きさ
		public Vector3 centerOfGravity; // 作成時の重心位置
	}
	
}
