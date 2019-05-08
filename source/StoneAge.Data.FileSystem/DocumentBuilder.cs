using System.IO;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    public class DocumentBuilder : IDocumentBuilder
    {
        private string _filePath;
        private string _name;
        private byte[] _bytes;
        
        public IDocumentBuilderData With_Name(string name)
        {
            _name = name;
            return this;
        }

        public IDocumentBuilderGeneration With_File(string file)
        {
            _filePath = file;
            return this;
        }
        
        public IDocumentBuilderGeneration With_Bytes(byte[] bytes)
        {
            _bytes = bytes;
            return this;
        }

        public IDocument Create_Document()
        {
            var data = Get_Data();
            return new Document
            {
                Name = _name,
                Data = data
            };
        }

        private byte[] Get_Data()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                return File.ReadAllBytes(_filePath);
            }

            return _bytes;
        }
    }
}
