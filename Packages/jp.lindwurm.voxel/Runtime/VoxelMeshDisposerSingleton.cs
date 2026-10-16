using UnityEngine;

namespace Lindwurm.Voxel
{
    public class VoxelMeshDisposerSingleton : MonoBehaviour
    {
        private static VoxelMeshDisposerSingleton _instance = null;
        private int watchListLengthAdd = 100;
        private int entryHead = 0;
        private int point = 0;
        private int entryCount = 0;
        struct ObjEntry
        {
            public VoxelMeshDisposer item;
            public Mesh mesh;
            public int NextFree;
            public bool Used;
        }
        private ObjEntry[] entries = new ObjEntry[100];
        public static VoxelMeshDisposerSingleton instance { get {
            if(_instance == null)
            {
                _instance = new GameObject("VoxelModelDisposerSingleton").AddComponent<VoxelMeshDisposerSingleton>();
                _instance.Init();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }}
        private void Init()
        {
            for (int i = 0; i < entries.Length - 1; i++)
            {
                entries[i].NextFree = i + 1;
            }
            entries[^1].NextFree = -1;
            entryHead = 0;
        }
        public void AddWatchItem(VoxelMeshDisposer v, Mesh mesh)
        {
            entryCount++;
            enabled = true;
            int id = entryHead;
            entryHead = entries[id].NextFree;
            entries[id].Used = true;
            entries[id].item = v;
            entries[id].mesh = mesh;
            if (entryHead == -1)
            {// 端まで行ってしまったら拡張
                var newEntries = new ObjEntry[entries.Length + watchListLengthAdd];
                for(var i = 0; i < entries.Length; i++)
                {
                    newEntries[i] = entries[i];
                }
                newEntries[id].NextFree = entries.Length;
                for(var i = entries.Length; i < newEntries.Length; i++)
                {
                    entries[i].NextFree = i + 1;
                }
                newEntries[^1].NextFree = -1;
                entries = newEntries;
            }
        }
        private void RemoveWatchItem(int id)
        {
            entries[id].item = null;
            entries[id].mesh = null;
            entries[id].Used = false;
            entries[id].NextFree = entryHead;
            entryHead = id;
            entryCount--;
            if (entryCount == 0)
            {
                enabled = false;
            }
        }
        private void Update()
        {
            if (entries[point].Used)
            {
                if (entries[point].item == null)
                {
                    if (entries[point].mesh != null)
                    {
                        Object.Destroy(entries[point].mesh);
                        RemoveWatchItem(point);
                    }
                }
                else
                {
                    // gameobjectがenableになったら自動削除されるのでリストから削除
                    if (entries[point].item.awaken)
                    {
                        RemoveWatchItem(point);
                    }
                }
            }
            point++;
            if(point >= entries.Length)
            {
                point = 0;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                if (!entries[i].Used)
                    continue;

                if (entries[i].item == null)
                {
                    if(entries[i].mesh != null)
                    {
                        Object.Destroy(entries[i].mesh);
                        RemoveWatchItem(i);
                    }
                }
            }
        }

    }
}
