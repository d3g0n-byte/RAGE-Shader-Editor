using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderEditor
{
	internal class Settings
	{
		public static int game;
		public static int shaderExportModePC;
		public static int shaderExportModeXenon;
		public static int platform;
		
		public static void CheckShaderExport()
		{

			//if (File.Exists($"{System.Reflection.Assembly.GetEntryAssembly().Location}\\dlls\\fx.dll"))
			//	shaderExportModePC = 1;
			//else shaderExportModePC = 0;
			shaderExportModePC = 1;
            if (!File.Exists($"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\dlls\\fx.dll")) throw new Exception("fx.dll not found");
		}
	}
}
