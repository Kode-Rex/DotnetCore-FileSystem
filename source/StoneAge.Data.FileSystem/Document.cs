using StoneAge.Data.FileSystem.Domain;

namespace StoneAge.Data.FileSystem
{
    public class Document : IDocument
    {
        public string Name { get; internal set; }
        public byte[] Data { get; internal set; }
    }
}
