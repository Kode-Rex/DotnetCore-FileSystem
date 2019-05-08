namespace StoneAge.FileStore.Domain
{
    public interface IDocumentBuilderData
    {
        IDocumentBuilderGeneration With_File(string path);
        IDocumentBuilderGeneration With_Bytes(byte[] bytes);
    }
}
