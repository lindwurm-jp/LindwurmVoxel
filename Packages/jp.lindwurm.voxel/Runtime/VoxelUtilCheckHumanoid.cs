using System.Collections.Generic;

namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
        public static readonly Dictionary<string, string> HUMAN_BONE_NAME_AND_PARENT = new Dictionary<string, string>()
        {
            { "Hips", "" },
            { "Spine", "Hips" },
            { "Chest", "Spine" },
            { "UpperChest", "Chest" },
            { "Neck", "UpperChest" },
            { "Head", "Neck" },
            { "LeftShoulder", "UpperChest" },
            { "LeftUpperArm", "LeftShoulder" },
            { "LeftLowerArm", "LeftUpperArm" },
            { "LeftHand", "LeftLowerArm" },
            { "RightShoulder", "UpperChest" },
            { "RightUpperArm", "RightShoulder" },
            { "RightLowerArm", "RightUpperArm" },
            { "RightHand", "RightLowerArm" },
            { "LeftUpperLeg", "Hips" },
            { "LeftLowerLeg", "LeftUpperLeg" },
            { "LeftFoot", "LeftLowerLeg" },
            { "RightUpperLeg", "Hips" },
            { "RightLowerLeg", "RightUpperLeg" },
            { "RightFoot", "RightLowerLeg" },
            { "LeftHandheld","LeftHand" },
            { "RightHandheld","RightHand" }
        };

        public static bool CheckHumanoid(VoxelBlock voxelBlock, bool checkHandHeld)
        {
            Dictionary<string, int> resultDic = new Dictionary<string, int>();
            var joints = voxelBlock.EnumGetJoint();
            while(joints.MoveNext())
            {
                //if(!voxelBlock.isEnableEdit(joints.Current.id))
                //{
                //    continue;
                //}
                var n = joints.Current.name;
                if(!string.IsNullOrEmpty(n) && HUMAN_BONE_NAME_AND_PARENT.ContainsKey(n))
                {
                    var pr = HUMAN_BONE_NAME_AND_PARENT[n];
                    var p = joints.Current.parentId;
                    if (string.IsNullOrEmpty(pr))
                    {
                        // Hipsのとき、親にHumanoid BoneがあったらNG
                        //Debug.Log($"Dic {n}");
                        resultDic[n] = 1;
                        while (p >= 0)
                        {
                            var joint = voxelBlock.GetJoint(p);
                            if (HUMAN_BONE_NAME_AND_PARENT.ContainsKey(joint.name))
                            {
                                return false;
                            }
                            p = joint.parentId;
                        }
                    }
                    else
                    {
                        // 親に正しいHumanoid Boneがあることを確認
                        while (p >= 0)
                        {
                            var joint = voxelBlock.GetJoint(p);
                            if (joint.name == pr)
                            {
                                //Debug.Log($"Dic.{n}");
                                resultDic[n] = 1;
                                break;
                            }
                            if (HUMAN_BONE_NAME_AND_PARENT.ContainsKey(joint.name))
                            {
                                return false;
                            }
                            p = joint.parentId;
                        }
                    }
                }
            }
            if(checkHandHeld && resultDic.Count != HUMAN_BONE_NAME_AND_PARENT.Count)
            {
                return false;
            }
            var c = 0;
            c += !resultDic.ContainsKey("LeftHandheld") ? 1 : 0;
            c += !resultDic.ContainsKey("RightHandheld") ? 1 : 0;
            if(resultDic.Count + c != HUMAN_BONE_NAME_AND_PARENT.Count)
            {
                return false;
            }

            return true;
        }
    }

}