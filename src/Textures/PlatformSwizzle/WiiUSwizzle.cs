using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.WiiU;

namespace Toolbox.Core.PlatformSwizzle
{
    public class WiiUSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGB8;

        public GX2.GX2AAMode AAMode { get; set; }
        public GX2.GX2TileMode TileMode { get; set; }
        public GX2.GX2RResourceFlags ResourceFlags { get; set; }
        public GX2.GX2SurfaceDimension SurfaceDimension { get; set; }
        public GX2.GX2SurfaceUse SurfaceUse { get; set; }
        public GX2.GX2SurfaceFormat Format { get; set; }

        public uint Pitch { get; set; }
        public uint Alignment { get; set; }
        public uint Swizzle { get; set; }

        public uint[] MipOffsets { get; set; }

        public byte[] MipData { get; set; }

        public WiiUSwizzle()
        {
            AAMode = GX2.GX2AAMode.GX2_AA_MODE_1X;
            TileMode = GX2.GX2TileMode.MODE_2D_TILED_THIN1;
            ResourceFlags = GX2.GX2RResourceFlags.GX2R_BIND_TEXTURE;
            SurfaceDimension = GX2.GX2SurfaceDimension.DIM_2D;
            SurfaceUse = GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
            Format = GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB;
            Alignment = 0;
            Pitch = 0;
        }

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {

            uint bpp = TextureFormatHelper.GetBytesPerPixel(OutputFormat);

            GX2.GX2Surface surf = new GX2.GX2Surface();
            surf.bpp = bpp;
            surf.height = texture.Height;
            surf.width = texture.Width;
            surf.depth = 1;
            surf.alignment = Alignment;
            surf.aa = (uint)AAMode;
            surf.dim = (uint)SurfaceDimension;
            surf.format = (uint)Format;
            surf.use = (uint)SurfaceUse;
            surf.pitch = Pitch;
            surf.data = data;
            surf.mipData = MipData != null ? MipData : data;
            surf.mipOffset = MipOffsets != null ? MipOffsets : new uint[0];
            surf.numMips = texture.MipCount;
            surf.numArray = texture.ArrayCount;
            surf.tileMode = (uint)TileMode;
            surf.swizzle = Swizzle;

            return GX2.Decode(surf, array, mip);
        }
    }
}
