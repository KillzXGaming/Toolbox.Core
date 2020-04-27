using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public interface IExportableTexture
    {
        void Export(STGenericTexture texture, string filePath);
    }
}
