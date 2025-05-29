using System.Text;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    /// <summary>
    /// Represents a file document with name and binary data.
    /// Implements the <see cref="IDocument"/> interface.
    /// </summary>
    public class Document : IDocument
    {
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the binary data of the document.
        /// </summary>
        public byte[] Data { get; internal set; }

        /// <summary>
        /// Converts the document's binary data to a string using UTF-8 encoding.
        /// </summary>
        /// <returns>
        /// A UTF-8 encoded string representation of the document's data,
        /// or an empty string if the data is null.
        /// </returns>
        /// <remarks>
        /// This method assumes that the document's data is encoded in UTF-8.
        /// If the data uses a different encoding, this method may not return the expected result.
        /// </remarks>
        public override string ToString()
        {
            return Data == null ? string.Empty : Encoding.UTF8.GetString(Data);
        }
    }
}
