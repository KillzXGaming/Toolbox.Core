using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Imaging
{
    public class GamecubeSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGBA8_UNORM;

        public Decode_Gamecube.TextureFormats Format = Decode_Gamecube.TextureFormats.RGBA32;
        public Decode_Gamecube.PaletteFormats PaletteFormat = Decode_Gamecube.PaletteFormats.RGB565;

        public override string ToString() {
            return $"{Format}" + (PaletteData.Length > 0 ? $"_p_{PaletteFormat}" : "");
        }

        public ushort[] PaletteData { get; set; }

        public GamecubeSwizzle() { }

        public GamecubeSwizzle(Decode_Gamecube.TextureFormats format)
        {
            Format = format;
            PaletteData = new ushort[0];
        }

        public GamecubeSwizzle(Decode_Gamecube.TextureFormats format, Decode_Gamecube.PaletteFormats paletteFormat) {
            Format = format;
            PaletteFormat = paletteFormat;
        }

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return Decode_Gamecube.DecodeData(data, PaletteData, width, height, Format, PaletteFormat);
        }

        public byte[] EncodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            var encoded = Decode_Gamecube.EncodeData(data, Format, PaletteFormat, (int)width, (int)height );
            PaletteData = encoded.Item2;
            return encoded.Item1;
        }
    }
}
