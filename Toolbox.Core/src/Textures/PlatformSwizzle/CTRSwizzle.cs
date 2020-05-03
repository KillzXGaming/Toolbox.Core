using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.PlatformSwizzle
{
    public class CTRSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGB8;
        public TexFormatType OutputFormatType { get; set; } = TexFormatType.Unorm;

        public CTR_3DS.Orientation SwizzleMode = CTR_3DS.Orientation.Default;
        public CTR_3DS.PICASurfaceFormat Format = CTR_3DS.PICASurfaceFormat.RGB8;

        public CTRSwizzle(CTR_3DS.PICASurfaceFormat format) {
            Format = format;
        }

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            var settings = new CTR_3DS.SwizzleSettings()
            {
                Orientation = SwizzleMode,
            };

            return CTR_3DS.DecodeBlock(data, (int)width, (int)height, Format, settings);
        }

        public byte[] EncodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return CTR_3DS.EncodeBlock(data, (int)width, (int)height, Format);
        }
    }
}
