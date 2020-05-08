using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Imaging
{
    public class NitroSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGBA8_UNORM;

        public bool isColor0 = true;

        public Nitro.NitroTex.NitroTexFormat Format;

        public NitroSwizzle(Nitro.NitroTex.NitroTexFormat format, bool isColor0 = true) {
            Format = format;
        }

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return Nitro.NitroTex.DecodeTexture((int)width, (int)height, Format, data, texture.GetPaletteData(), isColor0);
        }
    }
}
