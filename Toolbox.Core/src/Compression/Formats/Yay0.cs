using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Core.IO;
using System.Runtime.InteropServices;

namespace Toolbox.Core
{
    public class Yay0 : ICompressionFormat
    {
        public int Alignment = 0;

        public string[] Description { get; set; } = new string[] { "Yay0" };
        public string[] Extension { get; set; } = new string[] { "*.Yay0", "*.szp", };

        public override string ToString() { return "Yay0"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(4, "Yay0");
            }
        }

        public bool CanCompress { get; } = false;

        public Stream Decompress(Stream stream) {
            return new MemoryStream(DecompressData(stream));
        }

        public static byte[] DecompressData(Stream stream)
        {
            List<byte> output = new List<byte>();
            uint decompressedLength;
            uint compressedOffset;
            uint uncompressedOffset;
            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                reader.ReadUInt32(); //Magic
                decompressedLength = reader.ReadUInt32();
                compressedOffset = reader.ReadUInt32();
                uncompressedOffset = reader.ReadUInt32();
            }

            int currentOffset;
            int readerPos = 16;

            byte[] data = stream.ReadAllBytes();
            while (output.Count < decompressedLength)
            {
                byte bits = data[readerPos++];
                BitArray arrayOfBits = new BitArray(new byte[1] { bits });

                for (int i = 7; i > -1 && (output.Count < decompressedLength); i--) //iterate through layout bits
                {
                    currentOffset = readerPos;
                    if (arrayOfBits[i] == true)
                    {
                        //non-compressed
                        //add one byte from uncompressedOffset to newFile
                        output.Add(data[uncompressedOffset++]);
                    }
                    else
                    {
                        //compressed
                        //read 2 bytes
                        //4 bits = length
                        //12 bits = offset

                        byte byte1 = data[compressedOffset++];
                        byte byte2 = data[compressedOffset++];

                        byte byte1Upper = (byte)((byte1 & 0x0F));
                        byte byte1Lower = (byte)((byte1 & 0xF0) >> 4);

                        int finalOffset = ((byte1Upper << 8) | byte2) + 1;
                        int finalLength;

                        if (byte1Lower == 0)
                        {
                            finalLength = data[uncompressedOffset] + 0x12;
                            uncompressedOffset++;
                        }
                        else
                        {
                            finalLength = byte1Lower + 2;
                        }

                        for (int j = 0; j < finalLength; j++) //add data for finalLength iterations
                        {
                            output.Add(output[output.Count - finalOffset]); //add byte at offset (fileSize - finalOffset) to file
                        }
                    }
                    readerPos = (int)currentOffset;
                }
            }

            return output.ToArray();
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(EveryFileExplorer.YAZ0.Compress(
             stream.ToArray(), Runtime.Yaz0CompressionLevel, (uint)Alignment));
        }
    }
}