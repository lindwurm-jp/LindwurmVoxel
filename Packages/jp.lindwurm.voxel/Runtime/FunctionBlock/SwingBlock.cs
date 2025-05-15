using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class SwingBlock : FunctionBlockBase
	{
		public override string Name { get { return "swing"; } }
		private bool active = true;
		public override bool IsActive
		{
			get { return active; }
			set
			{
				active = value;
				t = 0f;
				if (active)
				{
					org = transform.localRotation;
				}
				else if (orgFlag)
					transform.localRotation = org;
			}
		}

		[SerializeField] private float angle = 45f;
		public float Angle { get {return angle; } set { angle = value; } }
		[SerializeField] private float period = 1.0f;
		public float Period { get { return period; } set { period = value; } }
		[SerializeField] private float phase_shift = 0f;
		public float PhaseShift { get { return Mathf.RoundToInt(phase_shift * 180f / Mathf.PI); } set { phase_shift = value * Mathf.PI / 180f; } }
		[SerializeField] private int axis = 1;
		public int Axis { get { return  axis; } set { SetAxis(value); } }
		private Quaternion org;
		private bool orgFlag = false;
		private float t = 0f;
		private bool oneside = false;

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_swing_<axis>_<angle>_<period>_<phase>
			if (args.Length > 5)
			{
				SetAxis(args[2][0]);
				float angle = System.Convert.ToSingle(args[3]);
				if (args[3][0] == '+')
				{
					angle /= 2f;
					oneside = true;
				}
				float period = System.Convert.ToSingle(args[4]);
				int phase = System.Convert.ToInt32(args[5]);
				Init(angle, period, phase);
			}
		}

		public override void ResetTransform()
		{
			if (orgFlag)
			{
				transform.localRotation = org;
				t = 0f;
			}
		}

		public override string GetParamString()
		{
			var ax = FunctionBlockFactory.GetAxisWord(axis);
			float ag = oneside ? angle * 2f : angle;
			string os = oneside? "+" : "";
			return $"fn_{Name}_{ax}_{os}{ag}_{period}_{PhaseShift}";
		}

		public void SetAxis(int d)
		{
			ResetTransform();

			if (d >= 1 && d <= 3)
				axis = d;
		}
		private void SetAxis(char d)
		{
			switch (d)
			{
				case 'x':
					SetAxis(1); break;
				case 'y':
					SetAxis(2); break;
				case 'z':
					SetAxis(3); break;
			}
		}

		private void Init(float angle, float period, int phase)
		{
			Angle = angle;
			Period = period;
			PhaseShift = phase;
			t = 0f;
		}

		void Start()
		{
			org = transform.localRotation;
			orgFlag = true;
		}

		void Update()
		{
			if (IsActive && period > 0)
			{
				t += Time.deltaTime;
				if (t > period)
				{
					t -= period;
				}

				var a = oneside
					 ? (angle * Mathf.Sin(2f * Mathf.PI * t / period + (Mathf.PI * 3 / 2) + phase_shift) + angle)
					 : (angle * Mathf.Sin(2f * Mathf.PI * t / period + phase_shift));
				Quaternion r = axis switch
				{
					1 => Quaternion.Euler(a, 0f, 0f),
					2 => Quaternion.Euler(0f, a, 0f),
					_ => Quaternion.Euler(0f, 0f, a)
				};
				transform.localRotation = org * r;
			}
		}
	}
}
