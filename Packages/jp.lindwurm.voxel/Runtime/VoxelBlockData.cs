using System.Collections.Generic;
using UnityEngine;

namespace Lindwurm.Voxel
{
    public class VoxelBlockData
    {
        public Dictionary<Vector3Int, SingleBlockData> blocks;
        public Dictionary<Vector3Int, SingleBlockData> quaterBlocks;
		public Dictionary<Vector3Int, SingleBlockData> faceColors;
		public Dictionary<Vector3Int, SingleBlockData> faceColorsQuater;
        static public Vector3Int vector3IntZZZ = new Vector3Int( 0, 0, 0);
        static public Vector3Int vector3IntZZM = new Vector3Int( 0, 0,-1);
        static public Vector3Int vector3IntZZP = new Vector3Int( 0, 0, 1);
        static public Vector3Int vector3IntZMZ = new Vector3Int( 0,-1, 0);
        static public Vector3Int vector3IntZMM = new Vector3Int( 0,-1,-1);
        static public Vector3Int vector3IntZMP = new Vector3Int( 0,-1, 1);
        static public Vector3Int vector3IntZPZ = new Vector3Int( 0, 1, 0);
        static public Vector3Int vector3IntZPM = new Vector3Int( 0, 1,-1);
        static public Vector3Int vector3IntZPP = new Vector3Int( 0, 1, 1);
        static public Vector3Int vector3IntMZZ = new Vector3Int(-1, 0, 0);
        static public Vector3Int vector3IntMZM = new Vector3Int(-1, 0,-1);
        static public Vector3Int vector3IntMZP = new Vector3Int(-1, 0, 1);
        static public Vector3Int vector3IntMMZ = new Vector3Int(-1,-1, 0);
        static public Vector3Int vector3IntMMM = new Vector3Int(-1,-1,-1);
        static public Vector3Int vector3IntMMP = new Vector3Int(-1,-1, 1);
        static public Vector3Int vector3IntMPZ = new Vector3Int(-1, 1, 0);
        static public Vector3Int vector3IntMPM = new Vector3Int(-1, 1,-1);
        static public Vector3Int vector3IntMPP = new Vector3Int(-1, 1, 1);
        static public Vector3Int vector3IntPZZ = new Vector3Int( 1, 0, 0);
        static public Vector3Int vector3IntPZM = new Vector3Int( 1, 0,-1);
        static public Vector3Int vector3IntPZP = new Vector3Int( 1, 0, 1);
        static public Vector3Int vector3IntPMZ = new Vector3Int( 1,-1, 0);
        static public Vector3Int vector3IntPMM = new Vector3Int( 1,-1,-1);
        static public Vector3Int vector3IntPMP = new Vector3Int( 1,-1, 1);
        static public Vector3Int vector3IntPPZ = new Vector3Int( 1, 1, 0);
        static public Vector3Int vector3IntPPM = new Vector3Int( 1, 1,-1);
        static public Vector3Int vector3IntPPP = new Vector3Int( 1, 1, 1);



        public VoxelBlockData()
        {
            blocks = new Dictionary<Vector3Int, SingleBlockData>();
            quaterBlocks = new Dictionary<Vector3Int, SingleBlockData>();
			faceColors = new Dictionary<Vector3Int, SingleBlockData>();
			faceColorsQuater = new Dictionary<Vector3Int, SingleBlockData>();
        }


        public void Clear()
        {
            blocks.Clear();
            quaterBlocks.Clear();
			faceColors.Clear();
			faceColorsQuater.Clear();
        }
        public void Add(Vector3Int pos, SingleBlockData dat)
        {
            blocks[pos] = dat;
        }
        public int blockCount { get { return blocks.Count; } }
        public int quaterBlockCount { get { return quaterBlocks.Count; } }

        /// <summary>
        /// ブロックの位置に普通ブロックかquaterブロックがいるかどうか確認する
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int ContainsBlock(Vector3Int pos)
        {
            if (blocks.ContainsKey(pos))
            {
                return 1;
            }
            var p = pos * 2;
            if(quaterBlocks.ContainsKey(p)
                | quaterBlocks.ContainsKey(p + vector3IntZZP)
                | quaterBlocks.ContainsKey(p + vector3IntZPZ)
                | quaterBlocks.ContainsKey(p + vector3IntZPP)
                | quaterBlocks.ContainsKey(p + vector3IntPZZ)
                | quaterBlocks.ContainsKey(p + vector3IntPZP)
                | quaterBlocks.ContainsKey(p + vector3IntPPZ)
                | quaterBlocks.ContainsKey(p + vector3IntPPP))
            {
                return 2;
            }
            return 0;
        }
        public SingleBlockData GetBlock(Vector3Int pos)
        {
            if (blocks.ContainsKey(pos))
            {
                return blocks[pos];
            }
            return null;
        }

        public bool ExistsFace(Vector3Int orgPos, Vector3Int byPos) // 横に面があればtrue, posは普通pos
        {
            
            if(blocks.ContainsKey(byPos))
            {
                return true;
            }
            var d = byPos - orgPos;
            byPos *= 2;
            if(d.x == 1)
            {
                if(quaterBlocks.ContainsKey(byPos)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZPZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZZP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZPP))
                {
                    return true;
                }
            }else if(d.x == -1)
            {
                if (quaterBlocks.ContainsKey(byPos + vector3IntPZZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPZP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPP))
                {
                    return true;
                }
            }
            else if(d.y == 1)
            {
                if (quaterBlocks.ContainsKey(byPos)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZZP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPZZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPZP))
                {
                    return true;
                }
            }
            else if(d.y == -1)
            {
                if (quaterBlocks.ContainsKey(byPos + vector3IntZPZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZPP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPP))
                {
                    return true;
                }
            }
            else if(d.z == 1)
            {
                if (quaterBlocks.ContainsKey(byPos)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPZZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZPZ)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPZ))
                {
                    return true;
                }
            }
            else if(d.z == -1)
            {
                if (quaterBlocks.ContainsKey(byPos + vector3IntZZP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPZP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntZPP)
                    && quaterBlocks.ContainsKey(byPos + vector3IntPPP))
                {
                    return true;
                }

            }
            return false;
        }
        public bool ExistsFaceQuater(Vector3Int byPos) // 横に面があればtrue,posは２倍pos
        {
            if(blocks.ContainsKey(ToNormalPos(byPos)))
            {
                // となりにブロックがある
                return true;
            }
            return quaterBlocks.ContainsKey(byPos);
        }
        private Vector3Int ToNormalPos(Vector3Int quaterPos)
        {
            var rt = quaterPos;
            if (rt.x < 0)
            {
                rt.x = (rt.x - 1) / 2;
            }
            else
            {
                rt.x /= 2;
            }
            if (rt.y < 0)
            {
                rt.y = (rt.y - 1) / 2;
            }
            else
            {
                rt.y /= 2;
            }
            if (rt.z < 0)
            {
                rt.z = (rt.z - 1) / 2;
            }
            else
            {
                rt.z /= 2;
            }
            return rt;
        }


        public void ToQuaterBlock(Vector3Int pos)    // posは普通pos
        {
            var bd = blocks[pos];
            blocks.Remove(pos);
			SingleBlockData[] faces = new SingleBlockData[6];
			var p = pos;
			for(var i = 0; i < 6; i++)
			{
				p.x = pos.x * 6 + i;
				if(faceColors.ContainsKey(p))
				{
					faces[i] = faceColors[p];
					faceColors.Remove(p);
				}else
				{
					faces[i] = null;
				}
			}
            p = pos * 2;
            quaterBlocks[p] = bd;
            quaterBlocks[p + vector3IntZZP] = bd.Clone();
            quaterBlocks[p + vector3IntZPZ] = bd.Clone();
            quaterBlocks[p + vector3IntZPP] = bd.Clone();
            quaterBlocks[p + vector3IntPZZ] = bd.Clone();
            quaterBlocks[p + vector3IntPZP] = bd.Clone();
            quaterBlocks[p + vector3IntPPZ] = bd.Clone();
            quaterBlocks[p + vector3IntPPP] = bd.Clone();
			Vector3Int fp;
			if (faces[0] != null)
			{
				// -x に面しているブロックZXX(右)
				fp = p;
				fp.x = fp.x * 6;
				faceColorsQuater[fp] = faces[0].Clone();
				fp = p + vector3IntZZP;
				fp.x = fp.x * 6;
				faceColorsQuater[fp] = faces[0].Clone();
				fp = p + vector3IntZPZ;
				fp.x = fp.x * 6;
				faceColorsQuater[fp] = faces[0].Clone();
				fp = p + vector3IntZPP;
				fp.x = fp.x * 6;
				faceColorsQuater[fp] = faces[0].Clone();
			}
			if (faces[1] != null)
			{
				// +x に面しているブロックPXX(左)
				fp = p + vector3IntPZZ;
				fp.x = fp.x * 6 + 1;
				faceColorsQuater[fp] = faces[1].Clone();
				fp = p + vector3IntPZP;
				fp.x = fp.x * 6 + 1;
				faceColorsQuater[fp] = faces[1].Clone();
				fp = p + vector3IntPPZ;
				fp.x = fp.x * 6 + 1;
				faceColorsQuater[fp] = faces[1].Clone();
				fp = p + vector3IntPPP;
				fp.x = fp.x * 6 + 1;
				faceColorsQuater[fp] = faces[1].Clone();
			}
			if (faces[2] != null)
			{
				// -yに面しているブロックXZX(下)
				fp = p;
				fp.x = fp.x * 6 + 2;
				faceColorsQuater[fp] = faces[2].Clone();
				fp = p + vector3IntZZP;
				fp.x = fp.x * 6 + 2;
				faceColorsQuater[fp] = faces[2].Clone();
				fp = p + vector3IntPZZ;
				fp.x = fp.x * 6 + 2;
				faceColorsQuater[fp] = faces[2].Clone();
				fp = p + vector3IntPZP;
				fp.x = fp.x * 6 + 2;
				faceColorsQuater[fp] = faces[2].Clone();
			}
			if (faces[3] != null)
			{
				// +yに面しているブロックXPX(上)
				fp = p + vector3IntZPZ;
				fp.x = fp.x * 6 + 3;
				faceColorsQuater[fp] = faces[3].Clone();
				fp = p + vector3IntZPP;
				fp.x = fp.x * 6 + 3;
				faceColorsQuater[fp] = faces[3].Clone();
				fp = p + vector3IntPPZ;
				fp.x = fp.x * 6 + 3;
				faceColorsQuater[fp] = faces[3].Clone();
				fp = p + vector3IntPPP;
				fp.x = fp.x * 6 + 3;
				faceColorsQuater[fp] = faces[3].Clone();
			}
			if (faces[4] != null)
			{
				// -zに面しているブロックXXZ(裏)
				fp = p;
				fp.x = fp.x * 6 + 4;
				faceColorsQuater[fp] = faces[4].Clone();
				fp = p + vector3IntZPZ;
				fp.x = fp.x * 6 + 4;
				faceColorsQuater[fp] = faces[4].Clone();
				fp = p + vector3IntPZZ;
				fp.x = fp.x * 6 + 4;
				faceColorsQuater[fp] = faces[4].Clone();
				fp = p + vector3IntPPZ;
				fp.x = fp.x * 6 + 4;
				faceColorsQuater[fp] = faces[4].Clone();
			}
			if (faces[5] != null)
			{
				// +zに面しているブロックXXP//前
				fp = p + vector3IntZZP;
				fp.x = fp.x * 6 + 5;
				faceColorsQuater[fp] = faces[5].Clone();
				fp = p + vector3IntZPP;
				fp.x = fp.x * 6 + 5;
				faceColorsQuater[fp] = faces[5].Clone();
				fp = p + vector3IntPZP;
				fp.x = fp.x * 6 + 5;
				faceColorsQuater[fp] = faces[5].Clone();
				fp = p + vector3IntPPP;
				fp.x = fp.x * 6 + 5;
				faceColorsQuater[fp] = faces[5].Clone();
			}
        }
		public void ToNormalBlock(Vector3Int pos, SingleBlockData baseCol, SingleBlockData[] faceCol)    // posは普通pos
        {
            blocks[pos] = baseCol;
			var p = pos;
			p.x = p.x * 6;
			for(var i = 0; i < 6; i++)
			{
				if (faceCol[i] != baseCol)
				{
					faceColors[p] = faceCol[i];
				}
				p.x++;
			}

			p = pos * 2;
			RemoveBlock(p, vector3IntZZZ);
			RemoveBlock(p, vector3IntZZP);
			RemoveBlock(p, vector3IntZPZ);
			RemoveBlock(p, vector3IntZPP);
			RemoveBlock(p, vector3IntPZZ);
			RemoveBlock(p, vector3IntPZP);
			RemoveBlock(p, vector3IntPPZ);
			RemoveBlock(p, vector3IntPPP);

			void RemoveBlock(Vector3Int rp, Vector3Int offs)
			{
				rp = rp + offs;
				if(quaterBlocks.ContainsKey(rp))
				{
					quaterBlocks.Remove(rp);
					rp.x *= 6;
					for(var i = 0; i < 6; i++)
					{
						if(faceColorsQuater.ContainsKey(rp))
						{
							faceColorsQuater.Remove(rp);
						}
						rp.x++;
					}
				}
			}

        }

    }
}
