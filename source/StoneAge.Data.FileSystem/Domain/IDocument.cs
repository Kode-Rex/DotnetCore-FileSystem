using System.Text;

namespace StoneAge.FileStore.Domain
{
    /// <summary>
    /// Defines the interface for a file document, containing the name and binary data.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Gets the name of the document, typically representing the file name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the binary data of the document as a byte array.
        /// </summary>
        byte[] Data { get; }
    }
}
