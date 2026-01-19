using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class RotateBlock : FunctionBlockBase
	{
		public override string Name { get { return "rot"; } }
		private bool active = true;
		public override bool IsActive
		{
			get { return active; }
			set
			{
				active = value;
				if (active)
				{
					org = transform.localRotation;
				}
				else if (orgFlag)
					transform.localRotation = org;
			}
		}

		[SerializeField] private float period = 1f;
		public float Period { get { return period; } set { period = value; } }
		[SerializeField] private int axis = 2;
		public int Axis{ get { return axis; } set { SetAxis(value); } }
		[SerializeField] private Vector3 axisV = Vector3.up;

		private bool orgFlag = false;
		private Quaternion org;

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_rot_<axis>_<period>
			if (args.Length > 3)
			{
				SetAxis(args[2][0]);
				period = System.Convert.ToSingle(args[3]);
				if (period == 0f)
					period = 1f;
			}
		}

		public override void ResetTransform()
		{
			if (orgFlag)
				transform.localRotation = org;
		}

		public override string GetParamString()
		{
			var a = FunctionBlockFactory.GetAxisWord(axis);
			return $"fn_{Name}_{a}_{period}";
		}

		public void SetAxis(int n)
		{
			ResetTransform();

			switch (n)
			{
				case 1:
					axis = 1;
					axisV = Vector3.right;
					break;
				case 2:
					axis = 2;
					axisV = Vector3.up;
					break;
				default:
					axis = 3;
					axisV = Vector3.forward;
					break;
			};
		}

		private void SetAxis(char n)
		{
			switch (n)
			{
				case 'x':
					SetAxis(1); break;
				case 'y':
					SetAxis(2); break;
				default:
					SetAxis(3); break;
			};
		}

		void Start()
		{
			org = transform.localRotation;
			orgFlag = true;
			SetAxis(axis);
		}

		void Update()
		{
			if (IsActive)
			{
				float angle = (period>0)? 360f * Time.deltaTime / period : 0;
				transform.Rotate(axisV, angle, Space.Self);
			}
		}
	}
}
