using UnityEngine;

namespace Lindwurm.Voxel
{
	public class EmitBlock : FunctionBlockBase
	{
		public override string Name { get { return "emit"; } }
		bool active = false;
		public override bool IsActive
		{
			get { return active; }
			set
			{
				active = value;
				if (particle)
				{
					particle.gameObject.SetActive(active);
				}
			}
		}
		[HideInInspector] public override bool IsColor { get; } = true;

		[SerializeField] private int direction = 2;
		[HideInInspector] public int Direction { get { return direction; } set { SetDirection(value); } }
        [SerializeField] private float rate = 10f;
		[HideInInspector] public float Rate { get { return rate; } set { SetRate(value); } }
        [SerializeField] private float angle = 25f;
		[HideInInspector] public float Angle { get { return angle; } set { SetAngle(value); } }
        [SerializeField] private float radius = 1f;
		[HideInInspector] public float Radius { get { return radius; } set { SetRadius(value); } }
        [SerializeField] private float life = 3f;
		[HideInInspector] public float Life { get { return life; } set { SetLife(value); } }
        [SerializeField] private float speed = 5f;
		[HideInInspector] public float Speed { get { return speed; } set { SetSpeed(value); } }
        [SerializeField] private float startSize = 1f;
		[HideInInspector] public float StartSize { get { return startSize; } set { SetStartSize(value); } }
        [SerializeField] private float endSize = 3f;
		[HideInInspector] public float EndSize { get { return endSize; } set { SetEndSize(value); } }
        [SerializeField] private int space = 1;
		[HideInInspector] public int Space { get { return space; } set { SetSpace(value); } }
		[System.NonSerialized, HideInInspector] public Color color;
		private ParticleSystem particle = null;
		public ParticleSystem Particle { get { return particle; } set { SetPerticle(value); } }
        [SerializeField] private float localScale = 1f;

		public override void Initialize(string[] args, float blockSize)
		{
			if (args.Length > 10)
			{
				localScale = blockSize;
				ColorUtility.TryParseHtmlString("#" + args[2], out color);
				Vector3 v;
				Quaternion q;
				(direction, v, q) = FunctionBlockFactory.GetForward(args[3]);
				rate = System.Convert.ToSingle(args[4]);
				angle = System.Convert.ToSingle(args[5]);
				radius = System.Convert.ToSingle(args[6]);// * blockSize;
				life = System.Convert.ToSingle(args[7]);
				speed = System.Convert.ToSingle(args[8]);// * blockSize;
				startSize = System.Convert.ToSingle(args[9]);// * blockSize;
				endSize = System.Convert.ToSingle(args[10]);// * blockSize;
				if (args.Length > 11)
				{
					space = System.Convert.ToInt32(args[11]);
				}
				if (particle != null)
					SetPerticle(particle);

			}
		}

		public override string GetParamString()
		{
			var c = ColorUtility.ToHtmlStringRGB(color);
			var f = FunctionBlockFactory.GetForwardWord(direction);
			return $"fn_{Name}_{c}_{f}_{rate}_{angle}_{radius}_{life}_{speed}_{startSize}_{endSize}_{space}";
		}

		public void Init(int direction, float rate, float angle, float radius, float life, float speed, float startSize, float endSize)
		{
			this.direction = direction;
			this.rate = rate;
			this.angle = angle;
			this.radius = radius;
			this.life = life;
			this.speed = speed;
			this.startSize = startSize;
			this.endSize = endSize;
		}

		public override void ResetTransform()
		{
		}
		public override void ChangeLocalScale(float localScale)
		{
			this.localScale = 1;	// つけたGameObjectの大きさに連動するので1でOK
		}
		public void SetPerticle(ParticleSystem particle)
		{
			this.particle = particle;
			SetDirection(direction);
			SetRate(rate);
			SetAngle(angle);
			SetRadius(radius);
			SetLife(life);
			SetSpeed(speed);
			SetLifeTimeSize(startSize, endSize);
			SetColor(color);
			SetSpace(space);
			particle.gameObject.SetActive(active);
		}

		public void SetDirection(int direction)
		{
			this.direction = direction;
			if (!particle)
				return;

			switch (direction)
			{
				case 0: //right
					particle.transform.localRotation = Quaternion.Euler(0, -90f, 0f);
					break;
				case 1: //left
					particle.transform.localRotation = Quaternion.Euler(0, 90f, 0f);
					break;
				case 2: //up
					particle.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
					break;
				case 3: //down
					particle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
					break;
				case 4: //front
					particle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
					break;
				default: //back
					particle.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
					break;
			}
		}

		public void SetRate(float v)
		{
			rate = v;
			if (!particle)
				return;

			var emission = particle.emission;
			emission.rateOverTime = v;
		}

		public void SetAngle(float v)
		{
			angle = v;
			if (!particle)
				return;

			var shape = particle.shape;
			shape.angle = v;
		}

		public void SetRadius(float v)
		{
			radius = v;
			if (!particle)
				return;

			var shape = particle.shape;
			shape.radius = v * localScale;
		}

		public void SetLife(float v)
		{
			life = v;
			if (!particle)
				return;

			var main = particle.main;
			main.startLifetime = v;
		}

		public void SetSpeed(float v)
		{
			speed = v;
			if (!particle)
				return;
			var main = particle.main;
			main.startSpeed = v * localScale;
		}

		public void SetStartSize(float v)
		{
			SetLifeTimeSize(v * localScale, endSize * localScale);
		}

		public void SetEndSize(float v)
		{
			SetLifeTimeSize(startSize * localScale, v * localScale);
		}

		public void SetLifeTimeSize(float start, float end)
		{
			startSize = start;
			endSize = end;
			if (!particle)
				return;

			float max = Mathf.Max(start * localScale, end * localScale);
			var sz = particle.sizeOverLifetime;
			sz.enabled = true;
			AnimationCurve sizeCurve = new AnimationCurve();
			sizeCurve.AddKey(0.0f, start * localScale / max);
			sizeCurve.AddKey(1.0f, end * localScale / max);
			sz.size = new ParticleSystem.MinMaxCurve(max, sizeCurve);
		}

		public override void SetColor(Color color)
		{
			this.color = color;
			if (!particle)
				return;

			var main = particle.main;
			main.startColor = color;
		}

		public void SetSpace(int space)
		{
			this.space = space;
			if (!particle)
				return;

			var main = particle.main;
			main.simulationSpace = space switch
			{
				0 => ParticleSystemSimulationSpace.Local,
				_ => ParticleSystemSimulationSpace.World,
			};
		}

		protected override void OnDestroy()
		{
			if (particle != null)
			{
				Destroy(particle.gameObject);
				particle = null;
			}
			base.OnDestroy();
		}
	}
}
