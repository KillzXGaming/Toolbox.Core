using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.PlatformSwizzle
{
    public class DefaultSwizzle : IPlatformSwizzle
    {
        /// <summary>
        /// The output <see cref="TexFormat"/> format of the image after being swizzled. 
        /// </summary>
        public TexFormat OutputFormat { get; set; } = TexFormat.RGB8;

        /// <summary>
        /// The <see cref="TexFormatType"/> type of the image. 
        /// </summary>
        public TexFormatType FormatType { get; set; } = TexFormatType.Unorm;

        public bool IsOuputRGBA8 => false;

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return data;
        }
    }
}
