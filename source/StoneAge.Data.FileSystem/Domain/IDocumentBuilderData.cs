namespace StoneAge.FileStore.Domain
{
    public interface IDocumentBuilderData
    {
        IDocumentBuilderGeneration With_File(string path);
        IDocumentBuilderGeneration With_Bytes(byte[] bytes);
        IDocumentBuilderGeneration With_String(string data);
    }
}
