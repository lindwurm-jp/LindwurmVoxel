using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Lindwurm.Voxel
{
    public class VoxelJoint
    {
        public int id;  // jointId
        public int parentId;
        public Vector3 position;
        public string name;

        public Dictionary<string, (Vector3 position,Vector3 rotation, float scale, bool touched)> poseLocalDic = new Dictionary<string, (Vector3 position, Vector3 rotation, float scale, bool touched)>();
        public string nowPose;

        public GameObject viewObject;   // 表示用GameObject
        public Vector3 localPosition;

        public float degree;

        public VoxelJoint(int newid, int jointParentId, Vector3 pos)
        {
            id = newid;
            parentId = jointParentId;// p != null ? p.id : -1;
            position = pos;
        }

        public (Vector3 position, Vector3 rotation, float scale, bool touched) GetPoseLocal(string poseName)
        {
            (Vector3 position, Vector3 rotation, float scale, bool touched) rt = (Vector3.zero, Vector3.zero, 1f, false);
            if(!string.IsNullOrEmpty(poseName))
            {
                if (poseLocalDic.ContainsKey(poseName))
                {
                    rt = poseLocalDic[poseName];
                }
            }
            return rt;
        }
        public void SetPoseLocal(string poseName, (Vector3 position, Vector3 rotation, float scale, bool touched) dat)
        {
            if(!string.IsNullOrEmpty(poseName))
            {
                poseLocalDic[poseName] = (dat.position, dat.rotation, dat.scale, dat.touched);
            }else
            {
                Debug.Log("Name is none");
            }

        }
        public void ChangeParentObject(VoxelJoint parentJoint)//GameObject parentObj)
        {
            if (viewObject != null)
            viewObject.transform.SetParent(parentJoint.viewObject.transform);
            localPosition = position - parentJoint.position;
        }
        public void Pose(bool tf, string name = null)
        {
            if(viewObject != null)
            {
                if(tf)
                {
                    var dat = GetPoseLocal(name);
                    nowPose = name;
                    viewObject.transform.localPosition = localPosition + dat.position;
                    viewObject.transform.localRotation = Quaternion.Euler(dat.rotation);
                    viewObject.transform.localScale = Vector3.one * dat.scale;
                }else
                {
                    nowPose = string.Empty;
                    viewObject.transform.localPosition = localPosition;
                    viewObject.transform.localRotation = Quaternion.identity;
                    viewObject.transform.localScale = Vector3.one;
                }
            }
        }
        public void Pose(string name)
        {
            if (viewObject != null)
            {
                var dat = GetPoseLocal(name);
                nowPose = name;
                viewObject.transform.localPosition = localPosition + dat.position;
                viewObject.transform.localRotation = Quaternion.Euler(dat.rotation);
                viewObject.transform.localScale = Vector3.one * dat.scale;
            }
        }

        public void SetConstraint(byte type, Transform target)
        {
            if (viewObject == null)
                return;
            if (target == null)
            {
                var com = viewObject.GetComponent<RotationConstraint>();
                if (com != null)
                    GameObject.Destroy(com);
            }
            else
            {
                if (type>=1 && type<=4)
                {
                    //var com = viewObject.GetOrAddComponent<RotationConstraint>();
                    var com = viewObject.GetComponent<RotationConstraint>();
                    if(com == null)
                    {
                        com = viewObject.AddComponent<RotationConstraint>();
                    }
                    if (com != null)
                    {
                        var s = new ConstraintSource();
                        s.weight = 1f;
                        s.sourceTransform = target;
                        com.AddSource(s);
                        com.constraintActive = true;
                        switch (type)
                        {
                            case 2:
                                com.rotationAxis = Axis.X;
                                break;
                            case 3:
                                com.rotationAxis = Axis.Y;
                                break;
                            case 4:
                                com.rotationAxis = Axis.Z;
                                break;
                        }
                    }
                }
            }
        }
    }

}