using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.IO;
using Toolbox.Core.Imaging;

namespace Toolbox.Core
{
    public class CTR_3DS
    {
        //From https://github.com/gdkchan/SPICA/blob/42c4181e198b0fd34f0a567345ee7e75b54cb58b/SPICA/PICA/Converters/TextureConverter.cs

        public enum PICASurfaceFormat
        {
            RGBA8,
            RGB8,
            RGBA5551,
            RGB565,
            RGBA4,
            LA8,
            HiLo8,
            L8,
            A8,
            LA4,
            L4,
            A4,
            ETC1,
            ETC1A4
        }

        public static TexFormat ConvertPICAToGenericFormat(PICASurfaceFormat format)
        {
            switch (format)
            {
                case PICASurfaceFormat.RGB565: return TexFormat.RGB5;
                case PICASurfaceFormat.RGB8: return TexFormat.RGB8;
                case PICASurfaceFormat.RGBA8: return TexFormat.RGBA8;
                case PICASurfaceFormat.RGBA5551: return TexFormat.RGB5A1;
                case PICASurfaceFormat.RGBA4: return TexFormat.RGBA4;
                case PICASurfaceFormat.LA8: return TexFormat.LA8;
                case PICASurfaceFormat.HiLo8: return TexFormat.HIL08;
                case PICASurfaceFormat.L8: return TexFormat.L8;
                case PICASurfaceFormat.A8: return TexFormat.A8;
                case PICASurfaceFormat.LA4: return TexFormat.LA4;
                case PICASurfaceFormat.L4: return TexFormat.L4;
                case PICASurfaceFormat.A4: return TexFormat.A4;
                case PICASurfaceFormat.ETC1: return TexFormat.ETC1;
                case PICASurfaceFormat.ETC1A4: return TexFormat.ETC1_A4;
                default:
                    throw new NotImplementedException("Unsupported format! " + format);
            }
        }

        public static PICASurfaceFormat ConvertToPICAFormat(TexFormat GenericFormat)
        {
            switch (GenericFormat)
            {
                case TexFormat.RGB565: return PICASurfaceFormat.RGB565;
                case TexFormat.RG8: return PICASurfaceFormat.RGB8;
                case TexFormat.RGB8: return PICASurfaceFormat.RGBA8;
                case TexFormat.RGB5A1: return PICASurfaceFormat.RGBA5551;
                case TexFormat.RGBA4: return PICASurfaceFormat.RGBA4;
                case TexFormat.LA8: return PICASurfaceFormat.LA8;
                case TexFormat.HIL08: return PICASurfaceFormat.HiLo8;
                case TexFormat.L8: return PICASurfaceFormat.L8;
                case TexFormat.A8: return PICASurfaceFormat.A8;
                case TexFormat.LA4: return PICASurfaceFormat.LA4;
                case TexFormat.L4: return PICASurfaceFormat.L4;
                case TexFormat.A4: return PICASurfaceFormat.A4;
                case TexFormat.ETC1: return PICASurfaceFormat.ETC1;
                case TexFormat.ETC1_A4: return PICASurfaceFormat.ETC1A4;
                default:
                    throw new NotImplementedException("Unsupported format! " + GenericFormat);
            }
        }

        private static int[] FmtBPP = new int[] { 32, 24, 16, 16, 16, 16, 16, 8, 8, 8, 4, 4, 4, 8 };

        public static int[] SwizzleLUT =
{
             0,  1,  8,  9,  2,  3, 10, 11,
            16, 17, 24, 25, 18, 19, 26, 27,
             4,  5, 12, 13,  6,  7, 14, 15,
            20, 21, 28, 29, 22, 23, 30, 31,
            32, 33, 40, 41, 34, 35, 42, 43,
            48, 49, 56, 57, 50, 51, 58, 59,
            36, 37, 44, 45, 38, 39, 46, 47,
            52, 53, 60, 61, 54, 55, 62, 63
        };

        public static System.Drawing.Bitmap DecodeBlockToBitmap(byte[] Input, int Width, int Height, PICASurfaceFormat picaFormat)
        {
            return BitmapExtension.CreateBitmap(ImageUtility.ConvertBgraToRgba(DecodeBlock(Input, Width, Height, picaFormat)),
                  Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public static byte[] DecodeBlock(byte[] Input, int Width, int Height, TexFormat Format)
        {
            return DecodeBlock(Input, Width, Height, ConvertToPICAFormat(Format));
        }

        public static byte[] DecodeBlock(byte[] Input, int Width, int Height, PICASurfaceFormat picaFormat)
        {
            if (picaFormat == PICASurfaceFormat.ETC1 || picaFormat == PICASurfaceFormat.ETC1A4)
                return FlipVertical(Width, Height, ETC1.ETC1Decompress(Input, Width, Height, picaFormat == PICASurfaceFormat.ETC1A4));

            byte[] Output = new byte[Width * Height * 4];

            int Increment = FmtBPP[(int)picaFormat] / 8;
            if (Increment == 0) Increment = 1;

            int IOffset = 0;
            for (int TY = 0; TY < Height; TY += 8)
            {
                for (int TX = 0; TX < Width; TX += 8)
                {
                    for (int Px = 0; Px < 64; Px++)
                    {
                        int X = SwizzleLUT[Px] & 7;
                        int Y = (SwizzleLUT[Px] - X) >> 3;

                        int OOffet = (TX + X + ((Height - 1 - (TY + Y)) * Width)) * 4;

                        switch (picaFormat)
                        {
                            case PICASurfaceFormat.RGBA8:
                                Output[OOffet + 0] = Input[IOffset + 3];
                                Output[OOffet + 1] = Input[IOffset + 2];
                                Output[OOffet + 2] = Input[IOffset + 1];
                                Output[OOffet + 3] = Input[IOffset + 0];
                                break;
                            case PICASurfaceFormat.RGB8:
                                Output[OOffet + 0] = Input[IOffset + 2];
                                Output[OOffet + 1] = Input[IOffset + 1];
                                Output[OOffet + 2] = Input[IOffset + 0];
                                Output[OOffet + 3] = 0xff;
                                break;
                            case PICASurfaceFormat.RGBA5551:
                                DecodeRGBA5551(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case PICASurfaceFormat.RGB565:
                                DecodeRGB565(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case PICASurfaceFormat.RGBA4:
                                DecodeRGBA4(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case PICASurfaceFormat.LA8:
                                Output[OOffet + 0] = Input[IOffset + 1];
                                Output[OOffet + 1] = Input[IOffset + 1];
                                Output[OOffet + 2] = Input[IOffset + 1];
                                Output[OOffet + 3] = Input[IOffset + 0];
                                break;
                            case PICASurfaceFormat.HiLo8:
                                Output[OOffet + 0] = Input[IOffset + 1];
                                Output[OOffet + 1] = Input[IOffset + 0];
                                Output[OOffet + 2] = 0;
                                Output[OOffet + 3] = 0xff;
                                break;
                            case PICASurfaceFormat.L8:
                                Output[OOffet + 0] = Input[IOffset];
                                Output[OOffet + 1] = Input[IOffset];
                                Output[OOffet + 2] = Input[IOffset];
                                Output[OOffet + 3] = 0xff;
                                break;
                            case PICASurfaceFormat.A8:
                                Output[OOffet + 0] = 0xff;
                                Output[OOffet + 1] = 0xff;
                                Output[OOffet + 2] = 0xff;
                                Output[OOffet + 3] = Input[IOffset];
                                break;
                            case PICASurfaceFormat.LA4:
                                Output[OOffet + 0] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 1] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 2] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 3] = (byte)((Input[IOffset] << 4) | (Input[IOffset] & 0x0f));
                                break;
                            case PICASurfaceFormat.L4:
                                int L = (Input[IOffset >> 1] >> ((IOffset & 1) << 2)) & 0xf;
                                Output[OOffet + 0] = (byte)((L << 4) | L);
                                Output[OOffet + 1] = (byte)((L << 4) | L);
                                Output[OOffet + 2] = (byte)((L << 4) | L);
                                Output[OOffet + 3] = 0xff;
                                break;
                            case PICASurfaceFormat.A4:
                                int A = (Input[IOffset >> 1] >> ((IOffset & 1) << 2)) & 0xf;
                                Output[OOffet + 0] = 0xff;
                                Output[OOffet + 1] = 0xff;
                                Output[OOffet + 2] = 0xff;
                                Output[OOffet + 3] = (byte)((A << 4) | A);
                                break;
                        }

                        IOffset += Increment;
                    }
                }
            }

            return FlipVertical(Width, Height, Output);
        }

        private static byte[] FlipVertical(int Width, int Height, byte[] Input)
        {
            byte[] FlippedOutput = new byte[Width * Height * 4];

            int Stride = Width * 4;
            for (int Y = 0; Y < Height; Y++)
            {
                int IOffs = Stride * Y;
                int OOffs = Stride * (Height - 1 - Y);

                for (int X = 0; X < Width; X++)
                {
                    FlippedOutput[OOffs + 0] = Input[IOffs + 0];
                    FlippedOutput[OOffs + 1] = Input[IOffs + 1];
                    FlippedOutput[OOffs + 2] = Input[IOffs + 2];
                    FlippedOutput[OOffs + 3] = Input[IOffs + 3];

                    IOffs += 4;
                    OOffs += 4;
                }
            }
            return FlippedOutput;
        }

        public static byte[] EncodeBlock(byte[] Input, int Width, int Height, TexFormat Format)
        {
            return EncodeBlock(Input, Width, Height, ConvertToPICAFormat(Format));
        }

        //Much help from encoding thanks to this
        // https://github.com/Cruel/3dstex/blob/master/src/Encoder.cpp
        public static byte[] EncodeBlock(byte[] Input, int Width, int Height, PICASurfaceFormat PicaFormat)
        {
            int ImageSize = CalculateLength(Width, Height, PicaFormat);

            if (PicaFormat == PICASurfaceFormat.ETC1)
                return SmashForge.RG_ETC1.encodeETC(BitmapExtension.CreateBitmap(Input, Width, Height));
            else if (PicaFormat == PICASurfaceFormat.ETC1A4)
                return SmashForge.RG_ETC1.encodeETCa4(BitmapExtension.CreateBitmap(Input, Width, Height));

            var mem = new System.IO.MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                for (int TY = 0; TY < Height; TY += 8)
                {
                    for (int TX = 0; TX < Width; TX += 8)
                    {
                        for (int Px = 0; Px < 64; Px++)
                        {
                            int X = SwizzleLUT[Px] & 7;
                            int Y = (SwizzleLUT[Px] - X) >> 3;

                            int IOffs = (TX + X + ((TY + Y) * Width)) * 4;
                            if (PicaFormat == PICASurfaceFormat.RGBA8)
                            {
                                writer.Write(Input[IOffs + 3]);
                                writer.Write(Input[IOffs + 0]);
                                writer.Write(Input[IOffs + 1]);
                                writer.Write(Input[IOffs + 2]);
                            }
                            else if (PicaFormat == PICASurfaceFormat.RGB8)
                            {
                                writer.Write(Input[IOffs + 0]);
                                writer.Write(Input[IOffs + 1]);
                                writer.Write(Input[IOffs + 2]);
                            }
                            else if (PicaFormat == PICASurfaceFormat.A8)
                            {
                                writer.Write(Input[IOffs]);
                            }
                            else if (PicaFormat == PICASurfaceFormat.L8)
                            {
                                writer.Write(ConvertBRG8ToL(
                                    new byte[]
                                    {
                                            Input[IOffs + 0],
                                            Input[IOffs + 1],
                                            Input[IOffs + 2]
                                    }));
                            }
                            else if (PicaFormat == PICASurfaceFormat.LA8)
                            {
                                writer.Write(Input[IOffs + 3]);
                                writer.Write(ConvertBRG8ToL(
                                    new byte[]
                                    {
                                            Input[IOffs + 0],
                                            Input[IOffs + 1],
                                            Input[IOffs + 2]
                                    }));
                            }
                            else if (PicaFormat == PICASurfaceFormat.RGB565)
                            {
                                ushort R = (ushort)(Convert8To5(Input[IOffs + 0]));
                                ushort G = (ushort)(Convert8To6(Input[IOffs + 1]) << 5);
                                ushort B = (ushort)(Convert8To5(Input[IOffs + 2]) << 11);

                                writer.Write((ushort)(R | G | B));
                            }
                            else if (PicaFormat == PICASurfaceFormat.RGBA4)
                            {
                                ushort R = (ushort)(Convert8To4(Input[IOffs]) << 4);
                                ushort G = (ushort)(Convert8To4(Input[IOffs + 1]) << 8);
                                ushort B = (ushort)(Convert8To4(Input[IOffs + 2]) << 12);
                                ushort A = (ushort)(Convert8To4(Input[IOffs + 3]));

                                writer.Write((ushort)(R | G | B | A));
                            }
                            else if (PicaFormat == PICASurfaceFormat.RGBA5551)
                            {
                                ushort R = (ushort)(Convert8To5(Input[IOffs + 0]) << 1);
                                ushort G = (ushort)(Convert8To5(Input[IOffs + 1]) << 6);
                                ushort B = (ushort)(Convert8To5(Input[IOffs + 2]) << 11);
                                ushort A = (ushort)(Convert8To1(Input[IOffs + 3]));

                                writer.Write((ushort)(R | G | B | A));
                            }
                            else if (PicaFormat == PICASurfaceFormat.LA4)
                            {
                                byte A = Input[IOffs + 3];
                                byte L = ConvertBRG8ToL(
                                    new byte[]
                                    {
                                            Input[IOffs + 0],
                                            Input[IOffs + 1],
                                            Input[IOffs + 2]
                                    });
                                writer.Write((byte)((A >> 4) | (L & 0xF0)));
                            }
                            else if (PicaFormat == PICASurfaceFormat.L4)
                            {
                                //Skip alpha channel
                                byte L1 = ConvertBRG8ToL(
                                    new byte[]
                                    {
                                            Input[IOffs + 0],
                                            Input[IOffs + 1],
                                            Input[IOffs + 2]
                                    });
                                byte L2 = ConvertBRG8ToL(
                                    new byte[]
                                    {
                                                Input[IOffs + 4],
                                                Input[IOffs + 5],
                                                Input[IOffs + 6]
                                    });

                                writer.Write((byte)((L1 >> 4) | (L2 & 0xF0)));
                                Px++;
                            }
                            else if (PicaFormat == PICASurfaceFormat.A4)
                            {
                                byte A1 = (byte)(Input[IOffs + 3] >> 4);
                                byte A2 = (byte)(Input[IOffs + 7] & 0xF0);
                                writer.Write((byte)(A1 | A2));
                                Px++;
                            }
                            else if (PicaFormat == PICASurfaceFormat.HiLo8)
                            {
                                writer.Write(Input[IOffs]);
                                writer.Write(Input[IOffs + 1]);
                            }
                        }
                    }
                }
            }

            byte[] newOutput = mem.ToArray();
            //    if (newOutput.Length != ImageSize)
            //       throw new Exception($"Invalid image size! Expected {ImageSize} got {newOutput.Length}");

            if (newOutput.Length > 0)
                return newOutput;
            else
                return new byte[CalculateLength(Width, Height, PicaFormat)];
        }

        // Convert helpers from Citra Emulator (citra/src/common/color.h)
        private static byte Convert8To1(byte val) { return (byte)(val == 0 ? 0 : 1); }
        private static byte Convert8To4(byte val) { return (byte)(val >> 4); }
        private static byte Convert8To5(byte val) { return (byte)(val >> 3); }
        private static byte Convert8To6(byte val) { return (byte)(val >> 2); }

        private static byte ConvertBRG8ToL(byte[] bytes)
        {
            byte L = (byte)(bytes[0] * 0.0722f);
            L += (byte)(bytes[1] * 0.7152f);
            L += (byte)(bytes[2] * 0.2126f);

            return L;
        }

        private static void DecodeRGB565(byte[] Buffer, int Address, ushort Value)
        {
            int R = ((Value >> 0) & 0x1f) << 3;
            int G = ((Value >> 5) & 0x3f) << 2;
            int B = ((Value >> 11) & 0x1f) << 3;

            SetColor(Buffer, Address, 0xff,
                B | (B >> 5),
                G | (G >> 6),
                R | (R >> 5));
        }

        private static void DecodeRGBA4(byte[] Buffer, int Address, ushort Value)
        {
            int R = (Value >> 4) & 0xf;
            int G = (Value >> 8) & 0xf;
            int B = (Value >> 12) & 0xf;

            SetColor(Buffer, Address, (Value & 0xf) | (Value << 4),
                B | (B << 4),
                G | (G << 4),
                R | (R << 4));
        }

        private static void DecodeRGBA5551(byte[] Buffer, int Address, ushort Value)
        {
            int R = ((Value >> 1) & 0x1f) << 3;
            int G = ((Value >> 6) & 0x1f) << 3;
            int B = ((Value >> 11) & 0x1f) << 3;

            SetColor(Buffer, Address, (Value & 1) * 0xff,
                B | (B >> 5),
                G | (G >> 5),
                R | (R >> 5));
        }

        private static void SetColor(byte[] Buffer, int Address, int A, int B, int G, int R)
        {
            Buffer[Address + 0] = (byte)B;
            Buffer[Address + 1] = (byte)G;
            Buffer[Address + 2] = (byte)R;
            Buffer[Address + 3] = (byte)A;
        }

        private static ushort GetUShort(byte[] Buffer, int Address)
        {
            return (ushort)(
                Buffer[Address + 0] << 0 |
                Buffer[Address + 1] << 8);
        }

        public static int CalculateLength(int Width, int Height, PICASurfaceFormat Format)
        {
            int Length = (Width * Height * FmtBPP[(int)Format]) / 8;

            if ((Length & 0x7f) != 0)
            {
                Length = (Length & ~0x7f) + 0x80;
            }

            return Length;
        }

    }
}
