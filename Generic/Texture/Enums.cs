using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public enum STCompressionMode
    {
        Slow,
        Normal,
        Fast
    }

    public enum STChannelType
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Alpha = 3,
        One = 4,
        Zero = 5,
    }

    public enum PlatformSwizzle
    {
        None = 0,
        Platform_3DS = 1,
        Platform_Wii = 2,
        Platform_Gamecube = 3,
        Platform_WiiU = 4,
        Platform_Switch = 5,
        Platform_Ps4 = 6,
        Platform_Ps3 = 7,
        Platform_Ps2 = 8,
        Platform_Ps1 = 9,
    }

    public enum STSurfaceType
    {
        Texture1D,
        Texture2D,
        Texture3D,
        TextureCube,
        Texture1D_Array,
        Texture2D_Array,
        Texture2D_Mulitsample,
        Texture2D_Multisample_Array,
        TextureCube_Array,
    }
}
