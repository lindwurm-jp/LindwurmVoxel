using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class LookAtBlock : FunctionBlockBase
	{
		public override string Name { get { return "lookat"; } }
		public override bool IsActive { get; set; }
		public Vector3 TargetPoint { get; set; }
		public bool IsTurretH { get; private set; } = true;

		[SerializeField] private int dir = 4;
		public int Direction { get { return dir; } set { SetDirection(dir); } }
		[SerializeField] private int axis = 2;
		public int Axis { get { return axis; } set { SetAxis(axis); } }
		[SerializeField] private Quaternion rotForward = Quaternion.identity;

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_lookat_<forward>_<axis>
			if (args.Length >= 3)
			{
				Vector3 v;
				(dir,v,rotForward) = FunctionBlockFactory.GetForward(args[2]);
				if (args.Length >= 4)
					SetAxis(args[3]);
			}
			TargetPoint = Vector3.zero;
		}

		public override void ResetTransform()
		{
		}

		public override string GetParamString()
		{
			var f = FunctionBlockFactory.GetForwardWord(dir);
			var a = FunctionBlockFactory.GetAxisWord(axis);
			return $"fn_{Name}_{f}_{a}";
		}

		void LateUpdate()
		{
			if (!IsActive) return;

			transform.localRotation = Quaternion.identity;
			if (TargetPoint != Vector3.zero)
			{
				var pos = transform.InverseTransformPoint(TargetPoint);
				switch (axis)
				{
					case 1://x
						pos.x = 0f;
						break;
					case 2://y
						pos.y = 0f;
						break;
					case 3://z
						pos.z = 0f;
						break;
				}
				transform.localRotation = Quaternion.LookRotation(pos, Vector3.up) * rotForward;
			}
		}

		public int SetDirection(int d)
		{
			if (d >= 0 && d <= 5)
				dir = d;
			return dir;
		}

		public int SetAxis(int d)
		{
			if (d >= 0 && d <= 3)
				axis = d;
			return axis;
		}
		private void SetAxis(string d)
		{
			axis = d switch
			{
				"x" => SetAxis(1),
				"y" => SetAxis(2),
				"z" => SetAxis(3),
				//free
				_ => SetAxis(0),
			};
			IsTurretH = axis == 0 || axis == 2;
		}
	}
}
