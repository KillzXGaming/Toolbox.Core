using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Toolbox.Core.IO;

namespace Toolbox.Core
{
    public class ArchiveFileInfo
    {
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Toggles visibily of the entry in GUI.
        /// </summary>
        public bool Visibile { get; set; } = true;

        protected Stream _stream;

        private byte[] dataBytes;
        public virtual Stream FileData
        {
            get
            {
                if (dataBytes != null)
                    return new MemoryStream(dataBytes);

                if (_stream != null)
                    _stream.Position = 0;
                return _stream;
            }
            set {  _stream = value; }
        }

        public void SetData(Stream data)
        {
            _stream = data;
        }

        public void SetData(byte[] data)
        {
            dataBytes = data;
        }

        public byte[] AsBytes() {
            if (dataBytes != null)
                return dataBytes;

            return FileData.ToArray();
        }

        //The attached file format instance when the file is opened.
        public IFileFormat FileFormat = null;

        public virtual IFileFormat OpenFile()
        {
            var data = FileData;
            var file = STFileLoader.OpenFileFormat(DecompressData(data), FileName);
            return file;
        }

        public virtual uint GetFileSize() { return 0; return (uint)FileData.Length; }

        public Task FileWriteAsync(string filePath) {
           return Task.Run(() => FileData.SaveToFile(filePath));
        }

        public void SaveFileFormat()
        {
            if (FileFormat != null && FileFormat.CanSave) {
                var mem = new System.IO.MemoryStream();
                FileFormat.Save(mem);
                FileData = CompressData(new MemoryStream(mem.ToArray()));
                //FileFormat.Load(FileData);
            }
        }

        public virtual Stream DecompressData(Stream compressed) {
            return compressed;
        }

        public virtual Stream CompressData(Stream decompressed) {
            return decompressed;
        }
    }
}
