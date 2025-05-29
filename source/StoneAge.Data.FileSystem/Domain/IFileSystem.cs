using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneAge.FileStore.Domain
{
    /// <summary>
    /// Defines an interface for file system operations.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Writes a document to the specified directory.
        /// </summary>
        /// <param name="directory">The directory path where the file should be written.</param>
        /// <param name="file">The document to write.</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
        Task<WriteFileResult> Write(string directory, IDocument file);

        /// <summary>
        /// Appends data to a file in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path where the file should be appended.</param>
        /// <param name="file">The document containing data to append.</param>
        /// <returns>A WriteFileResult containing information about the operation.</returns>
        Task<WriteFileResult> Append(string directory, IDocument file);

        /// <summary>
        /// Lists all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path to list files from.</param>
        /// <returns>A collection of FileInformation objects representing the files.</returns>
        IEnumerable<FileInformation> List(string directory);

        /// <summary>
        /// Asynchronously lists all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory path to list files from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of FileInformation objects representing the files.</returns>
        Task<IEnumerable<FileInformation>> ListAsync(string directory);

        /// <summary>
        /// Checks if a file or directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file or directory exists, false otherwise.</returns>
        bool Exists(string path);

        /// <summary>
        /// Asynchronously checks if a file or directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file or directory exists.</returns>
        Task<bool> ExistsAsync(string path);

        /// <summary>
        /// Deletes a file or directory at the specified path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        void Delete(string path);

        /// <summary>
        /// Asynchronously deletes a file or directory at the specified path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(string path);

        /// <summary>
        /// Reads a document from the specified path.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The document read from the path, or NullDocument if the file doesn't exist.</returns>
        IDocument Read(string path);

        /// <summary>
        /// Gets a document from the specified path.
        /// </summary>
        /// <param name="path">The path to get the document from.</param>
        /// <returns>The document from the path, or NullDocument if the file doesn't exist.</returns>
        IDocument GetDocument(string path);

        /// <summary>
        /// Asynchronously gets a document from the specified path.
        /// </summary>
        /// <param name="path">The path to get the document from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the document from the path, or NullDocument if the file doesn't exist.</returns>
        Task<IDocument> GetDocumentAsync(string path);

        /// <summary>
        /// Reads all lines from a file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>An enumerable collection of strings representing the lines in the file.</returns>
        Task<IEnumerable<string>> ReadAllLines(string path);

        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// <param name="currentPath">The source file path.</param>
        /// <param name="newPath">The destination file path.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        bool Move(string currentPath, string newPath);

        /// <summary>
        /// Asynchronously moves a file from one location to another.
        /// </summary>
        /// <param name="currentPath">The source file path.</param>
        /// <param name="newPath">The destination file path.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the move was successful.</returns>
        Task<bool> MoveAsync(string currentPath, string newPath);

        /// <summary>
        /// Moves a file from one location to another, overwriting the destination if it exists.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        bool MoveWithOverwrite(string file, string newLocation);

        /// <summary>
        /// Asynchronously moves a file from one location to another, overwriting the destination if it exists.
        /// </summary>
        /// <param name="file">The source file path.</param>
        /// <param name="newLocation">The destination file path.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the move was successful.</returns>
        Task<bool> MoveWithOverwriteAsync(string file, string newLocation);

        /// <summary>
        /// Renames a file.
        /// </summary>
        /// <param name="filePath">The path of the file to rename.</param>
        /// <param name="newName">The new name for the file.</param>
        /// <returns>True if the rename was successful, false otherwise.</returns>
        bool Rename(string filePath, string newName);

        /// <summary>
        /// Asynchronously renames a file.
        /// </summary>
        /// <param name="filePath">The path of the file to rename.</param>
        /// <param name="newName">The new name for the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the rename was successful.</returns>
        Task<bool> RenameAsync(string filePath, string newName);
    }
}
