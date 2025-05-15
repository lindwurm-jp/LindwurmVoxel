using UnityEngine;


namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
        [System.Serializable]
        public class Pose
        {
            public string name;
            public Vector3 position;
            public Vector3 rotation;
            public float scale;
            public bool touched;
        }
        [System.Serializable]
        public class Joint
        {
            public int id;  // jointId
            public string name; // name
            public Vector3 position; // position
            public int parentId;  // parentId
            public int enableEdit;  // enable edit
            public int lockJoint;  // lock Joint
            public int meshGroup;   // -1は単独,0～グループ番号
            public VoxelBlock.ConstraintDat constraint; // constraint type , constrent source id
            public Pose[] poses;
        }
        [System.Serializable]
        public class Block
        {
            public int id;
            public byte[] data;
        }
        [System.Serializable]
        public class Palette
        {
            public int color;
        }
        [System.Serializable]
        public class VoxelSaveFormat
        {
            public Joint[] joints;
            public Block[] blocks;
            public Block[] quaterBlocks;
			public Block[] faceColors;
			public Block[] faceColorsQuater;
            public Palette[] palettes;
        }
        [System.Serializable]
        public class VoxelSaveInfoFormatSerialized
        {
            public string title;
            public string author;
            public string type;
            public float scale;
			public uint n;
			public string cc;
            public float[] bound;   // Tポーズ時の外接矩形サイズ VoxelFormatVersion = "000004"から
            public byte[] thumbnail;
        }
        public const string TypeHumanoid = "Humanoid";
        public const string TypeHandheld = "Handheld";

        public class VoxelSaveInfoFormat : System.IDisposable
        {
            private static Texture2D dummyThumbnail = null;
            public string title;
            public string author;
            public string type;
            public float scale;
			public uint n;
			public string cc;
            public float[] bound;   // Tポーズ時の外接矩形サイズ VoxelFormatVersion = "000004"から
            private Texture2D _thumbnail;
            public Texture2D thumbnail {
                get {
                    if(_thumbnail != null) return _thumbnail;
                    if (thumbnailCache != null && thumbnailCache.Length > 24)
                    {
                        var x = (thumbnailCache[16] << 24) + (thumbnailCache[17] << 16) + (thumbnailCache[18] << 8) + thumbnailCache[19];
                        var y = (thumbnailCache[20] << 24) + (thumbnailCache[21] << 16) + (thumbnailCache[22] << 8) + thumbnailCache[23];
                        _thumbnail = new Texture2D(x, y);
                        _thumbnail.LoadImage(thumbnailCache);
                        thumbnailCache = null;
                        return _thumbnail;
                    }
                    else
                    {
                        if(dummyThumbnail != null)
                        {
                            return dummyThumbnail;
                        }
                        dummyThumbnail = new Texture2D(128, 128);
                        var cols = new Color[128 * 128];
                        var c = Color.gray;
                        for (var i = 0; i < cols.Length; i++)
                        {
                            cols[i] = c;
                        }
                        dummyThumbnail.SetPixels(0, 0, 128, 128, cols);
                    }
                    return dummyThumbnail;
                }
                set { _thumbnail = value; }
            }
            private byte[] thumbnailCache = null;
			private bool disposedValue;

            public VoxelSaveInfoFormat(string title, string author, string type,float scale, Texture2D thumbnail, uint n=0, string cc="")
            {
                this.title = title;
                this.author = author;
                this.type = type;
                this.scale = scale;
				this.n = n;
				this.cc = cc;
                this.bound = new float[3];
                this.thumbnail = thumbnail;
            }
            public VoxelSaveInfoFormat(VoxelSaveInfoFormatSerialized s)
            {
                title = s.title;
                author = s.author;
                type = s.type;
                scale = s.scale;
				n = s.n;
				cc = s.cc;
                bound = new float[3];
                if (s.bound != null && s.bound.Length == 3)
                {
                    bound[0] = s.bound[0];
                    bound[1] = s.bound[1];
                    bound[2] = s.bound[2];
                }else
                {
                    bound[0] = 0;
                    bound[1] = 0;
                    bound[2] = 0;
                }

                thumbnail = null;
                if (s.thumbnail.Length > 24)
                {
                    thumbnailCache = s.thumbnail;
                }else
                {
                    thumbnailCache = null;
                }
            }

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						if (_thumbnail)
							Object.Destroy(_thumbnail);
						_thumbnail = null;
					}
					disposedValue = true;
				}
			}

			public void Dispose()
			{
				Dispose(disposing: true);
				System.GC.SuppressFinalize(this);
			}
		}
		// 000005からFNBK追加
		// 000006-9なし
		// 000010からボクセルの面に色を塗る機能(faceColors, faceColorsQuater)追加)
        public static string VoxelFormatVersion = "000010";
        public static string VoxelDataHeader = "VoxelData";
        public static string VoxelDataHeaderLine = VoxelDataHeader + "." + VoxelFormatVersion + ".\x0\x0\x0";
    }
}
