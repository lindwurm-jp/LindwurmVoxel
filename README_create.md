<h4>VXP用ユーティリティ関数</h4>
namespaceはLindwurm.Voxel.VoxelUtilです。

<h5>ボクセルモデルの情報を取得します。同期版</h5>
public static VoxelSaveInfoFormat GetVoxelModelInfo(byte[] bytes); // バイナリから<br />
public static VoxelSaveInfoFormat GetVoxelModelInfo(string path); // ファイルパスから<br />
<h5>ボクセルモデルの情報を取得します。非同期版</h5>
public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(byte[] bytes, CancellationToken ct = default) // バイナリから<br />
public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(string path, CancellationToken ct = default) // ファイルパスから<br />
<h5>ボクセルモデルを構築します。同期版</h5>
public static VoxelSaveInfoFormat CreateVoxelModel(byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null) // バイナリから<br />
public static VoxelSaveInfoFormat CreateVoxelModel(string path, GameObject root, Material material = null, CreateModelOption option = null) // ファイルパスから<br />
<h5>ボクセルモデルを構築します。非同期版</h5>
public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default) // バイナリから<br />
public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, string path, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default) // ファイルパスから<br />
<h5>引数の説明</h5>
scontext:メインスレッドのSynchronizationContext <br/>
root:モデルを展開するルートのゲームオブジェクト<br />
material:表示用のマテリアル<br />
option:モデル作成オプション<br />
callback:作成完了時のコールバック<br />
<h5>CreateModelOptionの説明</h5>
createCollider (true) : BoxColliderを作成します。<br />
thumbnailPose (false) : trueで初期ポーズをサムネイル用ポーズにします。通常はTポーズです。<br />
isHumanoid (true) : HumanoidモデルとしてAvatarの作成を試みます。失敗した場合はGenericモデルとしてAvatarが作成されます。<br />
overrideScale (0) : １ブロックの大きさを上書きします。0で保存時の大きさになります。<br />
includeRotation (true) : trueで保存時の回転をMeshに焼きこみます。falseでTransformに回転が反映されます<br />
useMeshRenderer (false) : trueでMeshRendererを使用します。falseでSkinnedMeshRendererを使用します。<br />
splitAllGroup (false) : trueで全メッシュをジョイントごとで分割します。falseではボクセルクラフト王のMeshGroupで指定したグループで分割されます。<br />
createLightMapUV (false) : (未サポート)ライトマップ用のUV2を作成します。静的に配置をしてライトマップをベイクする場合にtrueにします。<br />
createTexture (false) : カラーテクスチャを作成します。falseの場合は頂点カラーになります。※カラーテクスチャを作成した場合はDestroy時に明示的に破棄してください。<br />
createVoxelInfo (false) : 各ボクセルの情報を集計したVoxelControllerコンポーネントをGameObjectに追加します。<br />
