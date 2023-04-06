using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderEditor
{
	internal class DataUtils
	{
		public enum ePlatform
		{
			Xenon,
			PC
		}
		public enum eGame
		{
			IV,
			BlueDudeBamboozle = 2,
			MP3
		}
		public static string ReadString(BinaryReader br)
		{
			byte size = br.ReadByte();
			char[] tmp = new char[size - 1];
			tmp = br.ReadChars(size - 1);
			size = br.ReadByte();
			string tempstring = new string(tmp);
			return tempstring;
		}
		public static string ReadStringAtOffset(uint p, BinaryReader br)
		{
			uint oldPos = (uint)br.BaseStream.Position;
			br.BaseStream.Position = p;
			string tempstring = "";
			char tmp;
			for (int a = 0; a < 1;)
			{
				tmp = br.ReadChar();
				if (tmp != 0) tempstring += tmp;
				else break;
			}
			br.BaseStream.Position = oldPos;
			return tempstring;
		}
		public static void WriteString(BinaryWriter bw, string value)
		{
			byte size = (byte)(value.Length+1);
			byte[] array2 = Encoding.ASCII.GetBytes(value);
			Array.Resize<byte>(ref array2, size);
			bw.Write(size);
			bw.Write(array2);
		}
		public static bool FileExists(string filename)
		{
			return System.IO.File.Exists(filename);
		}
		public static string TypeAsString(byte type)
		{
			switch (type)
			{
				case 1: return "int";
				case 2: return "float";
				case 3: return "float2";
				case 4: return "float3";
				case 5: return "float4";
				case 6: return "sampler";
				case 7: return "bool";
				case 8: return "float4x3";
				case 9: return "float4x4";
			}
			return "unkType";
		}
		public static byte TypeAsByte(string type)
		{
			switch (type)
			{
				case "int": return 1;
				case "float": return 2;
				case "float2": return 3;
				case "float3": return 4;
				case "float4": return 5;
				case "sampler": return 6;
				case "bool": return 7;
				case "float4x3": return 8;
				case "float4x4": return 9;
			}
			return 0;
		}
		public static byte AnnotationTypeAsByte(string type)
		{
			switch (type)
			{
				case "int": return 0;
				case "float": return 1;
				case "string": return 2;
			}
			return 3;
		}
		public static void Align(ref uint currentPos,uint value)
		{
			while (currentPos % value != 0)
				currentPos += 1;
		}

		public static void WriteValueToByteArray(ref byte[] array, string str, ref uint pos)
		{
			byte[] array2 = Encoding.ASCII.GetBytes(str);
			Array.Resize<byte>(ref array2, array2.Length + 1);
			WriteToByteArray(ref array, array2, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, uint value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, int value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, short value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, ushort value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, byte value, ref uint pos)
		{
			WriteToByteArray(ref array, new byte[] { value }, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, sbyte value, ref uint pos)
		{
			WriteToByteArray(ref array, new byte[] { (byte)value }, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, float value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		//public static void WriteValueToByteArray(ref byte[] array, Vector4 value, ref uint pos)
		//{
		//	WriteValueToByteArray(ref array, value.X, ref pos);
		//	WriteValueToByteArray(ref array, value.Y, ref pos);
		//	WriteValueToByteArray(ref array, value.Z, ref pos);
		//	WriteValueToByteArray(ref array, value.W, ref pos);
		//}
		//public static void WriteValueToByteArray(ref byte[] array, Matrix value, ref uint pos)
		//{
		//	WriteValueToByteArray(ref array, value.m0, ref pos);
		//	WriteValueToByteArray(ref array, value.m1, ref pos);
		//	WriteValueToByteArray(ref array, value.m2, ref pos);
		//	WriteValueToByteArray(ref array, value.m3, ref pos);
		//}


		public static void WriteToByteArray(ref byte[] array, byte[] array2, ref uint pos)
		{
			for (int a = 0; a < array2.Length; a++)
				array[pos + a] = array2[a];
			pos += (uint)array2.Length;
		}
	}
	public static class FileInfo
	{
		public static string fileName;
		public static string GetFileMask(string fileName)
		{
			return System.IO.Path.GetExtension(fileName);
		}
		public static string fileMask
		{
			get
			{
				if (fileName.Contains('.')) return GetFileMask(fileName);
				else return "";
			}
		}
		public static string baseFileName
		{
			get
			{
				return Path.GetFileName(FileInfo.fileName.Substring(0, FileInfo.fileName.Length - FileInfo.fileMask.Length));
			}
		}

		public static string filePath
		{
			get
			{
				return Path.GetDirectoryName(FileInfo.fileName.Substring(0, FileInfo.fileName.Length - FileInfo.fileMask.Length));
			}
		}

		public static string exePath
		{
			get
			{
				return System.Reflection.Assembly.GetEntryAssembly().Location;
			}
		}
		//


	}
}
