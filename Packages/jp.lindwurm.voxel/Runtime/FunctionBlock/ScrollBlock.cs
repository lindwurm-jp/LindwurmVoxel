using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class ScrollBlock : FunctionBlockBase
	{
		public override string Name { get { return "scroll"; } }
		private bool active = true;
		public override bool IsActive
		{
			get { return active; }
			set
			{
				active = value;
				t = 0;
				if (active)
				{
					org = transform.localPosition;
				}
				else if (orgFlag)
					transform.localPosition = org;
			}
		}

		[SerializeField] private float length = 5f;
		public float Length { get { return length; } set { length = value; } }
		[SerializeField] private float period = 1f;
		public float Period { get { return period; } set { period = value; } }
		[SerializeField] private float offset = 0f;
		public float Offset { get { return offset; } set { offset = value; offsetTime = period * offset / length;}
		}
		[SerializeField] private int dir = 2;
		public int Direction { get { return dir; } set { SetDirection(value); } }
		private bool orgFlag = false;
		private Vector3 org;
		private float t = 0f;
		private float scale = 1f;
		private float offsetTime = 0f;

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_recipro_<axis>_<length>_<time>
			scale = blockSize;
			if (args.Length > 5)
			{
				Vector3 v;
				Quaternion q;
				(dir, v, q) = FunctionBlockFactory.GetForward(args[2]);
				length = System.Convert.ToSingle(args[3]);
				period = System.Convert.ToSingle(args[4]);
				if (period <= 0f)
					period = 1f;
				offset = System.Convert.ToSingle(args[5]);
			}
		}

		public override void ResetTransform()
		{
			if (orgFlag)
			{
				transform.localPosition = org;
				t = 0;
			}
		}

		public override void ChangeLocalScale(float localScale)
		{
			scale = localScale;
		}
		public override void ApplyForPrefab()
		{
			length = length * scale;
			offset = offset * scale;
			scale = 1f;
		}

		public override string GetParamString()
		{
			var f = FunctionBlockFactory.GetForwardWord(dir);
			return $"fn_{Name}_{f}_{length}_{period}_{offset}";
		}

		public void SetDirection(int d)
		{
			ResetTransform();

			if (d >= 0 && d <= 5)
				dir = d;
		}

		public void Init(float length, float period, float offset)
		{
			if (period <= 0f)
				period = 1f;
			this.length = length;
			this.period = period;
			this.offset = offset;
			t = 0;
			offsetTime = period * offset / length;
		}

		void Start()
		{
			org = transform.localPosition;
			orgFlag = true;
			t = 0;
			offsetTime = period * offset / length;
		}

		void Update()
		{
			if (IsActive && period > 0)
			{
				t += Time.deltaTime;
				if (t > period - offsetTime)
				{
					t -= period;
				}

				var pos = org;
				var d = length * t / period;
				switch (dir)
				{
					case 0:
						pos.x += d; break;
					case 1:
						pos.x -= d; break;
					case 2:
						pos.y += d; break;
					case 3:
						pos.y -= d; break;
					case 4:
						pos.z += d; break;
					case 5:
						pos.z -= d; break;
				}
				transform.localPosition = transform.localRotation * pos;
			}
		}
	}
}
