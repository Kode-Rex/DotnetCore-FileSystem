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
        public static IDocument NullDocument = new Document{Name = "NullDocument"};

        public async Task<WriteFileResult> Write(string directory, IDocument file)
        {
            var writeFileResult = Check_For_Errors(file, directory);

            if (!writeFileResult.HadError)
            {
                Ensure_Directory_Is_Created(directory);

                var filePath = Path.Combine(directory, file.Name);
                await Write_File_To_Path(file, filePath);
            }

            return writeFileResult;
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

        public bool Exists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return Directory.Exists(path) || File.Exists(path);
        }

        public void Delete(string path)
        {
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

        public bool Move(string file, string newLocation)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            File.Move(file, newLocation);
            return true;

        }

        public bool Rename(string file, string newFileName)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            var directoryPath = Path.GetDirectoryName(file);
            var newFilePath = Path.Combine(directoryPath, newFileName);

            File.Move(file, newFilePath);

            return true;
        }

        private WriteFileResult Check_For_Errors(IDocument file, string directory)
        {
            var errorMessages = new List<string>();
            var writeFileResult = new WriteFileResult { ErrorMessages = errorMessages };

            if (Invalid_File_Name(file))
            {
                errorMessages.Add("No file name");
                return writeFileResult;
            }

            if (Invalid_Directory(directory))
            {
                errorMessages.Add("Invalid directory provided");
                return writeFileResult;
            }

            var path = Path.Combine(directory, file.Name);
            if (Exists(path))
            {
                errorMessages.Add($"File with the same name exists in directory [{directory}]");
            }

            return writeFileResult;
        }

        private static bool Invalid_Directory(string path)
        {
            bool IsValidPath()
            {
                try
                {
                    var root = Path.GetPathRoot(path);
                    return string.IsNullOrEmpty(root.Trim('\\', '/')) == false;
                    
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return string.IsNullOrWhiteSpace(path) || !IsValidPath();
        }

        private static bool Invalid_File_Name(IDocument file)
        {
            return string.IsNullOrWhiteSpace(file.Name);
        }

        private static async Task Write_File_To_Path(IDocument file, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(file.Data);
            }
        }

        private void Ensure_Directory_Is_Created(string currentDirectory)
        {
            if (!Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(currentDirectory);
            }
        }

        
    }
}
