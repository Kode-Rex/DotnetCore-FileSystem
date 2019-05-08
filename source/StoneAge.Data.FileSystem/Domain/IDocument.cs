namespace StoneAge.Data.FileSystem.Domain
{
    public interface IDocument
    {
        string Name { get; }
        byte[] Data { get; }
    }
}
