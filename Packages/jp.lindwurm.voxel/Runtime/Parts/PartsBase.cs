using UnityEngine;
using UnityEngine.Events;

namespace Lindwurm.Voxel
{
	public class PartsBase : MonoBehaviour
	{
		public VolumeInfo info;
		public UnityEvent<PartsBase> onDestroy { get; } = new();

		private void OnDestroy()
		{
			onDestroy?.Invoke(this);
		}
	}

	public static class PartsFactory
	{
		public static T AddPartsComponent<T>(VolumeInfo info) where T : PartsBase
		{
			if (info != null && info.gameObject != null)
			{
				var parts = info.gameObject.AddComponent<T>();
				parts.info = info;
				return parts;
			}
			return null;
		}
	}
}
