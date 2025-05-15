using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Lindwurm.Voxel
{
    public partial class VoxelUtil
    {
        const float BSH = 0.5f;
        static Vector3[] FaceYPlus = new Vector3[] { new Vector3(BSH, BSH, -BSH), new Vector3(-BSH, BSH, -BSH), new Vector3(-BSH, BSH, BSH), new Vector3(BSH, BSH, BSH) };
        static Vector3[] FaceYMinus = new Vector3[] { new Vector3(-BSH, -BSH, BSH), new Vector3(-BSH, -BSH, -BSH), new Vector3(BSH, -BSH, -BSH), new Vector3(BSH, -BSH, BSH) };
        static Vector3[] FaceXPlus = new Vector3[] { new Vector3(BSH, -BSH, BSH), new Vector3(BSH, -BSH, -BSH), new Vector3(BSH, BSH, -BSH), new Vector3(BSH, BSH, BSH) };
        static Vector3[] FaceXMinus = new Vector3[] { new Vector3(-BSH, -BSH, BSH), new Vector3(-BSH, BSH, BSH), new Vector3(-BSH, BSH, -BSH), new Vector3(-BSH, -BSH, -BSH) };
        static Vector3[] FaceZPlus = new Vector3[] { new Vector3(-BSH, BSH, BSH), new Vector3(-BSH, -BSH, BSH), new Vector3(BSH, -BSH, BSH), new Vector3(BSH, BSH, BSH) };
        static Vector3[] FaceZMinus = new Vector3[] { new Vector3(BSH, -BSH, -BSH), new Vector3(-BSH, -BSH, -BSH), new Vector3(-BSH, BSH, -BSH), new Vector3(BSH, BSH, -BSH) };

        static Vector3 NormalYPlus = Vector3.up;
        static Vector3 NormalYMinus = Vector3.down;
        static Vector3 NormalXPlus =  Vector3.right;
        static Vector3 NormalXMinus = Vector3.left;
        static Vector3 NormalZPlus = Vector3.forward;
        static Vector3 NormalZMinus = Vector3.back;

        static Vector3 QuaterOffset = new Vector3(-BSH/2f, -BSH/2f, -BSH/2f);

        public struct VertPoint
        {
            public Vector3 position;
            public Vector3 normal;
            public Color color;
            public byte paletteNo;

            public override bool Equals(object obj)
            {
                return obj is VertPoint point &&
                       position.Equals(point.position) &&
                       normal.Equals(point.normal) &&
                       color.Equals(point.color) &&
                       paletteNo.Equals(point.paletteNo);
            }
            public override int GetHashCode()
            {
                return System.HashCode.Combine(position, normal, color, paletteNo);
            }
        }
        public struct VertPointUV
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 uv;

            public override bool Equals(object obj)
            {
                return obj is VertPointUV point &&
                       position.Equals(point.position) &&
                       normal.Equals(point.normal) &&
                       uv.Equals(point.uv);
            }
            public override int GetHashCode()
            {
                return System.HashCode.Combine(position, normal, uv);
            }
        }

        public enum Face
        {
            YPlus, YMinus, XPlus, XMinus, ZPlus, ZMinus
        }
        static Vector3[][] FacePoints = { FaceYPlus, FaceYMinus, FaceXPlus, FaceXMinus, FaceZPlus, FaceZMinus};
        static Vector3[] FaceNormals = { NormalYPlus, NormalYMinus, NormalXPlus, NormalXMinus, NormalZPlus, NormalZMinus };

        public static int BindColor(int palNo, Color col)
        {
            return (palNo << 24) + (((int)(col.r * 255) & 255) << 16) + (((int)(col.g * 255) & 255) << 8) + ((int)(col.b * 255) & 255);
        }
        public static (int paletteNo, Color color) UnbindColor(int bindValue)
        {
            var cf = bindValue & 0xffffffff;
            var palNo = (int)((cf >> 24) & 255);
            var col = new Color(((cf >> 16) & 255) / 255f, ((cf >> 8) & 255) / 255f, (cf & 255) / 255f);

            return (palNo, col);
        }

        public static void AddFace(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts, List<VertPoint> indies)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            v.color = block.GetColor(palette);
            v.paletteNo = (block.paletteNo == 255 || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            v.normal = FaceNormals[f];
            var len = FacePoints[f].Length;
            VertPoint[] hashs = new VertPoint[len];
            for (var i = 0; i < len; i++)
            {
                v.position = pos + FacePoints[f][i];
                if (!verts.ContainsKey(v))
                {
                        verts.Add(v, 0);
                }
                hashs[i] = v;

            }
            indies.Add(hashs[0]);
            indies.Add(hashs[1]);
            indies.Add(hashs[2]);
            indies.Add(hashs[0]);
            indies.Add(hashs[2]);
            indies.Add(hashs[3]);
        }
        public static void AddFaceQuater(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts, List<VertPoint> indies)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            v.color = block.GetColor(palette);
            v.paletteNo = (block.paletteNo == 255 || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            v.normal = FaceNormals[f];
            var len = FacePoints[f].Length;
            VertPoint[] hashs = new VertPoint[len];
            for (var i = 0; i < len; i++)
            {
                v.position = (pos + FacePoints[f][i])/2f + QuaterOffset;
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
                hashs[i] = v;
            }
            indies.Add(hashs[0]);
            indies.Add(hashs[1]);
            indies.Add(hashs[2]);
            indies.Add(hashs[0]);
            indies.Add(hashs[2]);
            indies.Add(hashs[3]);
        }

        static Vector2[] FaceUV = { new Vector2(0,1), new Vector2(0,0), new Vector2(1,0), new Vector2(1,1)};

        public static void AddFaceJoint(Vector3 pos, Face face, Dictionary<VertPointUV, int> verts, List<VertPointUV> indies)
        {
            VertPointUV v = new VertPointUV();
            var f = (int)face;
            v.normal = FaceNormals[f];
            var len = FacePoints[f].Length;
            VertPointUV[] hashs = new VertPointUV[len];

            for (var i = 0; i < len; i++)
            {
                v.position = pos + FacePoints[f][i] * 1.005f;
                v.uv = FaceUV[i];
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
                hashs[i] = v;
            }
            indies.Add(hashs[0]);
            indies.Add(hashs[1]);
            indies.Add(hashs[2]);
            indies.Add(hashs[0]);
            indies.Add(hashs[2]);
            indies.Add(hashs[3]);
        }
        public static void AddFaceLine(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts, List<VertPoint> indies)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            var c = block.GetColor(palette);
            c = c.r + c.g + c.b > 1f ? Color.black : Color.white;
            v.color = c;
            v.paletteNo = (block.paletteNo == 255 || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            v.normal = Vector3.up;
            var len = FacePoints[f].Length;
            VertPoint[] hashs = new VertPoint[len];
            for (var i = 0; i < len; i++)
            {
                v.position = pos + FacePoints[f][i];
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
                hashs[i] = v;
            }
            indies.Add(hashs[0]);
            indies.Add(hashs[1]);
            indies.Add(hashs[1]);
            indies.Add(hashs[2]);
            indies.Add(hashs[0]);
            indies.Add(hashs[3]);
            indies.Add(hashs[2]);
            indies.Add(hashs[3]);
        }
        public static void AddFaceLineQuater(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts, List<VertPoint> indies)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            var c = block.GetColor(palette);
            c = c.r + c.g + c.b > 1f ? Color.black : Color.white;
            v.color = c;
            v.paletteNo = (block.paletteNo == 255 || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            v.normal = Vector3.up;
            var len = FacePoints[f].Length;
            VertPoint[] hashs = new VertPoint[len];
            for (var i = 0; i < len; i++)
            {
                v.position = (pos + FacePoints[f][i])/2f + QuaterOffset;
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
                hashs[i] = v;
            }
            indies.Add(hashs[0]);
            indies.Add(hashs[1]);
            indies.Add(hashs[1]);
            indies.Add(hashs[2]);
            indies.Add(hashs[0]);
            indies.Add(hashs[3]);
            indies.Add(hashs[2]);
            indies.Add(hashs[3]);
        }

        public static MeshData CreateMeshContents(VoxelJoint joint, VoxelBlockData data, VoxelPalette palette)
            {
            var center = joint.position;

            Dictionary<VertPoint, int> verts = new Dictionary<VertPoint, int>();
            List<VertPoint> indies = new List<VertPoint>();

            // 構成されているブロックを一つづつ取り出す
            foreach (var one in data.blocks)
            {
                var pos = one.Key;
				//var col = one.Value.GetColor(palette);
				var cpos = pos;
				cpos.x *= 6;

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFace(pos, Face.XMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.right;
                if(!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 1;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFace(pos, Face.XPlus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 2;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
                    AddFace(pos, Face.YMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 3;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFace(pos, Face.YPlus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 4;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
                    AddFace(pos, Face.ZMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 5;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
                    AddFace(pos, Face.ZPlus, o, palette, verts, indies);
                }
            }
            foreach (var one in data.quaterBlocks)
            {
                var pos = one.Key;
				var cpos = pos;
				cpos.x *= 6;

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.XMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.right;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 1;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.XPlus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 2;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.YMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 3;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.YPlus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 4;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.ZMinus, o, palette, verts, indies);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 5;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFaceQuater(pos, Face.ZPlus, o, palette, verts, indies);
                }
            }
            Vector3[] meshVerts = new Vector3[verts.Count];
            Vector3[] meshNormals = new Vector3[verts.Count];
            Color[] meshColors = new Color[verts.Count];
            int[] meshIndies = new int[indies.Count];

            Dictionary<VertPoint, int>hashToItem = new Dictionary<VertPoint, int>();

            var c = 0;
            foreach(var one in verts)
            {
                var ph = one.Key;
                var v = one.Key;
                meshVerts[c] = v.position - center;
                meshNormals[c] = v.normal;
                meshColors[c] = v.color.linear; // SRGBからリニアへ変換してあげる
                hashToItem.Add(ph, c);
                c++;
            }
            for(var i = 0; i < indies.Count; i++)
            {
                meshIndies[i] = hashToItem[indies[i]];
            }

            return new MeshData(meshVerts, meshNormals, meshColors, meshIndies);
        }


        /// <summary>
        /// メインスレッド上で実行しており、非同期で作成しない場合に使う。非同期にする場合は、CreateMeshContentsとCreateMeshesを使う
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="data"></param>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static Mesh CreateMesh(VoxelJoint joint, VoxelBlockData data, VoxelPalette palette)
        {
            if(data.blockCount == 0 && data.quaterBlockCount == 0)
            {
                return null;
            }
            MeshData meshData = CreateMeshContents(joint, data, palette);

            Mesh mesh = null;
            if (meshData.verts.Length == 0)
            {
                mesh = new Mesh();
                return mesh;
            }

            mesh = new Mesh();
            mesh.SetVertices(meshData.verts);
            mesh.SetNormals(meshData.normals);
            mesh.SetColors(meshData.colors);
            mesh.SetIndices(meshData.indies, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            return mesh;
        }
        private static void ContextProc(SynchronizationContext scontext, System.Action callProc)
        {
            if (scontext != null)
            {
                scontext.Send((_) => { callProc(); }, null);
            }
            else
            {
                callProc();
            }
            }


        public static Mesh CreateMeshJoint(VoxelJoint joint)
        {
            Mesh mesh = new Mesh();
            var center = joint.position;

            Dictionary<VertPointUV, int> verts = new Dictionary<VertPointUV, int>();
            List<VertPointUV> indies = new List<VertPointUV>();

            {
                var pos = joint.position;

                AddFaceJoint(pos, Face.XMinus, verts, indies);
                AddFaceJoint(pos, Face.XPlus, verts, indies);
                AddFaceJoint(pos, Face.YMinus, verts, indies);
                AddFaceJoint(pos, Face.YPlus, verts, indies);
                AddFaceJoint(pos, Face.ZMinus, verts, indies);
                AddFaceJoint(pos, Face.ZPlus,  verts, indies);
            }

            Vector3[] meshVerts = new Vector3[verts.Count];
            Vector3[] meshNormals = new Vector3[verts.Count];
            Vector2[] meshUVs = new Vector2[verts.Count];
            Color[] meshColors = new Color[verts.Count];
            int[] meshIndies = new int[indies.Count];

            Dictionary<VertPointUV, int> hashToItem = new Dictionary<VertPointUV, int>();

            var c = 0;
            foreach (var one in verts)
            {
                var ph = one.Key;
                var v = one.Key;
                meshVerts[c] = v.position - center;
                meshNormals[c] = v.normal;
                meshUVs[c] = v.uv;
                //meshColors[c] = v.color;
                hashToItem.Add(ph, c);
                c++;
            }
            for (var i = 0; i < indies.Count; i++)
            {
                meshIndies[i] = hashToItem[indies[i]];
            }

            mesh.SetVertices(meshVerts);
            mesh.SetNormals(meshNormals);
            mesh.SetUVs(0, meshUVs);
            mesh.SetIndices(meshIndies, MeshTopology.Triangles, 0);

            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            return mesh;
        }
        public static Mesh CreateMeshWireframe(VoxelJoint joint, VoxelBlockData data, VoxelPalette palette)
        {
            if (data.blockCount == 0 && data.quaterBlockCount == 0)
            {
                return null;
            }

            Mesh mesh = new Mesh();
            var center = joint.position;

            Dictionary<VertPoint, int> verts = new Dictionary<VertPoint, int>();
            List<VertPoint> indies = new List<VertPoint>();

            // 構成されているブロックを一つづつ取り出す
            foreach (var one in data.blocks)
            {
                var pos = one.Key;
                var col = one.Value.GetColor(palette);

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.XMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.right;
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.XPlus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.YMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.YPlus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.ZMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFace(pos, p))
                {
                    AddFaceLine(pos, Face.ZPlus, one.Value, palette, verts, indies);
                }
            }
            foreach (var one in data.quaterBlocks)
            {
                var pos = one.Key;
                var col = one.Value.GetColor(palette);

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.XMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.right;
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.XPlus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.YMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.YPlus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.ZMinus, one.Value, palette, verts, indies);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFaceQuater(p))
                {
                    AddFaceLineQuater(pos, Face.ZPlus, one.Value, palette, verts, indies);
                }
            }

            if (verts.Count == 0)
            {
                return mesh;
            }

            Vector3[] meshVerts = new Vector3[verts.Count];
            //Vector3[] meshNormals = new Vector3[verts.Count];
            Color[] meshColors = new Color[verts.Count];
            int[] meshIndies = new int[indies.Count];

            Dictionary<VertPoint, int> hashToItem = new Dictionary<VertPoint, int>();

            var c = 0;
            foreach (var one in verts)
            {
                var ph = one.Key;
                var v = one.Key;
                meshVerts[c] = v.position - center;
                //meshNormals[c] = v.normal;
                meshColors[c] = v.color;
                hashToItem.Add(ph, c);
                c++;
            }
            for (var i = 0; i < indies.Count; i++)
            {
                meshIndies[i] = hashToItem[indies[i]];
            }

            mesh.SetVertices(meshVerts);
            mesh.SetColors(meshColors);
            mesh.SetIndices(meshIndies, MeshTopology.Lines, 0);

            mesh.RecalculateBounds();

            return mesh;
        }


        // save用に使用パレット一覧を出すための関数
        public static void AddFacePalette(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            v.color = block.GetColor(palette);
            v.normal = FaceNormals[f];
            v.paletteNo = (block.paletteNo >= VoxelPalette.freeColorNumBegin || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            var len = FacePoints[f].Length;
            for (var i = 0; i < len; i++)
            {
                v.position = pos + FacePoints[f][i];
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
            }
        }
        public static void AddFacePaletteQuater(Vector3 pos, Face face, SingleBlockData block, VoxelPalette palette, Dictionary<VertPoint, int> verts)
        {
            VertPoint v = new VertPoint();
            var f = (int)face;
            v.color = block.GetColor(palette);
            v.normal = FaceNormals[f];
            v.paletteNo = (block.paletteNo >= VoxelPalette.freeColorNumBegin || (block.paletteNo < VoxelPalette.PaletteNum && !palette.exportAsMaterial[block.paletteNo])) ? (byte)255 : block.paletteNo;
            var len = FacePoints[f].Length;
            for (var i = 0; i < len; i++)
            {
                v.position = (pos + FacePoints[f][i])/2f + QuaterOffset;
                if (!verts.ContainsKey(v))
                {
                    verts.Add(v, 0);
                }
            }
        }

        // save用に使用パレット一覧を出すための関数
        public static int[] CreateMeshWithPalette(VoxelJoint joint, VoxelBlockData data, VoxelPalette palette)
        {
            if (data.blockCount == 0 && data.quaterBlockCount == 0)
            {
                return null;
            }

            var center = joint.position;

            Dictionary<VertPoint, int> verts = new Dictionary<VertPoint, int>();

            // 構成されているブロックを一つづつ取り出す
            foreach (var one in data.blocks)
            {
                var pos = one.Key;
				//var col = one.Value.GetColor(palette);
				var cpos = pos;
				cpos.x *= 6;

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.XMinus, o, palette, verts);
                }
                p = pos + Vector3Int.right;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 1;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.XPlus, o, palette, verts);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 2;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.YMinus, o, palette, verts);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 3;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.YPlus, o, palette, verts);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 4;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.ZMinus, o, palette, verts);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFace(pos, p))
                {
					var cp = cpos;
					cp.x += 5;
					var o = data.faceColors.GetValueOrDefault(cp, one.Value);
					AddFacePalette(pos, Face.ZPlus, o, palette, verts);
                }
            }
            //Debug.Log($"VertsCount:{verts.Count}");
            foreach (var one in data.quaterBlocks)
            {
                var pos = one.Key;
				var cpos = pos;
				cpos.x *= 6;
				//var col = one.Value.GetColor(palette);

                var p = pos + Vector3Int.left;
                // 隣り合ったブロックが無ければ面を追加する
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.XMinus, o, palette, verts);
                }
                p = pos + Vector3Int.right;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 1;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.XPlus, o, palette, verts);
                }
                p = pos + Vector3Int.down;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 2;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.YMinus, o, palette, verts);
                }
                p = pos + Vector3Int.up;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 3;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.YPlus, o, palette, verts);
                }
                p = pos + Vector3Int.back;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 4;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.ZMinus, o, palette, verts);
                }
                p = pos + Vector3Int.forward;
                if (!data.ExistsFaceQuater(p))
                {
					var cp = cpos;
					cp.x += 5;
					var o = data.faceColorsQuater.GetValueOrDefault(cp, one.Value);
					AddFacePaletteQuater(pos, Face.ZPlus, o, palette, verts);
                }
            }
            //Debug.Log($"VertsCount2:{verts.Count}");

            if (verts.Count == 0)
            {
                return null;
            }

            int[] rt = new int[verts.Count];

            var c = 0;
            foreach (var one in verts)
            {
                var ph = one.Key;
                var v = one.Key;
                rt[c] = v.paletteNo;
                c++;
            }

            return rt;
        }
    }
}
