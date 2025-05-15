using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class ShotBlock : FunctionBlockBase
	{
		public override string Name { get { return "shot"; } }
		public enum BulletType
		{
			Shot,
			Missile,
		}

		public override bool IsActive { get; set; }
		[SerializeField] private int dir = 4;
		public int Direction { get { return dir; } set { dir = SetDirection(value); } }
		public Vector3 Forward { get; private set; } = Vector3.forward;
		public BulletType Type { get; private set; } = BulletType.Shot;

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_shot_<dir>_<type>
			if (args.Length > 2)
			{
				Quaternion rotForward;
				(dir, Forward, rotForward) = FunctionBlockFactory.GetForward(args[2]);
				if (args.Length > 3)
					SetType(args[3]);
			}
		}

		public override void ResetTransform()
		{
		}

		public override string GetParamString()
		{
			var f = FunctionBlockFactory.GetForwardWord(dir);
			return $"fn_{Name}_{f}_{Type}";
		}

		public int SetDirection(int d)
		{
			if (d >= 0 && d <= 5)
				dir = d;
			Forward = FunctionBlockFactory.GetForwardVector(dir);
			return dir;
		}

		private void SetType(string type)
		{
			Type = type switch
			{
				"s" or "shot" => BulletType.Shot,
				"m" or "missile" => BulletType.Missile,
				_ => BulletType.Shot,
			};
		}
	}
}
