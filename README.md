<h3>概要</h3>
<p>
  ボクセルモデルデータVXPをUnityで利用できるようにするパッケージです。<br>
  VXPファイルは、ボクセルエディターである<a href="https://voxelcraftking.com/">「ボクセルクラフト王」</a>の保存データ形式です。<br>
</p>

<h3>インストール方法</h3>
<p>Package ManagerのInstall package from git URL...に以下のパスを入力しInstall</p>
<table>
  <tr><td>Lindwurm Voxel Library</td><td>https://github.com/lindwurm-jp/LindwurmVoxel.git?path=/Packages/jp.lindwurm.voxel</td></tr>
</table>

<h3>使い方</h3>
<h4>予め用意しておく場合</h4>
<p>
  UnityEditorにVXPファイルをドロップすると自動でインポートされます。
</p>
<h5>インポートオプション</h5>
<ul>
  <li>Try creating a Humanoid : HumanoidモデルとしてAvatarの作成を試みます。失敗した場合はGenericモデルとしてAvatarが作成されます。</li>
  <li>Import Thumbnail : vxpファイル内のサムネイルを画像として作成します。</li>
  <li>Attach Collider : コライダーを付けます</li>
  <li>Import for URP : URPを使用する場合にtrueにします。URPをサポートしていないUnityの場合はfalseにします。</li>
  <li>Include local rotation : vxpを作成したときに設定しているジョイントのローカル回転を反映した状態でモデルを作成します。Humanoidオプションをfalseにしたとき、全ての関節をIdentityにすると、ポーズがリセットされるようにするオプションです。</li>
  <li>Split all groups : trueで全メッシュをジョイントごとで分割します。falseではボクセルクラフト王のMeshGroupで指定したグループで分割されます。</li>
  <li>Free pose : trueで初期ポーズをサムネイル用ポーズにします。通常はTポーズです。</li>
  <li>Override Size : ボクセルモデルのボクセルの大きさを上書きします。</li>
  <li>Use Mesh Renderer : MeshRendererでモデルを作成します。falseの場合は、SkinnedMeshrendererを使用します。</li>
  <li>Create Texture : モデルをVertexColorでなくTextureを使用して作成します。Texture(色をパレットとして並べたもの)が作成されます。</li>
  <li>Create Vocel Info : 各ボクセルの情報を集計したVoxelControllerコンポーネントをGameObjectに追加します。</li>
  <li>Create Animator : Animatorをモデルに配置します</li>
  <li>Lit VertexColor Shader : StanderdでTextureを使用しない時に使用するシェーダーを指定します。</li>
  <li>URP Lit VertexColor Shader : URPでTextureを使用しない時にするシェーダーを設定します。</li>
  <li>Lit Shader : StanderdでTextureを使用する時に使用するシェーダーを設定します。</li>
  <li>URP Lit Shader : URPでTextureを使用する時に使用するシェーダーを設定します。</li>
</ul>

<h4>動的にVXPファイルからモデルを生成する場合</h4>
namespaceはLindwurm.Voxel.VoxelUtilです。
<div>
// ボクセルモデルの情報を取得します。同期版<br />
public static VoxelSaveInfoFormat GetVoxelModelInfo(byte[] bytes); // バイナリから<br />
public static VoxelSaveInfoFormat GetVoxelModelInfo(string path); // ファイルパスから<br />
// ボクセルモデルの情報を取得します。非同期版<br />
public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(byte[] bytes, CancellationToken ct = default) // バイナリから<br />
public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(string path, CancellationToken ct = default) // ファイルパスから<br />
// ボクセルモデルを構築します。同期版　<br />
public static VoxelSaveInfoFormat CreateVoxelModel(byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null) // バイナリから<br />
public static VoxelSaveInfoFormat CreateVoxelModel(string path, GameObject root, Material material = null, CreateModelOption option = null) // ファイルパスから<br />
// ボクセルモデルを構築します。非同期版　<br />
public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default) // バイナリから<br />
public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, string path, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default) // ファイルパスから<br />
// 引数の説明<br />
scontext:メインスレッドのSynchronizationContext <br/>
root:モデルを展開するルートのゲームオブジェクト<br />
material:表示用のマテリアル<br />
option:モデル作成オプション<br />
callback:作成完了時のコールバック<br />
// CreateModelOptionの説明<br />
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
</div>
<p>
  
  これらを使ってVXPファイル/データからモデルを生成できます。<br>
</p>

