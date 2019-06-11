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
            return await Task.Run(() =>
            {
                var result = new WriteFileResult();
                if (string.IsNullOrWhiteSpace(file.Name))
                {
                    result.ErrorMessages.Add("No file name provided");
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

        public IDocument Read(string path)
        {
            return GetDocument(path);
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
            return await File.ReadAllLinesAsync(path);
        }

        private WriteFileResult Write_File_To_Path(IDocument file, string filePath, FileMode fileMode)
        {
            var result = new WriteFileResult();
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
