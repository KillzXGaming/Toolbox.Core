using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Core.IO;
using System.Runtime.InteropServices;

namespace Toolbox.Core
{
    public class DDS : STGenericTexture, IFileFormat
    {
        public bool CanSave { get; set; } = true;

        public string[] Description { get; set; } = new string[] { "DDS" };
        public string[] Extension { get; set; } = new string[] { "*.dds" };

        public File_Info FileInfo { get; set; }

        public bool Identify(File_Info fileInfo, System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(4, "DDS ");
            }
        }

        #region Constants

        public const uint FOURCC_DXT1 = 0x31545844;
        public const uint FOURCC_DXT2 = 0x32545844;
        public const uint FOURCC_DXT3 = 0x33545844;
        public const uint FOURCC_DXT4 = 0x34545844;
        public const uint FOURCC_DXT5 = 0x35545844;
        public const uint FOURCC_ATI1 = 0x31495441;
        public const uint FOURCC_BC4U = 0x55344342;
        public const uint FOURCC_BC4S = 0x53344342;
        public const uint FOURCC_BC5U = 0x55354342;
        public const uint FOURCC_BC5S = 0x53354342;
        public const uint FOURCC_DX10 = 0x30315844;

        public const uint FOURCC_ATI2 = 0x32495441;
        public const uint FOURCC_RXGB = 0x42475852;

        // RGBA Masks
        private static int[] A1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x8000 };
        private static int[] X1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x0000 };
        private static int[] A4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0xF000 };
        private static int[] X4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0x0000 };
        private static int[] R5G6B5_MASKS = { 0xF800, 0x07E0, 0x001F, 0x0000 };
        private static int[] R8G8B8_MASKS = { 0xFF0000, 0x00FF00, 0x0000FF, 0x000000 };
        private static uint[] A8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
        private static int[] X8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0x00000000 };
        private static uint[] A8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000 };
        private static int[] X8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000 };

        private static int[] L8_MASKS = { 0x000000FF, 0x0000, };
        private static int[] A8L8_MASKS = { 0x000000FF, 0x0F00, };

        #endregion

        #region enums

        public enum CubemapFace
        {
            PosX,
            NegX,
            PosY,
            NegY,
            PosZ,
            NegZ
        }

        [Flags]
        public enum DDSD : uint
        {
            CAPS = 0x00000001,
            HEIGHT = 0x00000002,
            WIDTH = 0x00000004,
            PITCH = 0x00000008,
            PIXELFORMAT = 0x00001000,
            MIPMAPCOUNT = 0x00020000,
            LINEARSIZE = 0x00080000,
            DEPTH = 0x00800000
        }
        [Flags]
        public enum DDPF : uint
        {
            ALPHAPIXELS = 0x00000001,
            ALPHA = 0x00000002,
            FOURCC = 0x00000004,
            RGB = 0x00000040,
            YUV = 0x00000200,
            LUMINANCE = 0x00020000,
        }
        [Flags]
        public enum DDSCAPS : uint
        {
            COMPLEX = 0x00000008,
            TEXTURE = 0x00001000,
            MIPMAP = 0x00400000,
        }
        [Flags]
        public enum DDSCAPS2 : uint
        {
            CUBEMAP = 0x00000200,
            CUBEMAP_POSITIVEX = 0x00000400 | CUBEMAP,
            CUBEMAP_NEGATIVEX = 0x00000800 | CUBEMAP,
            CUBEMAP_POSITIVEY = 0x00001000 | CUBEMAP,
            CUBEMAP_NEGATIVEY = 0x00002000 | CUBEMAP,
            CUBEMAP_POSITIVEZ = 0x00004000 | CUBEMAP,
            CUBEMAP_NEGATIVEZ = 0x00008000 | CUBEMAP,
            CUBEMAP_ALLFACES = (CUBEMAP_POSITIVEX | CUBEMAP_NEGATIVEX |
                                  CUBEMAP_POSITIVEY | CUBEMAP_NEGATIVEY |
                                  CUBEMAP_POSITIVEZ | CUBEMAP_NEGATIVEZ),
            VOLUME = 0x00200000
        }

        #endregion

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Header
        {
            public Magic Magic = "DDS ";
            public uint Size = 0x7C;
            public uint Flags;
            public uint Height;
            public uint Width;
            public uint PitchOrLinearSize;
            public uint Depth;
            public uint MipCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public uint[] Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class DDSPFHeader
        {
            public uint Size = 0x20;
            public uint Flags;
            public uint FourCC;
            public uint RgbBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;
            public uint Caps1;
            public uint Caps2;
            public uint Caps3;
            public uint Caps4;
            public uint Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class DX10Header
        {
            public uint DxgiFormat;
            public uint ResourceDim;
            public uint MiscFlags1;
            public uint ArrayCount;
            public uint MiscFlags2;
        }

        public string FileName { get; set; }

        public Header MainHeader;
        public DDSPFHeader PfHeader;
        public DX10Header Dx10Header;

        public byte[] ImageData;

        public bool IsDX10 => PfHeader.FourCC == FOURCC_DX10;

        public bool IsCubeMap
        {
            get { return PfHeader.Caps2 == (uint)DDSCAPS2.CUBEMAP_ALLFACES; }
            set
            {
                if (value)
                    PfHeader.Caps2 = (uint)DDSCAPS2.CUBEMAP_ALLFACES;
                else
                    PfHeader.Caps2 = 0;
            }
        }

        public DDS()
        {
            MainHeader = new Header();
            PfHeader = new DDSPFHeader();
            Dx10Header = new DX10Header();
        }

        public DDS(uint width, uint height, uint depth, TexFormat format, List<byte[]> imageData) : base()
        {
            MainHeader.Width = width;
            MainHeader.Height = height;
            MainHeader.Depth = depth;
        }

        public DDS(string fileName) { Load(fileName); }

        public void ToGenericFormat()
        {
            TexFormatType formatType = TexFormatType.Unorm;
            TexFormat format = TexFormat.RGB8;

            if (IsDX10)
            {
                DXGI_FORMAT dxgiFormat = (DXGI_FORMAT)Dx10Header.DxgiFormat;
                string ddsformat = dxgiFormat.ToString();

                if (ddsformat.Contains("SRGB"))
                    formatType = TexFormatType.Srgb;
                if (ddsformat.Contains("SNORM"))
                    formatType = TexFormatType.Snorm;
                if (ddsformat.Contains("UF16"))
                    formatType = TexFormatType.UnsignedFloat;
                if (ddsformat.Contains("SF16"))
                    formatType = TexFormatType.SignedFloat;
                if (ddsformat.Contains("SINT"))
                    formatType = TexFormatType.SInt;
                if (ddsformat.Contains("UINT"))
                    formatType = TexFormatType.UInt;

                if (ddsformat.Contains("ASTC_4x4"))
                    format = TexFormat.ASTC_4x4;
                if (ddsformat.Contains("ASTC_5x4"))
                    format = TexFormat.ASTC_5x4;
                if (ddsformat.Contains("ASTC_5x4"))
                    format = TexFormat.ASTC_5x4;
                if (ddsformat.Contains("ASTC_5x5"))
                    format = TexFormat.ASTC_5x5;
                if (ddsformat.Contains("ASTC_6x5"))
                    format = TexFormat.ASTC_6x5;
                if (ddsformat.Contains("ASTC_6x6"))
                    format = TexFormat.ASTC_6x6;
                if (ddsformat.Contains("ASTC_8x5"))
                    format = TexFormat.ASTC_8x5;
                if (ddsformat.Contains("ASTC_8x6"))
                    format = TexFormat.ASTC_8x6;
                if (ddsformat.Contains("ASTC_8x8"))
                    format = TexFormat.ASTC_8x8;
                if (ddsformat.Contains("ASTC_8x6"))
                    format = TexFormat.ASTC_8x6;
                if (ddsformat.Contains("ASTC_10X10"))
                    format = TexFormat.ASTC_10x10;
                if (ddsformat.Contains("ASTC_10x5"))
                    format = TexFormat.ASTC_10x5;
                if (ddsformat.Contains("ASTC_10x6"))
                    format = TexFormat.ASTC_10x6;
                if (ddsformat.Contains("ASTC_10x8"))
                    format = TexFormat.ASTC_10x8;
                if (ddsformat.Contains("ASTC_12x10"))
                    format = TexFormat.ASTC_12x10;
                if (ddsformat.Contains("ASTC_12x12"))
                    format = TexFormat.ASTC_12x12;

                if (ddsformat.Contains("BC1"))
                    format = TexFormat.BC1;
                if (ddsformat.Contains("BC2"))
                    format = TexFormat.BC2;
                if (ddsformat.Contains("BC3"))
                    format = TexFormat.BC3;
                if (ddsformat.Contains("BC4"))
                    format = TexFormat.BC4;
                if (ddsformat.Contains("BC5"))
                    format = TexFormat.BC5;
                if (ddsformat.Contains("BC6"))
                    format = TexFormat.BC6;
                if (ddsformat.Contains("BC7"))
                    format = TexFormat.BC7;

                if (ddsformat.Contains("R8G8B8A8"))
                    format = TexFormat.RGB8;
                if (ddsformat.Contains("B4G4R4A4"))
                    format = TexFormat.BGRA4;
                if (ddsformat.Contains("B5G5R5A1"))
                    format = TexFormat.RGB5A1;
                if (ddsformat.Contains("B5G6R5"))
                    format = TexFormat.BGR5;
                if (ddsformat.Contains("B8G8R8A8"))
                    format = TexFormat.BGRA8;
                if (ddsformat.Contains("B5G6R5"))
                    format = TexFormat.RGB565;
                if (ddsformat.Contains("B8G8R8X8"))
                    format = TexFormat.BGRX8;

                if (ddsformat.Contains("B8G8R8X8"))
                    format = TexFormat.BGRX8;

                if (ddsformat.Contains("A8P8"))
                    format = TexFormat.A8P8;
                if (ddsformat.Contains("AYUV"))
                    format = TexFormat.AYUV;

                if (ddsformat.Contains("A8_UNORM"))
                    format = TexFormat.A8;
            }
            else
            {
                switch (PfHeader.FourCC)
                {
                    case FOURCC_DXT1:
                        format = TexFormat.BC1;
                        break;
                    case FOURCC_DXT2:
                    case FOURCC_DXT3:
                        format = TexFormat.BC2;
                        break;
                    case FOURCC_DXT4:
                    case FOURCC_DXT5:
                        format = TexFormat.BC3;
                        break;
                    case FOURCC_ATI1:
                    case FOURCC_BC4U:
                        format = TexFormat.BC4;
                        break;
                    case FOURCC_ATI2:
                    case FOURCC_BC5U:
                        format = TexFormat.BC5;
                        FormatType = TexFormatType.Unorm;
                        break;
                    case FOURCC_BC5S:
                        format = TexFormat.BC5;
                        FormatType = TexFormatType.Snorm;
                        break;
                    case FOURCC_RXGB:
                        format = TexFormat.RGBA8;
                        break;
                    default:
                        format = TexFormat.RGBA8;
                        break;
                }
            }

            Format = format;
            FormatType = formatType;
        }

        public void SetFlags(TexFormat format, bool isDX10, bool isCubemap)
        {

        }

        public void Load(string fileName)
        {
            FileName = fileName;
            Load(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public void Save(string fileName, List<Surface> surfaces)
        {
            List<byte[]> dataList = new List<byte[]>();
            foreach (var surface in surfaces)
                dataList.Add(ByteUtils.CombineArray(surface.mipmaps.ToArray()));
            ImageData = ByteUtils.CombineArray(dataList.ToArray());
            dataList.Clear();

            Save(fileName);
        }

        public void Save(string fileName)
        {
            Save(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        public void Load(Stream stream)
        {
            Name = FileInfo.FileName;
            using (var reader = new FileReader(stream))
            {
                MainHeader = reader.ReadStruct<Header>();
                PfHeader = reader.ReadStruct<DDSPFHeader>();
                if (IsDX10)
                    Dx10Header = reader.ReadStruct<DX10Header>();

                int Dx10Size = IsDX10 ? 20 : 0;

                reader.TemporarySeek((int)(4 + MainHeader.Size + Dx10Size), SeekOrigin.Begin);
                ImageData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

                ToGenericFormat();
                Width = MainHeader.Width;
                Height = MainHeader.Height;
                MipCount = MainHeader.MipCount;
                ArrayCount = IsCubeMap ? (uint)6 : 1;
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0) {
            return ImageData;
        }

        public void Save(Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.WriteStruct(MainHeader);
                writer.WriteStruct(PfHeader);
                if (IsDX10)
                    writer.WriteStruct(Dx10Header);

                writer.Write(ImageData);
            }
        }

        public List<byte[]> GetImageData()
        {
            return new List<byte[]>() { ImageData };
        }
    }
}