using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.PlatformSwizzle
{
    public class GamecubeSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGB8;
        public TexFormatType OutputFormatType { get; set; } = TexFormatType.Unorm;

        public Decode_Gamecube.TextureFormats Format = Decode_Gamecube.TextureFormats.RGBA32;
        public Decode_Gamecube.PaletteFormats PaletteFormat = Decode_Gamecube.PaletteFormats.RGB565;

        public ushort[] PaletteData { get; set; }

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
