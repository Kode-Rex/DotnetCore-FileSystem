using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    public class Document : IDocument
    {
        public string Name { get; internal set; }
        public byte[] Data { get; internal set; }
    }
}
