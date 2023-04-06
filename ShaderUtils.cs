using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderEditor
{
	public class ShaderUtils
	{
		//static string myDLLPath = $"{System.Reflection.Assembly.GetEntryAssembly().Location}\\dlls\\FX2.dll";
		[DllImport("dlls\\fx.dll", EntryPoint = "math_add", CallingConvention = CallingConvention.StdCall)]
		static extern int Add(int a, int b);

		[DllImport("dlls\\fx.dll", EntryPoint = "bytecode_to_asm", CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.BStr)]
		static extern int Disassemble(byte[] buf, int buflen, byte[] outShader, int[] newSize);

		[DllImport("dlls\\fx.dll", EntryPoint = "asm_to_bytecode", CallingConvention = CallingConvention.StdCall)]
		static extern int Assemble(string shader, uint shaderSize, byte[] pDestination, int[] DestSize, byte[] err, int[] errSize);

		//[DllImport("dlls\\fx.dll", EntryPoint = "get_asm_shader_size", CallingConvention = CallingConvention.StdCall)]
		//static extern int GetASMSize(string shader, uint shaderSize);

		[DllImport("dlls\\fx.dll", EntryPoint = "hlsl_to_bytecode", CallingConvention = CallingConvention.StdCall)]
		static extern int Compile(string shader, uint shaderSize, byte[] pDestination, int DestSize, string entryPoint, string profile);

		[DllImport("dlls\\fx.dll", EntryPoint = "get_hlsl_shader_size", CallingConvention = CallingConvention.StdCall)]
		static extern int GetHLSLSize(string shader, uint shaderSize, string entryPoint, string profile);

		int CompileHLSL(string inShader, ref byte[] pDestination, string entryPoint, string profile)
		{
			int size = GetHLSLSize(inShader, (uint)inShader.Length, entryPoint, profile);
			Array.Resize<byte>(ref pDestination, size);
			return Compile(inShader, (uint)inShader.Length, pDestination, pDestination.Length, entryPoint, profile);
		}
		public static void DisassembleShader(byte[] inShader, ref string outShader)
		{
			byte[] outShaderAsByte = new byte[50000];
			int[] newSize = new int[1];
			Disassemble(inShader, inShader.Length, outShaderAsByte, newSize);
			Array.Resize<byte>(ref outShaderAsByte, newSize[0]);

			//char[] sss = Convert.ToChar;
			//outShader = sss;
			outShader = System.Text.Encoding.Default.GetString(outShaderAsByte);

		}
		public static int AssembleShader(string inShader, ref byte[] outShader)
		{
			//int size = GetASMSize(inShader, (uint)inShader.Length);
			Array.Resize<byte>(ref outShader, 20000);
			//if (size == 0) return 0;
			byte[] error = new byte[500];
			int[] newSize = new int[1];
			int[] errorSize = new int[1];
			int e = 0;
			int result = Assemble(inShader, (uint)inShader.ToCharArray().Length, outShader, newSize, error, errorSize);

			if (result == 1)
			{
				Array.Resize<byte>(ref outShader, newSize[0]);
				return 1;
			}
			else
			{
				Array.Resize<byte>(ref error, errorSize[0]);
				//throw new Exception(System.Text.Encoding.Default.GetString(error));
				Console.WriteLine(System.Text.Encoding.Default.GetString(error));
				Console.ReadKey();
				return 0;
			}
		}
		public static int BuildComments(ref byte[] outShader, Structures.SHADER_VARIABLE[] shader_var, Structures.VARIABLE[] globalVariables, Structures.VARIABLE[] localVariables)
		{
			Structures.VARIABLE[] allVar = globalVariables.Concat(localVariables).ToArray();
			
			Structures.SM3 shader = new Structures.SM3();
			MemoryStream stream = new MemoryStream(outShader);
			BinaryReader br = new BinaryReader(stream);
			shader.version.version1 = br.ReadByte();
			shader.version.version2 = br.ReadByte();
			shader.shaderType = br.ReadUInt16();
			shader.bytecode = br.ReadBytes(outShader.Length - 4);
			stream.Close();
			br.Close();
			// а теперь дополняем шейдер
			shader._f4 = 65534;
			shader.CTAB = 1111577667;
			shader.comments._f0 = 28;
			shader.comments.creator = "Shvab's Shader Editor";
			shader.comments.version = shader.version;
			shader.comments.shaderType = shader.shaderType;
			shader.comments.paramCount = (uint)shader_var.Length;
			shader.comments._f10 = 28;
			shader.comments._f14 = 264;
			if (shader.comments.shaderType != 65535) shader.comments.profile = $"vs_{shader.comments.version.version2}_{shader.comments.version.version1}";
			else shader.comments.profile = $"ps_{shader.comments.version.version2}_{shader.comments.version.version1}";
			shader.comments.variables = new Structures.SM3_COMMENTS_VARIABLE[shader.comments.paramCount];
			for (int a = 0; a < shader.comments.paramCount; a++)
			{
				shader.comments.variables[a].name = shader_var[a].name;
				shader.comments.variables[a].index = shader_var[a].index;
				for (int b = 0; b < allVar.Length; b++)
				{
					if (allVar[b].name1 == shader.comments.variables[a].name)
					{
						if((allVar[b].type >= 2&& allVar[b].type <=5)|| (allVar[b].type >= 8 && allVar[b].type <= 9))
						{
							shader.comments.variables[a].registerType = 2;
							if (allVar[b].type < 6)
							{
								shader.comments.variables[a].types.type1 = allVar[b].type == 2 ? (ushort)0 : (ushort)1;
								shader.comments.variables[a].types.count2 = allVar[b].type == 2 ? (ushort)0 : (ushort)(allVar[b].type - 1);
								shader.comments.variables[a].size = allVar[b].arrayCount > 0 ? (ushort)(1 * allVar[b].arrayCount) : (ushort)1;
							}
							else
							{
								shader.comments.variables[a].types.type1 = 2;
								shader.comments.variables[a].types.count2 = (ushort)(allVar[b].type - 5);
								shader.comments.variables[a].size = allVar[b].arrayCount > 0 ? (ushort)((allVar[b].type - 5) * allVar[b].arrayCount) : (ushort)(allVar[b].type - 5);
							}
							shader.comments.variables[a].types.type2 = 3;
							shader.comments.variables[a].types.count1 = allVar[b].type < 6?(ushort)0: (ushort)(allVar[b].type - 5);
							shader.comments.variables[a].types.arrayCount = allVar[b].arrayCount;
							shader.comments.variables[a].types._fa = 0;
							shader.comments.variables[a].types._fc = 0;
						}
						else if(allVar[b].type == 6)
						{
							shader.comments.variables[a].registerType = 3;
							shader.comments.variables[a].types.type2 = 7;
							shader.comments.variables[a].types.count1 = 0;
							shader.comments.variables[a].types.arrayCount = 0;
							shader.comments.variables[a].types._fa = 0;
							shader.comments.variables[a].types._fc = 0;
							shader.comments.variables[a].size = allVar[b].arrayCount > 0 ? (ushort)(1 * allVar[b].arrayCount) : (ushort)1;
						}
						else if (allVar[b].type == 7)
						{
							shader.comments.variables[a].registerType = 0;
							shader.comments.variables[a].types.type2 = 1;
							shader.comments.variables[a].types.count1 = 0;
							shader.comments.variables[a].types.arrayCount = 0;
							shader.comments.variables[a].types._fa = 0;
							shader.comments.variables[a].types._fc = 0;
							shader.comments.variables[a].size = allVar[b].arrayCount > 0 ? (ushort)(1 * allVar[b].arrayCount) : (ushort)1;
						}
						else if (allVar[b].type == 1)
						{
							shader.comments.variables[a].registerType = 2;
							shader.comments.variables[a].types.type2 = 2;
							shader.comments.variables[a].types.count1 = 0;
							shader.comments.variables[a].types.arrayCount = 0;
							shader.comments.variables[a].types._fa = 0;
							shader.comments.variables[a].types._fc = 0;
							shader.comments.variables[a].size = allVar[b].arrayCount > 0 ? (ushort)(1 * allVar[b].arrayCount) : (ushort)1;
						}
					}
				}
				shader.comments.variables[a]._f14 = 0;
			}
			byte[] values = new byte[(shader.comments.paramCount * 20) + 28];
			byte[] trash = new byte[100* shader.comments.paramCount+100];
			for (int a = 0; a < trash.Length; a++) trash[a] = 171;
			uint posInValuesBuffer = 0;
			uint posTrashBuffer = 0;
			uint trashAlignVal = 4;
			DataUtils.WriteValueToByteArray(ref values, shader.comments._f0, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, (int)(posTrashBuffer + values.Length), ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref trash, shader.comments.creator, ref posTrashBuffer);
			DataUtils.Align(ref posTrashBuffer, trashAlignVal);// aa
			DataUtils.WriteValueToByteArray(ref values, shader.comments.version.version1, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, shader.comments.version.version2, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, shader.comments.shaderType, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, shader.comments.paramCount, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, shader.comments._f10, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, shader.comments._f14, ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref values, (int)(posTrashBuffer + values.Length), ref posInValuesBuffer);
			DataUtils.WriteValueToByteArray(ref trash, shader.comments.profile, ref posTrashBuffer);
			DataUtils.Align(ref posTrashBuffer, trashAlignVal);// aa
			for (int a = 0; a < shader.comments.paramCount; a++)
			{
				DataUtils.WriteValueToByteArray(ref values, (int)(posTrashBuffer + values.Length), ref posInValuesBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].name, ref posTrashBuffer);
				DataUtils.Align(ref posTrashBuffer, trashAlignVal);// aa
				DataUtils.WriteValueToByteArray(ref values, shader.comments.variables[a].registerType, ref posInValuesBuffer);
				DataUtils.WriteValueToByteArray(ref values, shader.comments.variables[a].index, ref posInValuesBuffer);
				DataUtils.WriteValueToByteArray(ref values, shader.comments.variables[a].size, ref posInValuesBuffer);
				DataUtils.WriteValueToByteArray(ref values, shader.comments.variables[a]._f0a, ref posInValuesBuffer);
				DataUtils.WriteValueToByteArray(ref values, (int)(posTrashBuffer + values.Length), ref posInValuesBuffer);
				
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types.type1, ref posTrashBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types.type2, ref posTrashBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types.count1, ref posTrashBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types.count2, ref posTrashBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types.arrayCount, ref posTrashBuffer);
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types._fa, ref posTrashBuffer);

				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types._fc, ref posTrashBuffer);//??
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types._fc, ref posTrashBuffer);//??
				DataUtils.WriteValueToByteArray(ref trash, shader.comments.variables[a].types._fc, ref posTrashBuffer);//??
				DataUtils.Align(ref posTrashBuffer, trashAlignVal);// aa

				DataUtils.WriteValueToByteArray(ref values, shader.comments.variables[a]._f14, ref posInValuesBuffer);
			}
			DataUtils.Align(ref posTrashBuffer, 4);
			Array.Resize<byte>(ref trash, (int)posTrashBuffer);
			shader.bytecodePosition = (ushort)(((values.Length + trash.Length) / 4)+1);
			outShader = new byte[values.Length + trash.Length + 12];
			uint posInOutShader = 0;
			DataUtils.WriteValueToByteArray(ref outShader, shader.version.version1, ref posInOutShader);
			DataUtils.WriteValueToByteArray(ref outShader, shader.version.version2, ref posInOutShader);
			DataUtils.WriteValueToByteArray(ref outShader, shader.shaderType, ref posInOutShader);
			DataUtils.WriteValueToByteArray(ref outShader, shader._f4, ref posInOutShader);
			DataUtils.WriteValueToByteArray(ref outShader, shader.bytecodePosition, ref posInOutShader);
			DataUtils.WriteValueToByteArray(ref outShader, shader.CTAB, ref posInOutShader);
			DataUtils.WriteToByteArray(ref outShader, values, ref posInOutShader);
			DataUtils.WriteToByteArray(ref outShader, trash, ref posInOutShader);
			Array.Resize<byte>(ref outShader, outShader.Length + shader.bytecode.Length);
			DataUtils.WriteToByteArray(ref outShader, shader.bytecode, ref posInOutShader);

			return 0;
			//return Assemble(inShader, (uint)inShader.Length, outShader, size);
		}

		public static void CheckPlatform_IV(Structures.FXC_IV fxc)
		/*
		Проверяем первый шейдер fxc файла.
		приоритет Xenon меньше, чем в PC, поэтому проверка не идеальная.
		Проверка состоит из проверки 4-х байтов в начале шейдера, которые совпадают только в однаковых профилях и платформах
		*/
		{
			int isPC = 0;
			int isXenon = 0;
			for (int a = 0; a < fxc.vertexFragmentCount; a++)
				if (fxc.vertexFragmentCount > 0)
					if (fxc.vertexFragment[0].shader[0] == 0x0 &&
						fxc.vertexFragment[0].shader[1] == 0x3 &&
						fxc.vertexFragment[0].shader[2] == 0xFE &&
						fxc.vertexFragment[0].shader[3] == 0xFF)
						isPC++;
					else
					if (fxc.vertexFragment[0].shader[0] == 0x10 &&
						fxc.vertexFragment[0].shader[1] == 0x2A &&
						fxc.vertexFragment[0].shader[2] == 0x11 &&
						fxc.vertexFragment[0].shader[3] == 0x01)
						throw new Exception("Unknown Shader");

			for (int a = 0; a < fxc.pixelFragmentCount; a++)
				if (fxc.pixelFragmentCount > 0)
					if (fxc.pixelFragment[0].shader[0] == 0x0 &&
						fxc.pixelFragment[0].shader[1] == 0x3 &&
						fxc.pixelFragment[0].shader[2] == 0xFF &&
						fxc.pixelFragment[0].shader[3] == 0xFF)
						isPC++;
					else
					if (fxc.pixelFragment[0].shader[0] == 0x10 &&
						fxc.pixelFragment[0].shader[1] == 0x2A &&
						fxc.pixelFragment[0].shader[2] == 0x11 &&
						fxc.pixelFragment[0].shader[3] == 0x00)
						throw new Exception("Unknown Shader");
			Settings.platform = (int)DataUtils.ePlatform.PC;
		}
		public static int CheckMagicMP3(BinaryReader br)
		// проверка предназначена для проверки игры по шейдеру
		// нужен стандартный v шейдер.
		{
			uint magic = br.ReadUInt32();
			uint vertexFlags = br.ReadUInt32();
			byte presetParamCount = br.ReadByte();
			byte tempstringSize;
			byte valueType;
			string tempstring;
			for (int a = 0; a < presetParamCount; a++)
			{
				tempstringSize = br.ReadByte();
				br.ReadBytes(tempstringSize);
				valueType = br.ReadByte();
				switch (valueType)
				{
					case 0:
						br.ReadInt32();
						break;
					case 1:
						br.ReadSingle();
						break;
					case 2:
						tempstringSize = br.ReadByte();
						br.ReadBytes(tempstringSize);
						break;
				}
			}
			byte vsCount = br.ReadByte();
			tempstring = DataUtils.ReadString(br);
			br.BaseStream.Position = 0;
			if (tempstring.Contains("NULL")) return 1;
			else return 2;
		}
		public static void WriteShader(byte[] shader, string filePath)
		// 0 - bytecode
		// 1 - asm
		{

			if (!Directory.Exists($"{FileInfo.filePath}\\{FileInfo.baseFileName}"))
				Directory.CreateDirectory($"{FileInfo.filePath}\\{FileInfo.baseFileName}");
			string shader1 = "";
			if (Settings.platform == (int)DataUtils.ePlatform.PC)
			{
				switch (Settings.shaderExportModePC)
				{
					case 0:
						File.WriteAllBytes(filePath, shader);
						break;
					case 1:
						//	File.WriteAllText(filePath, shaderASM);
						ShaderUtils.DisassembleShader(shader, ref shader1);
						File.WriteAllText(filePath, shader1);
						break;
				}
			}
			//else
			//{
			//	Settings.shaderExportModeXenon = 0;
			//	switch (Settings.shaderExportModeXenon)
			//	{
			//		case 0:
			//			File.WriteAllBytes(filePath, shader);
			//			break;
			//		case 1:
			//			//File.WriteAllText(filePath, Xenon.BytesToASM);
			//			break;
			//	}

			//}
		}
	}
}
