using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Lindwurm.Voxel
{
	public partial class VoxelUtil
	{
		public static void LoadFromFile(string filePath, VoxelBlock voxelBlock, bool onlyInfo, System.Action<VoxelSaveInfoFormat> callback)
		{
			VoxelSaveInfoFormat vsi = null;
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open))
				{
					using (BinaryReader br = new BinaryReader(fs))
					{
						vsi = DoLoad(br, voxelBlock, onlyInfo);
					}
				}
			}
			catch (System.Exception e)
			{
				Debug.Log("Exception: " + e.Message);
			}
			finally
			{
#if UNITY_EDITOR
//                    Debug.Log("End.");
#endif
			}
			callback(vsi);
		}
		public static void LoadFromBinary(byte[] bin, VoxelBlock voxelBlock, bool onlyInfo, System.Action<VoxelSaveInfoFormat> callback)
		{
			VoxelSaveInfoFormat vsi = null;
			try
			{
				using (MemoryStream ms = new MemoryStream(bin))
				{
					using (BinaryReader br = new BinaryReader(ms))
					{
						vsi = DoLoad(br, voxelBlock, onlyInfo);
					}
				}
			}
			catch (System.Exception e)
			{
				Debug.Log("Exception: " + e.Message);
			}
			finally
			{
#if UNITY_EDITOR
//                Debug.Log("End.");
#endif
			}
			callback?.Invoke(vsi);
		}
		private static string ReadString(BinaryReader inputBinaryReader, int bytes)
		{
			var buf = inputBinaryReader.ReadBytes(bytes);
			var cbuf = new char[buf.Length];
			for (var i = 0; i < buf.Length; i++)
			{
				cbuf[i] = (char)buf[i];
			}
			return new string(cbuf);
		}
		private static (bool validHeader, string version, long fileLen) CheckFileHeader(BinaryReader inputBinaryReader)
		{
			var len = (inputBinaryReader.BaseStream is FileStream) ? ((FileStream)inputBinaryReader.BaseStream).Length : ((MemoryStream)inputBinaryReader.BaseStream).Length;

			var header = ReadString(inputBinaryReader, 20).Trim().Split(".");
			if (header.Length < 3 || header[0] != "VoxelData")
			{
				return (false, string.Empty, 0);
			}
			return (true, header[1],len);
		}

		private static VoxelSaveInfoFormat DoLoad(BinaryReader inputBinaryReader, VoxelBlock voxelBlock, bool onlyInfo)
		{
			var header = CheckFileHeader(inputBinaryReader);
			if(!header.validHeader)
			{
				Debug.LogError("Not VoxelData");
				return null;
			}

			VoxelSaveInfoFormat vsi = null;

			var len = header.fileLen - 20;
			if (header.version != "000001")
			{
				while (true)
				{
					var infoTag = ReadString(inputBinaryReader, 4);
					len = inputBinaryReader.ReadInt32();
					if (infoTag == "VOXL")
					{
						break;
					} else if(infoTag == "INFO")
					{
						var j = inputBinaryReader.ReadBytes((int)len);
						using (var ms = new MemoryStream(j))
						{
							using (var gs = new GZipStream(ms, CompressionMode.Decompress))
							{
								using (var rs = new MemoryStream())
								{
									gs.CopyTo(rs);
									var bs = rs.ToArray();
									if (bs[0] != '{')
									{
										var p = 0;
										while (true)
										{
											if ((bs[p] & 0x80) == 0)
											{
												p++;
												break;
											}
											p++;
										}
										for (var i = 0; p < bs.Length; i++, p++)
										{
											bs[i] = bs[p];
											bs[p] = 0;
										}
									}
									var js = Encoding.UTF8.GetString(bs);
									var vsis = JsonUtility.FromJson<VoxelSaveInfoFormatSerialized>(js);
									vsi = new VoxelSaveInfoFormat(vsis);
									if(voxelBlock != null)
									{
										voxelBlock.voxelModelFileParameter = vsi;
									}
								}
							}
						}
						if(onlyInfo)
						{
							break;
						}
					} else if(infoTag == "FNBK" && !onlyInfo)	// 機能ブロック
					{
						var j = inputBinaryReader.ReadBytes((int)len);
						using (var ms = new MemoryStream(j))
						{
							using (var gs = new GZipStream(ms, CompressionMode.Decompress))
							{
								using (var rs = new MemoryStream())
								{
									gs.CopyTo(rs);
									var bs = rs.ToArray();
									if (bs[0] != '{')
									{
										var p = 0;
										while (true)
										{
											if ((bs[p] & 0x80) == 0)
											{
												p++;
												break;
											}
											p++;
										}
										for (var i = 0; p < bs.Length; i++, p++)
										{
											bs[i] = bs[p];
											bs[p] = 0;
										}
									}
									var js = Encoding.UTF8.GetString(bs);
									var infos = JsonUtility.FromJson<VoxelFuncBlockInfoSerialized>(js);
									if(infos != null && infos.dat != null)
									{
										voxelBlock.voxelFuncBlockInfoParameter = infos.dat.ToList();
									}
									else
									{
										voxelBlock.voxelFuncBlockInfoParameter.Clear();
									}
								}
							}
						}
					}
					else
					{
						break;
					}
				}
			}else
			{
				vsi = new VoxelSaveInfoFormat("", "", "", 0.05f, null);
			}
			if (onlyInfo || voxelBlock == null)
			{
				return vsi;
			}
			var palette = voxelBlock.GetPalette();

			byte[] zipData = new byte[len];
			var pp = 0;
			while (len > 0)
			{
				var t = 8192;
				if (t > len)
				{
					t = (int)len;
				}
				len -= t;
				var bbuf = inputBinaryReader.ReadBytes(t);
				for (var i = 0; i < bbuf.Length; i++)
				{
					zipData[pp + i] = bbuf[i];
				}
				pp += bbuf.Length;
			}


			string json;
			using (var ms = new MemoryStream(zipData))
			{
				using (var gs = new GZipStream(ms, CompressionMode.Decompress))
				{
					using (var br = new BinaryReader(gs))
					{
						json = br.ReadString();
					}
				}
			}

			var vf = JsonUtility.FromJson<VoxelSaveFormat>(json);
			for (var i = 0; i < vf.joints.Length; i++)
			{
				var j = vf.joints[i];
				var id = Mathf.Max(0, Mathf.Min(VoxelBlock.MAX_JOINT, j.id));
				var jd = voxelBlock.GetJoint(id);
				if (jd == null)
				{
					jd = new VoxelJoint(id, -1, Vector3.zero);
					voxelBlock.AddJoint(jd);
				}
				jd.name = j.name;
				jd.position = j.position;
				jd.parentId = j.parentId;
				voxelBlock.EnableEdit(id, j.enableEdit == 1);
				voxelBlock.LockJoint(id, j.lockJoint == 1);
				voxelBlock.SetMeshGroup(id, j.meshGroup);
				voxelBlock.SetConstraint(id, j.constraint.type, j.constraint.srcId);
				var p = jd.poseLocalDic;
				for(var k = 0; k < j.poses.Length; k++)
				{
					if (!string.IsNullOrEmpty(j.poses[k].name))
					{
						if (!p.ContainsKey(j.poses[k].name))
						{
							p.Add(j.poses[k].name, (j.poses[k].position, j.poses[k].rotation, (j.poses[k].scale == 0f ? 1.0f : j.poses[k].scale), j.poses[k].touched));
						}
					}
				}
			}
			var joints = voxelBlock.EnumGetJoint();
			while(joints.MoveNext())
			{
				if(joints.Current.parentId >= 0)
				{
					var j = voxelBlock.GetJoint(joints.Current.parentId);
					joints.Current.localPosition = joints.Current.position - j.position;    // ローカルでの位置計算
				}
			}

			for (var i = 0; i < vf.blocks.Length; i++)
			{
				var b = vf.blocks[i];
				var id = Mathf.Max(0, Mathf.Min(VoxelBlock.MAX_JOINT, b.id));
				SetData(voxelBlock.GetBlock(id, true).blocks, b.data);
			}
			if(vf.quaterBlocks != null)
			{
				for (var i = 0; i < vf.quaterBlocks.Length; i++)
				{
					var b = vf.quaterBlocks[i];
					var id = Mathf.Max(0, Mathf.Min(VoxelBlock.MAX_JOINT, b.id));
					SetData(voxelBlock.GetBlock(id, true).quaterBlocks, b.data);
				}
			}
			if(vf.faceColors != null)
			{
				for(var i = 0; i < vf.faceColors.Length; i++)
				{
					var b = vf.faceColors[i];
					var id = Mathf.Max(0, Mathf.Min(VoxelBlock.MAX_JOINT, b.id));
					SetData(voxelBlock.GetBlock(id, true).faceColors, b.data);
				}
			}
			if(vf.faceColorsQuater != null)
			{
				for (var i = 0; i < vf.faceColorsQuater.Length; i++)
				{
					var b = vf.faceColors[i];
					var id = Mathf.Max(0, Mathf.Min(VoxelBlock.MAX_JOINT, b.id));
					SetData(voxelBlock.GetBlock(id, true).faceColorsQuater, b.data);
				}
			}
			void SetData(Dictionary<Vector3Int, SingleBlockData> blk, byte[] dat)
			{
				short z;
				var offs = 0;

				while ((z = GetShortData(dat, ref offs)) != 8192 && dat.Length > offs)
				{
					var y = GetShortData(dat, ref offs);
					short x;
					while ((x = GetShortData(dat, ref offs)) != 8192 && dat.Length > offs)
					{
						var pos = new Vector3Int(x, y, z);
						var c = UnbindColor(GetIntData(dat, ref offs));
						SingleBlockData s = new SingleBlockData(c.color);
						if (c.paletteNo != 255)
						{
							s.SetPalette((byte)c.paletteNo);
						}
						blk[pos] = s;
					}
				}
			}
			for (var i = 0; i < vf.palettes.Length; i++)
			{
				var p = vf.palettes[i];
				var c = UnbindColor(p.color);
				palette.SetColor(c.paletteNo, c.color);

			}

			short GetShortData(byte[] byteData, ref int offset)
			{
				if (byteData.Length <= offset + 1)
				{
					offset = byteData.Length;
					return 0;
				}
				var rt = (short)(byteData[offset] + byteData[offset + 1] * 256);
				offset += 2;
				return rt;
			}
			int GetIntData(byte[] byteData, ref int offset)
			{
				if (byteData.Length <= offset + 3)
				{
					offset = byteData.Length;
					return 0;
				}
				var rt = byteData[offset] | (byteData[offset + 1] << 8) | (byteData[offset + 2] << 16 | (byteData[offset + 3] << 24));
				offset += 4;
				return rt;
			}

			return vsi;
		}
	}
}
