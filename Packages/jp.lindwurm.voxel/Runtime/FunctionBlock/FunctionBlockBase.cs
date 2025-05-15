using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lindwurm.Voxel
{
	public abstract class FunctionBlockBase : MonoBehaviour
	{
		public abstract string Name { get; }
		public abstract string GetParamString();
		public abstract bool IsActive { get; set; }
		public abstract void Initialize(string[] args, float blockSize);
		public abstract void ResetTransform();
		public virtual bool IsColor { get; } = false;
		public virtual void SetColor(Color color){}
		public virtual void ChangeLocalScale(float scale) { }   // Edit中にジョイントスケールを変更した場合に使用
		public virtual void ApplyForPrefab() { } // prefabに反映しておくべきこと(LocalScaleなど)を処理する
		public UnityEvent<FunctionBlockBase> onDestroy { get; } = new();

		protected virtual void OnDestroy()
		{
			onDestroy?.Invoke(this);
		}
	}

	public static class FunctionBlockFactory
	{
		static GameObject particlePrefab = null;

		/// <summary>
		/// Resourcesを使いたくない場合
		/// Resources内のVoxcelParticle.prefabとマテリアルを移動し、予めこれを呼んでプレハブをセットしておく
		/// </summary>
		/// <param name="prefab"></param>
		static void SetParticlePrefab(GameObject prefab)
		{
			particlePrefab = prefab;
		}

		public static void CreateOld(GameObject gameObject, float blockSize, List<FunctionBlockBase> list = null)
		{
			if (gameObject.name.StartsWith("fn_"))
			{
				var block = AddComponent(gameObject, gameObject.name, blockSize, list!=null);
				if (list != null && block != null)
					list.Add(block);
			}
			foreach (Transform child in gameObject.transform)
			{
				CreateOld(child.gameObject, blockSize, list);
			}
		}
		public static void Create(GameObject gameObject, string parameter, float blockSize, List<FunctionBlockBase> list = null)
		{
			if (parameter.StartsWith("fn_"))
			{
				var block = AddComponent(gameObject, parameter, blockSize, list != null);
				if (list != null && block != null)
					list.Add(block);
			}
		}
		public static void CollectFunctionBlockBase(GameObject gameObject, List<FunctionBlockBase> list)
		{
			var block = gameObject.GetComponent<FunctionBlockBase>();
			if (block != null)
			{
				list.Add(block);
			}
			foreach (Transform child in gameObject.transform)
			{
				CollectFunctionBlockBase(child.gameObject, list);
			}
		}
		public static FunctionBlockBase AddComponent(GameObject gameObject, string parameter, float blockSize, bool active)
		{
			var old = gameObject.GetComponent<FunctionBlockBase>();
			if (old)
			{
				GameObject.Destroy(old);
			}
			var pos = parameter.IndexOf("__");
			if (pos > 0)
			{
				parameter = parameter.Substring(0, pos);
			}
			var args = parameter.Split('_');

			FunctionBlockBase block = AddComponent(args[1], gameObject, blockSize, false);
			if (block != null)
			{
				block.Initialize(args, blockSize);
				block.IsActive = active;
			}

			return block;
		}
		public static FunctionBlockBase AddComponent(string type, GameObject gameObject, float blockSize, bool active)
		{
			var old = gameObject.GetComponent<FunctionBlockBase>();
			if (old)
			{
				GameObject.Destroy(old);
			}
			FunctionBlockBase block = type switch
			{
				"lookat" => gameObject.AddComponent<LookAtBlock>(),
				"rot" => gameObject.AddComponent<RotateBlock>(),
				"recipro" => gameObject.AddComponent<ReciprocateBlock>(),
				"swing" => gameObject.AddComponent<SwingBlock>(),
				"scroll" => gameObject.AddComponent<ScrollBlock>(),
				"shot" => gameObject.AddComponent<ShotBlock>(),
				"hinge" => gameObject.AddComponent<HingeBlock>(),
				"rocket" => gameObject.AddComponent<RocketBlock>(),
				"emit" => gameObject.AddComponent<EmitBlock>(),
				_ => null,
			};
			if (block != null)
			{
				block.IsActive = active;

				// for EmitBlock
				if (block is EmitBlock e)
				{
					if (particlePrefab == null)
						SetParticlePrefab((GameObject)Resources.Load("VXP/VoxelParticle"));
					if (particlePrefab != null)
					{
						var p = GameObject.Instantiate(particlePrefab, gameObject.transform);
						e.SetPerticle(p.GetComponent<ParticleSystem>());
					}
				}
			}

			return block;
		}

		public static (int d, Vector3 v, Quaternion r) GetForward(string dir)
		{
			int d;
			Vector3 v;
			Quaternion rotForward;
			switch (dir)
			{
				case "r":
				case "right":
					d = 0;
					v = Vector3.right;
					rotForward = Quaternion.Euler(0, 90f, 0);
					break;
				case "l":
				case "left":
					d = 1;
					v = Vector3.left;
					rotForward = Quaternion.Euler(0, -90f, 0);
					break;
				case "u":
				case "up":
					d = 2;
					v = Vector3.up;
					rotForward = Quaternion.Euler(90f, 0f, 0f);
					break;
				case "d":
				case "down":
					d = 3;
					v = Vector3.down;
					rotForward = Quaternion.Euler(-90f, 0f, 0f);
					break;
				case "f":
				case "forward":
					d = 4;
					v = Vector3.forward;
					rotForward = Quaternion.identity;
					break;
				default://back
					d = 5;
					v = Vector3.back;
					rotForward = Quaternion.Euler(0f, 180f, 0f);
					break;
			}
			return (d, v, rotForward);
		}

		public static Vector3 GetForwardVector(int dir)
		{
			return dir switch
			{
				0 => Vector3.right,
				1 => Vector3.left,
				2 => Vector3.up,
				3 => Vector3.down,
				4 => Vector3.forward,
				5 => Vector3.back,
				_ => Vector3.forward
			};
		}

		public static string GetForwardWord(int dir)
		{
			string f = dir switch
			{
				0 => "r",
				1 => "l",
				2 => "u",
				3 => "d",
				4 => "f",
				_ => "b"
			};
			return f;
		}

		public static string GetAxisWord(int axis)
		{
			string a = axis switch
			{
				0 => "f",
				1 => "x",
				2 => "y",
				3 => "z",
				_ => "y"
			};
			return a;
		}
	}
}
