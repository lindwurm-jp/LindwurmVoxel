using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class RocketBlock : FunctionBlockBase
	{
		public override string Name { get { return "rocket"; } }
		[SerializeField] private int dir = 4;
		public int Direction { get { return dir; } set { dir = SetDirection(value); } }
		public Quaternion RotForward = Quaternion.identity;
		public Vector3 Forward = Vector3.forward;
		public override bool IsActive { get; set; }

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_rocket_<forward>
			if (args.Length > 2)
			{
				(dir,Forward,RotForward) = FunctionBlockFactory.GetForward(args[2]);
			}
		}

		public override void ResetTransform()
		{
		}

		public override string GetParamString()
		{
			var f = FunctionBlockFactory.GetForwardWord(dir);
			return $"fn_{Name}_{f}";
		}

		public int SetDirection(int d)
		{
			if (d >= 0 && d <= 5)
				dir = d;
			Forward = FunctionBlockFactory.GetForwardVector(dir);
			return dir;
		}
	}
}
