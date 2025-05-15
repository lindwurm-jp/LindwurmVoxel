using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
		/// <summary>
		/// モデル作成のオプションを設定します。
		///   createCollider (true) : BoxColliderを作成します。
		///   thumbnailPose (false) : trueで初期ポーズをサムネイル用ポーズにします。通常はTポーズです。
		///   isHumanoid (true) : HumanoidモデルとしてAvatarの作成を試みます。失敗した場合はGenericモデルとしてAvatarが作成されます。
		///   overrideScale (0) : １ブロックの大きさを上書きします。0で保存時の大きさになります。
		///   includeRotation (true) : trueで保存時の回転をMeshに焼きこみます。falseでTransformに回転が反映されます
		///   useMeshRenderer (false) : trueでMeshRendererを使用します。falseでSkinnedMeshRendererを使用します。
		///   splitAllGroup (false) : trueで全メッシュをジョイントごとで分割します。falseではボクセルクラフト王のMeshGroupで指定したグループで分割されます。
		///   createLightMapUV (false) : ライトマップ用のUV2を作成します。静的に配置をしてライトマップをベイクする場合にtrueにします。
		///   createTexture (false) : カラーテクスチャを作成します。falseの場合は頂点カラーになります。※カラーテクスチャを作成した場合はDestroy時に明示的に破棄してください。
		///   createVoxelInfo (false) : 各ボクセルの情報を集計したVoxelInformationコンポーネントをGameObjectに追加します。
		/// </summary>
		public class CreateModelOption
        {
            public bool createCollider;
            public bool splitAllGroup;
            public bool thumbnailPose;
            public bool isHumanoid;
            public float overrideScale;
            public bool includeRotation;
            public bool useMeshRenderer;
            public bool createLightMapUV;
            public bool createTexture;
			public bool createVoxelInfo;
			public bool createAnimator;

            public CreateModelOption(bool useMeshRenderer)
            {
                createCollider = true;
                splitAllGroup = false;
                thumbnailPose = false;
                isHumanoid = true;
                overrideScale = 0f;
                includeRotation = true;
                this.useMeshRenderer = useMeshRenderer;
                createLightMapUV = false;
                createTexture = false;
				createVoxelInfo = false;
				createAnimator = false;
            }
        }
        public class MeshData
        {
            public Vector3[] verts;
            public Vector3[] normals;
            public Color[] colors;
            public int[] indies;
            //public Mesh mesh;
            public Bounds bounds;

            public MeshData(Vector3[] verts, Vector3[] normals, Color[] colors, int[]indies )
            {
                this.verts = verts;
                this.normals = normals;
                this.colors = colors;
                this.indies = indies;
                this.bounds = new Bounds();
                //mesh = null;
            }
        }
        public class ArmatureData
        {
            public GameObject root;
            public Dictionary<int, ArmatureBodyData> body;

            public ArmatureData(GameObject root,  Dictionary<int, ArmatureBodyData> body)
            {
                this.root = root;
                this.body = body;
            }
        }
        public class ArmatureBodyData
        {
            // メインスレッド以外でGameObjectの中を参照すると例外が発生するので、個別に記録しておく
            public GameObject gameObject;
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;
            public Matrix4x4 worldToLocalMatrix;
            public float lossyScale;
            public Quaternion modifiedRoataion;

            public ArmatureBodyData(GameObject obj)
            {
                gameObject = obj;
                transform = obj.transform;
                position = obj.transform.position;
                rotation = obj.transform.rotation;
                worldToLocalMatrix = obj.transform.worldToLocalMatrix;
                lossyScale = obj.transform.lossyScale.x;
                modifiedRoataion = Quaternion.identity;
            }
        }
        public class CreateVoxelDirect
        {
            private SynchronizationContext scontext;

            private VoxelBlock voxelBlock;
            private GameObject root;
            private Material material;
            private CreateModelOption option;

            private Vector3 savePos = Vector3.zero;
            private Quaternion saveRotation = Quaternion.identity;
            private Vector3 saveScale = Vector3.one;

            private List<int> groups = new List<int>();

            private float scale = 50 / 1000f;
            private float offsetY = 0f;
            private Vector3 hipOffset = Vector3.zero;

            private Animator animator = null;

            private Dictionary<int, MeshData> voxelBlockMesh = new Dictionary<int, MeshData>();
            private ArmatureData armature = null;

            int textureSize = 0;

            Dictionary<Color, int> colorDic = null;

            private System.Action callback = null;

            // meshrenderer 専用
            private Dictionary<int, SimpleMeshData> simpleMeshData = new Dictionary<int, SimpleMeshData>();

            // skinned mesh 専用
            private Dictionary<int, SkinnedMeshData> skinnedMeshData = new Dictionary<int, SkinnedMeshData>();
            private List<Transform> boneTransforms = new List<Transform>();
            List<Matrix4x4> poses = new List<Matrix4x4>();

			private Dictionary<int, VolumeInfo> volumeInfo = new Dictionary<int, VolumeInfo>();

			private bool isHumanoid;	// humanoidで作成されたかどうか

            public void Create(SynchronizationContext scontext, VoxelBlock voxelBlock, GameObject root, Material material, CreateModelOption option, System.Action callback)
            {
                this.scontext = scontext;
                this.voxelBlock = voxelBlock;
                this.root = root;
                this.material = material;
                this.option = option;
                this.callback = callback;
                this.abortCreate = false;

                scale = 50 / 1000f;
                if (voxelBlock.voxelModelFileParameter != null)
                {
                    scale = voxelBlock.voxelModelFileParameter.scale;
                }
                if (option.overrideScale > 0)
                {
                    scale = option.overrideScale / 1000f;
                }
                offsetY = 0f;
                hipOffset = Vector3.zero;

                var blocks = voxelBlock.GetBlock();
                while (blocks.MoveNext())
                {
                    var b = blocks.Current;
                    if (b.enabled)
                    {
                        var md = CreateMeshContents(voxelBlock.GetJoint(b.id), b.voxelBlockData, voxelBlock.GetPalette());
                        //meshData.Add(md);
                        voxelBlockMesh.Add(b.id, md);

						if(option.createVoxelInfo)
						{
							var v = new VolumeInfo();
							v.singleBlocks = b.voxelBlockData.blocks.Count;
							v.quaterBlocks = b.voxelBlockData.quaterBlocks.Count;
							var poss = Vector3Int.zero;
							foreach(var one in b.voxelBlockData.blocks)
							{
								poss += one.Key;
							}
							var posq = Vector3Int.zero;
							foreach(var one in b.voxelBlockData.quaterBlocks)
							{
								posq += one.Key;
							}
							var cogs = new Vector3(poss.x, poss.y, poss.z);
							var cogq = new Vector3(posq.x, posq.y, posq.z) / 2f - new Vector3(0.25f,0.25f,0.25f) * v.quaterBlocks;
							if(v.singleBlocks == 0 && v.quaterBlocks == 0)
							{
								v.centerOfGravity = Vector3.zero;
							}else
							{
								v.centerOfGravity = (cogs * 4 + cogq) / (v.singleBlocks * 4 + v.quaterBlocks) - voxelBlock.GetJoint(b.id).position;
							}
							v.meshGroupNo = -1;
							volumeInfo.Add(b.id, v);
						}
					}
                }
                ContextProc(SetupParameters);
                if (abortCreate)
                {
                    this.callback?.Invoke();
                    return;
                }

                // グループの設定
                var joints = voxelBlock.EnumGetJoint();
                var cnt = 0;
                while (joints.MoveNext())
                {
                    var gp = voxelBlock.GetMeshGroup(joints.Current.id);
                    if (option.splitAllGroup)
                    {
                        gp = cnt;
                        cnt++;
                    }
                    if (!groups.Contains(gp))
                    {
                        groups.Add(gp);
                    }
                }

                // テクスチャの生成と設定
                if (option.createTexture)
                {
                    (textureSize, colorDic) = MakeTexture(voxelBlockMesh, scontext, material, "_BaseMap");
                }

                // Meshrenderer / SkinnedMeshRenderer
                if(option.useMeshRenderer)
                {
                    UseMeshRenderer();
                }else
                {
                    UseSkinnedMeshRenderer();
                }
                if (abortCreate)
                {
                    this.callback?.Invoke();
                    return;
                }

                // 境界作成
                foreach (var one in voxelBlockMesh)
                {
                    var verts = one.Value.verts;
                    if (verts.Length > 0)
                    {
                        Vector3 min = verts[0];
                        Vector3 max = verts[0];
                        for (var i = 1; i < verts.Length; i++)
                        {
                            if (verts[i].x <= min.x)
                            {
                                min.x = verts[i].x;
                            }
                            else if (verts[i].x >= max.x)
                            {
                                max.x = verts[i].x;
                            }
                            if (verts[i].y <= min.y)
                            {
                                min.y = verts[i].y;
                            }
                            else if (verts[i].y >= max.y)
                            {
                                max.y = verts[i].y;
                            }
                            if (verts[i].z <= min.z)
                            {
                                min.z = verts[i].z;
                            }
                            else if (verts[i].z >= max.z)
                            {
                                max.z = verts[i].z;
                            }
                        }
                        one.Value.bounds = new Bounds((min+ max)/2, max-min);
                    }
                }
                ContextProc(MainThreadProc3);
                if (abortCreate)
                {
                    this.callback?.Invoke();
                    return;
                }
            }

            private bool abortCreate = false;
            private bool CheckDestroy()
            {
                if(abortCreate)
                {
                    return true;
                }
                if(this.root == null)
                {
                    abortCreate = true;
                }
                return abortCreate;
            }
            private void ContextProc(System.Action callProc)
            {
                if(scontext != null)
                {
                    scontext.Send((_)=> {
                        if(!CheckDestroy())
                        {
                            callProc();
                        }
                    },null);
                }else
                {
                    callProc();
                }
            }
            private void SetupParameters()
            {
                savePos = root.transform.position;
                saveRotation = root.transform.rotation;
                saveScale = root.transform.localScale;
                root.transform.position = Vector3.zero;
				root.transform.rotation = Quaternion.identity;
                animator = root.GetComponent<Animator>();
                if (animator == null && option.createAnimator)
                {
                    animator = root.AddComponent<Animator>();
                }
                // armature作成
                armature = CSMD_CreateArmature(voxelBlock, root, scale, option.thumbnailPose);

                // 各関節のrotationをIdentityにする(全部を0にすると初期ポーズに戻るGenericモデル用)
                if (option.includeRotation)
                {
                    Dictionary<int, (Vector3 pos, Quaternion qt)> buf = new Dictionary<int, (Vector3 pos, Quaternion qt)>();
                    foreach (var one in armature.body)
                    {
                        var id = one.Key;
                        buf.Add(id, (one.Value.gameObject.transform.position, one.Value.gameObject.transform.rotation));
                    }
                    SetArmatureToIdentity(armature.root.transform, buf);
                }

            }
            private void LastSetupParameters()
            {
                hipOffset = new Vector3(0, offsetY, 0);
                root.transform.localPosition = savePos;
                root.transform.localScale = saveScale;
                root.transform.rotation = saveRotation;
                // Genericアニメーションの場合のAvatar位置補正
                armature.body[0].gameObject.transform.localPosition = option.isHumanoid ? Vector3.zero : -hipOffset;
            }

            private void SetArmatureToIdentity(Transform tr, Dictionary<int, (Vector3 pos, Quaternion qt)> buf)
            {
                foreach (var one in armature.body)
                {
                    if (one.Value.gameObject.transform == tr && buf.ContainsKey(one.Key))
                    {
                        tr.position = buf[one.Key].pos;
                        tr.rotation = Quaternion.identity;
                        one.Value.rotation = Quaternion.identity;
                        if (voxelBlockMesh.ContainsKey(one.Key))
                        {
                            var qt = buf[one.Key].qt;
                            if (qt != Quaternion.identity)
                            {
                                one.Value.modifiedRoataion = qt;
                            }
                        }
                        break;
                    }
                }
                if (tr.childCount > 0)
                {
                    for (var i = 0; i < tr.childCount; i++)
                    {
                        SetArmatureToIdentity(tr.GetChild(i), buf);
                    }
                }
            }
            private float GetMinY(float y, Transform tr, Bounds meshBounds, float scale)
            {
                var ry = 10000f;
                var center = meshBounds.center * scale;
                var corner = meshBounds.extents * scale;
                var cc = new Vector3[] {
                    center + new Vector3(corner.x, corner.y, corner.z),
                    center + new Vector3(corner.x, -corner.y, corner.z),
                    center + new Vector3(-corner.x, corner.y, corner.z),
                    center + new Vector3(-corner.x, -corner.y, corner.z),
                    center + new Vector3(corner.x, corner.y, -corner.z),
                    center + new Vector3(corner.x, -corner.y, -corner.z),
                    center + new Vector3(-corner.x, corner.y, -corner.z),
                    center + new Vector3(-corner.x, -corner.y, -corner.z)
                };
                foreach (var one in cc)
                {
                    var p = tr.TransformPoint(one);
                    if (p.y < ry)
                    {
                        ry = p.y;
                    }
                }

                return ry < y ? ry : y;
            }

            private void UseMeshRenderer()
            {
                var ts = textureSize / 4;
                var pix1 = 1f / textureSize;

                Vector3 basePos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                ContextProc(MaintThreadProc1);
                if (abortCreate) return;

                var joints = voxelBlock.EnumGetJoint();
                int cnt = 0;
                while (joints.MoveNext())
                {
                    var id = joints.Current.id;
                    var en = voxelBlock.isEnableEdit(id);
                    var gp = voxelBlock.GetMeshGroup(id);
                    if (option.splitAllGroup)
                    {
                        gp = cnt;
                        cnt++;
                    }
                    var w = simpleMeshData[gp];
                    var offsetTris = w.verts.Count;

                    basePos = armature.body[id].position - w.armaturePosition;
                    var lossyScale = armature.body[id].lossyScale * scale;// scale; // エクスポートしてないので、したと仮定したサイズにする必要がある

                    if(option.includeRotation)
                    {
                        rot = armature.body[id].modifiedRoataion;
                    }
                    else
                    {
                        rot = w.armatureInverseRotation * armature.body[id].rotation;
                    }
                    if (en)
                    {
						if (option.createVoxelInfo)
						{
							var v = volumeInfo[id];
							v.size = lossyScale;
							v.volume = lossyScale * lossyScale * lossyScale * (v.singleBlocks + v.quaterBlocks / 8f);
							v.meshGroupNo = gp;
							v.gameObject = armature.body[id].gameObject;
							v.centerOfGravity = (option.includeRotation ? rot * v.centerOfGravity : v.centerOfGravity) * lossyScale;
							volumeInfo[id] = v;
						}
						var localMesh = voxelBlockMesh[id];
                        foreach (var one in localMesh.verts)
                        {
                            w.verts.Add(basePos + (rot * one) * lossyScale);
                        }
                        foreach (var one in localMesh.normals)
                        {
                            w.normals.Add(rot * one);
                        }
                        if (option.createTexture)
                        {
                            foreach (var one in localMesh.colors)
                            {
                                var p = colorDic[one];
                                w.uvs.Add(new Vector2((p % ts) * 4 / (float)textureSize + pix1, (p / ts) * 4 / (float)textureSize + pix1));
                            }
                        }
                        else
                        {
                            w.colors.AddRange(localMesh.colors);
                        }

                        for (var i = 0; i < localMesh.indies.Length; i++)
                        {
                            w.tris.Add(localMesh.indies[i] + offsetTris);
                        }
                        if (option.createLightMapUV)
                        {
                            // ひとつなぎのUVをグループ化する
                            List<Dictionary<int,Vector2>> uvGP = new List<Dictionary<int, Vector2>>();
                            // グループを作成していく。
                            for (var i = 0; i < localMesh.indies.Length; i+=3)
                            {
                                var tf = false;
                                var i0 = localMesh.indies[i] + offsetTris;
                                var i1 = localMesh.indies[i+1] + offsetTris;
                                var i2 = localMesh.indies[i+2] + offsetTris;
                                for (var j = 0; j < uvGP.Count; j++)
                                {
                                    if (uvGP[j].ContainsKey(i0))
                                    {
                                        if (!uvGP[j].ContainsKey(i1))
                                        {
                                            uvGP[j].Add(i1, V3ToV2(i1));
                                        }
                                        if (!uvGP[j].ContainsKey(i2))
                                        {
                                            uvGP[j].Add(i2, V3ToV2(i2));
                                        }
                                        tf = true;
                                        break;
                                    }
                                    if (uvGP[j].ContainsKey(i1))
                                    {
                                        if (!uvGP[j].ContainsKey(i0))
                                        {
                                            uvGP[j].Add(i0, V3ToV2(i0));
                                        }
                                        if (!uvGP[j].ContainsKey(i2))
                                        {
                                            uvGP[j].Add(i2, V3ToV2(i2));
                                        }
                                        tf = true;
                                        break;
                                    }
                                    if (uvGP[j].ContainsKey(i2))
                                    {
                                        if (!uvGP[j].ContainsKey(i0))
                                        {
                                            uvGP[j].Add(i0, V3ToV2(i0));
                                        }
                                        if (!uvGP[j].ContainsKey(i1))
                                        {
                                            uvGP[j].Add(i1, V3ToV2(i1));
                                        }
                                        tf = true;
                                        break;
                                    }
                                }
                                if (!tf)
                                {
                                    var l = new Dictionary<int, Vector2>();
                                    l.Add(i0, V3ToV2(i0));
                                    l.Add(i1, V3ToV2(i1));
                                    l.Add(i2, V3ToV2(i2));
                                    uvGP.Add(l);
                                }
                                Vector2 V3ToV2(int idx)
                                {
                                    var v = localMesh.verts[idx-offsetTris] * 4;
                                    if (localMesh.normals[idx-offsetTris].x != 0)
                                    {
                                        return new Vector2(v.y, v.z);
                                    } else if(localMesh.normals[idx-offsetTris].y != 0)
                                    {
                                        return new Vector2(v.x, v.z);
                                    }
                                    return new Vector2(v.x, v.y);
                                }
                            }
                            // グループ化しそこなったやつをまとめる
                            if (uvGP.Count > 0)
                            {
                                var f = true;
                                while(f)
                                {
                                    for(var i = uvGP.Count-1; i > 0; i--)
                                    {
                                        f = false;
                                        for (var j = 0; j < i; j++)
                                        {
                                            foreach (var p in uvGP[i])
                                            {
                                                if (uvGP[j].ContainsKey(p.Key))
                                                {
                                                    f = true;
                                                    break;
                                                }
                                            }
                                            if(f)
                                            {
                                                foreach (var p1 in uvGP[i])
                                                {
                                                    if (!uvGP[j].ContainsKey(p1.Key))
                                                    {
                                                        uvGP[j].Add(p1.Key, p1.Value);
                                                    }
                                                }
                                                uvGP.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                }
                                w.uvGroup.AddRange(uvGP);

                            }
                        }
                    }
                }

                if (option.createLightMapUV)
                {
                    foreach (var one in simpleMeshData)
                    {
                        List<(Dictionary<int,Vector2>gp, Vector2 min, Vector2 size, Vector2 offs)> vl = new List<(Dictionary<int, Vector2> gp, Vector2 min, Vector2 size, Vector2 offs)>();
                        foreach (var gp in one.Value.uvGroup)
                        {
                            var minx = gp.Values.Min(p => p.x);
                            var miny = gp.Values.Min(p => p.y);
                            var maxx = gp.Values.Max(p => p.x);
                            var maxy = gp.Values.Max(p => p.y);
                            vl.Add((gp, new Vector2(minx,miny), new Vector2(maxx - minx, maxy - miny), Vector2.zero));
                        }

                        var texl = new Vector2(16, 16);
                        var margin = new Vector2(4,4);
                        var retry = true;
                        var err = false;
                        var reverse = 0;
                        while (retry)
                        {
                            var offs = Vector2.zero;
                            err = false;
                            retry = false;
                            for (var i = 0; i < vl.Count && !err; i++)
                            {
                                var tgt = vl[i];
                                var tgtl = -tgt.min;
                                var tgth = tgt.size * texl + margin * 2;
                                if (tgth.x > 1023 || tgth.y > 1023)
                                {
                                    err = true;
                                    break;
                                }
                                err = false;
                                var retryOne = true;
                                while (retryOne && !err)
                                {
                                    retryOne = false;
                                    for (var j = 0; j < i; j++)
                                    {
                                        var o = vl[j].offs;
                                        var ol = -vl[j].min;

                                        var k1e = tgth + offs;
                                        var k2e = vl[j].size * texl + margin * 2 + o;
                                        if (!(offs.x <= k2e.x && o.x <= k1e.x && offs.y <= k2e.y && o.y <= k1e.y))
                                        {
                                            continue;
                                        }

                                        foreach (var t1 in vl[j].gp)
                                        {
                                            var pmin = (t1.Value + ol) * texl + o;
                                            var pmax = pmin + texl + margin * 2;    // texl分だけちょっとでかい

                                            if(offs.x <= pmax.x && pmin.x <= k1e.x && offs.y <= pmax.y && pmin.y <= k1e.y)
                                            {
                                                retryOne = true;
                                                offs.x += texl.x;
                                                if (offs.x + tgth.x >= 1024)
                                                {
                                                    offs.x = 0;
                                                    offs.y += texl.y;
                                                    if (offs.y + tgth.y >= 1024)
                                                    {
                                                        reverse++;
                                                        if(reverse > 3)
                                                        {
                                                            err = true;
                                                        }else
                                                        {
                                                            offs = Vector2.zero;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                            if (err || retryOne)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                {
                                    tgt.offs = offs;
                                    vl[i] = tgt;
                                }

                            }
                            if (err)
                            {
                                Debug.Log($"lightmap uv size overflow : {texl} texel, margin {margin}.");
                                if(margin.x > 2)
                                {
                                    margin /= 2;
                                }else
                                {
                                    texl /= 2;
                                }
                                if (texl.x == 1)
                                {
                                    Debug.Log("To many surfaces. Could not create lightmap uv.");
                                    break;
                                }
                                Debug.Log($"Try texel {texl}, margin {margin}");
                                retry = true;
                            }
                        }
                        if (!err)
                        {
                            var uv2 = new List<Vector2>();
                            for (var i = 0; i < one.Value.verts.Count; i++)
                            {
                                foreach (var gp in vl)
                                {
                                    if(gp.gp.ContainsKey(i))
                                    {
                                        uv2.Add(((gp.gp[i] - gp.min) * texl + gp.offs + margin) / 1024.0f);
                                        break;
                                    }
                                }

                            }
                            one.Value.uv2 = uv2;
                        }
                    }
                }

                var lst = simpleMeshData.ToArray();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();                
                for (var i = 0; i < lst.Length; i++)
                {
                    if (scontext != null)
                    {
                        scontext.Send((_) =>
                        {
                            if (CheckDestroy()) return;
                            sw.Restart();
                            for (var j = i; j < lst.Length; j++)
                            {
                                i = j;
                                lst[i].Value.SetMesh(material, null);
                                if (sw.ElapsedMilliseconds > 1)
                                {
                                    break;
                                }
                            }
                            sw.Stop();
                        }, null);
                    }
                    else
                    {
                        lst[i].Value.SetMesh(material, null);
                    }
                }
            }
            void UseSkinnedMeshRenderer()
            {
                var ts = textureSize / 4;
                var pix1 = 1f / textureSize;

                byte skeletonBonesNo = 1;
                Vector3 basePos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                ContextProc(MaintThreadProc2);
                if (abortCreate) return;

                var joints = voxelBlock.EnumGetJoint();
                int cnt = 0;
                while (joints.MoveNext())
                {
                    var id = joints.Current.id;
                    var en = voxelBlock.isEnableEdit(id);
                    var gp = voxelBlock.GetMeshGroup(id);
                    if (option.splitAllGroup)
                    {
                        gp = cnt;
                        cnt++;
                    }
                    var w = skinnedMeshData[gp];
                    var offsetTris = w.verts.Count;

                    basePos = armature.body[id].position - w.smrPosition;
                    var lossyScale = armature.body[id].lossyScale * scale;// scale; // エクスポートしてないので、したと仮定したサイズにする必要がある

                    if(option.includeRotation)
                    {
						rot = armature.body[id].modifiedRoataion * armature.body[id].modifiedRoataion;
                    }else
                    {
	                    rot = armature.body[id].rotation;
                    }
                    if (en)
                    {
						if (option.createVoxelInfo)
						{
							var v = volumeInfo[id];
							v.size = lossyScale;
							v.volume = lossyScale * lossyScale * lossyScale * (v.singleBlocks + v.quaterBlocks / 8f);
							v.meshGroupNo = gp;
							v.gameObject = armature.body[id].gameObject;
							v.centerOfGravity = (option.includeRotation ? armature.body[id].modifiedRoataion * v.centerOfGravity  : v.centerOfGravity) * lossyScale;
							volumeInfo[id] = v;
						}						
						var localMesh = voxelBlockMesh[id];
                        foreach (var one in localMesh.verts)
                        {
                            w.verts.Add(basePos + (rot * one) * lossyScale);
                        }
                        foreach (var one in localMesh.normals)
                        {
                            w.normals.Add(rot * one);
                        }
                        if (option.createTexture)
                        {
                            foreach (var one in localMesh.colors)
                            {
                                var p = colorDic[one];
                                w.uvs.Add(new Vector2((p % ts) * 4 / (float)textureSize + pix1, (p / ts) * 4 / (float)textureSize + pix1));
                            }
                        }
                        else
                        {
                            w.colors.AddRange(localMesh.colors);
                        }
                        for (var i = 0; i < localMesh.indies.Length; i++)
                        {
                            w.tris.Add(localMesh.indies[i] + offsetTris);
                        }
                        for (var i = 0; i < localMesh.verts.Length; i++)
                        {
                            w.boneIndex.Add(skeletonBonesNo);
                        }
                    }
                    boneTransforms.Add(armature.body[id].transform);
                    poses.Add(armature.body[id].worldToLocalMatrix * skinnedMeshData[gp].localToWorldMatrix);
                    skeletonBonesNo++;
                }

                var lst = skinnedMeshData.ToArray();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                for (var i = 0; i < lst.Length; i++)
                {
                    if (scontext != null)
                    {
                        scontext.Send((_) =>
                        {
                            if (CheckDestroy()) return;
                            sw.Restart();
                            for (var j = i; j < lst.Length; j++)
                            {
                                i = j;
                                lst[i].Value.SetMesh(material, null, boneTransforms, poses);
                                if (sw.ElapsedMilliseconds > 1)
                                {
                                    break;
                                }
                            }
                            sw.Stop();
                        }, null);
                    }
                    else
                    {
                        lst[i].Value.SetMesh(material, null, boneTransforms, poses);
                    }
                }
            }


            void MaintThreadProc1()
            {
                groups.ForEach(t =>
                {
                    simpleMeshData.Add(t,new SimpleMeshData(t));
                });

                // グループ内で一番根元に近いGameObjectを設定(共通の根元がない場合は作成時の順番でどちらかになる)
                var joints = voxelBlock.EnumGetJoint();
                int cnt = 0;
                while (joints.MoveNext())
                {
                    var id = joints.Current.id;
                    var gp = voxelBlock.GetMeshGroup(id);
                    if (option.splitAllGroup)
                    {
                        gp = cnt;
                        cnt++;
                    }
                    simpleMeshData[gp].TrySetArmature(root, armature.body[id].gameObject);
                }
                foreach (var one in simpleMeshData)
                {
                    one.Value.armaturePosition = one.Value.armature.transform.position;
                    if(option.includeRotation)
                    {
                        one.Value.armatureInverseRotation = Quaternion.identity;
                    }else
                    {
                        one.Value.armatureInverseRotation = Quaternion.Inverse(one.Value.armature.transform.rotation);
                    }
                }
            }
            void MaintThreadProc2()
            {
                groups.ForEach(t =>
                {
                    skinnedMeshData[t] = new SkinnedMeshData(root, armature.body[0].gameObject, t);
                    skinnedMeshData[t].localToWorldMatrix = skinnedMeshData[t].smr.transform.localToWorldMatrix;
                    skinnedMeshData[t].smrPosition = skinnedMeshData[t].smr.transform.position;
                });
                boneTransforms.Add(armature.root.transform);
                poses.Add(armature.root.transform.worldToLocalMatrix * skinnedMeshData.First().Value.smr.transform.localToWorldMatrix);
            }

            void MainThreadProc3()
            {
                if (option.createCollider)
                {
                    // コライダーの範囲を計算
                    foreach (var one in voxelBlockMesh)
                    {
                        var id = one.Key;
                        var bound = one.Value.bounds;
						if(option.includeRotation)
						{
							var rot = armature.body[id].modifiedRoataion;
							var ps = new Vector3[8];
							var p0 = bound.max;
							var p1 = bound.min;
							ps[0] = rot * p0;
							ps[1] = rot * p1;
							ps[2] = rot * new Vector3(p0.x, p0.y, p1.z);
							ps[3] = rot * new Vector3(p0.x, p1.y, p0.z);
							ps[4] = rot * new Vector3(p1.x, p0.y, p0.z);
							ps[5] = rot * new Vector3(p1.x, p1.y, p0.z);
							ps[6] = rot * new Vector3(p1.x, p0.y, p1.z);
							ps[7] = rot * new Vector3(p0.x, p1.y, p1.z);
							bound = new Bounds(rot * bound.center,
								new Vector3(
									Mathf.Max(ps.Select(v=>v.x).ToArray())
									, Mathf.Max(ps.Select(v => v.y).ToArray())
									, Mathf.Max(ps.Select(v => v.z).ToArray())
									)
								-
								new Vector3(
									Mathf.Min(ps.Select(v => v.x).ToArray())
									, Mathf.Min(ps.Select(v => v.y).ToArray())
									, Mathf.Min(ps.Select(v => v.z).ToArray())
									)
								);
						}
						var lossyScale = armature.body[id].lossyScale * scale;
                        var bc = armature.body[id].gameObject.AddComponent<BoxCollider>();
                        bc.center = bound.center * lossyScale;
                        bc.size = bound.size * lossyScale;
						if (option.createVoxelInfo)
						{
							var v = volumeInfo[id];
							v.collider = bc;
							volumeInfo[id] = v;
						}
					}
				}
				// viewObjectを一時割り当て
				var joints = voxelBlock.EnumGetJoint();
                while (joints.MoveNext())
                {
                    var j = joints.Current;
                    j.viewObject = armature.body[j.id].gameObject;
                }
				// Constraint付与
				voxelBlock.SetConstraintAll();
				// 機能ブロックパラメータを設定
				List<FunctionBlockBase> list = new List<FunctionBlockBase>();
				voxelBlock.SetFunctionBlockAll(voxelBlock.voxelModelFileParameter.scale, list);
				foreach(var one in list)
				{
					one.ApplyForPrefab();
				}

                joints = voxelBlock.EnumGetJoint();
				// viewObjectを解放
				while (joints.MoveNext())
                {
                    var j = joints.Current;
                    j.viewObject = null;
                }

				// Tポーズの下限（足元）に原点を置く
				foreach (var one in voxelBlockMesh)
                {
                    var id = one.Key;
                    offsetY = GetMinY(offsetY, armature.body[id].gameObject.transform, one.Value.bounds, scale * armature.body[id].lossyScale);
                }
                if (option.isHumanoid)
                {
                    // skeletonBone作成
                    var skeletonBones = CSMD_CreateSkeletonBone(voxelBlock, armature.root, root, new Vector3(0, -offsetY, 0), scale);

                    // humanBone
                    List<HumanBone> humanBones = CSMD_CreateHumanBone(voxelBlock);

                    var tf = humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.Hips])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.Spine])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.Head])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftUpperArm])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightUpperArm])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftLowerArm])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightLowerArm])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftHand])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightHand])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftUpperLeg])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightUpperLeg])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftLowerLeg])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightLowerLeg])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.LeftFoot])
                        && humanBones.Exists(x => x.humanName == HumanTrait.BoneName[(int)HumanBodyBones.RightFoot])
                        ;
                    if (tf)
                    {
                        // Avater
                        Avatar avatar = CSMD_CreateAvatar(root, skeletonBones, humanBones);

                        if (avatar != null)
                        {
							isHumanoid = true;
							if(animator != null)
							{
								avatar.name = "avatar";
								animator.avatar = avatar;
							}
						}
						else
                        {
							isHumanoid = false;
							if (animator != null)
							{
								avatar = AvatarBuilder.BuildGenericAvatar(root, "");
								avatar.name = "avatar";
								animator.avatar = avatar;
							}
						}
#if UNITY_EDITOR
						//                        Debug.Log($"Avatar.isHumanoid : {avatar.isHuman}");
#endif
					}
                    else
                    {
#if UNITY_EDITOR
						Debug.Log("Failed to try creating a Humanoid");
#endif
						isHumanoid = false;
						if (animator != null)
						{
							var avatar = AvatarBuilder.BuildGenericAvatar(root, "");
							avatar.name = "avatar";
							animator.avatar = avatar;
						}
					}
				}
                else
                {
					isHumanoid = false;
					if (animator != null)
					{
						Avatar avatar = AvatarBuilder.BuildGenericAvatar(root, "");
						avatar.name = "avatar";
						animator.avatar = avatar;
					}
#if UNITY_EDITOR
//                    Debug.Log($"Avatar.isHumanoid : {avatar.isHuman}");
#endif
                }
				if (option.createVoxelInfo)
				{
					var vi = root.AddComponent<VoxelController>();
					vi.author = voxelBlock.voxelModelFileParameter.author;
					vi.title = voxelBlock.voxelModelFileParameter.title;
					vi.volumes = volumeInfo != null && volumeInfo.Values.Count != 0 ? volumeInfo.Values.ToArray() : new VolumeInfo[0];
					vi.isHumanoid = isHumanoid;
					vi.functionBlocks = list != null && list.Count > 0 ? list.ToArray() : new FunctionBlockBase[0];
					vi.totalVolume = volumeInfo.Values.Sum(x => x.volume);
					vi.centerOfGravity = volumeInfo.Values.Aggregate(new Vector3(0, 0, 0), (t, x) => t + (x.centerOfGravity + x.gameObject.transform.position) * x.volume) / vi.totalVolume;
					vi.parts = new PartsBase[vi.volumes.Length];
					var i = 0;
					foreach (var volume in vi.volumes)
					{
						var parts = PartsFactory.AddPartsComponent<PartsBase>(volume);
						vi.parts[i++] = parts;
					}
				}
				LastSetupParameters();
                // Avatarを使用したアニメーションが開始されるまで体が埋まるので最後に位置補正しておく
                armature.body[0].gameObject.transform.localPosition = -hipOffset;
                if (callback != null)
                {
                    callback();
                }
            }
        }
    }
}
