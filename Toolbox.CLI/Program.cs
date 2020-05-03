using System;
using System.IO;
using Toolbox.Core;
using Toolbox.Core.Hashes;
using Toolbox.Core.IO;

namespace Toolbox.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Runtime.ExecutableDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var plugins = PluginManager.LoadPlugins();
            foreach (var plugin in plugins)
            {
                Console.WriteLine($"Plugin {plugin.PluginHandler.Name}");

                foreach (var file in plugin.FileFormats)
                    Console.WriteLine($"file {file.Extension[0]}");
            }

            ExportImage("type_icon_00_normal.bflim");
            ExportImage("ParamName_00.bflim");
            ExportImage("sick_icon_01_mahi.bflim");

            Console.Read();
        }

        static void ExportImage(string fileName)
        {
            var bflim = STFileLoader.OpenFileFormat(fileName) as STGenericTexture;
            if (bflim == null) return;

            Console.WriteLine($"tex {bflim.Name} {bflim.Format} {bflim.Width} {bflim.Height}");
            bflim.GetBitmap().Save($"{bflim.Name}.png");
        }
    }
}
