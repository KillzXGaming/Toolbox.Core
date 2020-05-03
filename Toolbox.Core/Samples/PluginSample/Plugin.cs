using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core;

namespace PluginSample
{
    /// <summary>
    /// Represents a plugin used to interact with this tool.
    /// </summary>
    public class Plugin : IPlugin
    {
        public string Name => "Sample Plugin";
    }
}
