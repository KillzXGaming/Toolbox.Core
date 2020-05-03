using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public enum PaletteFormat : uint
    {
        None,
        IA8,
        RGB565,
        RGB5A3,
    }

    public enum TexFormatType : uint
    {
        Unorm,
        Snorm,
        UnsignedFloat,
        SignedFloat,
        Srgb,
        Float,
        SInt,
        UInt,
    }

    //DXGI formats are first, then ASTC formats after
    public enum TexFormat : uint
    {
        UNKNOWN = 0,

        A8,
        R1,
        R8,
        R16,
        R32,
        RG8,
        RG4,
        RG16,
        RG32,
        RGB5,
        RGB8,
        RGB16,
        RGB32,
        RGB5A3,
        RGB565,
        RGBA4,
        RGBG8,
        RGB5A1,
        RGBA8,
        RGBA16,
        RGBA32,

        GRGB8,

        BGR5,
        BGRX8,
        BGR5A1,
        BGRA8,
        BGRA4,

        BC1,
        BC2,
        BC3,
        BC4,
        BC5,
        BC6,
        BC7,

        R32G8X24,
        RGB10A2,
        RG11B10,
        R24G8,
        X24G8,
        RGB9E5,

        D16,
        D32,
        D24S8,
        D24G8,

        AYUV,
        Y410,
        Y416,
        NV12,
        P010,
        P016,
        YUY2,
        Y210,
        Y216,
        NV11,
        AI44,
        IA44,
        P8,
        A8P8,
        P208,
        V208,
        V408,

        ASTC_4x4,
        ASTC_5x4,
        ASTC_5x5,
        ASTC_6x5,
        ASTC_6x6,
        ASTC_8x5,
        ASTC_8x6,
        ASTC_8x8,
        ASTC_10x5,
        ASTC_10x6,
        ASTC_10x8,
        ASTC_10x10,
        ASTC_12x10,
        ASTC_12x12,

        ETC1,
        ETC1_A4,

        L4,
        LA4,
        L8,
        LA8,
        HIL08,
        A4,
        I4,
        I8,
        IA4,
        IA8,
        C4,
        C8,
        C14X2,
        CMPR,
    }
}
