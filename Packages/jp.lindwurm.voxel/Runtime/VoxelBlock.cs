using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lindwurm.Voxel
{

    public class VoxelBlock
    {
        public string Name { get; set; }
        private Dictionary<int, VoxelBlockData> blocks = new Dictionary<int, VoxelBlockData>();
        public const int ROOT_JOINT_ID = 0;
        public const int ANCHOR_JOINT_ID = int.MaxValue;
        public const int MAX_JOINT = 100;
        private List<VoxelJoint> voxelJoints = new List<VoxelJoint>();
        private VoxelPalette palette = new VoxelPalette();
        [System.Serializable]
        public struct ConstraintDat
        {
            public byte type;
            public int srcId;

            public ConstraintDat(byte type, int srcId)
            {
                this.type = type;
                this.srcId = srcId;
            }
        }
        public class EditParameter
        {
            public Dictionary<int, bool> enableEdit = new Dictionary<int, bool>(); // 表示,非表示
            public Dictionary<int, bool> lockJoint = new Dictionary<int, bool>(); // ジョイント削除などの禁止
            public Dictionary<int, int> meshGroup = new Dictionary<int, int>(); // エクスポート時のメッシュグループ-1だと単独
            public Dictionary<int, ConstraintDat> constraint = new Dictionary<int, ConstraintDat>(); // コンストレイントジョイント

            public void Clear()
            {
                enableEdit.Clear();
                lockJoint.Clear();
                meshGroup.Clear();
                constraint.Clear();
            }
        }
        private EditParameter editParam = new EditParameter();

        private Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();
        private Dictionary<int, Mesh> meshesJoint = new Dictionary<int, Mesh>();
        private Dictionary<int, Mesh> meshesWireframe = new Dictionary<int, Mesh>();

        // ボクセルエディタでは使用せず(Config Menuの方を使う。それがファイルに保存され、ロードされたときにこっちに入る)、VoxelUtil.CreateVoxelDirectでインスタンスを作る際に使用する
        public VoxelUtil.VoxelSaveInfoFormat voxelModelFileParameter = null;
		// ロード、セーブの時に使うシリアライズしたファンクションブロック情報を一時的に置いておくところ
		public List<VoxelUtil.VoxelFuncBlockInfo> voxelFuncBlockInfoParameter = new List<VoxelUtil.VoxelFuncBlockInfo>();

        public int JointCount
        {
            get
            {
                return voxelJoints.Count;
            }
        }
        public int BlockCount
        {
            get
            {
                return voxelJoints.Count;
            }
        }
        public static bool canChangeParent(int id)
        {
            return id != ROOT_JOINT_ID && id != ANCHOR_JOINT_ID;
        }
        public static bool canConstraint(int id)
        {
            return id != ROOT_JOINT_ID && id != ANCHOR_JOINT_ID;
        }
        public bool isEnableEdit(int id)
        {
            if (editParam.enableEdit.ContainsKey(id))
            {
                return editParam.enableEdit[id];
            }
            return false;
        }
        public void EnableEdit(int id, bool tf)
        {
            editParam.enableEdit[id] = tf;
        }
        public bool isLocked(int id)
        {
            if(id == ROOT_JOINT_ID || id == ANCHOR_JOINT_ID)
            {
                return true;    // ルートジョイントは必ずロック
            }
            if (editParam.lockJoint.ContainsKey(id))
            {
                return editParam.lockJoint[id];
            }
            return false;
        }
        public void LockJoint(int id, bool tf)
        {
            if(id == ROOT_JOINT_ID || id == ANCHOR_JOINT_ID)
            {
                tf = true;
            }
            editParam.lockJoint[id] = tf;
        }
        public ConstraintDat GetConstraint(int id)
        {
            if (editParam.constraint.ContainsKey(id))
            {
                return editParam.constraint[id];
            }
            return new ConstraintDat(0,-1);
        }
        public bool SetConstraint(int id , byte type, int targetId)
        {
            bool constraint = (type>0 && targetId>=0);
            var j = GetJoint(id);
            if (j != null)
            {
                if (editParam.constraint.ContainsKey(id))
                {
                    if (editParam.constraint[id].srcId >= 0)
                    {
                        j.SetConstraint(type, null); // remove
                    }
                    if (!constraint)
                        editParam.constraint.Remove(id);
                }
                VoxelJoint t = null;
                if (constraint)
                {
                    editParam.constraint[id] = new ConstraintDat(type, targetId);
                    t = GetJoint(targetId);
                }
                j.SetConstraint(type, (t!=null && t.viewObject!=null)? t.viewObject.transform : null);
                return t!=null;
            }
            return false;
        }
        /// <summary>
        /// Info解釈時点ではConstraintのGameObjectがないので、CreateVoxelModelの最後に呼ばれる。
        /// </summary>
        public void SetConstraintAll()
        {
            foreach(var j in voxelJoints)
            {
                var id = j.id;
                if(editParam.constraint.ContainsKey(id))
                {
                    var t = GetJoint(editParam.constraint[id].srcId);
                    j.SetConstraint(editParam.constraint[id].type, (t != null && t.viewObject != null) ? t.viewObject.transform : null);
                }
            }
        }
		public void SetFunctionBlockAll(float scale, List<FunctionBlockBase> list = null)
		{
			if (voxelFuncBlockInfoParameter.Count > 0)
			{
				foreach (var one in voxelFuncBlockInfoParameter)
				{
					var joint = GetJoint(one.id);
					if (joint != null)
					{
						var p = CalcJointScale(one.id, joint.nowPose);
						FunctionBlockFactory.Create(joint.viewObject, one.pr, p * scale, list);
					}
				}
				//一度しか呼ばれないが、初期化情報がなくなるので念のためクリアするのを止める
				//voxelFuncBlockInfoParameter.Clear(); 
			}
			else
			{
				// 旧版（ジョイント名で実装）
				var root = GetJoint(0);
				if (root != null)
				{
					FunctionBlockFactory.CreateOld(root.viewObject, scale, list);
				}
			}
		}
		public int GetMeshGroup(int id)
        {           
            if (editParam.meshGroup.ContainsKey(id))
            {
                return editParam.meshGroup[id];
            }
            return 0;
        }
        public void SetMeshGroup(int id, int gpNo)
        {
            editParam.meshGroup[id] = gpNo;
        }
        public IEnumerator<VoxelJoint> EnumGetJoint()
        {
            foreach (var one in voxelJoints)
            {
                yield return one;
            }
        }
        public IEnumerator<(int id, bool enabled, VoxelBlockData voxelBlockData)> GetBlock()
        {
            foreach (var one in blocks)
            {
                yield return (one.Key, editParam.enableEdit[one.Key], one.Value);
            }
        }
        public List<(int id, bool enabled)> GetBlockIds()
        {
            return blocks.OrderBy(x => x.Key).Select(x => (x.Key, editParam.enableEdit[x.Key])).ToList();
        }
        public VoxelJoint[] GetJointsFromParentId(int parentId)
        {
            List<VoxelJoint> rt = null;

            foreach (var one in voxelJoints)
            {
                if (one.parentId == parentId)
                {
                    if (rt == null)
                    {
                        rt = new List<VoxelJoint>();
                    }
                    rt.Add(one);
                }
            }
            if (rt == null)
            {
                return null;
            }
            return rt.ToArray();
        }
        public IEnumerator<VoxelJoint> EnumGetJointsFromParentId(int parentId)
        {
            foreach (var one in voxelJoints)
            {
                if (one.parentId == parentId)
                {
                    yield return one;
                }
            }
        }


        public VoxelBlockData GetBlock(int index, bool createIfNotExist = false)
        {
            if (blocks.ContainsKey(index))
            {
                return blocks[index];
            }
            if (createIfNotExist)
            {
                blocks[index] = new VoxelBlockData();
                return blocks[index];
            }
            return null;
        }
        public VoxelPalette GetPalette()
        {
            return palette;
        }
        public Mesh GetMesh(int id)
        {
            if (meshes.ContainsKey(id))
            {
                return meshes[id];
            }
            return null;
        }
        public Mesh GetMeshJoint(int id)
        {
            if (meshesJoint.ContainsKey(id))
            {
                return meshesJoint[id];
            }
            return null;
        }
        public Mesh GetMeshWireframe(int id)
        {
            if (meshesWireframe.ContainsKey(id))
            {
                return meshesWireframe[id];
            }
            return null;
        }

        public void Initialize()
        {
            if (blocks.Count == 1 && blocks[0].blocks.ContainsKey(Vector3Int.zero) && blocks[0].blocks[Vector3Int.zero].paletteNo == 0
                && voxelJoints != null && voxelJoints.Count == 1 && voxelJoints[0].id == 0 && voxelJoints[0].parentId == -1 && voxelJoints[0].position == Vector3.zero)
            {
                return;
            }
            if (blocks.Count > 0)
            {
                foreach (var one in blocks)
                {
                    if (one.Value != null)
                    {
                        one.Value.Clear();
                    }
                }
            }
            blocks.Clear();
            editParam.Clear();
            voxelJoints.Clear();
            meshes.Clear();
            meshesJoint.Clear();
            meshesWireframe.Clear();
            //anchor
            /*
            var anchor = new VoxelBlockData();
            blocks.Add(ANCHOR_JOINT_ID, anchor);
            var ajd = new VoxelJoint(ANCHOR_JOINT_ID, -1, Vector3.zero);
            ajd.name = "Anchor";
            voxelJoints.Add(ajd);
            editParam.enableEdit.Add(ANCHOR_JOINT_ID, false);
            */
            //root
            var sd = new SingleBlockData(0);
            var root = new VoxelBlockData();// Dictionary<Vector3Int, SingleBlockData>();
            root.Add(Vector3Int.zero, sd);
            blocks.Add(ROOT_JOINT_ID, root);
            var jd = new VoxelJoint(ROOT_JOINT_ID, -1, Vector3.zero);
            jd.name = "Root";
            voxelJoints.Add(jd);
            CreateMesh(ROOT_JOINT_ID);
            editParam.enableEdit.Add(ROOT_JOINT_ID, true);
        }
        public void CreateMesh(int jointId = -1)
        {
            if (jointId == -1)
            {
                List<int> joints = new List<int>();
                foreach (var one in blocks)
                {
                    joints.Add(one.Key);
                }
                foreach (var one in joints)
                {
                    CreateMesh(one);
                }
                return;
            }

            if (!blocks.ContainsKey(jointId))
            {
                return;
            }

            var mesh = VoxelUtil.CreateMesh(GetJoint(jointId), blocks[jointId], palette);
            if (meshes.ContainsKey(jointId))
            {
                GameObject.Destroy(meshes[jointId]);
            }
            meshes[jointId] = mesh;

            // ジョイントブロック
            mesh = VoxelUtil.CreateMeshJoint(GetJoint(jointId));
            if (meshesJoint.ContainsKey(jointId))
            {
                GameObject.Destroy(meshesJoint[jointId]);
            }
            meshesJoint[jointId] = mesh;

            // ワイヤーフレーム
            mesh = VoxelUtil.CreateMeshWireframe(GetJoint(jointId), blocks[jointId], palette);
            if (meshesWireframe.ContainsKey(jointId))
            {
                GameObject.Destroy(meshesWireframe[jointId]);
            }
            meshesWireframe[jointId] = mesh;
        }

        /// <summary>
        /// セーブ時にmeshからパレット番号を取る用。meshのcolorsに対応したパレット番号の一覧が取れる
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int[]> CreateMeshPalette(bool colorOnly)
        {
            var paletteList = new Dictionary<int, int[]>();
            foreach (var one in blocks)
            {
                var id = one.Key;
                var vj = GetJoint(id);
                var blk = GetBlock(id);
                var pl = VoxelUtil.CreateMeshWithPalette(vj, blk, palette);
                if (colorOnly)
                {
                    for (var i = 0; i < pl.Length; i++)
                    {
                        pl[i] = 255;
                    }
                }
                paletteList.Add(one.Key, pl);
            }
            return paletteList;
        }


        public bool IsJoint(int index, Vector3 pos)
        {
            for (var i = 0; i < voxelJoints.Count; i++)
            {
                if (voxelJoints[i].id == index && voxelJoints[i].position == pos)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsJoint(int index)
        {
            for (var i = 0; i < voxelJoints.Count; i++)
            {
                if (voxelJoints[i].id == index)
                {
                    return true;
                }
            }
            return false;
        }
        public VoxelJoint GetJoint(int index)
        {
            for (var i = 0; i < voxelJoints.Count; i++)
            {
                if (voxelJoints[i].id == index)
                {
                    return voxelJoints[i];
                }
            }
            return null;
        }
        public Color GetJointColor(int id)
        {
            if (blocks.ContainsKey(id))
            {
                var j = GetJoint(id);
                Vector3Int p = new Vector3Int((int)j.position.x, (int)j.position.y, (int)j.position.z);
                var block = blocks[id].GetBlock(p);
                if (block != null)
                {
                    return block.GetColor(palette);
                }
            }
            return Color.black;
        }

        public int GetIdFromViewObject(GameObject go)
        {
            for (var i = 0; i < voxelJoints.Count; i++)
            {
                if (voxelJoints[i].viewObject == go)
                {
                    return voxelJoints[i].id;
                }
            }
            return -1;
        }

        public void AddJoint(VoxelJoint jointdata)
        {
            if (voxelJoints.Count >= MAX_JOINT)
            {
                return;
            }
            foreach (var one in voxelJoints)
            {
                if (one.id == jointdata.id)
                {
                    return;
                }
            }
            if (jointdata.id == -1)
            {
                for (var i = 0; i < MAX_JOINT; i++)
                {
                    var f = true;
                    for (var j = 0; j < voxelJoints.Count; j++)
                    {
                        if (voxelJoints[j].id == i)
                        {
                            f = false;
                            break;
                        }
                    }
                    if (f)
                    {
                        jointdata.id = i;
                        break;
                    }
                }
            }
            if (jointdata.id != -1)
            {
                voxelJoints.Add(jointdata);
                editParam.enableEdit.Add(jointdata.id, true);
            }
        }
        public List<int> RemoveJoint(int index)
        {
            var rt = new List<int>();
            for (var i = 0; i < voxelJoints.Count; i++)
            {
                if (voxelJoints[i].id == index)
                {
                    // 親の付け替え
                    for (var j = 0; j < voxelJoints.Count; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        if (voxelJoints[j].parentId == voxelJoints[i].id)
                        {
                            rt.Add(voxelJoints[j].id);
                            voxelJoints[j].parentId = voxelJoints[i].parentId;

                            var pj = GetJoint(voxelJoints[i].parentId);
                            if (pj != null && pj.viewObject != null)  // ルートは削除不能なのでnullにはならないはず
                            {
                                voxelJoints[j].ChangeParentObject(pj);
                            }
                        }
                    }
                    // 自身関連の情報削除
                    GameObject.Destroy(meshes[index]);
                    GameObject.Destroy(meshesJoint[index]);
                    GameObject.Destroy(meshesWireframe[index]);
                    meshes.Remove(index);
                    meshesJoint.Remove(index);
                    meshesWireframe.Remove(index);
                    blocks.Remove(index);
                    editParam.enableEdit.Remove(index);
                    voxelJoints.RemoveAt(i);
                    break;
                }
            }
            return rt;
        }

        public (Vector3 pos, Quaternion rot, float scale) CalcJointPositionRotation(int id, string poseName)
        {
            var joint = GetJoint(id);
            if (joint == null)
            {
                return (Vector3.zero, Quaternion.identity, 1f);
            }
            var poseOffset = joint.GetPoseLocal(poseName);
            var pos = joint.localPosition + poseOffset.position;
            var rot = Quaternion.Euler(poseOffset.rotation);
            float scale = poseOffset.scale;
            if (id > 0)
            {
                id = joint.parentId;
                var parent_joint = GetJoint(id);
                if (parent_joint != null)
                {
                    var (parent_pos, parent_rot, parent_scale) = CalcJointPositionRotation(id, poseName);
                    scale *= parent_scale;
                    pos = parent_rot * (pos * scale) + parent_pos;
                    rot = parent_rot * rot;
                }
            }
            return (pos, rot, scale);
        }
		public float CalcJointScale(int id, string poseName)
		{
			var joint = GetJoint(id);
			if (joint == null)
			{
				return 1f;
			}
			var poseOffset = joint.GetPoseLocal(poseName);
			float scale = poseOffset.scale;
			if (id > 0)
			{
				id = joint.parentId;
				var parent_joint = GetJoint(id);
				if (parent_joint != null)
				{
					var parent_scale = CalcJointScale(id, poseName);
					scale *= parent_scale;
				}
			}
			return scale;
		}


	}
}
