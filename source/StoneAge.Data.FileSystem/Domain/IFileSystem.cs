using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneAge.Data.FileSystem.Domain
{
    public interface IFileSystem
    {
        Task<WriteFileResult> Write(string directory, IDocument file);
        IEnumerable<FileInformation> List(string directory);
        bool Exists(string path);
        void Delete(string path);
    }
}
