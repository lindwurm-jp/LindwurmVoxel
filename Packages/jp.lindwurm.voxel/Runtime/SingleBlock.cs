using UnityEngine;

namespace Lindwurm.Voxel
{
    public class SingleBlockData
    {
        public byte paletteNo;  // パレット番号       224～リザーブ、255 : パレットなし
        public Color color;    // 色(パレットがない時)

        public SingleBlockData Clone()
        {
            var rt = new SingleBlockData(paletteNo);
            rt.color = color;
            return rt;
        }
        public SingleBlockData(byte paletteNo)
        {
            this.paletteNo = paletteNo;
            this.color = Color.white;
        }
        public SingleBlockData(Color color)
        {
            this.paletteNo = 255;   // パレット番号とは関係なしに255
            this.color = color;
        }
        public bool isUsePalette()
        {
            return paletteNo != 255;
        }

        public override bool Equals(object obj)
        {
            return obj is SingleBlockData data &&
                   paletteNo == data.paletteNo &&
                   color.Equals(data.color);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(paletteNo, color);
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }
        public void SetPalette(byte palette)
        {
            this.paletteNo = palette;
        }

        public Color GetColor(VoxelPalette pal)
        {
            if (paletteNo == 255)
            {
                return color;
            }
            return pal.GetColor(paletteNo);
        }
    }
}
