using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneAge.FileStore.Domain
{
    public interface IFileSystem
    {
        Task<WriteFileResult> Write(string directory, IDocument file);
        IEnumerable<FileInformation> List(string directory);
        bool Exists(string path);
        void Delete(string path);
        IDocument GetDocument(string path);
        bool Move(string currentPath, string newPath);
        bool Rename(string filePath, string newName);
    }
}
