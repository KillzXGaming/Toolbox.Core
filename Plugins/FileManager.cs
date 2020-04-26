using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Toolbox.Core
{
    public class FileManager
    {
        public static ICompressionFormat[] GetCompressionFormats()
        {
            List<ICompressionFormat> types = new List<ICompressionFormat>();
            foreach (var plugin in PluginManager.LoadPlugins())
                types.AddRange(plugin.CompressionFormats);

            return types.ToArray();
        }

        public static IFileFormat[] GetFileFormats()
        {
            List<IFileFormat> types = new List<IFileFormat>();
            foreach (var plugin in PluginManager.LoadPlugins())
                types.AddRange(plugin.FileFormats);

            return types.ToArray();
        }


    }
}
