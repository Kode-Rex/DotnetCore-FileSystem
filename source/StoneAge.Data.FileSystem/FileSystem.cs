using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    public class FileSystem : IFileSystem
    {
        public static readonly IDocument NullDocument = new Document{Name = "NullDocument"};

        public async Task<WriteFileResult> Write(string directory, IDocument file)
        {
            return await Task.Run(() =>
            {
                var result = new WriteFileResult();
                if (string.IsNullOrWhiteSpace(file.Name))
                {
                    result.ErrorMessages.Add("No file name provided");
                    return result;
                }
                
                if (string.IsNullOrWhiteSpace(directory))
                {
                    result.ErrorMessages.Add("No directory provided");
                    return result;
                }

                result = Ensure_Directory_Is_Created(directory);

                if (result.HadError)
                {
                    return result;
                }

                var filePath = Path.Combine(directory, file.Name);
                result = Write_File_To_Path(file, filePath, FileMode.Create);

                return result;
            });
        }

        public IEnumerable<FileInformation> List(string directory)
        {
            if (!Directory.Exists(directory)) return new List<FileInformation>();

            var directoryInfo = new DirectoryInfo(directory);
            var fileInformations = directoryInfo.GetFiles().Select(info => new FileInformation
            {
                Name = info.Name,
                Size = Convert_Bytes_To_Megabytes(info.Length).ToString()
            });

            double Convert_Bytes_To_Megabytes(long bytes)
            {
                var fileSizeInKB = bytes / 1024;
                var fileSizeInMB = fileSizeInKB / 1024;
                return fileSizeInMB;
            }

            return fileInformations;
        }

        public async Task<IEnumerable<FileInformation>> ListAsync(string directory)
        {
            return await Task.Run(() => List(directory));
        }

        public bool Exists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return Directory.Exists(path) || File.Exists(path);
        }

        public async Task<bool> ExistsAsync(string path)
        {
            return await Task.Run(() => Exists(path));
        }

        public void Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task DeleteAsync(string path)
        {
            await Task.Run(() => Delete(path));
        }

        public IDocument Read(string path)
        {
            return GetDocument(path);
        }

        public async Task<IDocument> ReadAsync(string path)
        {
            return await GetDocumentAsync(path);
        }

        public IDocument GetDocument(string path)
        {
            if (!File.Exists(path))
            {
                return NullDocument;
            }

            var name = Path.GetFileName(path);
            return new DocumentBuilder()
                .With_Name(name)
                .With_File(path)
                .Create_Document();
        }

        public async Task<IDocument> GetDocumentAsync(string path)
        {
            return await Task.Run(() => GetDocument(path));
        }

        public bool Move(string file, string newLocation)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            try
            {
                File.Move(file, newLocation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> MoveAsync(string file, string newLocation)
        {
            return await Task.Run(() => Move(file, newLocation));
        }

        public bool MoveWithOverwrite(string file, string newLocation)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            try
            {
                if (File.Exists(newLocation))
                {
                    File.Delete(newLocation);
                }

                File.Move(file, newLocation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> MoveWithOverwriteAsync(string file, string newLocation)
        {
            return await Task.Run(() => MoveWithOverwrite(file, newLocation));
        }

        public bool Rename(string file, string newFileName)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            try
            {
                var directoryPath = Path.GetDirectoryName(file);
                var newFilePath = Path.Combine(directoryPath, newFileName);

                if (File.Exists(newFilePath))
                {
                    return false;
                }

                File.Move(file, newFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenameAsync(string file, string newFileName)
        {
            return await Task.Run(() => Rename(file, newFileName));
        }

        public async Task<WriteFileResult> Append(string directory, IDocument file)
        {
            return await Task.Run(() =>
            {
                var result = new WriteFileResult();
                if (string.IsNullOrWhiteSpace(file.Name))
                {
                    result.ErrorMessages.Add("No file name provided");
                    return result;
                }
                
                if (string.IsNullOrWhiteSpace(directory))
                {
                    result.ErrorMessages.Add("No directory provided");
                    return result;
                }

                result = Ensure_Directory_Is_Created(directory);

                if (result.HadError)
                {
                    return result;
                }

                var filePath = Path.Combine(directory, file.Name);
                result = Write_File_To_Path(file, filePath, FileMode.Append);

                return result;
            });
        }

        public async Task<IEnumerable<string>> ReadAllLines(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}", path);
            }
            
            return await File.ReadAllLinesAsync(path);
        }

        private WriteFileResult Write_File_To_Path(IDocument file, string filePath, FileMode fileMode)
        {
            var result = new WriteFileResult();

            if (file.Data == null)
            {
                result.ErrorMessages.Add("No file data provided; cannot write file.");
                return result;
            }

            try
            {
                using (var stream = new FileStream(filePath, fileMode))
                {
                    stream.Write(file.Data);
                }

                result.FullFilePath = filePath;
            }
            catch (Exception e)
            {
                result.ErrorMessages.Add($"An error occured writing the file [{e.Message}]");
            }

            return result;
        }

        private WriteFileResult Ensure_Directory_Is_Created(string currentDirectory)
        {
            var result= new WriteFileResult();
            if (!Directory.Exists(currentDirectory))
            {
                try
                {
                    Directory.CreateDirectory(currentDirectory);
                }
                catch (Exception e)
                {
                    result.ErrorMessages.Add($"An error occured creating directory structure [{e.Message}]");
                    return result;
                }
            }

            return result;
        }
        
    }
}
