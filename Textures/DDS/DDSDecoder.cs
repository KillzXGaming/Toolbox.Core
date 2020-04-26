using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.TextureDecoding
{
    public class DDSDecoder
    {
        public static byte[] DecodeToRGBA8(byte[] input, int width, int height,
            TexFormat format, TexFormatType type)
        {
            bool isSRGB = type == TexFormatType.Srgb;
            bool isSNORM = type == TexFormatType.Snorm;
            bool isSignedFloat = type == TexFormatType.SignedFloat;
            bool isSignedInt = type == TexFormatType.SInt;

            switch (format)
            {
                case TexFormat.BC1:
                    return DXT.DecompressBC1(input, width, height, isSRGB);
                case TexFormat.BC2:
                    return DXT.DecompressBC2(input, width, height, isSRGB);
                case TexFormat.BC3:
                    return DXT.DecompressBC3(input, width, height, isSRGB);
                case TexFormat.BC4:
                    return DXT.DecompressBC4(input, width, height, isSNORM);
                case TexFormat.BC5:
                    return DXT.DecompressBC5(input, width, height, isSNORM);
                default:
                    return RGBAPixelDecoder.Decode(input, width, height, format);
            }
        }
    }
}
