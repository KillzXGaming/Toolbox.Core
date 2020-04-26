using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Toolbox.Core.IO;

namespace Toolbox.Core
{
    public class ArchiveFileInfo
    {
        public string FileName { get; set; } = string.Empty;

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
            var file = STFileLoader.OpenFileFormat(FileData, FileName);
            return file;
        }

        public virtual uint GetFileSize() { return 0; return (uint)FileData.Length; }

        public void SaveFileFormat()
        {
            if (FileFormat != null && FileFormat.CanSave) {
                var mem = new MemoryStream();
                STFileSaver.SaveFileFormat(FileFormat, mem);
                FileData = mem;
            }
        }
    }
}
