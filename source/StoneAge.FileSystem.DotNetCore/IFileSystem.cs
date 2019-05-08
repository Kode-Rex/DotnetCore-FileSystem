using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoneAge.Domain.FileSystem;
using StoneAge.Domain.FileSystem.Document;

namespace StoneAge.FileSystem.DotNetCore
{
    public interface IFileSystem
    {
        Task<WriteFileResult> Write(string directory, IDocument file);
        IEnumerable<FileInformation> List(string directory);
        bool Exists(string path);
        void Delete(string path);
    }
}
