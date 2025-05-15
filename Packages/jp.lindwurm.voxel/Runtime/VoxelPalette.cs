using UnityEngine;

namespace Lindwurm.Voxel
{
    public class VoxelPalette
    {
        public const int PaletteNum = 3 * 12 + 3;
        public Color[] colorPalette = new Color[PaletteNum];
        public const int freeColorNumBegin = 256 - 32;    // 224～255までがフリーカラー
        public Color[] freeColors = new Color[32];
        public bool[] exportAsMaterial = new bool[PaletteNum]; // exportするときにマテリアルにするフラグ

        public Color[] defaultColor = new Color[PaletteNum]
        {
            new Color(1,1,1)
            ,new Color(0.666666f,0.666666f,0.6666666f)
            ,new Color(0.5f,0.5f,0.5f)
,new Color(1.0f,0.0f,0.0f)
,new Color(0.6666666666666666f,0.0f,0.0f)
,new Color(0.5f,0.0f,0.0f)
,new Color(1.0f,0.4980392156862745f,0.0f)
,new Color(0.6666666666666666f,0.3320261437908497f,0.0f)
,new Color(0.5f,0.24901960784313726f,0.0f)
,new Color(1.0f,1.0f,0.0f)
,new Color(0.6666666666666666f,0.6666666666666666f,0.0f)
,new Color(0.5f,0.5f,0.0f)
,new Color(0.5f,1.0f,0.0f)
,new Color(0.3333333333333333f,0.6666666666666666f,0.0f)
,new Color(0.25f,0.5f,0.0f)
,new Color(0.0f,1.0f,0.0f)
,new Color(0.0f,0.6666666666666666f,0.0f)
,new Color(0.0f,0.5f,0.0f)
,new Color(0.0f,1.0f,0.4980392156862745f)
,new Color(0.0f,0.6666666666666666f,0.3320261437908497f)
,new Color(0.0f,0.5f,0.24901960784313726f)
,new Color(0.0f,1.0f,1.0f)
,new Color(0.0f,0.6666666666666666f,0.6666666666666666f)
,new Color(0.0f,0.5f,0.5f)
,new Color(0.0f,0.4980392156862745f,1.0f)
,new Color(0.0f,0.3320261437908497f,0.6666666666666666f)
,new Color(0.0f,0.24901960784313726f,0.5f)
,new Color(0.0f,0.0f,1.0f)
,new Color(0.0f,0.0f,0.6666666666666666f)
,new Color(0.0f,0.0f,0.5f)
,new Color(0.4980392156862745f,0.0f,1.0f)
,new Color(0.3320261437908497f,0.0f,0.6666666666666666f)
,new Color(0.24901960784313726f,0.0f,0.5f)
,new Color(1.0f,0.0f,1.0f)
,new Color(0.6666666666666666f,0.0f,0.6666666666666666f)
,new Color(0.5f,0.0f,0.5f)
,new Color(1.0f,0.0f,0.4980392156862745f)
,new Color(0.6666666666666666f,0.0f,0.3320261437908497f)
,new Color(0.5f,0.0f,0.24901960784313726f)        };

        public VoxelPalette()
        {
            for (var i = 0; i < PaletteNum; i++)
            {
                colorPalette[i] = defaultColor[i];
                exportAsMaterial[i] = true;
            }
            exportAsMaterial[0] = true;
            for (var i = 0; i < 32; i++)
            {
                freeColors[i] = Color.black;
            }
        }

        public Color GetColor(int index)
        {
            if (index < colorPalette.Length)
            {
                return colorPalette[index];
            }
            if (index >= freeColorNumBegin)
            {
                return freeColors[index - freeColorNumBegin];
            }
            return Color.white;
        }
        public void SetColor(int index, Color color)
        {
            if (index < colorPalette.Length)
            {
                colorPalette[index] = color;
            }
            if (index >= freeColorNumBegin)
            {
                freeColors[index - freeColorNumBegin] = color;
            }
        }
        public int SearchColorInFreeColorPalette(Color col)
        {
            var rt = freeColors.Length-1;
            for (var i = 0; i < freeColors.Length; i++)
            {
                if (freeColors[i] == col)
                {
                    return i + freeColorNumBegin;
                }
            }
            freeColors[rt] = col;
            return rt;
        }
    }
}
