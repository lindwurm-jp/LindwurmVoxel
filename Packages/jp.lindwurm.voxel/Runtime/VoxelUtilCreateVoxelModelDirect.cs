//#define UNITY_WEBGL
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
		/// <summary>
		/// ボクセルモデルの情報を取得します。同期版
		/// </summary>
		/// <param name="bytes">VXPデータのバイト列</param>
		/// <returns></returns>
		public static VoxelSaveInfoFormat GetVoxelModelInfo(byte[] bytes)
		{
			VoxelSaveInfoFormat vsi = null;
			LoadFromBinary(bytes, null, true, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			});
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルの情報を取得します。同期版
		/// </summary>
		/// <param name="bytes">VXPデータのファイル名パス</param>
		/// <returns></returns>
		public static VoxelSaveInfoFormat GetVoxelModelInfo(string path)
		{
			VoxelSaveInfoFormat vsi = null;
			LoadFromFile(path, null, true, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			});
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルの情報を取得します。非同期版
		/// </summary>
		/// <param name="bytes">VXPデータのバイト列</param>
		/// <param name="ct">CancellationToken</param>
		/// <returns></returns>
		public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(byte[] bytes, CancellationToken ct = default)
		{
			VoxelSaveInfoFormat vsi = null;
			await Task.Run(() => LoadFromBinary(bytes, null, true, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			}), ct);
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルの情報を取得します。非同期版
		/// </summary>
		/// <param name="bytes">VXPデータのファイル名パス</param>
		/// <param name="ct">CancellationToken</param>
		/// <returns></returns>
		public static async Task<VoxelSaveInfoFormat> GetVoxelModelInfoAsync(string path, CancellationToken ct = default)
		{
			VoxelSaveInfoFormat vsi = null;
			await Task.Run(() => LoadFromFile(path, null, true, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			}), ct);
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルを構築します。同期版
		/// </summary>
		/// <param name="bytes">VXPデータのバイト列</param>
		/// <param name="root">モデルを展開するルートのゲームオブジェクト</param>
		/// <param name="material">表示用のマテリアル</param>
		/// <param name="option">モデル作成オプション</param>
		/// <returns>VoxelSaveInfoFormat</returns>
		public static VoxelSaveInfoFormat CreateVoxelModel(byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Shader Graphs/URPLitVertexColor"));
			}
			VoxelSaveInfoFormat vsi = null;
			VoxelBlock voxelBlock = new();
			voxelBlock.Initialize();
			LoadFromBinary(bytes, voxelBlock, false, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			});
			CreateVoxelModelDirect(voxelBlock, root, material, option, null, null);
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルを構築します。同期版
		/// </summary>
		/// <param name="path">VXPデータのファイルパス</param>
		/// <param name="root">モデルを展開するルートのゲームオブジェクト</param>
		/// <param name="material">表示用のマテリアル</param>
		/// <param name="option">モデル作成オプション</param>
		/// <returns>VoxelSaveInfoFormat</returns>
		public static VoxelSaveInfoFormat CreateVoxelModel(string path, GameObject root, Material material = null, CreateModelOption option = null)
		{
			if (material == null)
			{
				material = new Material(Shader.Find("Shader Graphs/URPLitVertexColor"));
			}
			VoxelSaveInfoFormat vsi = null;
			VoxelBlock voxelBlock = new();
			voxelBlock.Initialize();
			LoadFromFile(path, voxelBlock, false, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			});
			CreateVoxelModelDirect(voxelBlock, root, material, option, null, null);
			return vsi;
		}

		/// <summary>
		/// ボクセルモデルを構築します。非同期版
		/// </summary>
		/// <param name="scontext">メインスレッドのSynchronizationContext</param>
		/// <param name="bytes">VXPデータのバイト列</param>
		/// <param name="root">モデルを展開するルートのゲームオブジェクト</param>
		/// <param name="material">表示用のマテリアル</param>
		/// <param name="option">モデル作成オプション</param>
		/// <param name="callback">作成完了時のコールバック</param>
		/// <param name="ct"></param>
		/// <returns>VoxelSaveInfoFormat</returns>
		public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, byte[] bytes, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default)
		{
			if (material == null)
			{
				material = new Material(Shader.Find("Shader Graphs/URPLitVertexColor"));
			}
			VoxelSaveInfoFormat vsi = null;
			VoxelBlock voxelBlock = new();
			voxelBlock.Initialize();
			await Task.Run(() => LoadFromBinary(bytes, voxelBlock, false, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			}), ct);
			await Task.Run(() =>
			{
				CreateVoxelModelDirect(voxelBlock, root, material, option, scontext);
			}, ct);
			callback?.Invoke(vsi);
			return vsi;
		}
		/// <summary>
		/// ボクセルモデルを構築します。非同期版
		/// </summary>
		/// <param name="scontext">メインスレッドのSynchronizationContext</param>
		/// <param name="path">VXPデータのファイルパス</param>
		/// <param name="root">モデルを展開するルートのゲームオブジェクト</param>
		/// <param name="material">表示用のマテリアル</param>
		/// <param name="option">モデル作成オプション</param>
		/// <param name="callback">作成完了時のコールバック</param>
		/// <param name="ct"></param>
		/// <returns>VoxelSaveInfoFormat</returns>
		public static async Task<VoxelSaveInfoFormat> CreateVoxelModelAsync(SynchronizationContext scontext, string path, GameObject root, Material material = null, CreateModelOption option = null, System.Action<VoxelSaveInfoFormat> callback = null, CancellationToken ct = default)
		{
			if (material == null)
			{
				material = new Material(Shader.Find("Shader Graphs/URPLitVertexColor"));
			}
			VoxelSaveInfoFormat vsi = null;
			VoxelBlock voxelBlock = new();
			voxelBlock.Initialize();
			await Task.Run(() => LoadFromFile(path, voxelBlock, false, (voxelSaveFileInfo) =>
			{
				vsi = voxelSaveFileInfo;
			}), ct);
			await Task.Run(() =>
			{
				CreateVoxelModelDirect(voxelBlock, root, material, option, scontext);
			}, ct);
			callback?.Invoke(vsi);
			return vsi;
		}

		public static void CreateVoxelModelDirectAsync(VoxelBlock voxelBlock, GameObject root, Material material, CreateModelOption option, System.Action callback, CancellationToken ct = default)
        {
#if UNITY_WEBGL
                CreateVoxelModelDirect(voxelBlock, root, material, option, null, callback);
#else
			var scontext = SynchronizationContext.Current;
            var t = Task.Run(() =>
            {
                CreateVoxelModelDirect(voxelBlock, root, material, option, scontext, callback);
            },ct);
#endif
        }
        public static async Task CreateVoxelModelDirectAsync(SynchronizationContext scontext, VoxelBlock voxelBlock, GameObject root, Material material, CreateModelOption option, CancellationToken ct = default)
        {
#if UNITY_WEBGL
			CreateVoxelModelDirect(voxelBlock, root, material, option);
			await Task.CompletedTask;
#else
			var t = Task.Run(() =>
			CreateVoxelModelDirect(voxelBlock, root, material, option,scontext),ct);
            await t;
#endif
        }

		public static void CreateVoxelModelDirect(VoxelBlock voxelBlock, GameObject root, Material material, CreateModelOption option = null, SynchronizationContext scontext = null, System.Action callback = null)
		{
			if (option == null)
			{
				option = new CreateModelOption(false);
			}
			var work = new CreateVoxelDirect();
            if (root == null)
            {
                callback?.Invoke();
            }
            else
            {
                work.Create(scontext, voxelBlock, root, material, option, callback);
            }
        }
        public static Vector3 CSMD_SwapVector(Vector3 v)
        {
            (v.y, v.z) = (v.z, v.y);
            return v;
        }

        public static List<HumanBone> CSMD_CreateHumanBone(VoxelBlock voxelBlock)
        {
            string[] humanName = HumanTrait.BoneName;
            List<HumanBone> humanBones = new List<HumanBone>();
            var joints = voxelBlock.EnumGetJoint();
            while (joints.MoveNext())
            {
                for (var i = 0; i < humanName.Length; i++)
                {
                    if (joints.Current.name == humanName[i])
                    {
                        HumanBone humanBone = new HumanBone();
                        humanBone.humanName = humanName[i];
                        humanBone.boneName = joints.Current.name;
                        humanBone.limit.useDefaultValues = true;
                        humanBones.Add(humanBone);
                        break;
                    }
                }
            }
            return humanBones;
        }

        public static ArmatureData CSMD_CreateArmature(VoxelBlock voxelBlock, GameObject root, float scale, bool thumbnailPose = false)
        {
            var dic = new Dictionary<int, ArmatureBodyData>();
            var nameList = new Dictionary<string, int>();
            var count = 1;

            var armature = new GameObject();
            armature.name = "Armature";
            armature.transform.SetParent(root.transform);
            armature.transform.position = Vector3.zero;
            armature.transform.rotation = Quaternion.identity;
            armature.transform.localScale = Vector3.one;
            var ls = armature.transform.lossyScale.x;
            MakeTree(armature, -1);
            var rt = new Dictionary<int, Vector3>();
            // ツリーのスケールをなくす
            // 絶対位置と、lossyScaleを記録
            foreach(var one in dic)
            {
                rt.Add(one.Key, one.Value.gameObject.transform.position);
                one.Value.lossyScale = MyLossyScale(armature.transform, one.Value.transform, 1f);
            }
            // スケールを1にする
            foreach (var one in dic)
            {
                one.Value.gameObject.transform.localScale = Vector3.one;
            }
            // 変わってしまった子の絶対位置を戻し、計算用のworldTolocalMatrixを更新
            foreach (var one in dic)
            {
                one.Value.gameObject.transform.position = rt[one.Key];
                one.Value.position = rt[one.Key];
                one.Value.worldToLocalMatrix = one.Value.transform.worldToLocalMatrix;
            }
            return new ArmatureData(armature, dic);

            // ただのlossyScaleでも大丈夫そう
            float MyLossyScale(Transform root, Transform target, float sc)
            {
                if(root == target)
                {
                    return sc;
                }
                sc = MyLossyScale(root, target.parent, sc * target.localScale.x);
                return sc;
            }

            void MakeTree(GameObject parentObj, int targetId)
            {
                var joints = voxelBlock.GetJointsFromParentId(targetId);
                if(joints == null)
                {
                    return;
                }
                foreach(var one in joints)
                {
                    var go = new GameObject();
                    var t = go.transform;
					var pname = thumbnailPose ? "Free" : "T";
					var dat = one.GetPoseLocal(pname);
					one.nowPose = pname;
                    var n = one.name;
                    if(string.IsNullOrEmpty(n))
                    {
                        n = $"__{count}";
                        count++;
                    }else if(nameList.ContainsKey(n))
                    {
                        n = $"{n}__{count}";
                        count++;
                    }
                    nameList.Add(n, 1);
                    go.name = n;
                    t.SetParent(parentObj.transform);

                    t.localPosition = (one.localPosition + dat.position) * scale;
                    t.localRotation = Quaternion.Euler(dat.rotation);
                    t.localScale = Vector3.one * dat.scale;
                    var ad = new ArmatureBodyData(go);
                    dic.Add(one.id, ad);
                    MakeTree(go, one.id);
                }
            }
        }
        public static List<SkeletonBone> CSMD_CreateSkeletonBone(VoxelBlock voxelBlock, GameObject armature, GameObject root, Vector3 offsetY, float scale)
        {
            List<SkeletonBone> skeleton = new List<SkeletonBone>();

            // BuildHumanAvatarで、Transform順じゃないと作成できない
            Transform[] avatarTransforms = root.GetComponentsInChildren<Transform>();
            foreach (Transform avatarTransform in avatarTransforms)
            {
                SkeletonBone bone = new SkeletonBone()
                {
                    name = avatarTransform.name,
                    position = avatarTransform.name != "Hips" ? avatarTransform.localPosition : avatarTransform.localPosition + offsetY,
                    rotation = avatarTransform.localRotation,
                    scale = avatarTransform.localScale
                };

                skeleton.Add(bone);
            }
            return skeleton;
        }

        public static Avatar CSMD_CreateAvatar(GameObject root, List<SkeletonBone> skeletonBones, List<HumanBone> humanBones)
        {
            var hd = new HumanDescription()
            {
                armStretch = 0.05f,//IK 使用時の腕の長さの引き伸ばし許容量
                feetSpacing = 0f,//ヒューマノイドモデル脚部の最小距離の調整
                hasTranslationDoF = false,   // Degree of Freedom(自由度：DoF) 表現を持つすべてのヒューマンに対して true を返します。デフォルトで false に設定されます。
                human = humanBones.ToArray(),//Mecanim ボーン名とリグのボーン名の間におけるマッピング
                legStretch = 0.05f, //  IK 使用のときに許容する脚の伸び幅
                lowerArmTwist = 0.5f,// 回転/ ひねりを肘と手首にどの割合で反映するか定義します
                lowerLegTwist = 0.5f,// 回転/ ひねりを膝や足首にどの割合で反映するか定義します
                skeleton = skeletonBones.ToArray(),// モデルに含めるボーン Transform のリスト
                upperArmTwist = 0.5f, //Defines how the upper arm's roll/twisting is distributed between the shoulder and elbow joints.
                upperLegTwist = 0.5f //
            };
            Avatar avatar = null;
            try
            {
                avatar = AvatarBuilder.BuildHumanAvatar(root, hd);
                if(!avatar.isValid || !avatar.isHuman)
                {
                    Debug.Log("Not Valid");
                }

            }
            catch (System.Exception)
            {
                Debug.LogError("avater could not create.");
            }

            return avatar;

        }
        private static (int, Dictionary<Color, int>) MakeTexture(Dictionary<int, MeshData> voxelBlockMesh, SynchronizationContext scontext, Material mat, string mapName)
        {
            Dictionary<Color, int> colorDic = new Dictionary<Color, int>();
            var pos = 0;
            foreach (var one in voxelBlockMesh)
            {
                var id = one.Key;
                var colors = one.Value.colors;
                foreach (var col in colors)
                {
                    if (!colorDic.ContainsKey(col))
                    {
                        colorDic.Add(col, pos);
                        pos++;
                    }
                }
            }
            var texSize = 64;
            if (colorDic.Count > 16 * 16)
            {
                texSize = 128;
            }
            if (colorDic.Count > 32 * 32)
            {
                texSize = 256;
            }
            if (colorDic.Count > 64 * 64)
            {
                texSize = 512;
            }
            if (colorDic.Count > 128 * 128)
            {
                texSize = 1024;
            }
            if (colorDic.Count > 256 * 256)
            {
                texSize = 2048;
            }
            Texture2D tex = null;
            var texSizeFloat = (float)texSize;
            if (scontext != null)
            {
                scontext.Send((_) =>
                {
                    TexProc();
                },null);
            }else
            {
                TexProc();
            }
            void TexProc()
            {
                tex = new Texture2D(texSize, texSize, TextureFormat.RGB24, false, true);
                var paintCol = new Color[] { Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black };
                var ts = texSize / 4;
                var pix1 = 1f / texSize;
                foreach (var c in colorDic)
                {
                    var uvPos = c.Value;
                    var x = uvPos % ts;
                    var y = uvPos / ts;
                    var col = c.Key;
                    for (var i = 0; i < paintCol.Length; i++)
                    {
                        paintCol[i] = col;
                    }
                    tex.SetPixels(x * 4, y * 4, 4, 4, paintCol);
                }
                tex.Apply();
                if(mat != null)
                {
                    mat.SetTexture(mapName, tex);
                }
            }
            return (texSize, colorDic);
        }

    }
}