namespace StoneAge.FileStore.Domain
{
    public interface IDocument
    {
        string Name { get; }
        byte[] Data { get; }
    }
}
