using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public interface IExportableModel
    {
        void Export(STGenericModel model, string filePath);
    }
}
