namespace StoneAge.Domain.FileSystem.Document
{
    public interface IDocumentBuilderData
    {
        IDocumentBuilderGeneration With_File(string path);
        IDocumentBuilderGeneration With_Bytes(byte[] bytes);
    }
}
