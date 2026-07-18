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
  UnityEditorにVXPファイルをドロップすると自動でインポートされます。<br>
  <a href="https://github.com/lindwurm-jp/LindwurmVoxel/blob/master/README_import.md">インポートオプションの詳細</a>  
</p>

<h4>動的にVXPファイルからモデルを生成する場合</h4>
<p>
ボクセルモデルを構築（同期版）<br>
CreateVoxelModel() // バイナリから<br>
CreateVoxelModel() // ファイルパスから<br>
ボクセルモデルを構築（非同期版）<br>
CreateVoxelModelAsync() // バイナリから<br>
CreateVoxelModelAsync() // ファイルパスから<br>
<a href="https://github.com/lindwurm-jp/LindwurmVoxel/blob/master/README_create.md">関数の詳細</a>  
</p>
