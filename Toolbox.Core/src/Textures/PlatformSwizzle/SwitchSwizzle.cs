using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.PlatformSwizzle
{
    public class SwitchSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGB8;
        public TexFormatType OutputFormatType { get; set; } = TexFormatType.Unorm;

        public int Target = 1; //Default or linear

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return Switch.TegraX1Swizzle.GetImageData(texture, data, array, mip, 1, Target, false);
        }
    }
}
