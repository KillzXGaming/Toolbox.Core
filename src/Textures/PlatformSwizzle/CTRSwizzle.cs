﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Imaging
{
    public class CTRSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGBA8_UNORM;

        public CTR_3DS.Orientation SwizzleMode = CTR_3DS.Orientation.Default;
        public CTR_3DS.PICASurfaceFormat Format = CTR_3DS.PICASurfaceFormat.RGB8;

        public CTRSwizzle(CTR_3DS.PICASurfaceFormat format) {
            Format = format;
        }

        public override string ToString() {
            return Format.ToString();
        }

        public byte[] DecodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip) {
            var settings = new CTR_3DS.SwizzleSettings()
            {
                Orientation = SwizzleMode,
            };

            try
            {
                return ImageUtility.ConvertBgraToRgba(CTR_3DS.DecodeBlock(data, (int)width, (int)height, Format, settings));
            }
            catch
            {
                return new byte[width * height * 4];
            }
        }

        public byte[] EncodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip) {
            return CTR_3DS.EncodeBlock(data, (int)width, (int)height, Format);
        }
    }
}
