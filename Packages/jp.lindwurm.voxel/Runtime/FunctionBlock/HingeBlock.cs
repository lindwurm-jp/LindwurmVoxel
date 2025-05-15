using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class HingeBlock : FunctionBlockBase
	{
		public override string Name { get { return "hinge"; } }
		public override bool IsActive { get; set; }
		private int axis = 1;
		private Vector3 axisV = Vector3.right;

		public override void Initialize(string[] args, float blockSize)
		{
			if (args.Length >= 3)
				SetAxis(args[2][0]);
		}

		public override void ResetTransform()
		{
		}

		public override string GetParamString()
		{
			var a = FunctionBlockFactory.GetAxisWord(axis);
			return $"fn_{Name}_{a}";
		}

		private void SetAxis(int n)
		{
			axis = n;
			axisV = n switch
			{
				1 => Vector3.right,
				2 => Vector3.up,
				3 => Vector3.forward,
				_ => Vector3.zero
			};
		}
		private void SetAxis(char n)
		{
			axisV = n switch
			{
				'x' => Vector3.right,
				'y' => Vector3.up,
				_ => Vector3.forward,
			};
		}

		void Update()
		{
			if (IsActive)
			{
				var lookRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
				transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.2f);
			}
		}
	}
}
