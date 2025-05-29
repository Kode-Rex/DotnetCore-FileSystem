using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneAge.FileStore.Domain
{
    public interface IFileSystem
    {
        Task<WriteFileResult> Write(string directory, IDocument file);
        Task<WriteFileResult> Append(string directory, IDocument file);
        IEnumerable<FileInformation> List(string directory);
        Task<IEnumerable<FileInformation>> ListAsync(string directory);
        bool Exists(string path);
        Task<bool> ExistsAsync(string path);
        void Delete(string path);
        Task DeleteAsync(string path);
        IDocument Read(string path);
        Task<IDocument> ReadAsync(string path);
        IDocument GetDocument(string path);
        Task<IDocument> GetDocumentAsync(string path);
        Task<IEnumerable<string>> ReadAllLines(string path);
        bool Move(string currentPath, string newPath);
        Task<bool> MoveAsync(string currentPath, string newPath);
        bool MoveWithOverwrite(string file, string newLocation);
        Task<bool> MoveWithOverwriteAsync(string file, string newLocation);
        bool Rename(string filePath, string newName);
        Task<bool> RenameAsync(string filePath, string newName);
    }
}
