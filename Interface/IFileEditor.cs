using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public interface IFileEditor
    {
        IFileFormat[] GetFileFormats();
    }
}
