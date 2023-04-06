using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShaderEditor
{
	internal class MP3
	{
		static void writeFragToXML(XmlWriter xmlWriter, Structures.FRAGMENT[] frag, byte fragCount, string fragName)
		{
			xmlWriter.WriteStartElement(fragName);
			for (int a = 0; a < fragCount; a++)
			{
				xmlWriter.WriteStartElement("Item");
				ShaderUtils.WriteShader(frag[a].shader, $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{frag[a].name}.fxc");
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
		static void writeVarToXML(XmlWriter xmlWriter, Structures.VARIABLE_MP3[] var, byte varCount, string nodeName)
		{
			xmlWriter.WriteStartElement(nodeName);
			for (int a = 0; a < varCount; a++)
			{
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteElementString("Type", DataUtils.TypeAsString(Convert.ToByte(var[a].type)));
				xmlWriter.WriteElementString("ArrayCount", $"{var[a].arrayCount}");
				xmlWriter.WriteElementString("Slot", $"{var[a].arrayCount}");
				xmlWriter.WriteElementString("Group", $"{var[a].arrayCount}");
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
		static void writeTeqToXML(XmlWriter xmlWriter, Structures.TECHNIQUE_MP3[] teq, byte teqCount, string nodeName, Structures.FRAGMENT[] vertFrag, Structures.FRAGMENT[] pixelFrag, Structures.FRAGMENT[] computeFrag, Structures.FRAGMENT[] domainFrag, Structures.FRAGMENT[] geometryFrag, Structures.FRAGMENT[] hullFrag)
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
					if (teq[a].pass[b].ps!=255) xmlWriter.WriteElementString("PixelShader", $"{pixelFrag[teq[a].pass[b].ps].name}");
					if (teq[a].pass[b].cs != 255) xmlWriter.WriteElementString("ComputeShader", $"{computeFrag[teq[a].pass[b].ps].name}");
					if (teq[a].pass[b].ds != 255) xmlWriter.WriteElementString("DomainShader", $"{domainFrag[teq[a].pass[b].ps].name}");
					if (teq[a].pass[b].gs != 255) xmlWriter.WriteElementString("GeometryShader", $"{geometryFrag[teq[a].pass[b].ps].name}");
					if (teq[a].pass[b].hs != 255) xmlWriter.WriteElementString("HullShader", $"{hullFrag[teq[a].pass[b].ps].name}");
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
		public static void GetFragVariablesData(ref Structures.FRAGMENT frag)
		{
			byte[] comment = frag.shader;
			MemoryStream stream = new MemoryStream(comment);
			BinaryReader br = new BinaryReader(stream);
			br.ReadUInt32();
			br.ReadUInt16();
			ushort size = br.ReadUInt16();
			size *= 4;
			stream.Close();
			br.Close();
			Array.Resize<byte>(ref comment, size+8);
			comment = comment.Skip(12).ToArray();
			stream = new MemoryStream(comment);
			br = new BinaryReader(stream);
			br.ReadUInt32();
			br.ReadUInt32();
			br.ReadUInt32();
			uint paramCount = br.ReadUInt32();
			br.ReadUInt32();
			br.ReadUInt32();
			br.ReadUInt32();
			string name;
			ushort type1;
			ushort type2;
			ushort type3;
			ushort type4;
			uint oldPos;
			ushort index;
			uint typeOffset;
			for (int a = 0; a < paramCount; a++)
			{
				name = DataUtils.ReadStringAtOffset(br.ReadUInt32(), br);
				br.ReadUInt16();
				index = br.ReadUInt16();
				br.ReadUInt32();
				typeOffset = br.ReadUInt32();
				oldPos = (uint)br.BaseStream.Position;
				br.BaseStream.Position = typeOffset;
				type1 = br.ReadUInt16();
				type2 = br.ReadUInt16();
				type3 = br.ReadUInt16();
				type4 = br.ReadUInt16();
				br.BaseStream.Position = oldPos;
				br.ReadUInt32();
				for (int b = 0; b < frag.variablesCount; b++)
				{
					if (frag.variable[b].name == name)
					{
						frag.variable[b].index = index;
						if (type1 == 0 && type2 == 2)
						{
							frag.variable[b].type = 1;
						}
						else if (type1 == 0 && type2 == 3 && type3 == 1 && type4 == 1)
						{
							frag.variable[b].type = 2;
						}
						else if (type1 == 1 && type2 == 3 && type4 == 2)
						{
							frag.variable[b].type = 3;
						}
						else if (type1 == 1 && type2 == 3 && type4 == 3)
						{
							frag.variable[b].type = 4;
						}
						else if (type1 == 1 && type2 == 3 && type4 == 4)
						{
							frag.variable[b].type = 5;
						}
						else if (type1 == 2 && type2 == 3 && type4 == 3)
						{
							frag.variable[b].type = 8;
						}
						else if (type1 == 2 && type2 == 3 && type4 == 4)
						{
							frag.variable[b].type = 9;
						}
						else if (type1 == 0 && type2 == 1)
						{
							frag.variable[b].type = 7;
						}
						else if (type1 == 4 && type2 == 12)
						{
							frag.variable[b].type = 6;
						}
						break;
					}
				}

			}

			stream.Close();
			br.Close();

		}
		public static void FXCToIV(BinaryReader br)
		{
			Settings.CheckShaderExport(); // проверяем есть ли dll для работы с шейдером
			Structures.FXC_MP3_SM3 fxc = new Structures.FXC_MP3_SM3();
			ReadStructures.ReadFXC_MP3(br, ref fxc);

			for (int a = 0; a < fxc.vertexFragmentCount; a++) GetFragVariablesData(ref fxc.vertexFragment[a]);
			for (int a = 0; a < fxc.pixelFragmentCount; a++) GetFragVariablesData(ref fxc.pixelFragment[a]);


		}
		public static void ReadFXC(BinaryReader br)
		{
			Settings.CheckShaderExport(); // проверяем есть ли dll для работы с шейдером
			Structures.FXC_MP3_SM3 fxc = new Structures.FXC_MP3_SM3();
			ReadStructures.ReadFXC_MP3(br, ref fxc);

			for (int a = 0; a < fxc.vertexFragmentCount; a++) GetFragVariablesData(ref fxc.vertexFragment[a]);
			for (int a = 0; a < fxc.pixelFragmentCount; a++) GetFragVariablesData(ref fxc.pixelFragment[a]);

			Settings.platform = (int)DataUtils.ePlatform.PC;
			Settings.shaderExportModePC = 0;
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			// other settings...
			//settings.Encoding = Encoding.ASCII;
			settings.IndentChars = ("\t");
			//settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.DoNotEscapeUriAttributes = true;
			settings.OmitXmlDeclaration = false;

			XmlWriter xmlWriter = XmlWriter.Create($"{FileInfo.filePath}\\{FileInfo.baseFileName}.xml", settings);
			xmlWriter.WriteStartElement("Effect");
			xmlWriter.WriteElementString("Magic", fxc.magic.ToString());
			xmlWriter.WriteElementString("Game", "MaxPayne3");
			xmlWriter.WriteElementString("Platform", "PC");
			xmlWriter.WriteElementString("VertexFormat", $"{fxc.vertexFormat}");
			xmlWriter.WriteStartElement("PresetParam");
			for (int a = 0; a < fxc.presetParamCount; a++)
			{
				
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("name", fxc.presetParam[a].name);
				switch (fxc.presetParam[a].valueType)
				{
					case 0:
						xmlWriter.WriteAttributeString("type", "int");
						xmlWriter.WriteAttributeString("value", $"{fxc.presetParam[a].value.intValue}");
						break;
					case 1:
						xmlWriter.WriteAttributeString("type", "float");
						xmlWriter.WriteAttributeString("value", $"{fxc.presetParam[a].value.floatValue}");
						break;
					case 2:
						xmlWriter.WriteAttributeString("type", "string");
						xmlWriter.WriteAttributeString("value", $"{fxc.presetParam[a].value.stringValue}");
						break;
				}
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Shaders");
			writeFragToXML(xmlWriter, fxc.vertexFragment, fxc.vertexFragmentCount, "VertexShaders");
			writeFragToXML(xmlWriter, fxc.pixelFragment, fxc.pixelFragmentCount, "PixelShaders");
			writeFragToXML(xmlWriter, fxc.computeFragment, fxc.computeFragmentCount, "ComputeShaders");
			writeFragToXML(xmlWriter, fxc.domainFragment, fxc.domainFragmentCount, "DomainShaders");
			writeFragToXML(xmlWriter, fxc.geometryFragment, fxc.geometryFragmentCount, "GeometryShaders");
			writeFragToXML(xmlWriter, fxc.hullFragment, fxc.hullFragmentCount, "HullShaders");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Variables");
			writeVarToXML(xmlWriter, fxc.globalVariable, fxc.globalVariablesCount, "GlobalVariables");
			writeVarToXML(xmlWriter, fxc.localVariable, fxc.localVariablesCount, "LocalVariables");
			xmlWriter.WriteEndElement();
			writeTeqToXML(xmlWriter, fxc.technique, fxc.techniquesCount, "Techniques", fxc.vertexFragment, fxc.pixelFragment, fxc.computeFragment, fxc.domainFragment, fxc.geometryFragment, fxc.hullFragment);
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();

		}
	}
}
