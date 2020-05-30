using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.Switch;

namespace Toolbox.Core.Imaging
{
    public class SwitchSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGBA8_UNORM;

        public int BlockHeightLog2;

        public bool LinearMode = false;

        public int Target = 1; //Platform PC or NX

        public SwitchSwizzle(TexFormat format) {
            OutputFormat = format;
        }

        public override string ToString() {
            return OutputFormat.ToString();
        }

        public byte[] DecodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {

            if (BlockHeightLog2 == 0)
            {
                uint blkHeight = TextureFormatHelper.GetBlockHeight(OutputFormat);
                uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight));
                BlockHeightLog2 = (int)Convert.ToString(blockHeight, 2).Length - 1;
            }

            return Switch.TegraX1Swizzle.GetImageData(texture, data, array, mip, 0, (uint)BlockHeightLog2, Target, LinearMode);
        }

        public byte[] EncodeImage(STGenericTexture texture, byte[] data, uint width, uint height, int array, int mip) {
            return null;
        }
    }
}
