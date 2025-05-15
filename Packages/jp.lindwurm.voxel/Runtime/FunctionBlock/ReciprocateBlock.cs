using UnityEngine;

namespace Lindwurm.Voxel
{
	public sealed class ReciprocateBlock : FunctionBlockBase
	{
		public override string Name { get { return "recipro"; } }
		private bool active = true;
		public override bool IsActive {
			get { return active; }
			set {
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

		[SerializeField] private float amplitude = 3f;
		public float Amplitude { get { return amplitude; } set { amplitude = value; } }
		[SerializeField] private float period = 1f;
		public float Period { get { return period; } set { period = value; } }
		[SerializeField] private float phase_shift = 0f;
		public float PhaseShift { get { return Mathf.RoundToInt(phase_shift * 180f/Mathf.PI); } set { phase_shift = value * Mathf.PI / 180f; } }
		[SerializeField] private int axis = 1;
		public int Axis { get { return axis; } set { SetAxis(value); } }
		private bool orgFlag = false;
		private Vector3 org;
		private float t = 0f;
		private float scaleAmpLocal = 1f;   // 保存されるampletudeに入れていけないオブジェクトスケール分。エディタではviewObjectのLocalScale、ローダーではrootからviewObjectまでのポーズスケール

		public override void Initialize(string[] args, float blockSize)
		{
			// fn_recipro_<axis>_<amplitude>_<period>_<phaseShift>
			float amp = amplitude;
			float len = 1.0f;
			float phase = 0;
			scaleAmpLocal = blockSize;
			if (args.Length > 5)
			{
				SetAxis(args[2][0]);
				amp = System.Convert.ToSingle(args[3]);
				len = System.Convert.ToSingle(args[4]);
				phase = System.Convert.ToSingle(args[5]);
			}
			Init(amp, len, phase * Mathf.PI / 180f);
		}

		public override void ResetTransform()
		{
			if (orgFlag)
			{
				transform.localPosition = org;
				t = 0f;
			}
		}

		public override void ChangeLocalScale(float localScale)
		{
			// ジョイントのlocalScaleが変更されてブロックが大きくなっても移動距離は変わらないので変動分を記録
			scaleAmpLocal = localScale;
		}
		public override void ApplyForPrefab()
		{
			amplitude = amplitude * scaleAmpLocal;
			scaleAmpLocal = 1f;
		}

		public override string GetParamString()
		{
			var a = FunctionBlockFactory.GetAxisWord(axis);
			return $"fn_{Name}_{a}_{amplitude}_{period}_{PhaseShift}";
		}

		public void SetAxis(int d)
		{
			ResetTransform();

			if (d >= 0 && d <=3)
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
				case 'f':
					SetAxis(0); break;
			}
		}

		private void Init(float amplitude, float period, float phase_shift)
		{
			this.amplitude = amplitude;
			this.period = period;
			this.phase_shift = phase_shift;
			t = 0f;
		}

		void Start ()
		{
			org = transform.localPosition;
			orgFlag = true;
		}

		void Update()
		{
			if (IsActive && period > 0) {

				t += Time.deltaTime;
				if (t > period)
				{
					t -= period;
				}

				var pos = org;
				var a = amplitude * scaleAmpLocal * Mathf.Sin(2f * Mathf.PI * t / period + phase_shift);
				switch (axis)
				{
					case 1:
						pos.x += a; break;
					case 2:
						pos.y += a; break;
					case 3:
						pos.z += a; break;
				}
				transform.localPosition = transform.localRotation * pos;
			}
		}
	}
}
