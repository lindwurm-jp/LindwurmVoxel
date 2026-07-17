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
  UnityEditorにVXPファイルを入れるとエディタースクリプトでVXPモデルのプレハブが生成されます。
</p>
<h4>動的にVXPファイルからモデルを生成する場合</h4>
<p>
  VoxelUtil.LoadFromFile();
  VolelUtil.LoadFromBinary();
  これらを使ってVXPファイル/データからモデルを生成できます。
</p>
