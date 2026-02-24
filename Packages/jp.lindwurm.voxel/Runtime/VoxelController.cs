using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace Lindwurm.Voxel
{
	public class VoxelController : MonoBehaviour
	{
		public string title = default;
		public string author = default;

		public bool isHumanoid = false;
		public float totalVolume = 0f;
		public Vector3 centerOfGravity; // 作成時の重心位置
		public VolumeInfo[] volumes = default;
		public FunctionBlockBase[] functionBlocks = default;
		public PartsBase[] parts = default;

		private readonly List<GameObject> removedParts = new();
		private ReservePartsParam[] partsParams;
		private ReserveBlockParam[] blockParams;

		/// <summary>
		/// 機能ブロックの一括オンオフ
		/// </summary>
		/// <param name="isOn"></param>
		public void ActiveFunctionBlock(bool isOn)
		{
			if (functionBlocks == null)
				return;
			foreach (var block in functionBlocks)
			{
				if (block)
					block.IsActive = isOn;
			}
		}

		/// <summary>
		/// LookAtBlockの視点を設定
		/// </summary>
		/// <param name="aimPoint">視点</param>
		/// <param name="maxDegreesDelta">最大旋回角（angular speed * dt）</param>
		public void Aim(Vector3 aimPoint, float maxDegreesDelta)
		{
			if (functionBlocks == null)
				return;
			var turrets = functionBlocks.OfType<LookAtBlock>();
			foreach (var turret in turrets)
			{
				if (turret && turret.IsActive)
				{
                    var basePoint = turret.transform.position;
                    var forward = FunctionBlockFactory.GetForwardVector(turret.Direction);
                    var from = Quaternion.LookRotation(turret.transform.rotation * forward);
                    var dir = aimPoint - basePoint;
                    var to = Quaternion.LookRotation(dir);
                    var rotation = Quaternion.RotateTowards(from, to, maxDegreesDelta);
                    turret.TargetPoint = rotation * Vector3.forward * dir.magnitude + basePoint;
                }
            }
		}

		/// <summary>
		/// LookAtBlockの視点をリセット
		/// </summary>
		public void ResetAim()
		{
			if (functionBlocks == null)
				return;
			var turrets = functionBlocks.OfType<LookAtBlock>();
			foreach (var turret in turrets)
			{
				if (turret)
					turret.TargetPoint = Vector3.zero;
			}
		}

		/// <summary>
		/// パーツを切り離す
		/// </summary>
		/// <param name="parts">切り離すパーツ</param>
		/// <param name="ground">地面のTransform</param>
		/// <returns></returns>
		public IEnumerable<PartsBase> Purge(PartsBase parts, Transform ground = null)
		{
			var purged = new List<PartsBase>();
			foreach (Transform child in parts.transform)
			{
				if (child.TryGetComponent<PartsBase>(out var childParts))
					purged.AddRange(Purge(childParts));
			}
			if (parts.TryGetComponent<FunctionBlockBase>(out var function))
			{
				function.IsActive = false;
			}
			if (parts.TryGetComponent<Collider>(out var collider))
			{
				collider.enabled = true;
				collider.isTrigger = false;
			}
			if (parts.TryGetComponent<RotationConstraint>(out var constraint))
			{
				constraint.constraintActive = false;
			}
			parts.transform.SetParent(ground, true);
			if (parts.TryGetComponent<Rigidbody>(out var rb) == false)
				rb = parts.gameObject.AddComponent<Rigidbody>();
			rb.mass = parts.info.volume;
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			rb.isKinematic = false;
			purged.Add(parts);
			return purged;
		}

		private class ReservePartsParam
		{
			public Transform parent;
			public Vector3 position;
			public Quaternion rotation;
			public Collider collider;
			public bool colliderEnabled;
			public bool colliderIsTrigger;
			public RotationConstraint constraint;
			public bool constraintActive;
			public Rigidbody rigidbody;
		}

		private class ReserveBlockParam
		{
			public FunctionBlockBase block;
			public bool isActive;
		}

		/// <summary>
		/// 爆発させる
		/// </summary>
		/// <param name="position">爆心</param>
		/// <param name="power">爆発力</param>
		/// <param name="forceMode">Rigidbody.AddFoceのFoceMode</param>
		/// <param name="ground">地面のTransform</param>
		/// <param name="mass">ブロックの1m^3当たりの質量</param>
		/// <param name="disposeParts">分離したパーツを本体Destroy時に一緒にDestroyする</param>
		/// <param name="reversible">爆発後、元に戻る処理の設定</param>
		public void Explode(Vector3 position, float power, ForceMode forceMode = ForceMode.Force, Transform ground = null,
			float mass = 1f,
			bool disposeParts = true,
			bool reversible = false,
			bool colliderEnable = true)
		{
			if (blockParams != null || partsParams != null) return;

			blockParams = null;
			if (functionBlocks != null)
			{
				if (reversible)
					blockParams = new ReserveBlockParam[functionBlocks.Length];
				for (int i = 0; i < functionBlocks.Length; i++)
				{
					var block = functionBlocks[i];
					if (block)
					{
						if (reversible)
						{
							blockParams[i] = new()
							{
								block = block,
								isActive = block.IsActive,
							};
						}
						block.IsActive = false;
					}
				}
			}

			partsParams = null;
			if (volumes != null)
			{
				if (reversible)
					partsParams = new ReservePartsParam[volumes.Length];
				for (int i = 0; i < volumes.Length; i++)
				{
					var obj = volumes[i].gameObject;
					var collider = volumes[i].collider;
					if (obj && collider)
					{
						var constraint = obj.GetComponent<RotationConstraint>();
						if (reversible)
						{
							partsParams[i] = new()
							{
								parent = obj.transform.parent,
								position = obj.transform.position,
								rotation = obj.transform.rotation,
								collider = collider,
								colliderEnabled = collider.enabled,
								colliderIsTrigger = collider.isTrigger,
								constraint = constraint,
								constraintActive = constraint ? constraint.constraintActive : false,
							};
						}
						// コライダーを有効にする
						collider.enabled = colliderEnable;
						collider.isTrigger = false;

						// コンストレイントを停止する
						if (constraint != null)
							constraint.constraintActive = false;

						// Transformの親子関係を切る
						obj.transform.SetParent(ground, true);
						if (disposeParts && reversible == false)
							removedParts.Add(obj);

						// Rigidbodyを追加する
						if (obj.TryGetComponent<Rigidbody>(out var rb) == false)
							rb = obj.AddComponent<Rigidbody>();
						rb.mass = volumes[i].volume * mass;
						rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
						rb.isKinematic = false;
						var r = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
						rb.AddForce((obj.transform.position - position + r).normalized * power, forceMode);
						if (reversible)
							partsParams[i].rigidbody = rb;
					}
				}
			}
		}

		/// <summary>
		/// 爆発したパーツを元に戻す
		/// </summary>
		/// <param name="delay">復元開始遅延</param>
		/// <param name="duration">復元時間</param>
		/// <param name="onReversed">復元後のコールバック</param>
		/// <returns></returns>
		public void DelayReverse(float delay = 0, float duration = 0, System.Action onReversed = null)
		{
			StartCoroutine(Reverse(delay, duration, onReversed));
		}

		private IEnumerator Reverse(float delay, float duration, System.Action onReversed)
		{
			if (delay > 0)
				yield return new WaitForSeconds(delay);

			if (partsParams != null)
			{
				var now = new Vector3[partsParams.Length];
				for (int i = 0; i < partsParams.Length; i++)
				{
					var param = partsParams[i];
					if (param != null)
					{
						param.rigidbody.isKinematic = true;
						param.collider.enabled = param.colliderEnabled;
						param.collider.isTrigger = param.colliderIsTrigger;
						if (param.constraint != null)
							param.constraint.constraintActive = param.constraintActive;
						now[i] = param.collider.gameObject.transform.position;
					}
				}
				if (duration > 0f)
				{
					for (float t = 0f; t < duration; t += Time.deltaTime)
					{
						var dt = t / duration;
						for (int i = 0; i < partsParams.Length; i++)
						{
							var param = partsParams[i];
							if (param != null)
							{
								param.collider.transform.position = Vector3.Lerp(now[i], param.position, dt);
							}
						}
						yield return null;
					}
				}
				for (int i = 0; i < partsParams.Length; i++)
				{
					var param = partsParams[i];
					param?.collider.transform.SetPositionAndRotation(param.position, param.rotation);
				}
				for (int i = 0; i < partsParams.Length; i++)
				{
					var param = partsParams[i];
					param?.collider.transform.SetParent(param.parent);
				}
			}
			if (blockParams != null)
			{
				for (int i = 0; i < blockParams.Length; i++)
				{
					var param = blockParams[i];
					if (param != null)
					{
						param.block.IsActive = param.isActive;
					}
				}
			}
			partsParams = null;
			blockParams = null;
			onReversed?.Invoke();
		}

		private void OnDestroy()
		{
			foreach (var parts in removedParts)
			{
				if (parts)
					Destroy(parts);
			}
		}
	}
}
