using System.Text;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    public class Document : IDocument
    {
        public string Name { get; internal set; }
        public byte[] Data { get; internal set; }

        public override string ToString()
        {
            return Data == null ? string.Empty : Encoding.UTF8.GetString(Data);
        }
    }
}
