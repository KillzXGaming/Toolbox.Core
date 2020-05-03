using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.WiiU
{
    public class GX2TextureSwizzle
    {
        public static byte[] Decode(STGenericTexture texture, byte[] data, int ArrayLevel, int MipLevel)
        {
            uint bpp = TextureFormatHelper.GetBytesPerPixel(texture.Format);

            GX2.GX2Surface surf = new GX2.GX2Surface();
            surf.bpp = bpp;
            surf.height = texture.Height;
            surf.width = texture.Width;
            surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
            surf.alignment = 0;
            surf.depth = 1;
            surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
            surf.format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
            surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
            surf.pitch = 0;
            surf.data = data;
            surf.numMips = texture.MipCount;
            surf.mipOffset = new uint[0];
            surf.mipData = data;
            surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;

            //  surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
            surf.swizzle = 0;
            surf.numArray = 1;

            return GX2.Decode(surf, ArrayLevel, MipLevel);
        }

        Dictionary<GX2.GX2SurfaceFormat, FormatInfo> FormatList = new Dictionary<GX2.GX2SurfaceFormat, FormatInfo>()
        {
            { GX2.GX2SurfaceFormat.TCD_R16_UNORM, new FormatInfo(TexFormat.R16, TexFormatType.Unorm) },
            { GX2.GX2SurfaceFormat.TC_R16_FLOAT, new FormatInfo(TexFormat.R16, TexFormatType.Float) },
            { GX2.GX2SurfaceFormat.TC_R16_UINT, new FormatInfo(TexFormat.R16, TexFormatType.UInt) },
            { GX2.GX2SurfaceFormat.TC_R16_SINT, new FormatInfo(TexFormat.R16, TexFormatType.SInt) },
            { GX2.GX2SurfaceFormat.TC_R16_SNORM, new FormatInfo(TexFormat.R16, TexFormatType.Snorm) },
        };

        public class FormatInfo
        {
            public TexFormat Format;
            public TexFormatType Type;

            public FormatInfo(TexFormat format, TexFormatType type) {
                Format = format;
                Type = type;
            }
        }
    }
}
