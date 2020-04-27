using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Core.IO;
using Toolbox.Core;

namespace PluginSample
{
    public class ARC : IFileFormat, IArchiveFile
    {
        public bool CanSave { get; set; } = true; 

        public string[] Description { get; set; } = new string[] { "ARC" };
        public string[] Extension { get; set; } = new string[] { "*.arc"};
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public File_Info FileInfo { get; set; }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public bool Identify(File_Info fileInfo, Stream stream)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(4, "ARC0");
            }
        }

        public List<ArcFile> files = new List<ArcFile>();

        public IEnumerable<ArchiveFileInfo> Files => files;
        public void ClearFiles() { files.Clear(); }

        public void Load(Stream stream) {
            //Set this to keep stream open
            FileInfo.KeepOpen = true;
            using (var reader = new FileReader(stream, true))
            {
                //Parse the file
            }
        }

        public void Save(Stream stream) {
            using (var writer = new FileWriter(stream, true))
            {
                //write the file
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Add(new ArcFile()
            {
                FileData = archiveFileInfo.FileData,
                FileName = archiveFileInfo.FileName,
            });
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Remove((ArcFile)archiveFileInfo);
            return false;
        }

        public class ArcFile : ArchiveFileInfo
        {

        }
    }
}
