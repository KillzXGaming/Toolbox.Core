using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public class TextureFormatHelper
    {
        private enum TargetBuffer
        {
            Color = 1,
            Depth = 2,
            Stencil = 3,
            DepthStencil = 4,
        }

        public static uint GetBlockWidth(TexFormat Format)
        {
            if (!FormatTable.ContainsKey(Format)) return 8;
            return FormatTable[Format].BlockWidth;
        }

        public static uint GetBlockHeight(TexFormat Format)
        {
            if (!FormatTable.ContainsKey(Format)) return 8;
            return FormatTable[Format].BlockHeight;
        }

        public static uint GetBytesPerPixel(TexFormat Format)
        {
            if (!FormatTable.ContainsKey(Format)) return 8;
            return FormatTable[Format].BytesPerPixel;
        }

        public static uint GetBlockDepth(TexFormat Format)
        {
            if (!FormatTable.ContainsKey(Format)) return 8;
            return FormatTable[Format].BlockDepth;
        }

        public static bool IsBCNCompressed(TexFormat Format)
        {
            switch (Format)
            {
                case TexFormat.BC1:
                case TexFormat.BC2:
                case TexFormat.BC3:
                case TexFormat.BC4:
                case TexFormat.BC5:
                case TexFormat.BC6:
                case TexFormat.BC7:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasFormatTableKey(TexFormat format) {
            return FormatTable.ContainsKey(format);
        }

        private static readonly Dictionary<TexFormat, FormatInfo> FormatTable =
                   new Dictionary<TexFormat, FormatInfo>()
         {
            { TexFormat.RGBA32,               new FormatInfo(16, 1,  1, 1, TargetBuffer.Color) },
            { TexFormat.RGB32,                new FormatInfo(8, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RGBA16,               new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RG32,                 new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RGBA8,                new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.R32G8X24,             new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RGBG8,                new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BGRX8,                new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BGR5A1,               new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RGB5A1,               new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BGRA8,                new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RGB5,                 new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },


            { TexFormat.RGB10A2,               new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.R32,                   new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BGRA4,                 new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RG16,                  new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RG8,                   new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.R16,                   new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.R8,                    new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RG4,                   new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.RG11B10,               new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BGR5,                  new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TexFormat.BC1,                   new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC2,                   new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC3,                   new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC4,                   new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC5,                   new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC6,                   new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.BC7,                   new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },

            { TexFormat.ASTC_4x4,              new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TexFormat.ASTC_5x4,              new FormatInfo(16, 5,  4, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_5x5,              new FormatInfo(16, 5,  5, 1,  TargetBuffer.Color) },
            { TexFormat.ASTC_6x5,              new FormatInfo(16, 6,  5, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_6x6,              new FormatInfo(16, 6,  6, 1,  TargetBuffer.Color) },
            { TexFormat.ASTC_8x5,              new FormatInfo(16, 8,  5,  1, TargetBuffer.Color) },
            { TexFormat.ASTC_8x6,              new FormatInfo(16, 8,  6, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_8x8,              new FormatInfo(16, 8,  8, 1,  TargetBuffer.Color) },
            { TexFormat.ASTC_10x5,             new FormatInfo(16, 10, 5, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_10x6,             new FormatInfo(16, 10, 6, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_10x8,             new FormatInfo(16, 10, 8, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_10x10,            new FormatInfo(16, 10, 10, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_12x10,            new FormatInfo(16, 12, 10, 1, TargetBuffer.Color) },
            { TexFormat.ASTC_12x12,            new FormatInfo(16, 12, 12, 1, TargetBuffer.Color) },
            { TexFormat.ETC1,                  new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.ETC1_A4,               new FormatInfo(8, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.HIL08,                 new FormatInfo(16, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.L4,                    new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.LA4,                   new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.L8,                    new FormatInfo(8, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.LA8,                   new FormatInfo(16, 1, 1, 1, TargetBuffer.Color) },
            { TexFormat.A4,                    new FormatInfo(4, 1,  1, 1, TargetBuffer.Color) },
            { TexFormat.A8,                    new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },

            { TexFormat.D16,                  new FormatInfo(2, 1, 1, 1, TargetBuffer.Depth) },
            { TexFormat.D24S8,                new FormatInfo(4, 1, 1, 1, TargetBuffer.Depth) },
            { TexFormat.D32,                  new FormatInfo(4, 1, 1, 1, TargetBuffer.Depth) },
            { TexFormat.I4,                   new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) },
            { TexFormat.I8,                   new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TexFormat.IA4,                  new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TexFormat.IA8,                  new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TexFormat.RGB565,               new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TexFormat.RGB5A3,               new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TexFormat.C4,                   new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) },
            { TexFormat.C8,                   new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TexFormat.C14X2,                new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TexFormat.CMPR,                 new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) }
        };

        class FormatInfo
        {
            public uint BytesPerPixel { get; private set; }
            public uint BlockWidth { get; private set; }
            public uint BlockHeight { get; private set; }
            public uint BlockDepth { get; private set; }

            public TargetBuffer TargetBuffer;

            public FormatInfo(uint bytesPerPixel, uint blockWidth, uint blockHeight, uint blockDepth, TargetBuffer targetBuffer)
            {
                BytesPerPixel = bytesPerPixel;
                BlockWidth = blockWidth;
                BlockHeight = blockHeight;
                BlockDepth = blockDepth;
                TargetBuffer = targetBuffer;
            }
        }
    }
}
