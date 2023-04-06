using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ShaderEditor.Structures;

namespace ShaderEditor
{
	public static class IV
	{
		public static string tempstring;
		static void writeFragToXML(XmlWriter xmlWriter, Structures.FRAGMENT[] frag, byte fragCount, string fragName)
		{
			xmlWriter.WriteStartElement(fragName);
			for (int a = 0; a < fragCount; a++)
			{
				xmlWriter.WriteStartElement("Item");
				//tempstring = ShaderUtils.shaderExportMode < 1 ? "bytecode" : "asm";
				ShaderUtils.WriteShader(frag[a].shader, $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{frag[a].name}.fxc");
				//if (Settings.platform == (int)DataUtils.ePlatform.Xenon)
				//	if (Settings.shaderExportModeXenon == 0) tempstring = "bytecode";
				//	else tempstring = "asm";
				//else
				//	if (Settings.shaderExportModePC == 0) tempstring = "bytecode";
				//	else tempstring = "asm";
				//tempstring = "asm";
				xmlWriter.WriteElementString("Name", frag[a].name);
				xmlWriter.WriteElementString("DataType", $"{"asm"}");
				xmlWriter.WriteElementString("File", $"{FileInfo.baseFileName}\\{frag[a].name}.fxc");
				xmlWriter.WriteStartElement("Variables");
				for (int b = 0; b < frag[a].variablesCount; b++)
				{
					xmlWriter.WriteStartElement("Item");
					xmlWriter.WriteAttributeString("name", frag[a].variable[b].name);
					xmlWriter.WriteAttributeString("type", DataUtils.TypeAsString(Convert.ToByte(frag[a].variable[b].type)));
					xmlWriter.WriteAttributeString("index", $"{frag[a].variable[b].index}");
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();

		}
		static void writeVarToXML(XmlWriter xmlWriter, Structures.VARIABLE[] var, byte varCount, string nodeName)
		{
			xmlWriter.WriteStartElement(nodeName);
			for (int a = 0; a < varCount; a++)
			{
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteElementString("Type", DataUtils.TypeAsString(Convert.ToByte(var[a].type)));
				xmlWriter.WriteElementString("ArrayCount", $"{var[a].arrayCount}");
				xmlWriter.WriteElementString("Name1", var[a].name1);
				xmlWriter.WriteElementString("Name2", var[a].name2);
				xmlWriter.WriteStartElement("Annotation");
				for (int b = 0; b < var[a].annotationCount; b++)
				{
					xmlWriter.WriteStartElement("Item");
					xmlWriter.WriteAttributeString("name", var[a].annotation[b].name);
					switch (var[a].annotation[b].valueType)
					{
						case 0:
							xmlWriter.WriteAttributeString("type", "int");
							xmlWriter.WriteAttributeString("value", $"{var[a].annotation[b].value.intValue}");
							break;
						case 1:
							xmlWriter.WriteAttributeString("type", "float");
							xmlWriter.WriteAttributeString("value", $"{var[a].annotation[b].value.floatValue}");
							break;
						case 2:
							xmlWriter.WriteAttributeString("type", "string");
							xmlWriter.WriteAttributeString("value", $"{var[a].annotation[b].value.stringValue}");
							break;
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("Value");
				if (var[a].type == 6) // sampler
				{
					for (int b = 0; b < var[a].valueCount; b += 2)
					{
						xmlWriter.WriteString("\n");
						for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						xmlWriter.WriteString($"{var[a].value[b].intValue} {var[a].value[b + 1].intValue}");
						if (b == var[a].valueCount - 2)
						{
							xmlWriter.WriteString("\n");
							for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						}
					}
				}
				else if (var[a].type == 1 || var[a].type == 7)
				{
					for (int b = 0; b < var[a].valueCount; b++)
					{
						xmlWriter.WriteString("\n");
						for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						xmlWriter.WriteString($"{var[a].value[b].intValue}");
						if (b == var[a].valueCount - 1)
						{
							xmlWriter.WriteString("\n");
							for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						}
					}
				}
				else
				{
					for (int b = 0; b < var[a].valueCount; b++)
					{
						xmlWriter.WriteString("\n");
						for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						xmlWriter.WriteString($"{var[a].value[b].floatValue}");
						if (b == var[a].valueCount - 1)
						{
							xmlWriter.WriteString("\n");
							for (int c = 0; c < 4; c++) xmlWriter.WriteString(xmlWriter.Settings.IndentChars);
						}
					}
				}

				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
		static void writeTeqToXML(XmlWriter xmlWriter, Structures.TECHNIQUE[] teq, byte teqCount, string nodeName, Structures.FRAGMENT[] pixelFrag, Structures.FRAGMENT[] vertFrag)
		{
			xmlWriter.WriteStartElement(nodeName);
			for (int a = 0; a < teqCount; a++)
			{
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteElementString("Name", teq[a].name);
				xmlWriter.WriteStartElement("Passes");
				for (int b = 0; b < teq[a].passCount; b++)
				{
					xmlWriter.WriteStartElement("Item");
					xmlWriter.WriteElementString("VertexShader", $"{vertFrag[teq[a].pass[b].vs].name}");
					xmlWriter.WriteElementString("PixelShader", $"{pixelFrag[teq[a].pass[b].ps].name}");
					xmlWriter.WriteStartElement("Value");
					for (int c = 0; c < teq[a].pass[b].valueCount; c++)
					{
						xmlWriter.WriteStartElement("Item");
						xmlWriter.WriteAttributeString("type", $"{teq[a].pass[b].value[c].type}");
						xmlWriter.WriteAttributeString("value", $"{teq[a].pass[b].value[c].value}");
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
		public static void ReadFXC(BinaryReader br)
		{
			Settings.CheckShaderExport();
			Structures.FXC_IV fxc = new Structures.FXC_IV();
			ReadStructures.ReadFXC_IV(br, ref fxc);
			// проверяем платформу
			ShaderUtils.CheckPlatform_IV(fxc);
			// приступаем к записи шейдера в xml файл
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			// other settings...
			//settings.Encoding = Encoding.ASCII;
			settings.IndentChars = ("\t");
			//settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.DoNotEscapeUriAttributes = true;
			settings.OmitXmlDeclaration = false;

			XmlWriter xmlWriter = XmlWriter.Create($"{FileInfo.filePath}\\{FileInfo.baseFileName}.xml", settings);
			//XmlWriter xmlWriter = XmlWriter.Create("test.xml");

				xmlWriter.WriteStartDocument();
			//xmlWriter.WriteString("<?xml version=\"1.0\" encoding=\"utf-8\"?>");// XD
			xmlWriter.WriteStartElement("Effect");
			xmlWriter.WriteElementString("Magic", fxc.magic.ToString());
			xmlWriter.WriteElementString("Game", "IV");
			tempstring = Settings.platform == (int)DataUtils.ePlatform.Xenon ? "Xenon" : "PC";
			xmlWriter.WriteElementString("Platform", tempstring);
			xmlWriter.WriteStartElement("Shaders");

			writeFragToXML(xmlWriter, fxc.vertexFragment, fxc.vertexFragmentCount, "VertexShaders");
			writeFragToXML(xmlWriter, fxc.pixelFragment, fxc.pixelFragmentCount, "PixelShaders");

			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("Variables");

			writeVarToXML(xmlWriter, fxc.globalVariable, fxc.globalVariablesCount, "GlobalVariables");
			writeVarToXML(xmlWriter, fxc.localVariable, fxc.localVariablesCount, "LocalVariables");


			xmlWriter.WriteEndElement();

			writeTeqToXML(xmlWriter, fxc.technique, fxc.techniquesCount, "Techniques", fxc.pixelFragment, fxc.vertexFragment);

			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
		}
		public static string ReadStringValue(this XmlReader xml, string nodeName)
		{
			string value;
			xml.Read();
			if (xml.NodeType == XmlNodeType.Element|| xml.Name == nodeName)
			{
				xml.Read();
				value = xml.Value;
				xml.Read();
				if (xml.NodeType == XmlNodeType.EndElement || xml.Name == nodeName)
				{
					return value;
				}
				else return "";
			}
			return "";
		}
		public static void ReadStartElement(this XmlReader xml, string nodeName)
		{
			xml.Read();
			if (xml.NodeType == XmlNodeType.Element || xml.Name == nodeName)
			{
				return;
			}
			return;
		}
		public static void ReadEndElement(this XmlReader xml, string nodeName)
		{
			xml.Read();
			if (xml.NodeType == XmlNodeType.EndElement || xml.Name == nodeName)
			{
				return;
			}
			return;
		}
		static void SetArraySizes(ref Structures.FXC_IV fxc_iv)
		{
			int max = 255;
			Array.Resize<Structures.FRAGMENT>(ref fxc_iv.vertexFragment, max);
			for (int a = 0; a < max; a++)
				Array.Resize<Structures.SHADER_VARIABLE>(ref fxc_iv.vertexFragment[a].variable, max);
			Array.Resize<Structures.FRAGMENT>(ref fxc_iv.pixelFragment, max);
			for (int a = 0; a < max; a++)
				Array.Resize<Structures.SHADER_VARIABLE>(ref fxc_iv.pixelFragment[a].variable, max);

			Array.Resize<Structures.VARIABLE>(ref fxc_iv.globalVariable, max);
			for (int a = 0; a < max; a++)
			{
				Array.Resize<Structures.ANNOTATION>(ref fxc_iv.globalVariable[a].annotation, max);
				Array.Resize<Structures.VARIABLE_VALUE>(ref fxc_iv.globalVariable[a].value, max);
			}

			Array.Resize<Structures.VARIABLE>(ref fxc_iv.localVariable, max);
			for (int a = 0; a < max; a++)
			{
				Array.Resize<Structures.ANNOTATION>(ref fxc_iv.localVariable[a].annotation, max);
				Array.Resize<Structures.VARIABLE_VALUE>(ref fxc_iv.localVariable[a].value, max);
			}

			Array.Resize<Structures.TECHNIQUE>(ref fxc_iv.technique, max);
			for (int a = 0; a < max; a++)
			{
				Array.Resize<Structures.PASS>(ref fxc_iv.technique[a].pass, max);
				for (int b = 0; b < max; b++)
					Array.Resize<Structures.PASS_VALUE>(ref fxc_iv.technique[a].pass[b].value, max);
			}
		}
		static void ReadFrag(ref XmlReader xml, ref Structures.FRAGMENT[] frag, ref byte fragCount, string nodeName)
		{
			for (int a = 0; a < 255; a++)
			{
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
					if (xml.NodeType == XmlNodeType.EndElement || xml.Name == nodeName)
					{
						fragCount = (byte)a;
						Array.Resize<Structures.FRAGMENT>(ref frag, a);
						break;
					}
				frag[a].name = xml.ReadStringValue("Name");
				if (xml.ReadStringValue("DataType") != "asm") throw new Exception("This game only support shader assembly");

				//if (xml.ReadStringValue("DataType") == "asm")
				//{
					string shaderASM = File.ReadAllText($"{FileInfo.filePath}\\{xml.ReadStringValue("File")}");
					ShaderUtils.AssembleShader(shaderASM, ref frag[a].shader);
				//}
				//else if (xml.ReadStringValue("DataType") == "bytecode")
				//{
				//	frag[a].shader = File.ReadAllBytes($"{FileInfo.filePath}\\{xml.ReadStringValue("File")}");
				//}
				frag[a].shaderSize = frag[a].shaderSize2 = (ushort)frag[a].shader.Length;
				// variables
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Variables") { xml.Close(); return; }
				if (!xml.IsEmptyElement)
				{
					for (int b = 0; b < 255; b++)
					{
						xml.Read();
						if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
							if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "Variables")
							{
								frag[a].variablesCount = (byte)b;
								Array.Resize<Structures.SHADER_VARIABLE>(ref frag[a].variable, b);
								break;
							}
						frag[a].variable[b].type = DataUtils.TypeAsByte(xml.GetAttribute("type"));
						frag[a].variable[b].name = xml.GetAttribute("name");
						if (!ushort.TryParse(xml.GetAttribute("index"), out frag[a].variable[b].index))
						{
							throw new Exception($"{xml.Value} is not int");
						}
					}
				}
				else
				{
					frag[a].variablesCount = 0;
					Array.Resize<Structures.SHADER_VARIABLE>(ref frag[a].variable, 0);
				}
				ReadEndElement(xml, "Item");
			}
		}
		static void ReadVar(XmlReader xml,ref Structures.VARIABLE[] var, ref byte varCount, string nodeName)
		{
			for (int a = 0; a < 255; a++)
			{
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
					if (xml.NodeType == XmlNodeType.EndElement || xml.Name == nodeName)
					{
						varCount = (byte)a;
						Array.Resize<Structures.VARIABLE>(ref var, a);
						break;
					}
				var[a].type = DataUtils.TypeAsByte(xml.ReadStringValue("Type"));
				var[a].arrayCount = Convert.ToByte(xml.ReadStringValue("ArrayCount"));
				var[a].name1 = xml.ReadStringValue("Name1");
				var[a].name2 = xml.ReadStringValue("Name2");
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Annotation") { xml.Close(); return; }
				else
				{
					if (!xml.IsEmptyElement)
					{
						for (int b = 0; b < 255; b++)
						{
							xml.Read();
							if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
								if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "Annotation")
								{
									var[a].annotationCount = (byte)b;
									Array.Resize<Structures.ANNOTATION>(ref var[a].annotation, b);
									break;
								}
							var[a].annotation[b].name = xml.GetAttribute("name");
							var[a].annotation[b].valueType = DataUtils.AnnotationTypeAsByte(xml.GetAttribute("type"));
							if (var[a].annotation[b].valueType == 0) var[a].annotation[b].value.intValue = Convert.ToInt32(xml.GetAttribute("value"));
							else if (var[a].annotation[b].valueType == 1) var[a].annotation[b].value.floatValue = Convert.ToSingle(xml.GetAttribute("value"));
							else if (var[a].annotation[b].valueType == 2) var[a].annotation[b].value.stringValue = xml.GetAttribute("value");
						}
					}
					else
					{
						Array.Resize<Structures.ANNOTATION>(ref var[a].annotation, 0);
						var[a].annotationCount = 0;
					}
				}
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Value") { xml.Close(); return; }
				else
				{
					if (!xml.IsEmptyElement)
					{
						xml.Read();
						SplitSamplerValues(xml.Value, '\t', ref var[a].value,ref var[a].valueCount, var[a].type);
						xml.Read();
						if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Value") { xml.Close(); return; }
					}
					else
					{
						Array.Resize<Structures.VARIABLE_VALUE>(ref var[a].value, 0);
						var[a].valueCount = 0;
					}
				}
				ReadEndElement(xml, "Item");
			}
		}
		static void ReadTeq(XmlReader xml, ref Structures.TECHNIQUE[] teq, ref byte teqCount, string nodeName, Structures.FRAGMENT[] vertFrag, Structures.FRAGMENT[] pixFrag)
		{
			for (int a = 0; a < 255; a++)
			{
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
					if (xml.NodeType == XmlNodeType.EndElement || xml.Name == nodeName)
					{
						teqCount = (byte)a;
						Array.Resize<Structures.TECHNIQUE>(ref teq, a);
						break;
					}
				teq[a].name = ReadStringValue(xml, "Name");
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Passes") { xml.Close(); return; }
				else
				{
					for (int b = 0; b < 255; b++)
					{
						xml.Read();
						if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
							if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "Passes")
							{
								teq[a].passCount = (byte)b;
								Array.Resize<Structures.PASS>(ref teq[a].pass, b);
								break;
							}
						teq[a].pass[b].vs = GetTechniqueId(vertFrag, ReadStringValue(xml, "VertexShader"));
						teq[a].pass[b].ps = GetTechniqueId(pixFrag, ReadStringValue(xml, "PixelShader"));
						teq[a].pass[b].ps++;
						xml.Read();
						if (xml.NodeType != XmlNodeType.Element || xml.Name != "Value") { xml.Close(); return; }
						else
						{
							if (!xml.IsEmptyElement)
							{
								for (int c = 0; c < 255; c++)
								{
									xml.Read();
									if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
										if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "Value")
										{
											teq[a].pass[b].valueCount = (byte)c;
											Array.Resize<Structures.PASS_VALUE>(ref teq[a].pass[b].value, c);
											break;
										}
									teq[a].pass[b].value[c].type = Convert.ToUInt32(xml.GetAttribute("type"));
									teq[a].pass[b].value[c].value = Convert.ToUInt32(xml.GetAttribute("value"));
								}
							}
							else
							{
								teq[a].pass[b].valueCount = 0;
								Array.Resize<Structures.PASS_VALUE>(ref teq[a].pass[b].value, 0);
							}
						}
						ReadEndElement(xml, "Item");
					}
				}
				ReadEndElement(xml, "Item");
			}
		}
		static byte GetTechniqueId(Structures.FRAGMENT[] frag, string fragName)
		{
			for (byte a = 0; a < frag.Length; a++)
			{
				if (frag[a].name == fragName) return a;
			}
			throw new Exception($"{fragName} not found");
		}
		static void SplitSamplerValues(string values, char indentChars, ref Structures.VARIABLE_VALUE[] val, ref byte valCount, byte type)
		{
			values = values.Substring(1);
			string[] tmp = values.Split('\n');
			Array.Resize<string>(ref tmp, tmp.Length - 1);
			valCount = (byte)tmp.Length;
			Structures.VARIABLE_VALUE[] retVal = new Structures.VARIABLE_VALUE[tmp.Length * 2];
			if (type == 6)
			{
				int c = 0;
				for (int a = 0; a < tmp.Length; a++)
					for (int b = 0; b < 2; b++) retVal[c++].intValue = Convert.ToInt32(tmp[a].Split(' ')[b]);
				val = retVal;
				valCount *= 2;
			}
			else if (type == 1 || type == 7)
			{ for (int a = 0; a < tmp.Length; a++) val[a].intValue = Convert.ToInt32(tmp[a]); Array.Resize<Structures.VARIABLE_VALUE>(ref val, tmp.Length); }
			else
			{ for (int a = 0; a < tmp.Length; a++) val[a].floatValue = Convert.ToSingle(tmp[a]); Array.Resize<Structures.VARIABLE_VALUE>(ref val, tmp.Length); }
		}
		public static void FXC_MP3_TO_IV(string inPath, string outPath)
		{
			byte[] buffer;
			Console.WriteLine($"{inPath}");
			if (DataUtils.FileExists(inPath)) buffer = File.ReadAllBytes(inPath);
			else throw new Exception("File not found");
			MemoryStream ms = new MemoryStream(buffer);
			BinaryReader br = new BinaryReader(ms);

			Settings.CheckShaderExport(); // проверяем есть ли dll для работы с шейдером
			Structures.FXC_MP3_SM3 fxc_mp3 = new Structures.FXC_MP3_SM3();
			ReadStructures.ReadFXC_MP3(br, ref fxc_mp3);
			Structures.FXC_IV fxc_iv = new Structures.FXC_IV();
			ms.Close();
			br.Close();
			for (int a = 0; a < fxc_mp3.vertexFragmentCount; a++) MP3.GetFragVariablesData(ref fxc_mp3.vertexFragment[a]);
			for (int a = 0; a < fxc_mp3.pixelFragmentCount; a++) MP3.GetFragVariablesData(ref fxc_mp3.pixelFragment[a]);

			fxc_iv.magic = 1635280754;
			fxc_iv.vertexFragmentCount = fxc_mp3.vertexFragmentCount;
			fxc_iv.vertexFragment= fxc_mp3.vertexFragment;
			for (int a = 0; a < fxc_iv.vertexFragment.Length; a++) fxc_iv.vertexFragment[a].shaderSize2 = fxc_iv.vertexFragment[a].shaderSize;
			fxc_iv.pixelFragmentCount = fxc_mp3.pixelFragmentCount;
			fxc_iv.unk1 = fxc_mp3.pixelUnk1;
			fxc_iv.unk2 = fxc_mp3.pixelUnk2;
			fxc_iv.pixelFragment = fxc_mp3.pixelFragment;
			for (int a = 0; a < fxc_iv.pixelFragment.Length; a++) fxc_iv.pixelFragment[a].shaderSize2 = fxc_iv.pixelFragment[a].shaderSize;
			fxc_iv.globalVariablesCount = fxc_mp3.globalVariablesCount;
			Array.Resize<Structures.VARIABLE>(ref fxc_iv.globalVariable, fxc_iv.globalVariablesCount);
			for (int a = 0; a < fxc_iv.globalVariablesCount; a++)
			{
				fxc_iv.globalVariable[a].type = fxc_mp3.globalVariable[a].type;
				fxc_iv.globalVariable[a].arrayCount = fxc_mp3.globalVariable[a].arrayCount;
				fxc_iv.globalVariable[a].name1 = fxc_mp3.globalVariable[a].name1;
				fxc_iv.globalVariable[a].name2 = fxc_mp3.globalVariable[a].name2;
				fxc_iv.globalVariable[a].annotationCount = fxc_mp3.globalVariable[a].annotationCount;
				fxc_iv.globalVariable[a].annotation = fxc_mp3.globalVariable[a].annotation;
				fxc_iv.globalVariable[a].valueCount = fxc_mp3.globalVariable[a].valueCount;
				fxc_iv.globalVariable[a].value= fxc_mp3.globalVariable[a].value;
			}
			fxc_iv.localVariablesCount = fxc_mp3.localVariablesCount;
			Array.Resize<Structures.VARIABLE>(ref fxc_iv.localVariable, fxc_iv.localVariablesCount);
			for (int a = 0; a < fxc_iv.localVariablesCount; a++)
			{
				fxc_iv.localVariable[a].type = fxc_mp3.localVariable[a].type;
				fxc_iv.localVariable[a].arrayCount = fxc_mp3.localVariable[a].arrayCount;
				fxc_iv.localVariable[a].name1 = fxc_mp3.localVariable[a].name1;
				fxc_iv.localVariable[a].name2 = fxc_mp3.localVariable[a].name2;
				fxc_iv.localVariable[a].annotationCount = fxc_mp3.localVariable[a].annotationCount;
				fxc_iv.localVariable[a].annotation = fxc_mp3.localVariable[a].annotation;
				fxc_iv.localVariable[a].valueCount = fxc_mp3.localVariable[a].valueCount;
				fxc_iv.localVariable[a].value = fxc_mp3.localVariable[a].value;
			}
			fxc_iv.techniquesCount = fxc_mp3.techniquesCount;
			Array.Resize<Structures.TECHNIQUE>(ref fxc_iv.technique, fxc_iv.techniquesCount);
			for (int a = 0; a < fxc_iv.techniquesCount; a++)
			{
				fxc_iv.technique[a].name = fxc_mp3.technique[a].name;
				fxc_iv.technique[a].passCount = fxc_mp3.technique[a].passCount;
				Array.Resize<Structures.PASS>(ref fxc_iv.technique[a].pass, fxc_iv.technique[a].passCount);
				for (int b = 0; b < fxc_iv.technique[a].passCount; b++)
				{
					fxc_iv.technique[a].pass[b].vs = fxc_mp3.technique[a].pass[b].vs;
					fxc_iv.technique[a].pass[b].ps = fxc_mp3.technique[a].pass[b].ps;
					fxc_iv.technique[a].pass[b].ps++;
					fxc_iv.technique[a].pass[b].valueCount = fxc_mp3.technique[a].pass[b].valueCount;
					fxc_iv.technique[a].pass[b].value = fxc_mp3.technique[a].pass[b].value;
					//for (int c = 0; c < fxc_iv.technique[a].pass[b].valueCount; c++)
					//{
					//	fxc_iv.technique[a].pass[b].value[c] = fxc_mp3.technique[a].pass[b].value[c];
					//}
				}
			}
			// скопировано из ReadXML
			BinaryWriter bw = new BinaryWriter(File.Create(outPath));
			bw.Write(1635280754);
			bw.Write(fxc_iv.vertexFragmentCount);
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
			{
				bw.Write(fxc_iv.vertexFragment[a].variablesCount);
				for (int b = 0; b < fxc_iv.vertexFragment[a].variablesCount; b++)
				{
					bw.Write(fxc_iv.vertexFragment[a].variable[b].type);
					bw.Write(fxc_iv.vertexFragment[a].variable[b].index);
					DataUtils.WriteString(bw, fxc_iv.vertexFragment[a].variable[b].name);
				}
				bw.Write(fxc_iv.vertexFragment[a].shaderSize);
				bw.Write(fxc_iv.vertexFragment[a].shaderSize2);
				bw.Write(fxc_iv.vertexFragment[a].shader);
			}
			bw.Write((byte)(fxc_iv.pixelFragmentCount + 1));
			bw.Write(fxc_iv.unk1);
			bw.Write(fxc_iv.unk2);
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
			{
				bw.Write(fxc_iv.pixelFragment[a].variablesCount);
				for (int b = 0; b < fxc_iv.pixelFragment[a].variablesCount; b++)
				{
					bw.Write(fxc_iv.pixelFragment[a].variable[b].type);
					bw.Write(fxc_iv.pixelFragment[a].variable[b].index);
					DataUtils.WriteString(bw, fxc_iv.pixelFragment[a].variable[b].name);
				}
				bw.Write(fxc_iv.pixelFragment[a].shaderSize);
				bw.Write(fxc_iv.pixelFragment[a].shaderSize2);
				bw.Write(fxc_iv.pixelFragment[a].shader);
			}

			bw.Write(fxc_iv.globalVariablesCount);
			for (int a = 0; a < fxc_iv.globalVariablesCount; a++)
			{
				bw.Write(fxc_iv.globalVariable[a].type);
				bw.Write(fxc_iv.globalVariable[a].arrayCount);
				DataUtils.WriteString(bw, fxc_iv.globalVariable[a].name1);
				DataUtils.WriteString(bw, fxc_iv.globalVariable[a].name2);
				bw.Write(fxc_iv.globalVariable[a].annotationCount);
				for (int b = 0; b < fxc_iv.globalVariable[a].annotationCount; b++)
				{
					DataUtils.WriteString(bw, fxc_iv.globalVariable[a].annotation[b].name);
					bw.Write(fxc_iv.globalVariable[a].annotation[b].valueType);
					switch (fxc_iv.globalVariable[a].annotation[b].valueType)
					{
						case 0:
							bw.Write(fxc_iv.globalVariable[a].annotation[b].value.intValue);
							break;
						case 1:
							bw.Write(fxc_iv.globalVariable[a].annotation[b].value.floatValue);
							break;
						case 2:
							DataUtils.WriteString(bw, fxc_iv.globalVariable[a].annotation[b].value.stringValue);
							break;
					}
				}
				bw.Write(fxc_iv.globalVariable[a].valueCount);
				for (int b = 0; b < fxc_iv.globalVariable[a].valueCount; b++)
					if (fxc_iv.globalVariable[a].type == 0 || fxc_iv.globalVariable[a].type == 6 || fxc_iv.globalVariable[a].type == 7) bw.Write(fxc_iv.globalVariable[a].value[b].intValue);
					else bw.Write(fxc_iv.globalVariable[a].value[b].floatValue);
			}
			bw.Write(fxc_iv.localVariablesCount);
			for (int a = 0; a < fxc_iv.localVariablesCount; a++)
			{
				bw.Write(fxc_iv.localVariable[a].type);
				bw.Write(fxc_iv.localVariable[a].arrayCount);
				DataUtils.WriteString(bw, fxc_iv.localVariable[a].name1);
				DataUtils.WriteString(bw, fxc_iv.localVariable[a].name2);
				bw.Write(fxc_iv.localVariable[a].annotationCount);
				for (int b = 0; b < fxc_iv.localVariable[a].annotationCount; b++)
				{
					DataUtils.WriteString(bw, fxc_iv.localVariable[a].annotation[b].name);
					bw.Write(fxc_iv.localVariable[a].annotation[b].valueType);
					switch (fxc_iv.localVariable[a].annotation[b].valueType)
					{
						case 0:
							bw.Write(fxc_iv.localVariable[a].annotation[b].value.intValue);
							break;
						case 1:
							bw.Write(fxc_iv.localVariable[a].annotation[b].value.floatValue);
							break;
						case 2:
							DataUtils.WriteString(bw, fxc_iv.localVariable[a].annotation[b].value.stringValue);
							break;
					}
				}
				bw.Write(fxc_iv.localVariable[a].valueCount);
				for (int b = 0; b < fxc_iv.localVariable[a].valueCount; b++)
					if (fxc_iv.localVariable[a].type == 0 || fxc_iv.localVariable[a].type == 6 || fxc_iv.localVariable[a].type == 7) bw.Write(fxc_iv.localVariable[a].value[b].intValue);
					else bw.Write(fxc_iv.localVariable[a].value[b].floatValue);
			}
			bw.Write(fxc_iv.techniquesCount);
			for (int a = 0; a < fxc_iv.techniquesCount; a++)
			{
				DataUtils.WriteString(bw, fxc_iv.technique[a].name);
				bw.Write(fxc_iv.technique[a].passCount);
				for (int b = 0; b < fxc_iv.technique[a].passCount; b++)
				{
					bw.Write(fxc_iv.technique[a].pass[b].vs);
					bw.Write(fxc_iv.technique[a].pass[b].ps);
					bw.Write(fxc_iv.technique[a].pass[b].valueCount);
					for (int c = 0; c < fxc_iv.technique[a].pass[b].valueCount; c++)
					{
						bw.Write(fxc_iv.technique[a].pass[b].value[c].type);
						bw.Write(fxc_iv.technique[a].pass[b].value[c].value);
					}
				}
			}
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++) DataUtils.WriteString(bw, fxc_iv.vertexFragment[a].name);
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++) DataUtils.WriteString(bw, fxc_iv.pixelFragment[a].name);
			bw.Close();
		}

		public static void ReadXML(XmlReader xml)
		{
			Structures.FXC_IV fxc_iv = new Structures.FXC_IV();
			SetArraySizes(ref fxc_iv);

			//Structures.FRAGMENT[] vertFrag = new Structures.FRAGMENT[255];
			//Structures.SHADER_VARIABLE_IV[] vertFragVar = new Structures.SHADER_VARIABLE_IV[255];
		
			//Structures.FRAGMENT[] pixFrag = new Structures.FRAGMENT[] { };
			//Structures.SHADER_VARIABLE_IV[] pixFragVar = new Structures.SHADER_VARIABLE_IV[] { };
			Settings.platform = xml.ReadStringValue("Platform") == "Xenon" ? (int)DataUtils.ePlatform.Xenon : (int)DataUtils.ePlatform.PC;

			xml.Read();
			//if (xml.NodeType != XmlNodeType.Element || xml.Name != "Effect") { xml.Close(); return; }
			//xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "Shaders") { xml.Close(); return; }
			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "VertexShaders") { xml.Close(); return; }
			ReadFrag(ref xml, ref fxc_iv.vertexFragment, ref fxc_iv.vertexFragmentCount, "VextexShaders");
			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "PixelShaders") { xml.Close(); return; }
			ReadFrag(ref xml, ref fxc_iv.pixelFragment, ref fxc_iv.pixelFragmentCount, "PixelShaders");
			xml.Read();
			if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Shaders") { xml.Close(); return; }

			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "Variables") { xml.Close(); return; }
			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "GlobalVariables") { xml.Close(); return; }
			ReadVar(xml, ref fxc_iv.globalVariable, ref fxc_iv.globalVariablesCount, "GlobalVariables");
			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "LocalVariables") { xml.Close(); return; }
			ReadVar(xml, ref fxc_iv.localVariable, ref fxc_iv.localVariablesCount, "LocalVariables");
			xml.Read();
			if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Variables") { xml.Close(); return; }

			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "Techniques") { xml.Close(); return; }
			ReadTeq(xml, ref fxc_iv.technique, ref fxc_iv.techniquesCount, "Techniques", fxc_iv.vertexFragment, fxc_iv.pixelFragment);
			// дополняем шейдер чтобы он имел более уподобающий вид
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
			{
				ShaderUtils.BuildComments(ref fxc_iv.vertexFragment[a].shader, fxc_iv.vertexFragment[a].variable, fxc_iv.globalVariable, fxc_iv.localVariable);
				fxc_iv.vertexFragment[a].shaderSize = (ushort)fxc_iv.vertexFragment[a].shader.Length;
				fxc_iv.vertexFragment[a].shaderSize2 = (ushort)fxc_iv.vertexFragment[a].shader.Length;
			}
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
			{
				ShaderUtils.BuildComments(ref fxc_iv.pixelFragment[a].shader, fxc_iv.pixelFragment[a].variable, fxc_iv.globalVariable, fxc_iv.localVariable);
				fxc_iv.pixelFragment[a].shaderSize = (ushort)fxc_iv.pixelFragment[a].shader.Length;
				fxc_iv.pixelFragment[a].shaderSize2 = (ushort)fxc_iv.pixelFragment[a].shader.Length;
			}

			// пишем
			BinaryWriter bw = new BinaryWriter(File.Create($"{FileInfo.filePath}\\{FileInfo.baseFileName}.fxc"));
			bw.Write(1635280754);
			bw.Write(fxc_iv.vertexFragmentCount);
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++)
			{
				bw.Write(fxc_iv.vertexFragment[a].variablesCount);
				for (int b = 0; b < fxc_iv.vertexFragment[a].variablesCount; b++)
				{
					bw.Write(fxc_iv.vertexFragment[a].variable[b].type);
					bw.Write(fxc_iv.vertexFragment[a].variable[b].index);
					DataUtils.WriteString(bw, fxc_iv.vertexFragment[a].variable[b].name);
				}
				bw.Write(fxc_iv.vertexFragment[a].shaderSize);
				bw.Write(fxc_iv.vertexFragment[a].shaderSize2);
				bw.Write(fxc_iv.vertexFragment[a].shader);
			}
			bw.Write((byte)(fxc_iv.pixelFragmentCount+1));
			bw.Write(fxc_iv.unk1);
			bw.Write(fxc_iv.unk2);
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++)
			{
				bw.Write(fxc_iv.pixelFragment[a].variablesCount);
				for (int b = 0; b < fxc_iv.pixelFragment[a].variablesCount; b++)
				{
					bw.Write(fxc_iv.pixelFragment[a].variable[b].type);
					bw.Write(fxc_iv.pixelFragment[a].variable[b].index);
					DataUtils.WriteString(bw, fxc_iv.pixelFragment[a].variable[b].name);
				}
				bw.Write(fxc_iv.pixelFragment[a].shaderSize);
				bw.Write(fxc_iv.pixelFragment[a].shaderSize2);
				bw.Write(fxc_iv.pixelFragment[a].shader);
			}

			bw.Write(fxc_iv.globalVariablesCount);
			for (int a = 0; a < fxc_iv.globalVariablesCount; a++)
			{
				bw.Write(fxc_iv.globalVariable[a].type);
				bw.Write(fxc_iv.globalVariable[a].arrayCount);
				DataUtils.WriteString(bw, fxc_iv.globalVariable[a].name1);
				DataUtils.WriteString(bw, fxc_iv.globalVariable[a].name2);
				bw.Write(fxc_iv.globalVariable[a].annotationCount);
				for (int b = 0; b < fxc_iv.globalVariable[a].annotationCount; b++)
				{
					DataUtils.WriteString(bw, fxc_iv.globalVariable[a].annotation[b].name);
					bw.Write(fxc_iv.globalVariable[a].annotation[b].valueType);
					switch (fxc_iv.globalVariable[a].annotation[b].valueType)
					{
						case 0:
							bw.Write(fxc_iv.globalVariable[a].annotation[b].value.intValue);
							break;
						case 1:
							bw.Write(fxc_iv.globalVariable[a].annotation[b].value.floatValue);
							break;
						case 2:
							DataUtils.WriteString(bw, fxc_iv.globalVariable[a].annotation[b].value.stringValue);
							break;
					}
				}
				bw.Write(fxc_iv.globalVariable[a].valueCount);
				for (int b = 0; b < fxc_iv.globalVariable[a].valueCount; b++)
					if(fxc_iv.globalVariable[a].type==0|| fxc_iv.globalVariable[a].type == 6|| fxc_iv.globalVariable[a].type == 7) bw.Write(fxc_iv.globalVariable[a].value[b].intValue);
					else bw.Write(fxc_iv.globalVariable[a].value[b].floatValue);
			}
			bw.Write(fxc_iv.localVariablesCount);
			for (int a = 0; a < fxc_iv.localVariablesCount; a++)
			{
				bw.Write(fxc_iv.localVariable[a].type);
				bw.Write(fxc_iv.localVariable[a].arrayCount);
				DataUtils.WriteString(bw, fxc_iv.localVariable[a].name1);
				DataUtils.WriteString(bw, fxc_iv.localVariable[a].name2);
				bw.Write(fxc_iv.localVariable[a].annotationCount);
				for (int b = 0; b < fxc_iv.localVariable[a].annotationCount; b++)
				{
					DataUtils.WriteString(bw, fxc_iv.localVariable[a].annotation[b].name);
					bw.Write(fxc_iv.localVariable[a].annotation[b].valueType);
					switch (fxc_iv.localVariable[a].annotation[b].valueType)
					{
						case 0:
							bw.Write(fxc_iv.localVariable[a].annotation[b].value.intValue);
							break;
						case 1:
							bw.Write(fxc_iv.localVariable[a].annotation[b].value.floatValue);
							break;
						case 2:
							DataUtils.WriteString(bw, fxc_iv.localVariable[a].annotation[b].value.stringValue);
							break;
					}
				}
				bw.Write(fxc_iv.localVariable[a].valueCount);
				for (int b = 0; b < fxc_iv.localVariable[a].valueCount; b++)
					if (fxc_iv.localVariable[a].type == 0 || fxc_iv.localVariable[a].type == 6 || fxc_iv.localVariable[a].type == 7) bw.Write(fxc_iv.localVariable[a].value[b].intValue);
					else bw.Write(fxc_iv.localVariable[a].value[b].floatValue);
			}
			bw.Write(fxc_iv.techniquesCount);
			for (int a = 0; a < fxc_iv.techniquesCount; a++)
			{
				DataUtils.WriteString(bw, fxc_iv.technique[a].name);
				bw.Write(fxc_iv.technique[a].passCount);
				for (int b = 0; b < fxc_iv.technique[a].passCount; b++)
				{
					bw.Write(fxc_iv.technique[a].pass[b].vs);
					bw.Write(fxc_iv.technique[a].pass[b].ps);
					bw.Write(fxc_iv.technique[a].pass[b].valueCount);
					for (int c = 0; c < fxc_iv.technique[a].pass[b].valueCount; c++)
					{
						bw.Write(fxc_iv.technique[a].pass[b].value[c].type);
						bw.Write(fxc_iv.technique[a].pass[b].value[c].value);
					}
				}
			}
			for (int a = 0; a < fxc_iv.vertexFragmentCount; a++) DataUtils.WriteString(bw, fxc_iv.vertexFragment[a].name);
			for (int a = 0; a < fxc_iv.pixelFragmentCount; a++) DataUtils.WriteString(bw, fxc_iv.pixelFragment[a].name);
			bw.Close();
		}
	}
}
