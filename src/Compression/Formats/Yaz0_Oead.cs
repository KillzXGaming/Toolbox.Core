using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrresTool
{
    public class Yaz0_Oead_Native
    {
        public const string DllName = "oead";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int yaz0_compress(IntPtr src, int size, IntPtr dst, uint data_alignment, int level);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int yaz0_decompress(IntPtr src, int size, IntPtr dst);
    }

    public class Yaz0_Oead
    {
        public static bool CanUse()
        {
            if (File.Exists("oead.dll") && Environment.Is64BitProcess && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return true;
            if (File.Exists("oead_32.dll") && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return true;
            if (File.Exists("oead.so") && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return true;

            return false;
        }

        public static byte[] Decompress(byte[] source)
        {
            uint decomp_size = (uint)(source[4] << 24 | source[5] << 16 | source[6] << 8 | source[7]);

            byte[] destination = new byte[decomp_size];
            GCHandle srcHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);

            int size = 0;

            try
            {
                size = Yaz0_Oead_Native.yaz0_decompress(
                    srcHandle.AddrOfPinnedObject(),
                    source.Length,
                    dstHandle.AddrOfPinnedObject());
            }
            finally
            {
                srcHandle.Free();
                dstHandle.Free();
            }

            byte[] result = new byte[size];
            Array.Copy(destination, result, result.Length);
            return result;
        }

        public static byte[] Compress(byte[] source, uint alignment = 0, int level = 9)
        {

            byte[] destination = new byte[source.Length];
            GCHandle srcHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);

            int size = 0;

            try
            {
                size = Yaz0_Oead_Native.yaz0_compress(
                    srcHandle.AddrOfPinnedObject(),
                    source.Length,
                    dstHandle.AddrOfPinnedObject(), alignment, level);
            }
            finally
            {
                srcHandle.Free();
                dstHandle.Free();
            }

            byte[] result = new byte[size];
            Array.Copy(destination, result, result.Length);
            return result;
        }
    }
}
