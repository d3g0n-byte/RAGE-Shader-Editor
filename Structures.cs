using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ShaderEditor
{
	public class Structures
	{
		//struct STRING
		//{
		//	byte size;
		//	char[] name;
		//}
		public struct SM3_VERSION
		{
			public byte version2;
			public byte version1;
		}
		public struct SM3_COMMENTS_VARIABLE_TYPE
		{
			public ushort type1;
			public ushort type2;
			public ushort count1;
			public ushort count2;// основной
			public ushort arrayCount;
			public ushort _fa;
			public ushort _fc;
		}
		public struct SM3_COMMENTS_VARIABLE
		{
			public string name;
			public ushort registerType; 
			public ushort index;
			public ushort size;
			public ushort _f0a;
			public SM3_COMMENTS_VARIABLE_TYPE types;
			public uint _f14;
		}

		public struct SM3_COMMENTS
		{
			public uint _f0;
			public string creator; // = pos + 12
			public SM3_VERSION version;
			public ushort shaderType;
			public uint paramCount;
			public uint _f10;
			public uint _f14;
			public string profile; //
			public SM3_COMMENTS_VARIABLE[] variables;
		}
		public struct SM3
		{
			public SM3_VERSION version;
			public ushort shaderType;   // 0xffff - pixel, 0xfeffff - vertex
			public ushort _f4;   // 0xfeffff
			public ushort bytecodePosition;//= pos + 8
			public uint CTAB;//
			public SM3_COMMENTS comments;
			public byte[] bytecode;

		}
		public struct ANNOTATION_VALUE
		{
			public int intValue;       // 0
			public float floatValue;   // 1
			public string stringValue; // 2
		}
		public struct ANNOTATION
		{
			public string name;
			public byte valueType;
			public ANNOTATION_VALUE value;
		}
		public struct VARIABLE_VALUE
		{
			public float floatValue;
			public int intValue;
		}
		public struct VARIABLE
		{
			public byte type;
			public byte arrayCount;
			public string name1;
			public string name2;
			public byte annotationCount;
			public ANNOTATION[] annotation;
			public byte valueCount;
			public VARIABLE_VALUE[] value;
		}
		public struct SHADER_VARIABLE
		{
			public ushort type;
			public ushort index;
			public string name;
		}
		public struct PASS_VALUE
		{
			public uint type;
			public uint value;
		}
		public struct PASS
		{
			public byte vs;
			public byte ps;
			public byte valueCount;
			public PASS_VALUE[] value;
		}
		public struct TECHNIQUE
		{
			public string name;
			public byte passCount;
			public PASS[] pass;
		}
		public struct FRAGMENT
		{
			public string name;			// в оригинальном шейдера нет имени, но для удобства я добавил.
			public byte variablesCount;
			public SHADER_VARIABLE[] variable;
			public ushort shaderSize;
			public ushort shaderSize2;   // во всех rage-играх кроме rdr
			public byte[] shader;
		}
		public struct FXC_IV
		{
			public uint magic;
			public byte vertexFragmentCount;
			public FRAGMENT[] vertexFragment;
			public byte pixelFragmentCount;
			public byte unk1;
			public uint unk2;
			public FRAGMENT[] pixelFragment;
			public byte globalVariablesCount;
			public VARIABLE[] globalVariable;
			public byte localVariablesCount;
			public VARIABLE[] localVariable;
			public byte techniquesCount;
			public TECHNIQUE[] technique;
		}
		// Max Payne 3
		public struct PASS_MP3
		{
			public byte vs;
			public byte ps;
			public byte cs; // compute
			public byte ds; // domain
			public byte gs; // geometry
			public byte hs; // hull
			public byte valueCount;
			public PASS_VALUE[] value;
		}
		public struct TECHNIQUE_MP3
		{
			public string name;
			public byte passCount;
			public PASS_MP3[] pass;
		}

		public struct VARIABLE_MP3
		{
			public byte type;
			public byte arrayCount;
			public byte slot;
			public byte group;
			public string name1;
			public string name2;
			public byte unk0;//offset. -1 
			public byte unk1;//variant. -1
			public byte unk2;//unused0
			public byte unk3;//unused1
			public byte unk4;
			public byte unk5;
			public byte unk6;
			public byte unk7;
			public byte annotationCount;
			public ANNOTATION[] annotation;
			public byte valueCount;
			public VARIABLE_VALUE[] value;
		}
		public struct PRESETPARAM_VALUE
		{
			public int intValue;       // 0
			public float floatValue;   // 1
			public string stringValue; // 2
		}
		public struct PRESETPARAM
		{
			public string name;
			public byte valueType;
			public PRESETPARAM_VALUE value;
		}
		public struct FXC_MP3_SM3
		{
			public uint magic;
			public uint vertexFormat;
			public byte presetParamCount;
			public PRESETPARAM[] presetParam;
			public byte vertexFragmentCount;
			public FRAGMENT[] vertexFragment;
			public byte pixelFragmentCount;
			public string pixelUnkName;
			public byte pixelUnk1;
			public uint pixelUnk2;
			public FRAGMENT[] pixelFragment;
			public byte computeFragmentCount;
			public string computeUnkName;
			public byte computeUnk1;
			public uint computeUnk2;
			public FRAGMENT[] computeFragment;
			public byte domainFragmentCount;
			public string domainUnkName;
			public byte domainUnk1;
			public uint domainUnk2;
			public FRAGMENT[] domainFragment;
			public byte geometryFragmentCount;
			public string geometryUnkName;
			public byte geometryUnk1;
			public uint geometryUnk2;
			public FRAGMENT[] geometryFragment;
			public byte hullFragmentCount;
			public string hullUnkName;
			public byte hullUnk1;
			public uint hullUnk2;
			public byte unk0;
			public FRAGMENT[] hullFragment;
			public byte globalVariablesCount;
			public VARIABLE_MP3[] globalVariable;
			public byte unk1;
			public byte localVariablesCount;
			public VARIABLE_MP3[] localVariable;
			public byte techniquesCount;
			public TECHNIQUE_MP3[] technique;
		}

	}
	public class ReadStructures
	{
		static void ReadStringWithoutSize(BinaryReader br, ref string text)
		{
			text = "";
			char tmp;
			while(true)
			{
				tmp = br.ReadChar();
				if (tmp != 0) text += tmp;
				else break;
			}
		}
		static void ReadSM3_COMMENTS(BinaryReader br, ref Structures.SM3_COMMENTS comment)
		{
			comment._f0= br.ReadUInt16();
		}
		static void ReadAssembledSM3(BinaryReader br, ref Structures.SM3 shader, int shaderSize)
		{
			shader.version.version1 = br.ReadByte();
			shader.version.version2 = br.ReadByte();
			shader.shaderType = br.ReadUInt16();


		}

		/*---=================---*/
		public static void ReadANNOTATION_VALUE(BinaryReader fxc, ref Structures.ANNOTATION_VALUE var, byte type)
		{
			//Structures.ANNOTATION_VALUE tmp;
			switch (type)
			{
				case 0:
					var.intValue = fxc.ReadInt32();
					break;
				case 1:
					var.floatValue = fxc.ReadSingle();
					break;
				case 2:
					var.stringValue = DataUtils.ReadString(fxc);
					break;
			}
		}
		public static void ReadANNOTATION(BinaryReader fxc, ref Structures.ANNOTATION var)
		{
			//Structures::ANNOTATION tmp;
			var.name = DataUtils.ReadString(fxc);
			var.valueType = fxc.ReadByte();
			ReadANNOTATION_VALUE(fxc, ref var.value, var.valueType);
		}
		public static void ReadVARIABLE_VALUE(BinaryReader fxc, ref Structures.VARIABLE_VALUE val, byte paramType)
		{
			//Structures::VARIABLE_VALUE tmp;
			if (paramType > 7)
			{
				throw new Exception($"Unknown param value type {paramType}");
			}
			if (paramType == 1 || paramType == 6 || paramType == 7) val.intValue = fxc.ReadInt32();
			else val.floatValue = fxc.ReadSingle();
		}
		public static void ReadVARIABLE_IV(BinaryReader fxc, ref Structures.VARIABLE var)
		{
			//Structures.VARIABLE tmp;
			var.type = fxc.ReadByte();
			var.arrayCount = fxc.ReadByte();
			var.name1 = DataUtils.ReadString(fxc);
			var.name2 = DataUtils.ReadString(fxc);
			var.annotationCount = fxc.ReadByte();
			Array.Resize<Structures.ANNOTATION>(ref var.annotation, var.annotationCount);
			for (int a = 0; a < var.annotationCount; a++)
				ReadANNOTATION(fxc, ref var.annotation[a]);
			var.valueCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE_VALUE>(ref var.value, var.valueCount);
			for (int a = 0; a < var.valueCount; a++)
				ReadVARIABLE_VALUE(fxc, ref var.value[a], var.type);
		}
		public static void ReadPASS_VALUE(BinaryReader fxc, ref Structures.PASS_VALUE val)
		{
			//Structures::PASS_VALUE tmp;
			val.type = fxc.ReadUInt32();
			val.value = fxc.ReadUInt32();
		}
		public static void ReadPASS(BinaryReader fxc, ref Structures.PASS pass)
		{
			//Structures::PASS tmp;
			pass.vs = fxc.ReadByte();
			pass.ps = fxc.ReadByte();
			pass.ps--;
			pass.valueCount = fxc.ReadByte();
			Array.Resize<Structures.PASS_VALUE>(ref pass.value, pass.valueCount);
			for (int a = 0; a < pass.valueCount; a++)
				ReadPASS_VALUE(fxc, ref pass.value[a]);
		}
		public static void ReadTECHNIQUE(BinaryReader fxc, ref Structures.TECHNIQUE teq)
		{
			//Structures.TECHNIQUE tmp;
			teq.name = DataUtils.ReadString(fxc);
			teq.passCount = fxc.ReadByte();
			Array.Resize<Structures.PASS>(ref teq.pass, teq.passCount);
			for (int a = 0; a < teq.passCount; a++)
				ReadPASS(fxc, ref teq.pass[a]);
		}
		public static void ReadSHADER_VARIABLE(BinaryReader fxc, ref Structures.SHADER_VARIABLE var)
		{
			var.type = fxc.ReadUInt16();
			var.index = fxc.ReadUInt16();
			var.name = DataUtils.ReadString(fxc);
		}
		public static void ReadFRAGMENT(BinaryReader fxc, ref Structures.FRAGMENT frag)
		{
			frag.variablesCount = fxc.ReadByte();
			Array.Resize<Structures.SHADER_VARIABLE>(ref frag.variable, frag.variablesCount);
			for (int a = 0; a < frag.variablesCount; a++)
				ReadSHADER_VARIABLE(fxc, ref frag.variable[a]);
			frag.shaderSize = fxc.ReadUInt16();
			frag.shaderSize2 = fxc.ReadUInt16();
			frag.shader = new byte[frag.shaderSize];
			frag.shader = fxc.ReadBytes(frag.shaderSize);
			//memmove(tmp.shader, tmpShader, tmp.shaderSize);
			//delete[] tmpShader;
		}

		public static void ReadFXC_IV(BinaryReader fxc, ref Structures.FXC_IV fxc_iv)
		{
			//Structures::FXC_IV tmp;
			fxc_iv.magic = fxc.ReadUInt32();
			fxc_iv.vertexFragmentCount = fxc.ReadByte();
			Array.Resize<Structures.FRAGMENT>(ref fxc_iv.vertexFragment, fxc_iv.vertexFragmentCount);
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
				ReadFRAGMENT(fxc, ref fxc_iv.vertexFragment[a]);
			fxc_iv.pixelFragmentCount = fxc.ReadByte();
			fxc_iv.pixelFragmentCount--;
			fxc_iv.unk1 = fxc.ReadByte();
			fxc_iv.unk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_iv.pixelFragment, fxc_iv.pixelFragmentCount);
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
				ReadFRAGMENT(fxc, ref fxc_iv.pixelFragment[a]);
			fxc_iv.globalVariablesCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE>(ref fxc_iv.globalVariable, fxc_iv.globalVariablesCount);
			for (int a = 0; a < fxc_iv.globalVariablesCount; a++)
				ReadVARIABLE_IV(fxc, ref fxc_iv.globalVariable[a]);
			fxc_iv.localVariablesCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE>(ref fxc_iv.localVariable, fxc_iv.localVariablesCount);
			for (int a = 0; a < fxc_iv.localVariablesCount; a++)
				ReadVARIABLE_IV(fxc, ref fxc_iv.localVariable[a]);
			fxc_iv.techniquesCount = fxc.ReadByte();
			Array.Resize<Structures.TECHNIQUE>(ref fxc_iv.technique, fxc_iv.techniquesCount);
			for (int a = 0; a < fxc_iv.techniquesCount; a++)
				ReadTECHNIQUE(fxc, ref fxc_iv.technique[a]);
			// и имена. Их нет в оригинальном fxc файле. Добавлены чтобы соотвествовать fxc файлам остальных игр.
			if (fxc.BaseStream.Length>fxc.BaseStream.Position)// проверяем есть ли мена
			{
				for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
					fxc_iv.vertexFragment[a].name = DataUtils.ReadString(fxc);
				for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
					fxc_iv.pixelFragment[a].name = DataUtils.ReadString(fxc);
			}
			else
			{
				for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
					fxc_iv.vertexFragment[a].name = $"vs{a}";
				for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
					fxc_iv.pixelFragment[a].name = $"ps{a}";
			}
		}
		// Max Payne 3
		public static void ReadPASS_MP3(BinaryReader fxc, ref Structures.PASS_MP3 pass)
		{
			//Structures::PASS tmp;
			pass.vs = fxc.ReadByte();
			pass.ps = fxc.ReadByte();
			pass.cs = fxc.ReadByte();
			pass.ds = fxc.ReadByte();
			pass.gs = fxc.ReadByte();
			pass.hs = fxc.ReadByte();
			pass.ps--;
			pass.cs--;
			pass.ds--;
			pass.gs--;
			pass.hs--;
			pass.valueCount = fxc.ReadByte();
			Array.Resize<Structures.PASS_VALUE>(ref pass.value, pass.valueCount);
			for (int a = 0; a < pass.valueCount; a++)
				ReadPASS_VALUE(fxc, ref pass.value[a]);
		}
		public static void ReadTECHNIQUE_MP3(BinaryReader fxc, ref Structures.TECHNIQUE_MP3 teq)
		{
			//Structures.TECHNIQUE tmp;
			teq.name = DataUtils.ReadString(fxc);
			teq.passCount = fxc.ReadByte();
			Array.Resize<Structures.PASS_MP3>(ref teq.pass, teq.passCount);
			for (int a = 0; a < teq.passCount; a++)
				ReadPASS_MP3(fxc, ref teq.pass[a]);
		}

		public static void ReadSHADER_VARIABLE_MP3(BinaryReader fxc, ref Structures.SHADER_VARIABLE var)
		{
		//	var.type = fxc.ReadUInt16();
		//	var.index = fxc.ReadUInt16();
			var.name = DataUtils.ReadString(fxc);
		}
		public static void ReadFRAGMENT_MP3(BinaryReader fxc, ref Structures.FRAGMENT frag)
		{
			frag.name = DataUtils.ReadString(fxc);
			frag.variablesCount = fxc.ReadByte();
			Array.Resize<Structures.SHADER_VARIABLE>(ref frag.variable, frag.variablesCount);
			for (int a = 0; a < frag.variablesCount; a++)
				ReadSHADER_VARIABLE_MP3(fxc, ref frag.variable[a]);
			frag.shaderSize = fxc.ReadUInt16();
			frag.shaderSize2 = fxc.ReadUInt16();
			frag.shader = new byte[frag.shaderSize];
			frag.shader = fxc.ReadBytes(frag.shaderSize);
			//memmove(tmp.shader, tmpShader, tmp.shaderSize);
			//delete[] tmpShader;
		}
		public static void ReadVARIABLE_MP3(BinaryReader fxc, ref Structures.VARIABLE_MP3 var)
		{
			//Structures.VARIABLE tmp;
			var.type = fxc.ReadByte();
			var.arrayCount = fxc.ReadByte();
			var.slot = fxc.ReadByte();
			var.group = fxc.ReadByte();
			var.name1 = DataUtils.ReadString(fxc);
			var.name2 = DataUtils.ReadString(fxc);
			var.unk0 = fxc.ReadByte();
			var.unk1 = fxc.ReadByte();
			var.unk2 = fxc.ReadByte();
			var.unk3 = fxc.ReadByte();
			var.unk4 = fxc.ReadByte();
			var.unk5 = fxc.ReadByte();
			var.unk6 = fxc.ReadByte();
			var.unk7 = fxc.ReadByte();
			var.annotationCount = fxc.ReadByte();
			Array.Resize<Structures.ANNOTATION>(ref var.annotation, var.annotationCount);
			for (int a = 0; a < var.annotationCount; a++)
				ReadANNOTATION(fxc, ref var.annotation[a]);
			var.valueCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE_VALUE>(ref var.value, var.valueCount);
			for (int a = 0; a < var.valueCount; a++)
				ReadVARIABLE_VALUE(fxc, ref var.value[a], var.type);
		}
		public static void ReadPRESETPARAM_VALUE(BinaryReader fxc, ref Structures.PRESETPARAM_VALUE var, byte type)
		{
			//Structures.ANNOTATION_VALUE tmp;
			switch (type)
			{
				case 0:
					var.intValue = fxc.ReadInt32();
					break;
				case 1:
					var.floatValue = fxc.ReadSingle();
					break;
				case 2:
					var.stringValue = DataUtils.ReadString(fxc);
					break;
			}
		}
		public static void ReadPRESETPARAM(BinaryReader fxc, ref Structures.PRESETPARAM var)
		{
			//Structures::ANNOTATION tmp;
			var.name = DataUtils.ReadString(fxc);
			var.valueType = fxc.ReadByte();
			ReadPRESETPARAM_VALUE(fxc, ref var.value, var.valueType);
		}
		public static void ReadFXC_MP3(BinaryReader fxc, ref Structures.FXC_MP3_SM3 fxc_mp3)
		{
			fxc_mp3.magic = fxc.ReadUInt32();
			fxc_mp3.vertexFormat = fxc.ReadUInt32();
			fxc_mp3.presetParamCount = fxc.ReadByte();
			for (int a = 0; a < fxc_mp3.presetParamCount; a++)
				ReadPRESETPARAM(fxc, ref fxc_mp3.presetParam[a]);

			fxc_mp3.vertexFragmentCount = fxc.ReadByte();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.vertexFragment, fxc_mp3.vertexFragmentCount);
			for (int a = 0; a < fxc_mp3.vertexFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.vertexFragment[a]);

			fxc_mp3.pixelFragmentCount = fxc.ReadByte();
			fxc_mp3.pixelFragmentCount--;
			fxc_mp3.pixelUnkName = DataUtils.ReadString(fxc);
			fxc_mp3.pixelUnk1 = fxc.ReadByte();
			fxc_mp3.pixelUnk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.pixelFragment, fxc_mp3.pixelFragmentCount);
			for (int a = 0; a < fxc_mp3.pixelFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.pixelFragment[a]);

			fxc_mp3.computeFragmentCount = fxc.ReadByte();
			fxc_mp3.computeFragmentCount--;
			fxc_mp3.computeUnkName = DataUtils.ReadString(fxc);
			fxc_mp3.computeUnk1 = fxc.ReadByte();
			fxc_mp3.computeUnk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.computeFragment, fxc_mp3.computeFragmentCount);
			for (int a = 0; a < fxc_mp3.computeFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.computeFragment[a]);

			fxc_mp3.domainFragmentCount = fxc.ReadByte();
			fxc_mp3.domainFragmentCount--;
			fxc_mp3.domainUnkName = DataUtils.ReadString(fxc);
			fxc_mp3.domainUnk1 = fxc.ReadByte();
			fxc_mp3.domainUnk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.domainFragment, fxc_mp3.domainFragmentCount);
			for (int a = 0; a < fxc_mp3.domainFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.domainFragment[a]);

			fxc_mp3.geometryFragmentCount = fxc.ReadByte();
			fxc_mp3.geometryFragmentCount--;
			fxc_mp3.geometryUnkName = DataUtils.ReadString(fxc);
			fxc_mp3.geometryUnk1 = fxc.ReadByte();
			fxc_mp3.geometryUnk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.geometryFragment, fxc_mp3.geometryFragmentCount);
			for (int a = 0; a < fxc_mp3.geometryFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.geometryFragment[a]);

			fxc_mp3.hullFragmentCount = fxc.ReadByte();
			fxc_mp3.hullFragmentCount--;
			fxc_mp3.hullUnkName = DataUtils.ReadString(fxc);
			fxc_mp3.hullUnk1 = fxc.ReadByte();
			fxc_mp3.hullUnk2 = fxc.ReadUInt32();
			Array.Resize<Structures.FRAGMENT>(ref fxc_mp3.hullFragment, fxc_mp3.hullFragmentCount);
			for (int a = 0; a < fxc_mp3.hullFragmentCount; a++)
				ReadFRAGMENT_MP3(fxc, ref fxc_mp3.hullFragment[a]);
		
			fxc_mp3.unk0 = fxc.ReadByte();

			fxc_mp3.globalVariablesCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE_MP3>(ref fxc_mp3.globalVariable, fxc_mp3.globalVariablesCount);
			for (int a = 0; a < fxc_mp3.globalVariablesCount; a++)
				ReadVARIABLE_MP3(fxc, ref fxc_mp3.globalVariable[a]);

			fxc_mp3.unk1 = fxc.ReadByte();

			fxc_mp3.localVariablesCount = fxc.ReadByte();
			Array.Resize<Structures.VARIABLE_MP3>(ref fxc_mp3.localVariable, fxc_mp3.localVariablesCount);
			for (int a = 0; a < fxc_mp3.localVariablesCount; a++)
				ReadVARIABLE_MP3(fxc, ref fxc_mp3.localVariable[a]);

			fxc_mp3.techniquesCount = fxc.ReadByte();
			Array.Resize<Structures.TECHNIQUE_MP3>(ref fxc_mp3.technique, fxc_mp3.techniquesCount);
			for (int a = 0; a < fxc_mp3.techniquesCount; a++)
				ReadTECHNIQUE_MP3(fxc, ref fxc_mp3.technique[a]);

		}
	}
}
