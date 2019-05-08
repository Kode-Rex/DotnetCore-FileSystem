namespace StoneAge.Domain.FileSystem.Document
{
    public interface IDocument
    {
        string Name { get; }
        byte[] Data { get; }
    }
}
