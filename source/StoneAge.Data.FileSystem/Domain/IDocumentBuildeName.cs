namespace StoneAge.FileStore.Domain
{
    public interface IDocumentBuilderName
    {
        IDocumentBuilderData With_Name(string name);
        IDocumentBuilderData Using_Name_From_Path(string path);
    }
}
