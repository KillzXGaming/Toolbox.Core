using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.src.Textures.Decoders
{
    public class RGBAFormat
    {
        public int RedDepth;
        public int GreenDepth;
        public int BlueDepth;
        public int AlphaDepth;

        public int RedShift;
        public int GreenShift;
        public int BlueShift;
        public int AlphaShift;

        public RGBAFormat(int R, int B, int G, int A, int Rshift, int Gshift, int Bshift, int Ashift)
        {
            RedDepth = R;
            BlueDepth = B;
            GreenDepth = G;
            AlphaDepth = A;
            RedShift = Rshift;
            GreenShift = Gshift;
            BlueShift = Bshift;
            AlphaShift = Ashift;
        }
    }
}
