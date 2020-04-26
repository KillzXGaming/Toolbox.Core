using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Toolbox.Core.Imaging;
using Ryujinx.Graphics.Gal.Texture; //For ASTC
using Toolbox.Core.TextureDecoding;

namespace Toolbox.Core
{
    public abstract class STGenericTexture
    {
        public RenderableTex RenderableTex;

        public string Name { get; set; }

        public STGenericTexture()
        {
            RenderableTex = new RenderableTex();
            RenderableTex.GLInitialized = false;

            RedChannel = STChannelType.Red;
            GreenChannel = STChannelType.Green;
            BlueChannel = STChannelType.Blue;
            AlphaChannel = STChannelType.Alpha;
            DisplayProperties = new DefaultTextureProperties(this); 
        }

        /// <summary>
        /// The properties to display on the texture editor.
        /// </summary>
        public object DisplayProperties { get; set; }

        /// <summary>
        /// A Surface contains mip levels of compressed/uncompressed texture data
        /// </summary>
        public class Surface
        {
            public List<byte[]> mipmaps = new List<byte[]>();
        }

        /// <summary>
        /// The type of surface the texture uses which can determine how to use array levels and cubemaps.
        /// </summary>
        public STSurfaceType SurfaceType = STSurfaceType.Texture2D;

        /// <summary>
        /// The swizzle method to use when decoding or encoding back a texture.
        /// </summary>
        public PlatformSwizzle PlatformSwizzle;

        /// <summary>
        /// Is the texture edited or not. Used for the image editor for saving changes.
        /// </summary>
        public bool IsEdited { get; set; } = false;

        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// The depth of the image in pixels. Used for 3D types.
        /// </summary>
        public uint Depth { get; set; }

        /// <summary>
        /// The total amount of surfaces for the texture.
        /// </summary>
        public uint ArrayCount { get; set; } = 1;

        /// <summary>
        /// The total amount of mipmaps for the texture.
        /// </summary>
        public uint MipCount
        {
            get { return mipCount; }
            set
            {
                if (value == 0)
                    mipCount = 1;
                else if (value > 17)
                    throw new Exception($"Invalid mip map count! Texture: {Name} Value: {value}");
                else
                    mipCount = value;
            }
        }

        /// <summary>
        /// The <see cref="TexFormat"/> format of the image. 
        /// </summary>
        public TexFormat Format { get; set; } = TexFormat.RGB8;

        /// <summary>
        /// The <see cref="TexFormatType"/> type of the image. 
        /// </summary>
        public TexFormatType FormatType { get; set; } = TexFormatType.Unorm;

        /// <summary>
        /// The <see cref="PaletteFormat"/> which controls palette information.
        /// </summary>
        public PaletteFormat PaletteFormat { get; set; } = PaletteFormat.None;

        /// <summary>
        /// Determines if the image supports replacing and editing.
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// Parameters on how to replace and save back the texture.
        /// </summary>
        public ImageParameters Parameters = new ImageParameters();

        /// <summary>
        /// Gets the image size from bytes into a string format.
        /// </summary>
        public string DataSize { get { return STMath.GetFileSize(DataSizeInBytes, 5); } }

        /// <summary>
        /// A list of all the formats supported by the texture.
        /// </summary>
        public virtual TexFormat[] SupportedFormats => new TexFormat[0];

        /// <summary>
        /// Determines which component to use for the red channel.
        /// </summary>
        public STChannelType RedChannel = STChannelType.Red;


        /// <summary>
        /// Determines which component to use for the green channel.
        /// </summary>
        public STChannelType GreenChannel = STChannelType.Green;


        /// <summary>
        /// Determines which component to use for the blue channel.
        /// </summary>
        public STChannelType BlueChannel = STChannelType.Blue;

        /// <summary>
        /// Determines which component to use for the alpha channel.
        /// </summary>
        public STChannelType AlphaChannel = STChannelType.Alpha;

        /// <summary>
        /// Determines if the texture is a cubemap based on the surface type.
        /// </summary>
        public bool IsCubemap
        {
            get { 
                return SurfaceType == STSurfaceType.TextureCube || 
                    SurfaceType == STSurfaceType.TextureCube_Array; }
        }

        public abstract byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0);

        public virtual byte[] GetPaletteData() { return paletteData; }
        private byte[] paletteData = new byte[0];
        private uint mipCount = 1;

        public void LoadOpenGLTexture()
        {
            if (!Runtime.UseOpenGL)
                return;

            if (RenderableTex == null)
                RenderableTex = new RenderableTex();

            RenderableTex.GLInitialized = false;
            RenderableTex.LoadOpenGLTexture(this);
        }

        public virtual void SetPaletteData(byte[] data, PaletteFormat format)
        {
            paletteData = data;
            PaletteFormat = format;
        }

        public List<Surface> Get3DSurfaces(int IndexStart = 0, bool GetAllSurfaces = true, int GetSurfaceAmount = 1)
        {
            if (GetAllSurfaces)
                GetSurfaceAmount = (int)Depth;

            var surfaces = new List<Surface>();
            for (int depthLevel = 0; depthLevel < Depth; depthLevel++)
            {
                bool IsLower = depthLevel < IndexStart;
                bool IsHigher = depthLevel >= (IndexStart + GetSurfaceAmount);
                if (!IsLower && !IsHigher)
                {
                    List<byte[]> mips = new List<byte[]>();
                    for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                    {
                        mips.Add(GetImageData(0, mipLevel, depthLevel));
                    }

                    surfaces.Add(new Surface() { mipmaps = mips });
                }
            }

            return surfaces;
        }

        //
        //Gets a list of surfaces given the start index of the array and the amount of arrays to obtain
        //
        public List<Surface> GetSurfaces(int ArrayIndexStart = 0, bool GetAllSurfaces = true, int GetSurfaceAmount = 1)
        {
            if (GetAllSurfaces)
                GetSurfaceAmount = (int)ArrayCount;

            var surfaces = new List<Surface>();
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                bool IsLower = arrayLevel < ArrayIndexStart;
                bool IsHigher = arrayLevel >= (ArrayIndexStart + GetSurfaceAmount);
                if (!IsLower && !IsHigher)
                {
                    List<byte[]> mips = new List<byte[]>();
                    for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                    {
                        mips.Add(GetImageData(arrayLevel, mipLevel));
                    }

                    surfaces.Add(new Surface() { mipmaps = mips });
                }
            }

            return surfaces;
        }

        public void SaveBitmap(string filePath, TextureExportSettings settings = null)
        {
            var bitmap = GetBitmap(settings.ArrayLevel, settings.MipLevel);
            bitmap.Save(filePath);
        }

        public void SaveTGA(string filePath, TextureExportSettings settings = null)
        {

        }

        public void SaveASTC(string filePath, TextureExportSettings settings = null) {
            List<Surface> surfaces = GetExportableSurfaces(settings);

            ASTC atsc = new ASTC();
            atsc.Width = Width;
            atsc.Height = Height;
            atsc.Depth = Depth;
            atsc.BlockDimX = (byte)TextureFormatHelper.GetBlockWidth(Format);
            atsc.BlockDimY = (byte)TextureFormatHelper.GetBlockHeight(Format);
            atsc.BlockDimZ = (byte)TextureFormatHelper.GetBlockDepth(Format);
            atsc.DataBlock = ByteUtils.CombineArray(surfaces[0].mipmaps.ToArray());
            atsc.Save(new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite));
        }

        public void SaveDDS(string filePath, TextureExportSettings settings = null) {
            List<Surface> surfaces = GetExportableSurfaces(settings);

            DDS dds = new DDS();
            dds.MainHeader = new DDS.Header();
            dds.MainHeader.Width = Width;
            dds.MainHeader.Height = Height;
            dds.MainHeader.Depth = Depth;
            dds.MainHeader.MipCount = (uint)MipCount;
            dds.MainHeader.PitchOrLinearSize = (uint)surfaces[0].mipmaps[0].Length;

            if (surfaces.Count > 1) //Use DX10 format for array surfaces as it can do custom amounts
                dds.SetFlags(Format, true, IsCubemap);
            else
                dds.SetFlags(Format, false, IsCubemap);

            if (dds.IsDX10) {
                if (dds.Dx10Header == null)
                    dds.Dx10Header = new DDS.DX10Header();
                dds.Dx10Header.ResourceDim = 3;
                if (IsCubemap)
                    dds.Dx10Header.ArrayCount = (uint)(ArrayCount / 6);
                else
                    dds.Dx10Header.ArrayCount = (uint)ArrayCount;
            }
            dds.Save(filePath, surfaces);
        }

        private List<Surface> GetExportableSurfaces(TextureExportSettings settings)
        {
            if (!settings.ExportArrays)
                return GetSurfaces(settings.ArrayLevel, false);
            else if (Depth > 1)
                return Get3DSurfaces();
            else
                return GetSurfaces();
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> given an array and mip index.
        /// </summary>
        /// <param name="ArrayIndex">The index of the surface/array. Cubemaps will have 6</param>
        /// <param name="MipLevel">The index of the mip level.</param>
        /// <returns></returns>
        public Bitmap GetBitmap(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            uint width = Math.Max(1, Width >> MipLevel);
            uint height = Math.Max(1, Height >> MipLevel);
            byte[] data = GetImageData(ArrayLevel, MipLevel, DepthLevel);
            byte[] paletteData = GetPaletteData();

            if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube || 
                PlatformSwizzle == PlatformSwizzle.Platform_Wii)
            {
                return BitmapExtension.CreateBitmap(Decode_Gamecube.DecodeData(data, paletteData, width, height, Format, PaletteFormat),
                      (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else  if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
                return CTR_3DS.DecodeBlockToBitmap(data, (int)width, (int)height, CTR_3DS.ConvertToPICAFormat(Format));

            //Platforms after these will be decoded after deswizzled as they output as compressed data
            if (PlatformSwizzle == PlatformSwizzle.Platform_Switch)
                data = Switch.TegraX1Swizzle.GetImageData(this, data, ArrayLevel, MipLevel, DepthLevel, 1, false);

            data = DecodeBlock(data, width, height, Format, FormatType);
            return BitmapExtension.CreateBitmap(data, (int)width, (int)height);

            return null;
        }

        public static byte[] DecodeBlock(byte[] data, uint width, uint height, TexFormat format,
            byte[] paletteData, PaletteFormat paletteFormat, PlatformSwizzle platform)
        {
            byte[] output = data;

            if (platform == PlatformSwizzle.Platform_Gamecube ||
                platform == PlatformSwizzle.Platform_Wii)
            {
                output = Decode_Gamecube.DecodeData(data, paletteData, width, height, format, paletteFormat);
            }

            return output;
        }

        public static byte[] DecodeBlock(byte[] data, uint Width, uint Height, 
            TexFormat format, TexFormatType type)
        {
            int blockWidth = (int)TextureFormatHelper.GetBlockWidth(format);
            int blockHeight = (int)TextureFormatHelper.GetBlockHeight(format);
            int blockDepth = (int)TextureFormatHelper.GetBlockDepth(format);

            if (IsAtscFormat(format))
                return ASTCDecoder.DecodeToRGBA8888(data,
                    (int)blockWidth,
                    (int)blockHeight, 1, (int)Width, (int)Height, 1);
            else
                return DDSDecoder.DecodeToRGBA8(data,
                    (int)Width, (int)Height, format, type);
        }

        public static bool IsAtscFormat(TexFormat format) {
            return format.ToString().StartsWith("ASTC");
        }

        /// <summary>
        /// The total length of all the bytes given from GetImageData.
        /// </summary>
        public long DataSizeInBytes
        {
            get
            {
                if (PlatformSwizzle == PlatformSwizzle.Platform_3DS)
                    return GetImageSize3DS();
                if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
                    return GetImageSizeGCN();
                else
                    return GetImageSizeDefault();
            }
        }

        private int GetImageSizeDefault()
        {
            int totalSize = 0;
            if (TextureFormatHelper.HasFormatTableKey(Format))
            {
                uint bpp = TextureFormatHelper.GetBytesPerPixel(Format);

                for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
                {
                    for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                    {
                        uint width = (uint)Math.Max(1, Width >> mipLevel);
                        uint height = (uint)Math.Max(1, Height >> mipLevel);

                        uint size = width * height * bpp;
                        if (TextureFormatHelper.IsBCNCompressed(Format))
                        {
                            size = ((width + 3) >> 2) * ((Height + 3) >> 2) * bpp;
                            if (size < bpp)
                                size = bpp;
                        }

                        totalSize += (int)size;
                    }
                }
            }
            return totalSize;
        }

        private int GetImageSizeGCN()
        {
            int totalSize = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);

                    totalSize += Decode_Gamecube.GetDataSize((uint)Decode_Gamecube.FromGenericFormat(Format), width, height);
                }
            }

            return totalSize;
        }

        private int GetImageSize3DS()
        {
            int totalSize = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    totalSize += CTR_3DS.CalculateLength((int)width, (int)height, CTR_3DS.ConvertToPICAFormat(Format));
                }
            }
            return totalSize;
        }
    }
}
