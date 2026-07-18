<h4>インポートオプション</h4>
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
