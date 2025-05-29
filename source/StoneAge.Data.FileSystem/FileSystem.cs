using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    /// <summary>
    /// A concrete implementation of the IFileSystem interface that interacts with the actual file system.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <summary>
        /// A null document instance to be returned when a document cannot be found.
        /// </summary>
        public static readonly IDocument NullDocument = new Document{Name = "NullDocument"};

        /// <summary>
        /// Writes a document to the specified directory.
        /// </summary>
        /// <param name="directory">The directory path where the file should be written.</param>
        /// <param name="file">The document to write.</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
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

                // Validate that the file name doesn't contain path separators
                if (file.Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    result.ErrorMessages.Add("File name contains invalid characters");
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

        /// <summary>
        /// Lists all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path to list files from.</param>
        /// <returns>A collection of FileInformation objects representing the files.</returns>
        public IEnumerable<FileInformation> List(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return new List<FileInformation>();

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

        /// <summary>
        /// Asynchronously lists all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path to list files from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of FileInformation objects representing the files.</returns>
        public async Task<IEnumerable<FileInformation>> ListAsync(string directory)
        {
            return await Task.Run(() => List(directory));
        }

        /// <summary>
        /// Checks if a file or directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file or directory exists, false otherwise.</returns>
        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return Directory.Exists(path) || File.Exists(path);
        }

        /// <summary>
        /// Asynchronously checks if a file or directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file or directory exists.</returns>
        public async Task<bool> ExistsAsync(string path)
        {
            return await Task.Run(() => Exists(path));
        }

        /// <summary>
        /// Deletes a file or directory at the specified path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
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

        /// <summary>
        /// Asynchronously deletes a file or directory at the specified path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(string path)
        {
            await Task.Run(() => Delete(path));
        }

        /// <summary>
        /// Reads a document from the specified path.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The document read from the path, or NullDocument if the file doesn't exist.</returns>
        public IDocument Read(string path)
        {
            return GetDocument(path);
        }

        /// <summary>
        /// Gets a document from the specified path.
        /// </summary>
        /// <param name="path">The path to get the document from.</param>
        /// <returns>The document from the path, or NullDocument if the file doesn't exist.</returns>
        /// <remarks>
        /// The document's content is assumed to be in UTF-8 encoding when converted to string.
        /// </remarks>
        public IDocument GetDocument(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return NullDocument;
            }

            var name = Path.GetFileName(path);
            return new DocumentBuilder()
                .With_Name(name)
                .With_File(path)
                .Create_Document();
        }

        /// <summary>
        /// Asynchronously gets a document from the specified path.
        /// </summary>
        /// <param name="path">The path to get the document from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the document from the path, or NullDocument if the file doesn't exist.</returns>
        public async Task<IDocument> GetDocumentAsync(string path)
        {
            return await Task.Run(() => GetDocument(path));
        }

        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public bool Move(string file, string newLocation)
        {
            if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(newLocation) || !File.Exists(file))
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

        /// <summary>
        /// Asynchronously moves a file from one location to another.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the move was successful.</returns>
        public async Task<bool> MoveAsync(string file, string newLocation)
        {
            return await Task.Run(() => Move(file, newLocation));
        }

        /// <summary>
        /// Moves a file from one location to another, overwriting the destination if it exists.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public bool MoveWithOverwrite(string file, string newLocation)
        {
            if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(newLocation) || !File.Exists(file))
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

        /// <summary>
        /// Asynchronously moves a file from one location to another, overwriting the destination if it exists.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the move was successful.</returns>
        public async Task<bool> MoveWithOverwriteAsync(string file, string newLocation)
        {
            return await Task.Run(() => MoveWithOverwrite(file, newLocation));
        }

        /// <summary>
        /// Renames a file.
        /// </summary>
        /// <param name="filePath">The path of the file to rename.</param>
        /// <param name="newName">The new name for the file.</param>
        /// <returns>True if the rename was successful, false otherwise.</returns>
        public bool Rename(string filePath, string newName)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(newName) || !File.Exists(filePath))
            {
                return false;
            }

            try
            {
                // Validate that the new name doesn't contain path separators
                if (newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    return false;
                }

                var directoryPath = Path.GetDirectoryName(filePath);
                var newFilePath = Path.Combine(directoryPath, newName);

                File.Move(filePath, newFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously renames a file.
        /// </summary>
        /// <param name="filePath">The path of the file to rename.</param>
        /// <param name="newName">The new name for the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the rename was successful.</returns>
        public async Task<bool> RenameAsync(string filePath, string newName)
        {
            return await Task.Run(() => Rename(filePath, newName));
        }

        /// <summary>
        /// Appends data to a file in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path where the file should be appended.</param>
        /// <param name="file">The document containing data to append.</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
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

                // Validate that the file name doesn't contain path separators
                if (file.Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    result.ErrorMessages.Add("File name contains invalid characters");
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

        /// <summary>
        /// Reads all lines from a file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>An enumerable collection of strings representing the lines in the file.</returns>
        /// <remarks>
        /// This method assumes the file is encoded in the default encoding for the system.
        /// </remarks>
        public async Task<IEnumerable<string>> ReadAllLines(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}", path);
            }

            return await File.ReadAllLinesAsync(path);
        }

        /// <summary>
        /// Writes file data to the specified path with the given file mode.
        /// </summary>
        /// <param name="file">The document to write.</param>
        /// <param name="filePath">The path to write to.</param>
        /// <param name="fileMode">The file mode to use (Create or Append).</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
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
                // Modern using declaration (C# 8.0+)
                using var stream = new FileStream(filePath, fileMode);
                stream.Write(file.Data);

                result.FullFilePath = filePath;
            }
            catch (Exception e)
            {
                result.ErrorMessages.Add($"An error occured writing the file [{e.Message}]");
            }

            return result;
        }

        /// <summary>
        /// Ensures that the specified directory exists, creating it if necessary.
        /// </summary>
        /// <param name="currentDirectory">The directory path to check or create.</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
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
