using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Lindwurm.Voxel
{
	public class SampleVoxelLoader : MonoBehaviour
	{
		[SerializeField] TextAsset vxpData;

		private void Start()
		{
			_ = LoadModel(vxpData.bytes);
		}

		public async Task LoadModel(byte[] bytes, CancellationToken ct = default, object option = null)
		{
			try
			{
				// ルートのGameObjectを作成します。
				var go = new GameObject("Voxel");
				go.SetActive(false);
				// マテリアルを作成します。テクスチャを作成しない場合は、同じマテリアルを使いまわすようにするとリソース削減になります。
				// シェーダは、頂点カラーを反映できるシェーダー(createTexture = false時)、テクスチャカラー(UV0)を使用できるシェーダー(createTexture = true時)を指定してください。
				var material = new Material(Shader.Find("Shader Graphs/URPLitVertexColor"));
				// モデル作成のオプションを設定します。
				// 下記は、作成したモデルをバラバラにできるように、Meshrendererを使用しメッシュグループを全て分ける設定のサンプルです
				if (option is not VoxelUtil.CreateModelOption opt)
					opt = new VoxelUtil.CreateModelOption(true)
					{
						splitAllGroup = true,	// 各パーツごとにMeshを作成する
						isHumanoid = true,		// 人体モデルのAvatar作成を要求
						createVoxelInfo = true,	// パーツごとのボクセル情報を作成する
						createAnimator = true	// Animatorを作成
					};
				// 非同期CreateVoxelModelAsyncを呼ぶ場合は、メインスレッドのSynchronizationContextを入力して下さい。
				var scontext = SynchronizationContext.Current;
				var metaData = await VoxelUtil.CreateVoxelModelAsync(scontext, bytes, go, material, opt, null, ct);
				SetMetaData(metaData);
				if (ct.IsCancellationRequested)
					throw new System.OperationCanceledException();
				go.SetActive(true);
			}
			catch (System.OperationCanceledException) { throw; }
		}

		private void SetMetaData(VoxelUtil.VoxelSaveInfoFormat info)
		{
			// thumbnailは初参照時にテクスチャが作成されます。infoの廃棄時に破棄されるので、永続的に使用する場合はinfoを保持しておくか、保存してnullを入れてください。
			//myThumbnail = info.thumbnail;
			//info.thumbnail = null;
		}
	}
}
