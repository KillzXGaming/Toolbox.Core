using Syroot.BinaryData;
using System.IO;
using System;
using System.IO.Compression;
using OpenTK;

namespace Toolbox.Core.IO
{
    public class STFileLoader
    {
        public class Settings
        {
            /// <summary>
            /// Keeps the file stream open.
            /// </summary>
            public bool KeepOpen = false;

            /// <summary>
            /// The compression format set.
            /// </summary>
            public ICompressionFormat CompressionFormat = null;

            /// <summary>
            /// Filters file formats to only load the specified types.
            /// </summary>
            public Type[] FileFilter = null;

            //Keep these sizes stored for useful file information
            internal uint DecompressedSize = 0;
            internal uint CompressedSize = 0;
        }

        /// <summary>
        /// Gets the <see cref="IFileFormat"/> from a file or byte array. 
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="data">The byte array of the data</param>
        /// <param name="InArchive">If the file is in an archive so it can be saved back</param>
        /// <param name="archiveNode">The node being replaced from an archive</param>
        /// <param name="Compressed">If the file is being compressed or not</param>
        /// <param name="CompType">The type of <see cref="CompressionType"/> being used</param>
        /// <returns></returns>
        public static IFileFormat OpenFileFormat(string FileName, Settings settings = null)
        {
            return OpenFileFormat(File.OpenRead(FileName), FileName, settings);
        }

        /// <summary>
        /// Gets the <see cref="IFileFormat"/> from a file or byte array. 
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="data">The byte array of the data</param>
        /// <param name="InArchive">If the file is in an archive so it can be saved back</param>
        /// <param name="Compressed">If the file is being compressed or not</param>
        /// <param name="CompressionFormat">The type of <see cref="ICompressionFormat"/> being used</param>
        /// <returns></returns>
        public static IFileFormat OpenFileFormat(Stream stream, string FileName, Settings settings = null)
        {
            //File is empty so return
            if (stream == null || stream.Length < 8) return null;

            //Create settings if none set
            if (settings == null) settings = new Settings();
            //Set the current stream as the decompressed size if a compression format is not set yet
            if (settings.CompressionFormat == null)
                settings.DecompressedSize = (uint)stream.Length;

            long streamStartPos = stream.Position;

            //Try catch incase it fails, continute to load the file anyways if the check may be false 
            try
            {
                //Check all supported compression formats and decompress. Then loop back
                if (settings.CompressionFormat == null)
                {
                    foreach (ICompressionFormat compressionFormat in FileManager.GetCompressionFormats())
                    {
                        stream.Position = streamStartPos;
                        if (compressionFormat.Identify(stream, FileName))
                        {
                            stream.Position = streamStartPos;

                            settings.CompressedSize = (uint)stream.Length;
                            stream = compressionFormat.Decompress(stream);
                            settings.DecompressedSize = (uint)stream.Length;
                            settings.CompressionFormat = compressionFormat;

                            return OpenFileFormat(stream, FileName, settings);
                        }
                    }
                }
            } //It's possible some types fail to compress if identify was incorrect so we should skip any errors
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var info = new File_Info();
            info.FileName = Path.GetFileName(FileName);
            info.FilePath = FileName;

            stream.Position = streamStartPos;
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                Console.WriteLine($"fileFormat {fileFormat}!");

                //Set the file name so we can check it's extension in the identifier. 
                //Most is by magic but some can be extension or name.

                if (fileFormat.Identify(info, stream) && !IsFileFiltered(fileFormat, settings))
                {
                    fileFormat.FileInfo = info;
                    return SetFileFormat(fileFormat, FileName, stream, settings);
                }
            }

            stream.Close();

            return null;
        }

        private static bool IsFileFiltered(IFileFormat fileFormat, Settings settings)
        {
            if (settings.FileFilter == null || settings.FileFilter.Length == 0)
                return false;

            foreach (var type in settings.FileFilter)
            {
                if (type == fileFormat.GetType())
                    return false;

                foreach (var inter in type.GetInterfaces())
                {
                    if (inter.IsGenericType && inter.GetGenericTypeDefinition() == fileFormat.GetType())
                        return false;
                }
            }
            return true;
        }

        private static IFileFormat SetFileFormat(IFileFormat fileFormat, string FileName, Stream stream, Settings settings = null)
        {
            fileFormat.FileInfo.DecompressedSize = (uint)settings.DecompressedSize;
            fileFormat.FileInfo.CompressedSize = (uint)settings.CompressedSize;
            fileFormat.FileInfo.Compression = settings.CompressionFormat;
            fileFormat.Load(stream);
            //After file has been loaded and read, we'll dispose unless left open

            if (!settings.KeepOpen && !fileFormat.FileInfo.KeepOpen)
            {
                stream.Dispose();
                GC.SuppressFinalize(stream);
            }

            return fileFormat;
        }
    }
}
