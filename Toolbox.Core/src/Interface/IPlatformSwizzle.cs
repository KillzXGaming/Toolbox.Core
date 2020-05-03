using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public interface IPlatformSwizzle
    {
        TexFormat OutputFormat { get; set; }
        TexFormatType OutputFormatType { get; set; }

        byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip);
    }
}
